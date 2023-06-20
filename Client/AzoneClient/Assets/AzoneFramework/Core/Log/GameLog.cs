using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace AzoneFramework
{
    // ��־�ȼ�
    [Flags]
    public enum ELogLevel
    {
        Normal = 1 << 1,
        Warning = 1 << 2,
        Error = 1 << 3,

        All = Normal | Warning | Error
    }

    /// <summary>
    /// ��־��
    /// </summary>
    public static class GameLog
    {
        // ��־�ȼ�
        private static ELogLevel _logLevel;

        // ��־�ļ�
        private static string _logFile;

        // ��־ר���ַ���
        private static StringBuilder _logStrBuilder = new StringBuilder();

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="level">��־�ȼ�</param>
        /// <param name="isNeedSave">�Ƿ񱣴�</param>
        public static void Init(ELogLevel level, bool isSave)
        {
            _logLevel = level;

            if (isSave)
            {
                Application.logMessageReceived += SaveLogToFile;
                if (!Directory.Exists(ApplicationPath.logPath))
                {
                    Directory.CreateDirectory(ApplicationPath.logPath);
                }

                string currentTime = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                _logFile = $"{ApplicationPath.logPath}/{currentTime}.txt";
                if (!File.Exists(_logFile))
                {
                    File.CreateText(_logFile).Dispose();
                }
            }

            Normal("===��Ϸ��־ģ���������===");
            Normal($"===��ǰ��־�ȼ���{_logLevel}===");
        }

        /// <summary>
        /// �����־���ļ�
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private static void SaveLogToFile(string condition, string stackTrace, LogType type)
        {
            _logStrBuilder.Clear();

            _logStrBuilder.AppendLine("============Begin===============");
            _logStrBuilder.AppendLine(DateTime.UtcNow.ToString());
            _logStrBuilder.Append(type);
            _logStrBuilder.Append(":     ");
            _logStrBuilder.AppendLine(condition);
            _logStrBuilder.AppendLine(stackTrace);
            _logStrBuilder.AppendLine("============End===============");
            File.AppendAllText(_logFile, _logStrBuilder.ToString());
        }

        /// <summary>
        /// �����־
        /// </summary>
        /// <param name="msg"></param>
        public static void Normal(object msg)
        {
            Normal(msg.ToString());
        }

        /// <summary>
        /// �����־
        /// </summary>
        /// <param name="logContent"></param>
        /// <param name="args"></param>
        public static void Normal(string logContent, params object[] args)
        {
            if (!_logLevel.HasFlag(ELogLevel.Normal))
            {
                return;
            }

            _logStrBuilder.Clear();
            _logStrBuilder.AppendFormat(logContent, args);
            Debug.Log(_logStrBuilder.ToString());
        }

        /// <summary>
        /// �����־
        /// </summary>
        /// <param name="msg"></param>
        public static void Warning(object msg)
        {
            Warning(msg.ToString());
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="logContent"></param>
        /// <param name="args"></param>
        public static void Warning(string logContent, params object[] args)
        {
            if (!_logLevel.HasFlag(ELogLevel.Warning))
            {
                return;
            }

            _logStrBuilder.Clear();
            _logStrBuilder.AppendFormat(logContent, args);
            Debug.LogWarning(_logStrBuilder.ToString());
        }



        /// <summary>
        /// �����־
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(object msg)
        {
            Warning(msg.ToString());
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="logContent"></param>
        /// <param name="args"></param>
        public static void Error(string logContent, params object[] args)
        {
            if (!_logLevel.HasFlag(ELogLevel.Error))
            {
                return;
            }

            _logStrBuilder.Clear();
            _logStrBuilder.AppendFormat(logContent, args);
            Debug.LogError(_logStrBuilder.ToString());
        }
    }
}
