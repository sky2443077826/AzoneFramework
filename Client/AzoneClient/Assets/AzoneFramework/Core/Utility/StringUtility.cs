using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AzoneFramework
{
    public class StringUtility
    {
        /// <summary>
        /// �����ַ���
        /// </summary>
        public static StringBuilder globalBuilder = new StringBuilder(1000);

        // ���ʲ���ַ��ʽ
        private static readonly string ADDRESSABLE_SUB_ASSET_FORMAT = "{0}[{1}]";

        #region ͨ�ù���

        /// <summary>
        /// ��ʽ��
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

        /// <summary>
        /// ƴ��
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Concat(params object[] args)
        {
            globalBuilder.Clear();
            if (args == null)
            {
                return string.Empty;
            }
            for (int i = 0; i < args.Length; i++)
            {
                globalBuilder.Append(args[i]);
            }
            return globalBuilder.ToString();
        }

        #endregion


        /// <summary>
        /// ��ȡ���ʲ���ַ
        /// </summary>
        /// <param name="address">���ʲ���ַ</param>
        /// <param name="subName">���ʲ�����</param>
        /// <returns></returns>
        public static string GetSubAssetAddress(string address, string subName)
        {
            return Format(ADDRESSABLE_SUB_ASSET_FORMAT, address, subName);
        }
    }
}
