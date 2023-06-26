using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// 协程等待类
    /// </summary>
    public class Yielder
    {
        /// <summary>
        /// 帧末尾
        /// </summary>
        public static WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

        /// <summary>
        /// 固定帧
        /// </summary>
        public static WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();

        private static Dictionary<float, WaitForSeconds> _secondCache = new Dictionary<float, WaitForSeconds>();

        private static Dictionary<float, WaitForSecondsRealtime> _secondRealtimeCache = new Dictionary<float, WaitForSecondsRealtime>();
        
        /// <summary>
        /// 获取等待时间
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
        /// 获取等待（真实）时间
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
