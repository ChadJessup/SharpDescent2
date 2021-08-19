using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;
using SharpDescent2.Core.Loaders;

namespace SharpDescent2.Core.Systems;

public class MissionSystem : IGamePlatformManager
{
    public const int MISSION_NAME_LEN = 25;

    private readonly ILogger<MissionSystem> logger;
    private readonly ILibraryManager library;

    public MissionSystem(
        ILogger<MissionSystem> logger,
        ILibraryManager libraryManager)
    {
        this.logger = logger;
        this.library = libraryManager;
    }

    public bool IsInitialized { get; }

    public MissionListEntry[] Mission_list { get; } = new MissionListEntry[MAX.MISSIONS];
    public int Current_mission_num { get; set; } = 0;
    public int N_secret_levels;		//	Made a global by MK for scoring purposes.  August 1, 1995.

    public async ValueTask<bool> Initialize()
    {
        var hog = (HOGArchive)this.library.GetLibrary("descent2.hog");
        var builtinMissionHeader = hog.FileHeaders.FirstOrDefault(fh => fh.FileName.Equals("d2.mn2", StringComparison.OrdinalIgnoreCase));

        var builtinMission = await hog.ReadFile(builtinMissionHeader);

        this.ReadMissionFile(builtinMission.Span, builtinMissionHeader.FileName, 0, ML.CURDIR);

        return true;
    }

    private void ReadMissionFile(Span<byte> builtinMission, string filename, int count, ML location)
    {
        // TODO: cleanup

        var contents = Encoding.ASCII.GetString(builtinMission)
            .TrimEnd('\u001a')
            .TrimEnd();

        var newline = "\r\n";

        var lines = contents.Split(newline, StringSplitOptions.RemoveEmptyEntries);

        this.TryGetIndexFor(lines, "type = ", out var _, out var typeValue);
        this.TryGetIndexFor(lines, "name = ", out var _, out var missionName);

        var mle = new MissionListEntry
        {
            filename = Path.GetFileNameWithoutExtension(filename),
            anarchy_only_flag = typeValue.Equals("anarchy", StringComparison.OrdinalIgnoreCase),
            location = location,
            mission_name = missionName,
        };

        if (this.TryGetIndexFor(lines, "num_levels = ", out var numLvlIndex, out var numLevelsString)
            && int.TryParse(numLevelsString, out var numLevelsCount))
        {
            numLvlIndex++;

            mle.levelNames = new string[numLevelsCount];
            for (int i = 0; i < numLevelsCount; i++)
            {
                mle.levelNames[i] = lines[i + numLvlIndex];
            }
        }

        if (this.TryGetIndexFor(lines, "num_secrets = ", out var secLvlIndex, out var secLevelsString)
            && int.TryParse(secLevelsString, out var secLevelsCount))
        {
            secLvlIndex++;

            mle.secretLevelNames = new string[secLevelsCount];

            for (int j = 0; j < secLevelsCount; j++)
            {
                mle.secretLevelNames[j] = lines[j + secLvlIndex];
            }
        }

        this.Mission_list[count] = mle;
    }

    private bool TryGetIndexFor(string[] lines, string filter, out int index, out string value)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith(filter))
            {
                index = i;
                value = lines[i][filter.Length..];
                return true;
            }
        }

        index = -1;
        value = string.Empty;

        return false;
    }

    public void Dispose()
    {
    }
}

public struct MissionListEntry
{
    public string filename;                    //path and filename without extension
    public string mission_name; //[MISSION_NAME_LEN + 1];
    public bool anarchy_only_flag;                    //if true, mission is anarchy only
    public ML location;                             //see defines below
    public string[] levelNames;
    public string[] secretLevelNames;
}

public enum ML
{
    //values that describe where a mission is located
    CURDIR = 0,
    MISSIONDIR = 1,
    CDROM = 2,
}
