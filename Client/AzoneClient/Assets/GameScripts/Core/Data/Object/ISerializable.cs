using System.Xml;

namespace AzoneFramework
{
    /// <summary>
    /// �������л����ļ��Ķ���Ľӿ�
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// ���л���xml
        /// </summary>
        /// <returns></returns>
        bool SerializeToXml(XmlElement root);

        /// <summary>
        /// ���л�����
        /// </summary>
        /// <param name=""></param>
        bool SerializeProperty(XmlElement node);

        /// <summary>
        /// ���л����
        /// </summary>
        /// <param name="node"></param>
        bool SerializeRecord(XmlElement node);

        /// <summary>
        /// ���л��Ӷ���
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool SerializeChildren(XmlElement node);

        /// <summary>
        /// ���ⲿ����
        /// </summary>
        /// <returns></returns>
        bool ParseFrom(string data);

        /// <summary>
        /// ���л�����
        /// </summary>
        /// <param name=""></param>
        bool ParsePropertyFromXML(XmlNode node);

        /// <summary>
        /// ���л����
        /// </summary>
        /// <param name="node"></param>
        bool ParseRecordFromXML(XmlNode node);

        /// <summary>
        /// ���л��Ӷ���
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool ParseChildrenFromXML(XmlNode node);
    }
}

