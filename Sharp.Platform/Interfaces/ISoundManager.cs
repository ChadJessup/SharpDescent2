namespace Sharp.Platform.Interfaces
{
    public interface ISoundManager : IGamePlatformManager
    {
        void SoundStopAll();
        ValueTask<bool> InitSound();
        void SetSoundEffectsVolume(int iNewValue);
        void SoundStop(uint uiOptionToggleSound);
        int GetSoundEffectsVolume();
        int GetSpeechVolume();
        void SetSpeechVolume(int iNewValue);
        bool SoundIsPlaying(uint uiLastPlayingSoundID);
    }
}
