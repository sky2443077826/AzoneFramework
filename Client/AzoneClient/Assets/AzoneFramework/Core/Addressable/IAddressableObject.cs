namespace AzoneFramework
{
    /// <summary>
    /// ��ѰַObject
    /// </summary>
    public interface IAddresableObect
    {
        public string Address { get; set; }

        /// <summary>
        /// ��������ʱ
        /// </summary>
        /// <param name="address"></param>
        public void OnCreate(string address);

        /// <summary>
        /// ����ʵ��
        /// </summary>
        public void DestoryInstance();
    }
}
