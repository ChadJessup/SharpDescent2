namespace Sharp.Platform.Interfaces;

public interface IMusicManager : IGamePlatformManager
{
    bool MusicPoll(bool force);
    void SetMusicMode(MusicMode nONE);
    void MusicSetVolume(int value);
    int MusicGetVolume();
}

public enum MusicMode
{
    Unknown = 0,
    NONE,
    RESTORE,
    MAIN_MENU,
    TACTICAL_NOTHING,
    TACTICAL_ENEMYPRESENT,
    TACTICAL_BATTLE,
    TACTICAL_VICTORY,
    TACTICAL_DEATH,
    LAPTOP,
};
