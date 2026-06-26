using System;

public static class EventBus
{
    public static event Action OnPlayerDied;
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;

    public static void PublishPlayerDied() => OnPlayerDied?.Invoke();
    public static void PublishGamePaused() => OnGamePaused?.Invoke();
    public static void PublishGameResumed() => OnGameResumed?.Invoke();
}
