using System.Collections.Generic;

namespace AzoneFramework
{
    /// <summary>
    /// ���Թ�����
    /// </summary>
    public class PropertyManager : DataManager
    {
        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// ���л����Ե�����
        /// </summary>
        /// <returns></returns>
        public string SerializeTo()
        {
            return default;
        }

        /// <summary>
        /// �������ط�����
        /// </summary>
        /// <returns></returns>
        public bool ParseFrom()
        {
            return false;
        }

        /// <summary>
        /// �������Ե�����������
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public void CloneTo(ref PropertyManager other)
        {
            foreach (KeyValuePair<string, Property> item in _dataList)
            {
                Property property = new Property(item.Value);
                other.Add(item.Key, property);
            }
        }
    }
}
