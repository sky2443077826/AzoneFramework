using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// ���ó���
    /// </summary>
    internal class ConfigConstant
    {
        // configת��Ϊtype��λƫ��
        public const int kConfig2TypeBase = 10000;

        /// <summary>
        /// ���ñ�ṹ�����ļ�
        /// </summary>
        public const string kLogicClassCfg = "LogicClass.xml";

        /// <summary>
        /// �����ඨ��
        /// </summary>
        public const string kStructPath = "Class/";

        /// <summary>
        /// ����������
        /// </summary>
        public const string kDataPath = "Data/";
    }

    /// <summary>
    /// �������ýṹ
    /// </summary>
    internal class ClassConfig
    {
        /// <summary>
        /// ����
        /// </summary>
        public string className;

        /// <summary>
        /// ��������
        /// </summary>
        public ClassConfig parent;

        /// <summary>
        /// �������ݶ���
        /// </summary>
        public string clsDataPath;

        /// <summary>
        /// �����ļ�
        /// </summary>
        public List<string> incFiles;

        /// <summary>
        /// ��ǰ���Ͱ�����ID
        /// </summary>
        public List<int> IDs;

        /// <summary>
        /// ���Թ�����
        /// </summary>
        public PropertyManager propMgr;

        /// <summary>
        /// ��������
        /// </summary>
        public RecordManager recMgr;

        public ClassConfig(string className, ClassConfig parent = null)
        {
            propMgr = new PropertyManager();
            recMgr = new RecordManager();

            incFiles = new List<string>();
            IDs = new List<int>();

            this.className = className;
            this.parent = parent;
        }

    }

    /// <summary>
    /// ���ݽṹ
    /// </summary>
    internal class ClassData
    {
        public ClassData()
        {
            propMgr = new PropertyManager();
            recMgr = new RecordManager();
        }

        public PropertyManager propMgr;
        public RecordManager recMgr;
    }

    /// <summary>
    /// ��Ϸ���ü���ģ��
    /// </summary>
    public class ConfigManager : Singleton<ConfigManager>
    {
        // ��������
        // key: ������ value: ��������
        private Dictionary<string, ClassConfig> _classConfigList;
        // ��������
        // key: ����ID value: ��������
        private Dictionary<int, ClassData> _classDataList;

        /// <summary>
        /// ��ʼ��
        /// </summary>
        protected override void OnCreate()
        {
            _classConfigList = new Dictionary<string, ClassConfig>();
            _classDataList = new Dictionary<int, ClassData>();

            //// ��������
            LoadClassConfig();
            //// ��������
            LoadClassData();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }

        /// <summary>
        /// �������ͻ�ȡ���е����������ID
        /// </summary>
        /// <param name="className">����</param>
        /// <param name="IDs">����ID�б�</param>
        public bool GetConfigIDsByClass(string className, out List<int> IDs)
        {
            IDs = null;
            if (!_classConfigList.ContainsKey(className))
            {
                return false;
            }

            if (!_classConfigList.TryGetValue(className, out ClassConfig clsConfig))
            {
                return false;
            }

            IDs = new List<int>();
            IDs.AddRange(clsConfig.IDs);
            return true;
        }

        /// <summary>
        /// ������������ȡ��ṹ
        /// </summary>
        /// <param name="className"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool GetClassPropMgr(string className, out PropertyManager propMgr)
        {
            propMgr = null;
            if (!_classConfigList.ContainsKey(className))
            {
                return false;
            }

            if (!_classConfigList.TryGetValue(className, out ClassConfig clsConfig))
            {
                return false;
            }

            propMgr = clsConfig.propMgr;

            return true;
        }

        /// <summary>
        /// �������ͻ�ȡ���ṹ
        /// </summary>
        /// <param name="className"></param>
        /// <param name="recMgr"></param>
        /// <returns></returns>
        public bool GetClassRecMgr(string className, out RecordManager recMgr)
        {
            recMgr = null;
            if (!_classConfigList.ContainsKey(className)) return false;
            if (!_classConfigList.TryGetValue(className, out ClassConfig clsConfig)) return false;
            recMgr = clsConfig.recMgr;
            return true;
        }

        /// <summary>
        /// �Ƿ����������
        /// </summary>
        /// <param name="cls">����������</param>
        /// <returns>���ڷ���true�����򷵻�false</returns>
        public bool HasClass(string cls)
        {
            return _classConfigList.ContainsKey(cls);
        }

        /// <summary>
        /// �Ƿ��������ID
        /// </summary>
        /// <param name="id">����ID</param>
        /// <returns>���ڷ���true�����򷵻�false</returns>
        public bool HasConfig(int id)
        {
            return _classDataList.ContainsKey(id);
        }

        /// <summary>
        /// ��������ID��ȡ����
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public eObjectType GetType(int id)
        {
            return (eObjectType)(id / ConfigConstant.kConfig2TypeBase);
        }

        /// <summary>
        /// �������Ե�ָ��������
        /// </summary>
        /// <param name="otherPropMgr"></param>
        /// <returns></returns>
        public bool CloneTo(int ID, ref PropertyManager otherPropMgr, ref RecordManager otherRecordMgr)
        {
            if (!_classDataList.ContainsKey(ID))
            {
                return false;
            }

            ClassData clsData;
            if (!_classDataList.TryGetValue(ID, out clsData))
            {
                return false;
            }

            clsData.propMgr.CloneTo(ref otherPropMgr);
            clsData.recMgr.CloneTo(ref otherRecordMgr);
            return true;
        }

        /// <summary>
        /// ����ID��ȡĬ������ֵ
        /// </summary>
        /// <param name="id">����ID</param>
        /// <param name="name">������</param>
        /// <returns></returns>
        public bool GetConfigBool(int id, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (!_classDataList.ContainsKey(id))
            {
                return false;
            }

            ClassData clsData;
            if (!_classDataList.TryGetValue(id, out clsData))
            {
                return false;
            }

            PropertyManager propMgr = clsData.propMgr;
            if (propMgr == null)
            {
                return false;
            }

            if (!propMgr.Find(name))
            {
                return false;
            }

            return propMgr.GetBool(name);
        }

        /// <summary>
        /// ����ID��ȡĬ������ֵ
        /// </summary>
        /// <param name="id">����ID</param>
        /// <param name="name">������</param>
        /// <returns></returns>
        public int GetConfigInt(int id, string name, int def = 0)
        {
            if (string.IsNullOrEmpty(name))
            {
                return def;
            }

            if (!_classDataList.ContainsKey(id))
            {
                return def;
            }

            ClassData clsData;
            if (!_classDataList.TryGetValue(id, out clsData))
            {
                return def;
            }

            PropertyManager propMgr = clsData.propMgr;
            if (propMgr == null)
            {
                return def;
            }

            if (!propMgr.Find(name))
            {
                return def;
            }

            return propMgr.GetInt(name);
        }

        /// <summary>
        /// ����ID��ȡĬ������ֵ
        /// </summary>
        /// <param name="id">����ID</param>
        /// <param name="name">������</param>
        /// <returns></returns>
        public long GetConfigLong(int id, string name, long def = 0)
        {
            if (string.IsNullOrEmpty(name))
            {
                return def;
            }

            if (!_classDataList.ContainsKey(id))
            {
                return def;
            }

            ClassData clsData;
            if (!_classDataList.TryGetValue(id, out clsData))
            {
                return def;
            }

            PropertyManager propMgr = clsData.propMgr;
            if (propMgr == null)
            {
                return def;
            }

            if (!propMgr.Find(name))
            {
                return def;
            }

            return propMgr.GetLong(name);
        }

        /// <summary>
        /// ����ID��ȡĬ������ֵ
        /// </summary>
        /// <param name="id">����ID</param>
        /// <param name="name">������</param>
        /// <returns></returns>
        public ulong GetConfigULong(int id, string name, ulong def = 0)
        {
            if (string.IsNullOrEmpty(name))
            {
                return def;
            }

            if (!_classDataList.ContainsKey(id))
            {
                return def;
            }

            ClassData clsData;
            if (!_classDataList.TryGetValue(id, out clsData))
            {
                return def;
            }

            PropertyManager propMgr = clsData.propMgr;
            if (propMgr == null)
            {
                return def;
            }

            if (!propMgr.Find(name))
            {
                return def;
            }

            return propMgr.GetULong(name);
        }

        /// <summary>
        /// ����ID��ȡĬ������ֵ
        /// </summary>
        /// <param name="id">����ID</param>
        /// <param name="name">������</param>
        /// <returns></returns>
        public float GetConfigFloat(int id, string name, float def = 0.0f)
        {
            if (string.IsNullOrEmpty(name))
            {
                return def;
            }

            if (!_classDataList.ContainsKey(id))
            {
                return def;
            }

            ClassData clsData;
            if (!_classDataList.TryGetValue(id, out clsData))
            {
                return def;
            }

            PropertyManager propMgr = clsData.propMgr;
            if (propMgr == null)
            {
                return def;
            }

            if (!propMgr.Find(name))
            {
                return def;
            }

            return propMgr.GetFloat(name);
        }

        /// <summary>
        /// ����ID��ȡĬ������ֵ
        /// </summary>
        /// <param name="id">����ID</param>
        /// <param name="name">������</param>
        /// <returns></returns>
        public double GetConfigDouble(int id, string name, double def = 0.0)
        {
            if (string.IsNullOrEmpty(name))
            {
                return def;
            }

            if (!_classDataList.ContainsKey(id))
            {
                return def;
            }

            ClassData clsData;
            if (!_classDataList.TryGetValue(id, out clsData))
            {
                return def;
            }

            PropertyManager propMgr = clsData.propMgr;
            if (propMgr == null)
            {
                return def;
            }

            if (!propMgr.Find(name))
            {
                return def;
            }

            return propMgr.GetDouble(name);
        }

        /// <summary>
        /// ����ID��ȡĬ������ֵ
        /// </summary>
        /// <param name="id">����ID</param>
        /// <param name="name">������</param>
        /// <returns></returns>
        public string GetConfigString(int id, string name, string def = default)
        {
            if (string.IsNullOrEmpty(name))
            {
                return def;
            }

            if (!_classDataList.ContainsKey(id))
            {
                return def;
            }

            ClassData clsData;
            if (!_classDataList.TryGetValue(id, out clsData))
            {
                return def;
            }

            PropertyManager propMgr = clsData.propMgr;
            if (propMgr == null)
            {
                return def;
            }

            if (!propMgr.Find(name))
            {
                return def;
            }

            return propMgr.GetString(name);
        }

        /// <summary>
        /// �������ñ�
        /// </summary>
        private void LoadClassConfig()
        {
            string classMapPath = StringUtility.Concat(ApplicationPath.configPath, ConfigConstant.kLogicClassCfg);
            XmlDocument doc = new XmlDocument();
            doc.Load(classMapPath);

            // �������ͽṹ
            //             
            //             XmlNodeList allNodeList = doc.GetElementsByTagName("XML");
            //             if (allNodeList.Count == 0)
            //             {
            //                 return;
            //             }

            // ��һ��Ԫ��
            XmlElement rootNode = doc.DocumentElement;
            // �������ڵ��������е�����
            foreach (XmlNode node in rootNode.ChildNodes)
            {
                ClassConfig cls = null;
                LoadClassConfig(node, ref cls);
            }
        }

        /// <summary>
        /// �����߼�������
        /// </summary>
        /// <param name="classNode">�߼�����ڵ�</param>
        private void LoadClassConfig(XmlNode classNode, ref ClassConfig cls)
        {
            // ��ṹ�ļ�
            if (classNode.Attributes["Struct"] == null)
            {
                GameLog.Error("{0} �ṹ�������", ConfigConstant.kLogicClassCfg);
                return;
            }

            // ������
            if (classNode.Attributes["ID"] == null)
            {
                GameLog.Error("Class Name ����Ϊ�ա���������ʧ�ܡ�{0}", ConfigConstant.kLogicClassCfg);
                return;
            }

            // ������
            string clsName = classNode.Attributes["ID"].Value;
            // �ṹ·��
            string structPath = classNode.Attributes["Struct"].Value;

            if (string.IsNullOrEmpty(structPath))
            {
                GameLog.Error("Struct ����Ϊ�ա���������ʧ�ܡ�");
                return;
            }

            if (_classConfigList.ContainsKey(clsName))
            {
                GameLog.Error("�Ѿ�����{0}���������á�", clsName);
                return;
            }

            // �������ͽṹ
            ClassConfig clsConfig = new ClassConfig(clsName, cls);

            // ����·��
            if (classNode.Attributes["Data"] != null)
            {
                // �������Ͷ�Ӧ������
                clsConfig.clsDataPath = classNode.Attributes["Data"].Value;
            }

            // �������������
            if (!AddClass(structPath, ref clsConfig))
            {
                GameLog.Error("{0}:�����ṹʧ�ܡ�", clsName);
                return;
            }

            // �����ӽڵ�
            foreach (XmlNode node in classNode.ChildNodes)
            {
                LoadClassConfig(node, ref clsConfig);
            }

            // ��ӵ�����
            _classConfigList.Add(clsName, clsConfig);
        }


        /// <summary>
        /// ����ָ�����͵������Լ����ṹ
        /// </summary>
        /// <param name="structPath">�ṹ����·��</param>
        /// <param name="clsConfig">�ṹ��</param>
        /// <returns></returns>
        private bool AddClass(string structPath, ref ClassConfig clsConfig)
        {
            if (clsConfig == null || string.IsNullOrEmpty(structPath))
            {
                return false;
            }

            // ��ȡ����
            ClassConfig parentClsConfig = clsConfig.parent;
            while (parentClsConfig != null)
            {
                List<string> incFiles = parentClsConfig.incFiles;
                foreach (string inc in incFiles)
                {
                    if (string.IsNullOrEmpty(inc))
                    {
                        continue;
                    }
                    if (clsConfig.incFiles.Contains(inc))
                    {
                        continue;
                    }
                    if (AddInclude(inc, ref clsConfig))
                    {
                        clsConfig.incFiles.Add(inc);
                    }
                }

                parentClsConfig = parentClsConfig.parent;
            }

            if (AddInclude(structPath, ref clsConfig))
            {
                clsConfig.incFiles.Add(structPath);
            }

            return true;
        }

        /// <summary>
        /// ���ذ����ļ�
        /// </summary>
        /// <param name="inc"></param>
        /// <param name="clsConfig"></param>
        /// <returns></returns>
        private bool AddInclude(string inc, ref ClassConfig clsConfig)
        {
            if (clsConfig.incFiles.Contains(inc))
            {
                return false;
            }

            string struct_file = StringUtility.Concat(ApplicationPath.configPath, ConfigConstant.kStructPath, inc);
            XmlDocument doc = new XmlDocument();
            doc.Load(struct_file);

            //             XmlNodeList allNodeList = doc.GetElementsByTagName("XML");
            //             if (allNodeList.Count == 0)
            //             {
            //                 return false;
            //             }
            // 
            //             // ��ȡ��һ��Ԫ��
            //             XmlNode root = allNodeList[0];

            // ��һ��Ԫ��
            XmlElement root = doc.DocumentElement;

            // �������ڵ��������е�����
            // ���Խڵ�
            XmlNode propertyNode = root["Property"];
            if (propertyNode != null)
            {
                AddProperty(propertyNode, ref clsConfig);
            }

            // ���ڵ�
            XmlNode recordNode = root["Record"];
            if (recordNode != null)
            {
                AddRecord(recordNode, ref clsConfig);
            }

            // �����ļ�
            XmlNode includeNode = root["Include"];
            if (includeNode != null)
            {
                foreach (XmlNode node in includeNode.ChildNodes)
                {
                    if (node.Attributes["ID"] == null)
                    {
                        continue;
                    }

                    string structIncFile = node.Attributes["ID"].Value;
                    if (string.IsNullOrEmpty(structIncFile))
                    {
                        continue;
                    }

                    if (AddInclude(structIncFile, ref clsConfig))
                    {
                        clsConfig.incFiles.Add(structIncFile);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="rootNode">���Խڵ�</param>
        /// <param name="clsConfig">���Զ�Ӧ����ṹ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        private bool AddProperty(XmlNode rootNode, ref ClassConfig clsConfig)
        {
            foreach (XmlNode node in rootNode)
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                // ������
                string name = node.Attributes["ID"].Value;
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }
                if (clsConfig.propMgr.Find(name))
                {
                    GameLog.Error("Same property. {0} at {1}.", name, clsConfig.className);
                    continue;
                }

                string strType = node.Attributes["Type"].Value;
                if (string.IsNullOrEmpty(strType))
                {
                    continue;
                }

                // ת������
                eDataType type = DataUtility.Str2DataType(strType);
                if (type == eDataType.Invalid)
                {
                    continue;
                }
                Property prop = new Property(name, type);

                // �����Ƿ񱣴���
                if (node.Attributes["Save"] != null)
                {
                    // �б�����
                    prop.Save = XmlConvert.ToInt32(node.Attributes["Save"].Value) != 0;
                }

                // �Ƿ�ͬ�����
                if (node.Attributes["Sync"] != null)
                {
                    // �б�����
                    prop.Sync = XmlConvert.ToInt32(node.Attributes["Sync"].Value) != 0;
                }

                // �Ƿ�����
                if (node.Attributes["Shared"] != null)
                {
                    // �б�����
                    prop.Shared = XmlConvert.ToInt32(node.Attributes["Shared"].Value) != 0;
                }

                // ��ӵ����Թ�����
                if (!clsConfig.propMgr.Add(name, prop))
                {
                    GameLog.Error("Add property failed. {0} - {1}.", name, strType);
                    continue;
                }
            }

            return true;
        }

        /// <summary>
        /// ��ӱ��
        /// </summary>
        /// <param name="rootNode">���ڵ�</param>
        /// <param name="clsConfig">����Ӧ����ṹ</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        private bool AddRecord(XmlNode rootNode, ref ClassConfig clsConfig)
        {
            foreach (XmlNode node in rootNode)
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                string recName = node.Attributes["ID"]?.Value;
                if (string.IsNullOrEmpty(recName))
                {
                    GameLog.Error($"Error record name. {clsConfig.className}��");
                    continue;
                }

                if (clsConfig.recMgr.HasRecord(recName))
                {
                    GameLog.Error("Same record. {0} at {1}.", recName, clsConfig.className);
                    continue;
                }

                string rowStr = node.Attributes["Row"]?.Value;
                int row = Convert.ToInt32(rowStr);

                string columStr = node.Attributes["Col"]?.Value;
                int col = Convert.ToInt32(columStr);

                // ����������
                XmlNodeList columnNodeList = node.SelectNodes("Data");
                if (columnNodeList.Count != col)
                {
                    GameLog.Error("Error record column count. {0} at {1}.", recName, clsConfig.className);
                    continue;
                }
                RecordColumnInfo[] recordColumnInfos = new RecordColumnInfo[col];
                for (int i = 0; i < recordColumnInfos.Length; i++)
                {
                    XmlNode columnNode = columnNodeList[i];
                    string strType = columnNode.Attributes["Type"]?.Value;
                    if (string.IsNullOrEmpty(strType))
                    {
                        continue;
                    }
                    // ת������
                    eDataType type = DataUtility.Str2DataType(strType);
                    if (type == eDataType.Invalid)
                    {
                        continue;
                    }
                    recordColumnInfos[i].type = type;

                    string tag = columnNode.Attributes["Tag"]?.Value;
                    if (string.IsNullOrEmpty(tag))
                    {
                        GameLog.Error("Error record column tag. {0} at {1} column {2}.", recName, clsConfig.className, columnNode.InnerText);
                        continue;
                    }
                    recordColumnInfos[i].tag = tag;
                }

                string saveStr = node.Attributes["Save"]?.Value;
                bool save = Convert.ToInt32(saveStr) != 0; //  != "0";

                Record record = new Record(recName, row, recordColumnInfos, save);

                // ��ӵ���������
                if (!clsConfig.recMgr.Add(recName, record))
                {
                    GameLog.Error("Add record failed. {0} - {1}.", record);
                    continue;
                }
            }
            return true;
        }

        /// <summary>
        /// ����������
        /// </summary>
        private void LoadClassData()
        {
            foreach (var iter in _classConfigList)
            {
                // ����
                string clsName = iter.Key;

                // ��ṹ
                ClassConfig clsConfig = iter.Value;
                if (string.IsNullOrEmpty(clsName) || clsConfig == null)
                {
                    continue;
                }

                // ����·��
                string dataPath = clsConfig.clsDataPath;
                if (string.IsNullOrEmpty(dataPath))
                {
                    continue;
                }

                // �����ļ�
                string path = StringUtility.Concat(ApplicationPath.configPath, ConfigConstant.kDataPath, dataPath);
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                // 
                //                 XmlNodeList allNodeList = doc.GetElementsByTagName("XML");
                //                 if (allNodeList.Count == 0)
                //                 {
                //                     continue;
                //                 }

                // ��ȡ��һ��Ԫ��
                XmlNode root = doc.DocumentElement;

                foreach (XmlNode node in root.ChildNodes)
                {
                    if (!LoadClassData(node, ref clsConfig))
                    {
                        GameLog.Error("Load class data failed. Class:{0}.", clsConfig.className);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="node">�ڵ�</param>
        /// <param name="clsConfig">����</param>
        private bool LoadClassData(XmlNode node, ref ClassConfig clsConfig)
        {
            int configID = XmlConvert.ToInt32(node.Attributes["ID"].Value);
            if (0 == configID)
            {
                return false;
            }

            if (_classDataList.ContainsKey(configID))
            {
                GameLog.Error("Load class data failed. Same ConfigID: {0}.", configID);
                return false;
            }

            // ����һ������
            ClassData clsData = new ClassData();
            clsConfig.IDs.Add(configID);

            // ���Թ�����
            clsConfig.propMgr.CloneTo(ref clsData.propMgr);

            // ��������
            clsConfig.recMgr.CloneTo(ref clsData.recMgr);

            // ��������
            foreach (XmlAttribute attribute in node.Attributes)
            {
                // ������
                string name = attribute.Name;
                // ����ֵ
                string value = attribute.Value;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
                {
                    continue;
                }

                if (!clsData.propMgr.Find(name))
                {
                    // GameLog.Error("û���ҵ����ԣ���ʼ������ֵʧ�ܡ�{0}:{1}", configID, name);
                    continue;
                }

                Property prop = clsData.propMgr.GetProperty(name);
                if (prop == null)
                {
                    GameLog.Error("û���ҵ����ԣ���ȡ���Խṹʧ�ܡ�{0}:{1}", configID, name);
                    continue;
                }

                try
                {
                    switch (prop.Type)
                    {
                        case eDataType.Bool:
                            {
                                prop.SetBool(ConvertUtility.BoolConvert(value));
                            }
                            break;

                        case eDataType.Int:
                            {
                                prop.SetInt(ConvertUtility.IntConvert(value));
                            }
                            break;

                        case eDataType.Long:
                            {
                                prop.SetLong(ConvertUtility.LongConvert(value));
                            }
                            break;

                        case eDataType.ULong:
                            {
                                prop.SetULong(ConvertUtility.ULongConvert(value));
                            }
                            break;

                        case eDataType.Float:
                            {
                                prop.SetFloat(ConvertUtility.FloatConvert(value));
                            }
                            break;

                        case eDataType.Double:
                            {
                                prop.SetDouble(ConvertUtility.DoubleConvert(value));
                            }
                            break;

                        case eDataType.String:
                            {
                                prop.SetString(value);
                            }
                            break;
                        case eDataType.Vector2:
                            {
                                string[] arr = value.Split(',');
                                if (arr.Length != 2)
                                {
                                    GameLog.Error("����vector2ʧ�ܣ���ʽ����{0}", value);
                                    break;
                                }
                                Vector2 v = new Vector2(ConvertUtility.FloatConvert(arr[0]), ConvertUtility.FloatConvert(arr[1]));
                                prop.SetVector2(v);
                            }
                            break;

                        case eDataType.Vector3:
                            {
                                string[] arr = value.Split(',');
                                if (arr.Length != 3)
                                {
                                    GameLog.Error("����vector3ʧ�ܣ���ʽ����{0}", value);
                                    break;
                                }
                                Vector3 v = new Vector3(ConvertUtility.FloatConvert(arr[0]), ConvertUtility.FloatConvert(arr[1]), ConvertUtility.FloatConvert(arr[2]));
                                prop.SetVector3(v);
                            }
                            break;
                        default:
                            {
                                GameLog.Error("Invalid data type.{0}", prop.Type);
                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    GameLog.Error($"���ñ�[{clsConfig.className}]����][{configID}]������[{name}]���ʹ���Message:{e.Message}");
                    throw;
                }
            }
            clsData.propMgr.SetString("Class", clsConfig.className);
            // clsData.propMgr.SetInt("ConfigID", configID);

            // ��ӵ������б�
            _classDataList.Add(configID, clsData);

            return true;
        }

        /// <summary>
        /// ���ַ���ת��Ϊָ����property�ṹ
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="prop"></param>
        public static void ConverToProperty(eDataType type, string name, string value, out Property prop)
        {
            prop = new Property(name, type);

            switch (type)
            {
                case eDataType.Bool:
                    {
                        prop.SetBool(ConvertUtility.BoolConvert(value));
                    }
                    break;

                case eDataType.Int:
                    {
                        prop.SetInt(ConvertUtility.IntConvert(value));
                    }
                    break;

                case eDataType.Long:
                    {
                        prop.SetLong(ConvertUtility.LongConvert(value));
                    }
                    break;

                case eDataType.ULong:
                    {
                        prop.SetULong(ConvertUtility.ULongConvert(value));
                    }
                    break;

                case eDataType.Float:
                    {
                        prop.SetFloat(ConvertUtility.FloatConvert(value));
                    }
                    break;

                case eDataType.Double:
                    {
                        prop.SetDouble(ConvertUtility.DoubleConvert(value));
                    }
                    break;

                case eDataType.String:
                    {
                        prop.SetString(value);
                    }
                    break;
                case eDataType.Vector2:
                    {
                        string[] arr = value.Split(',');
                        if (arr.Length != 2)
                        {
                            GameLog.Error("����vector2ʧ�ܣ���ʽ����{0}", value);
                            break;
                        }
                        Vector2 v = new Vector2(ConvertUtility.FloatConvert(arr[0]), ConvertUtility.FloatConvert(arr[1]));
                        prop.SetVector2(v);
                    }
                    break;

                case eDataType.Vector3:
                    {
                        string[] arr = value.Split(',');
                        if (arr.Length != 3)
                        {
                            GameLog.Error("����vector3ʧ�ܣ���ʽ����{0}", value);
                            break;
                        }
                        Vector3 v = new Vector3(ConvertUtility.FloatConvert(arr[0]), ConvertUtility.FloatConvert(arr[1]), ConvertUtility.FloatConvert(arr[2]));
                        prop.SetVector3(v);
                    }
                    break;
                default:
                    {
                        GameLog.Error("Invalid data type.{0}", prop.Type);
                        return;
                    }
            }

            return;
        } // end ConvertToProperty

    }
}