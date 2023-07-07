using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// 配置常量
    /// </summary>
    internal class ConfigConstant
    {
        // config转换为type的位偏移
        public const int kConfig2TypeBase = 10000;

        /// <summary>
        /// 配置表结构描述文件
        /// </summary>
        public const string kLogicClassCfg = "LogicClass.xml";

        /// <summary>
        /// 配置类定义
        /// </summary>
        public const string kStructPath = "Class/";

        /// <summary>
        /// 配置类数据
        /// </summary>
        public const string kDataPath = "Data/";
    }

    /// <summary>
    /// 类型配置结构
    /// </summary>
    internal class ClassConfig
    {
        /// <summary>
        /// 类名
        /// </summary>
        public string className;

        /// <summary>
        /// 父类数据
        /// </summary>
        public ClassConfig parent;

        /// <summary>
        /// 类型数据定义
        /// </summary>
        public string clsDataPath;

        /// <summary>
        /// 包含文件
        /// </summary>
        public List<string> incFiles;

        /// <summary>
        /// 当前类型包含的ID
        /// </summary>
        public List<int> IDs;

        /// <summary>
        /// 属性管理器
        /// </summary>
        public PropertyManager propMgr;

        /// <summary>
        /// 表格管理器
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
    /// 数据结构
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
    /// 游戏配置加载模块
    /// </summary>
    public class ConfigManager : Singleton<ConfigManager>
    {
        // 类型配置
        // key: 类型名 value: 类型数据
        private Dictionary<string, ClassConfig> _classConfigList;
        // 数据配置
        // key: 配置ID value: 配置数据
        private Dictionary<int, ClassData> _classDataList;

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnCreate()
        {
            _classConfigList = new Dictionary<string, ClassConfig>();
            _classDataList = new Dictionary<int, ClassData>();

            //// 加载配置
            LoadClassConfig();
            //// 加载数据
            LoadClassData();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }

        /// <summary>
        /// 根据类型获取所有的类型下面的ID
        /// </summary>
        /// <param name="className">类型</param>
        /// <param name="IDs">返回ID列表</param>
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
        /// 根据类型名获取类结构
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
        /// 根据类型获取表格结构
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
        /// 是否存在配置类
        /// </summary>
        /// <param name="cls">配置类名称</param>
        /// <returns>存在返回true，否则返回false</returns>
        public bool HasClass(string cls)
        {
            return _classConfigList.ContainsKey(cls);
        }

        /// <summary>
        /// 是否存在配置ID
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <returns>存在返回true，否则返回false</returns>
        public bool HasConfig(int id)
        {
            return _classDataList.ContainsKey(id);
        }

        /// <summary>
        /// 根据配置ID获取类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public eObjectType GetType(int id)
        {
            return (eObjectType)(id / ConfigConstant.kConfig2TypeBase);
        }

        /// <summary>
        /// 拷贝属性到指定管理器
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
        /// 根据ID获取默认配置值
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <param name="name">属性名</param>
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
        /// 根据ID获取默认配置值
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <param name="name">属性名</param>
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
        /// 根据ID获取默认配置值
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <param name="name">属性名</param>
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
        /// 根据ID获取默认配置值
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <param name="name">属性名</param>
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
        /// 根据ID获取默认配置值
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <param name="name">属性名</param>
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
        /// 根据ID获取默认配置值
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <param name="name">属性名</param>
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
        /// 根据ID获取默认配置值
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <param name="name">属性名</param>
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
        /// 加载配置表
        /// </summary>
        private void LoadClassConfig()
        {
            string classMapPath = StringUtility.Concat(ApplicationPath.configPath, ConfigConstant.kLogicClassCfg);
            XmlDocument doc = new XmlDocument();
            doc.Load(classMapPath);

            // 解析类型结构
            //             
            //             XmlNodeList allNodeList = doc.GetElementsByTagName("XML");
            //             if (allNodeList.Count == 0)
            //             {
            //                 return;
            //             }

            // 第一个元素
            XmlElement rootNode = doc.DocumentElement;
            // 解析根节点下面所有的类型
            foreach (XmlNode node in rootNode.ChildNodes)
            {
                ClassConfig cls = null;
                LoadClassConfig(node, ref cls);
            }
        }

        /// <summary>
        /// 加载逻辑类配置
        /// </summary>
        /// <param name="classNode">逻辑类根节点</param>
        private void LoadClassConfig(XmlNode classNode, ref ClassConfig cls)
        {
            // 类结构文件
            if (classNode.Attributes["Struct"] == null)
            {
                GameLog.Error("{0} 结构定义出错", ConfigConstant.kLogicClassCfg);
                return;
            }

            // 类型名
            if (classNode.Attributes["ID"] == null)
            {
                GameLog.Error("Class Name 属性为空。解析类型失败。{0}", ConfigConstant.kLogicClassCfg);
                return;
            }

            // 类型名
            string clsName = classNode.Attributes["ID"].Value;
            // 结构路径
            string structPath = classNode.Attributes["Struct"].Value;

            if (string.IsNullOrEmpty(structPath))
            {
                GameLog.Error("Struct 属性为空。解析类型失败。");
                return;
            }

            if (_classConfigList.ContainsKey(clsName))
            {
                GameLog.Error("已经包含{0}，请检查配置。", clsName);
                return;
            }

            // 分配类型结构
            ClassConfig clsConfig = new ClassConfig(clsName, cls);

            // 数据路径
            if (classNode.Attributes["Data"] != null)
            {
                // 设置类型对应的数据
                clsConfig.clsDataPath = classNode.Attributes["Data"].Value;
            }

            // 添加类配置数据
            if (!AddClass(structPath, ref clsConfig))
            {
                GameLog.Error("{0}:添加类结构失败。", clsName);
                return;
            }

            // 加载子节点
            foreach (XmlNode node in classNode.ChildNodes)
            {
                LoadClassConfig(node, ref clsConfig);
            }

            // 添加到配置
            _classConfigList.Add(clsName, clsConfig);
        }


        /// <summary>
        /// 加载指定类型的属性以及表格结构
        /// </summary>
        /// <param name="structPath">结构配置路径</param>
        /// <param name="clsConfig">结构类</param>
        /// <returns></returns>
        private bool AddClass(string structPath, ref ClassConfig clsConfig)
        {
            if (clsConfig == null || string.IsNullOrEmpty(structPath))
            {
                return false;
            }

            // 获取父类
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
        /// 加载包含文件
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
            //             // 获取第一个元素
            //             XmlNode root = allNodeList[0];

            // 第一个元素
            XmlElement root = doc.DocumentElement;

            // 解析根节点下面所有的类型
            // 属性节点
            XmlNode propertyNode = root["Property"];
            if (propertyNode != null)
            {
                AddProperty(propertyNode, ref clsConfig);
            }

            // 表格节点
            XmlNode recordNode = root["Record"];
            if (recordNode != null)
            {
                AddRecord(recordNode, ref clsConfig);
            }

            // 包含文件
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
        /// 添加属性
        /// </summary>
        /// <param name="rootNode">属性节点</param>
        /// <param name="clsConfig">属性对应的类结构</param>
        /// <returns>成功返回true，否则返回false</returns>
        private bool AddProperty(XmlNode rootNode, ref ClassConfig clsConfig)
        {
            foreach (XmlNode node in rootNode)
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                // 属性名
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

                // 转化类型
                eDataType type = DataUtility.Str2DataType(strType);
                if (type == eDataType.Invalid)
                {
                    continue;
                }
                Property prop = new Property(name, type);

                // 设置是否保存标记
                if (node.Attributes["Save"] != null)
                {
                    // 有保存标记
                    prop.Save = XmlConvert.ToInt32(node.Attributes["Save"].Value) != 0;
                }

                // 是否同步标记
                if (node.Attributes["Sync"] != null)
                {
                    // 有保存标记
                    prop.Sync = XmlConvert.ToInt32(node.Attributes["Sync"].Value) != 0;
                }

                // 是否共享标记
                if (node.Attributes["Shared"] != null)
                {
                    // 有保存标记
                    prop.Shared = XmlConvert.ToInt32(node.Attributes["Shared"].Value) != 0;
                }

                // 添加到属性管理器
                if (!clsConfig.propMgr.Add(name, prop))
                {
                    GameLog.Error("Add property failed. {0} - {1}.", name, strType);
                    continue;
                }
            }

            return true;
        }

        /// <summary>
        /// 添加表格
        /// </summary>
        /// <param name="rootNode">表格节点</param>
        /// <param name="clsConfig">表格对应的类结构</param>
        /// <returns>成功返回true，否则返回false</returns>
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
                    GameLog.Error($"Error record name. {clsConfig.className}！");
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

                // 处理行数据
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
                    // 转化类型
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

                // 添加到表格管理器
                if (!clsConfig.recMgr.Add(recName, record))
                {
                    GameLog.Error("Add record failed. {0} - {1}.", record);
                    continue;
                }
            }
            return true;
        }

        /// <summary>
        /// 加载类数据
        /// </summary>
        private void LoadClassData()
        {
            foreach (var iter in _classConfigList)
            {
                // 类名
                string clsName = iter.Key;

                // 类结构
                ClassConfig clsConfig = iter.Value;
                if (string.IsNullOrEmpty(clsName) || clsConfig == null)
                {
                    continue;
                }

                // 数据路径
                string dataPath = clsConfig.clsDataPath;
                if (string.IsNullOrEmpty(dataPath))
                {
                    continue;
                }

                // 加载文件
                string path = StringUtility.Concat(ApplicationPath.configPath, ConfigConstant.kDataPath, dataPath);
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                // 
                //                 XmlNodeList allNodeList = doc.GetElementsByTagName("XML");
                //                 if (allNodeList.Count == 0)
                //                 {
                //                     continue;
                //                 }

                // 获取第一个元素
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
        /// 加载数据
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="clsConfig">配置</param>
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

            // 创建一条数据
            ClassData clsData = new ClassData();
            clsConfig.IDs.Add(configID);

            // 属性管理器
            clsConfig.propMgr.CloneTo(ref clsData.propMgr);

            // 表格管理器
            clsConfig.recMgr.CloneTo(ref clsData.recMgr);

            // 解析属性
            foreach (XmlAttribute attribute in node.Attributes)
            {
                // 属性名
                string name = attribute.Name;
                // 属性值
                string value = attribute.Value;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
                {
                    continue;
                }

                if (!clsData.propMgr.Find(name))
                {
                    // GameLog.Error("没有找到属性，初始化属性值失败。{0}:{1}", configID, name);
                    continue;
                }

                Property prop = clsData.propMgr.GetProperty(name);
                if (prop == null)
                {
                    GameLog.Error("没有找到属性，获取属性结构失败。{0}:{1}", configID, name);
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
                                    GameLog.Error("加载vector2失败，格式错误。{0}", value);
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
                                    GameLog.Error("加载vector3失败，格式错误。{0}", value);
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
                    GameLog.Error($"配置表[{clsConfig.className}]数据][{configID}]的属性[{name}]类型错误！Message:{e.Message}");
                    throw;
                }
            }
            clsData.propMgr.SetString("Class", clsConfig.className);
            // clsData.propMgr.SetInt("ConfigID", configID);

            // 添加到配置列表
            _classDataList.Add(configID, clsData);

            return true;
        }

        /// <summary>
        /// 将字符串转换为指定的property结构
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
                            GameLog.Error("加载vector2失败，格式错误。{0}", value);
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
                            GameLog.Error("加载vector3失败，格式错误。{0}", value);
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