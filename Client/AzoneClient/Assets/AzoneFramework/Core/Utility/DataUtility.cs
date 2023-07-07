using System;
using System.Collections.Generic;

namespace AzoneFramework
{
    /// <summary>
    /// QSͨ�ú�����
    /// </summary>
    public class DataUtility : Singleton<DataUtility>
    {
        /// <summary>
        /// ��ͼ���͵���ͼID��ӳ��
        /// </summary>
        private Dictionary<eViewPort, int> _viewType2Config;
        /// <summary>
        /// �Ա�����ӳ�䵽��ɫID
        /// </summary>
        private Dictionary<eGender, int> _genderType2Config;

        /// <summary>
        /// UID����
        /// </summary>
        private static uint gUIDIndex = 0;

        /// <summary>
        /// RID ��������
        /// </summary>
        private static uint gBaseRID = 1672502400;

        protected override void OnCreate()
        {
            _viewType2Config = new Dictionary<eViewPort, int>();
            _genderType2Config = new Dictionary<eGender, int>();
            // ������ͼ�ṹ
            if (!ParseViewPortConfig())
            {
                GameLog.Error("����viewportӳ��ʧ�ܡ�");
                return;
            }

            // ������ɫ�ṹ
            if (!ParseRoleConfig())
            {
                GameLog.Error("����Roleӳ��ʧ�ܡ�");
                return;
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }

        /// <summary>
        /// ����configID����UID
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ulong GenerateUID(int config)
        {
            /**
             *     | Config | index | unixʱ��� |
             *     | 16bit  | 16bit |   32bit   |
             * */
            if (gUIDIndex >= 0xfff0)
            {
                gUIDIndex = 0;
            }

            uint lowValue = (uint)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() & UInt32.MaxValue);
            ulong highValue = (ushort)(config & 0xffff);
            return (((highValue << 16) + gUIDIndex++) << 32) + lowValue;
        }

        /// <summary>
        /// ����RID
        /// </summary>
        /// <returns></returns>
        public static int GenerateRID()
        {
            return (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - gBaseRID);
        }

        /// <summary>
        /// �����ַ���ת��Ϊ����
        /// </summary>
        public static eDataType Str2DataType(string str)
        {
            if (!Enum.TryParse(str, true, out eDataType type))
            {
                GameLog.Error("δ֧�ֵ��������ͣ�{0}", str);
                return eDataType.Invalid;
            }

            return type;
        }

        /// <summary>
        /// ������ͼ���ͻ�ȡ��ͼid
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetViewPortConfig(eViewPort type)
        {
            if (!_viewType2Config.ContainsKey(type))
            {
                GameLog.Error($"û���ҵ�{type}���͵���ͼ��");
                return 0;
            }
            return _viewType2Config[type];
        }

        /// <summary>
        /// �����Ա����ͻ�ȡ��ɫID
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetRoleConfig(eGender type)
        {
            if (!_genderType2Config.ContainsKey(type))
            {
                GameLog.Error($"û���ҵ��Ա�{type}�Ľ�ɫ��");
                return 0;
            }
            return _genderType2Config[type];
        }


        /// <summary>
        /// ����viewport���͵�����
        /// </summary>
        /// <returns></returns>
        private bool ParseViewPortConfig()
        {
            // ��ȡ���е�veiwport����
            if (!ConfigManager.Instance.GetConfigIDsByClass("ViewPort", out List<int> IDs))
            {
                GameLog.Error("��ȡ����ViewPortʧ�ܡ�");
                return false;
            }

            if (IDs.Count == 0)
            {
                GameLog.Error("ViewPort���͵�����Ϊ0");
                return false;
            }

            for (int index = 0; index < IDs.Count; ++index)
            {
                // ��ȡ��ͼ����
                int type = ConfigManager.Instance.GetConfigInt(IDs[index], "ViewType");
                if (!Enum.IsDefined(typeof(eViewPort), type))
                {
                    GameLog.Error($"��Ч����ͼ���͡�{IDs[index]} -> {type}");
                    continue;
                }

                if (_viewType2Config.ContainsKey((eViewPort)type))
                {
                    GameLog.Error($"�ظ�����ͼ�ṹ��{IDs[index]} -> {type}");
                    continue;
                }

                _viewType2Config.Add((eViewPort)type, IDs[index]);
            }

            return true;
        }

        /// <summary>
        /// ����Role���͵�����
        /// </summary>
        /// <returns></returns>
        private bool ParseRoleConfig()
        {
            // ��ȡ���е�Role����
            if (!ConfigManager.Instance.GetConfigIDsByClass("Role", out List<int> IDs))
            {
                GameLog.Error("��ȡ����Roleʧ�ܡ�");
                return false;
            }

            if (IDs.Count == 0)
            {
                GameLog.Error("Role���͵�����Ϊ0");
                return false;
            }

            for (int index = 0; index < IDs.Count; ++index)
            {
                // ��ȡ��ͼ����
                int type = ConfigManager.Instance.GetConfigInt(IDs[index], "Gender");
                if (!Enum.IsDefined(typeof(eGender), type))
                {
                    GameLog.Error($"��Ч���Ա����͡�{IDs[index]} -> {type}");
                    continue;
                }

                if (_genderType2Config.ContainsKey((eGender)type))
                {
                    GameLog.Error($"�ظ�����ͼ�ṹ��{IDs[index]} -> {type}");
                    continue;
                }

                _genderType2Config.Add((eGender)type, IDs[index]);
            }

            return true;
        }
    }
}
