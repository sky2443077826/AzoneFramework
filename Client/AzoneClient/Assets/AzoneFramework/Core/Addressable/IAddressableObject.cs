namespace AzoneFramework
{
    /// <summary>
    /// 可寻址Object
    /// </summary>
    public interface IAddresableObect
    {
        public string Address { get; set; }

        /// <summary>
        /// 当被创建时
        /// </summary>
        /// <param name="address"></param>
        public void OnCreate(string address);

        /// <summary>
        /// 销毁实例
        /// </summary>
        public void DestoryInstance();
    }
}
