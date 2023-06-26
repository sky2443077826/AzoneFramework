using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// Э�̵ȴ���
    /// </summary>
    public class Yielder
    {
        /// <summary>
        /// ֡ĩβ
        /// </summary>
        public static WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

        /// <summary>
        /// �̶�֡
        /// </summary>
        public static WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();

        private static Dictionary<float, WaitForSeconds> _secondCache = new Dictionary<float, WaitForSeconds>();

        private static Dictionary<float, WaitForSecondsRealtime> _secondRealtimeCache = new Dictionary<float, WaitForSecondsRealtime>();
        
        /// <summary>
        /// ��ȡ�ȴ�ʱ��
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static WaitForSeconds GetSecond(float second)
        {
            if (!_secondCache.TryGetValue(second, out WaitForSeconds value))
            {
                value = new WaitForSeconds(second);
                _secondCache[second] = value;
            }

            return value;
        }

        /// <summary>
        /// ��ȡ�ȴ�����ʵ��ʱ��
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static WaitForSecondsRealtime GetSecondRealtime(float second)
        {
            if (!_secondRealtimeCache.TryGetValue(second, out WaitForSecondsRealtime value))
            {
                value = new WaitForSecondsRealtime(second);
                _secondRealtimeCache[second] = value;
            }

            return value;
        }
    }
}
