

using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// Ӧ�ó���·����
    /// </summary>
    public static class ApplicationPath
    {
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
        /// ��־Ŀ¼
        /// </summary>
        public static string logPath = $"{PersistentDataPath}/Log";
    }
}
