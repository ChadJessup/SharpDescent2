using System.Threading.Tasks;
using Sharp.Platform.Interfaces;

namespace Sharp.Platform.NullManagers
{
    public class NullMusicManager : IMusicManager
    {
        public bool IsInitialized => true;

        public ValueTask<bool> Initialize()
        {
            return ValueTask.FromResult(true);
        }

        public void Dispose()
        {
        }

        public bool MusicPoll(bool force)
        {
            return true;
        }

        public void SetMusicMode(MusicMode mode)
        {
        }

        public void MusicSetVolume(int value)
        {
        }

        public int MusicGetVolume()
        {
            return 10;
        }
    }
}
