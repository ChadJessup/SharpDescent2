namespace Sharp.Platform.Interfaces
{
    public interface IClockManager : IGamePlatformManager
    {
        bool GamePaused { get; set; }

        long GetClock();
        long GetTickCount();
        void UpdateClock();
        void UnPauseGame();
        void PauseGame();
    }
}
