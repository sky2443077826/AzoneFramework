using AzoneFramework.Addressable;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AzoneFramework.Controller
{
    public class ControllerManager : Singleton<ControllerManager>
    {
        // 场景配置资产地址
        private static readonly string _CONTROLLER_PROFILE_ADDRESS = "ControllerProfile";

        // 控制器配置文件
        private ControllerProfile _profile;

        // 控制器字典
        private Dictionary<EControllerDefine, ControllerBase> _controllers;

        protected override void OnCreate()
        {
            base.OnCreate();

            _controllers = new Dictionary<EControllerDefine, ControllerBase>();

            _profile = AddressableLoader.Instance.InstantiateScriptableObject<ControllerProfile>(_CONTROLLER_PROFILE_ADDRESS);
            if (_profile == null)
            {
                GameLog.Error($"初始化ControllerManager失败！ ---> 未正确加载配置文件{_CONTROLLER_PROFILE_ADDRESS}。");
                return;
            }

        }

        protected override void OnDispose()
        {
            foreach (var controller in _controllers.Values)
            {
                controller?.OnDispose();
            }
            _controllers.Clear();
            _controllers = null;

            base.OnDispose();
        }

        /// <summary>
        /// 是否包含控制器
        /// </summary>
        /// <param name="define"></param>
        /// <returns></returns>
        public bool ContainController(EControllerDefine define)
        {
            if (!_profile.Contains(define))
            {
                return false;
            }

            return _controllers.ContainsKey(define);
        }

        /// <summary>
        /// 创建控制器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="define"></param>
        /// <returns></returns>
        public T CreateController<T>(EControllerDefine define) where T : ControllerBase
        {
            T controller = default;
            if (_controllers.ContainsKey(define))
            {
                GameLog.Error($"创建控制器失败！---> 已存在控制器【{define}】，无法重复创建。");
                return controller;
            }

            // 获取配置
            if (!_profile.TryGetConfig(define, out ControllerConfig config))
            {
                GameLog.Error($"创建控制器失败！---> 未定义的控制器【{define}】");
                return controller;
            }

            // 创建控制器
            Type scripType = Assembly.Load("Assembly-CSharp").GetType(config.scriptType);
            if (scripType == null)
            {
                GameLog.Error($"创建控制器失败！---> 未定义的控制器脚本【{config.scriptType}】");
                return controller;
            }
            controller = Activator.CreateInstance(scripType) as T;
            if (controller == null)
            {
                GameLog.Error($"创建控制器失败！---> 控制器定义与脚本不匹配：【{define}】,【{typeof(T).Name}】");
                return controller;
            }

            _controllers[define] = controller;
            return controller;
        }

        /// <summary>
        /// 获取控制器
        /// </summary>
        public T GetController<T>(EControllerDefine define) where T : ControllerBase
        {
            T controller = default;
            if (!_controllers.TryGetValue(define, out ControllerBase controllerBase))
            {
                GameLog.Error($"获取控制器失败！---> 未创建控制器【{define}】!");
                return controller;
            }

            controller = controllerBase as T;
            if (controller == null)
            {
                GameLog.Error($"获取控制器失败！---> 控制器定义和类型不匹配：【{define}】，【{typeof(T).Name}】!");
                return controller;
            }

            return controller;
        }

        /// <summary>
        /// 销毁控制器
        /// </summary>
        /// <param name="define"></param>
        public void DestoryController(EControllerDefine define)
        {
            if (!_controllers.ContainsKey(define))
            {
                return;
            }

            ControllerBase controllerBase = _controllers[define];
            controllerBase.OnDispose();
            _controllers.Remove(define);
            return;
        }
    }
}


