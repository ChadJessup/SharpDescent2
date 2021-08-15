using Sharp.Platform.Interfaces;

namespace Sharp.Platform.NullManagers
{
    public class NullSoundManager : ISoundManager, ISound2dManager, ISound3dManager
    {
        public bool IsInitialized { get; } = true;

        public ValueTask<bool> Initialize()
        {
            return ValueTask.FromResult(true);
        }

        public void Dispose()
        {
        }

        public void SoundStopAll()
        {
            throw new NotImplementedException();
        }

        public ValueTask<bool> InitSound()
        {
            throw new NotImplementedException();
        }

        public void SetSoundEffectsVolume(int iNewValue)
        {
            throw new NotImplementedException();
        }

        public void SoundStop(uint uiOptionToggleSound)
        {
            throw new NotImplementedException();
        }

        public int GetSoundEffectsVolume()
        {
            throw new NotImplementedException();
        }

        public int GetSpeechVolume()
        {
            throw new NotImplementedException();
        }

        public void SetSpeechVolume(int iNewValue)
        {
            throw new NotImplementedException();
        }

        public bool SoundIsPlaying(uint uiLastPlayingSoundID)
        {
            return true;
        }
    }
}
