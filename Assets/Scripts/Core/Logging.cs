using System;

namespace Core
{
    public static class Logging
    {
        public enum LogLevel
        {
            Default, Warning, Error
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void Write(string msg, LogLevel level = LogLevel.Default)
        {
            #if UNITY_EDITOR
            switch (level)
            {
                case LogLevel.Default:
                    UnityEngine.Debug.Log(msg);
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(msg);
                    break;
                case LogLevel.Error:
                    UnityEngine.Debug.LogError(msg);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
            #endif
        }
    }
}