using System.Xml;

namespace AzoneFramework
{
    /// <summary>
    /// 可以序列化到文件的对象的接口
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// 序列化到xml
        /// </summary>
        /// <returns></returns>
        bool SerializeToXml(XmlElement root);

        /// <summary>
        /// 序列化属性
        /// </summary>
        /// <param name=""></param>
        bool SerializeProperty(XmlElement node);

        /// <summary>
        /// 序列化表格
        /// </summary>
        /// <param name="node"></param>
        bool SerializeRecord(XmlElement node);

        /// <summary>
        /// 序列化子对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool SerializeChildren(XmlElement node);

        /// <summary>
        /// 从外部解析
        /// </summary>
        /// <returns></returns>
        bool ParseFrom(string data);

        /// <summary>
        /// 序列化属性
        /// </summary>
        /// <param name=""></param>
        bool ParsePropertyFromXML(XmlNode node);

        /// <summary>
        /// 序列化表格
        /// </summary>
        /// <param name="node"></param>
        bool ParseRecordFromXML(XmlNode node);

        /// <summary>
        /// 序列化子对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool ParseChildrenFromXML(XmlNode node);
    }
}

