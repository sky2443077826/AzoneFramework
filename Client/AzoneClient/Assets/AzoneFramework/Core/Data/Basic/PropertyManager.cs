using System.Collections.Generic;

namespace AzoneFramework
{
    /// <summary>
    /// 属性管理器
    /// </summary>
    public class PropertyManager : DataManager
    {
        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// 序列化属性到本地
        /// </summary>
        /// <returns></returns>
        public string SerializeTo()
        {
            return default;
        }

        /// <summary>
        /// 从其他地方加载
        /// </summary>
        /// <returns></returns>
        public bool ParseFrom()
        {
            return false;
        }

        /// <summary>
        /// 拷贝属性到其他管理器
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
