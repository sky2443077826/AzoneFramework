

using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// 应用程序路径类
    /// </summary>
    public static class ApplicationPath
    {
        // 持久数据目录
        private static string _persistentDataPath;
        public static string PersistentDataPath 
        {
            get
            {
                if (string.IsNullOrEmpty(_persistentDataPath))
                {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                    _persistentDataPath = $"{Application.dataPath}/PersistentData";
#else
                    _persistentDataPath = Application.persistentDataPath;
#endif
                }

                return _persistentDataPath;
            }
        }

        /// <summary>
        /// 日志目录
        /// </summary>
        public static string logPath = $"{PersistentDataPath}/Log";
    }
}
