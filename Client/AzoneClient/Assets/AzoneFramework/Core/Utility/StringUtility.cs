using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AzoneFramework
{
    public class StringUtility
    {
        /// <summary>
        /// 共享字符串
        /// </summary>
        public static StringBuilder globalBuilder = new StringBuilder(1000);

        // 子资产地址格式
        private static readonly string ADDRESSABLE_SUB_ASSET_FORMAT = "{0}[{1}]";

        #region 通用工具

        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="target"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static string Format(string target, params object[] objs)
        {
            globalBuilder.Clear();
            string value = globalBuilder.AppendFormat(target, objs).ToString();
            return value;
        }

        #endregion


        /// <summary>
        /// 获取子资产地址
        /// </summary>
        /// <param name="address">父资产地址</param>
        /// <param name="subName">子资产名称</param>
        /// <returns></returns>
        public static string GetSubAssetAddress(string address, string subName)
        {
            return Format(ADDRESSABLE_SUB_ASSET_FORMAT, address, subName);
        }
    }
}
