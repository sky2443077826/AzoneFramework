

using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// 应用程序路径类
    /// </summary>
    public static class ApplicationPath
    {
        // 数据目录
        public static string DataPath => Application.dataPath;

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
        /// ios平台如果通过www读取stremingasset下的数据，需要加上file:///前缀
        /// </summary>
        public static string StreamingAssetsPath
        {
            get
            {
                return Application.streamingAssetsPath;
            }
        }

        /// <summary>
        /// 日志目录
        /// </summary>
        public static string logPath = $"{PersistentDataPath}/Log";

        /// <summary>
        /// 配置表文件路径
        /// </summary>
        public static string configPath = StringUtility.Concat(StreamingAssetsPath, "/Configs/");


        /// <summary>
        /// 获取相对路径(相对于Unity中的Assets/下的目录)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetRelativeDataPath(string path)
        {
            return path.Substring(DataPath.Length - 6);
        }
    }
}
