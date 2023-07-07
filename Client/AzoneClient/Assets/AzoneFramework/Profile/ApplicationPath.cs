

using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// Ӧ�ó���·����
    /// </summary>
    public static class ApplicationPath
    {
        // ����Ŀ¼
        public static string DataPath => Application.dataPath;

        // �־�����Ŀ¼
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
        /// iosƽ̨���ͨ��www��ȡstremingasset�µ����ݣ���Ҫ����file:///ǰ׺
        /// </summary>
        public static string StreamingAssetsPath
        {
            get
            {
                return Application.streamingAssetsPath;
            }
        }

        /// <summary>
        /// ��־Ŀ¼
        /// </summary>
        public static string logPath = $"{PersistentDataPath}/Log";

        /// <summary>
        /// ���ñ��ļ�·��
        /// </summary>
        public static string configPath = StringUtility.Concat(StreamingAssetsPath, "/Configs/");


        /// <summary>
        /// ��ȡ���·��(�����Unity�е�Assets/�µ�Ŀ¼)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetRelativeDataPath(string path)
        {
            return path.Substring(DataPath.Length - 6);
        }
    }
}
