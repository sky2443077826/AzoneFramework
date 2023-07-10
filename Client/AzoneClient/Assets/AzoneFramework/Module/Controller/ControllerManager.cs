using AzoneFramework.Addressable;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AzoneFramework.Controller
{
    public class ControllerManager : Singleton<ControllerManager>
    {
        // ���������ʲ���ַ
        private static readonly string _CONTROLLER_PROFILE_ADDRESS = "ControllerProfile";

        // �����������ļ�
        private ControllerProfile _profile;

        // �������ֵ�
        private Dictionary<EControllerDefine, ControllerBase> _controllers;

        protected override void OnCreate()
        {
            base.OnCreate();

            _controllers = new Dictionary<EControllerDefine, ControllerBase>();

            _profile = AddressableLoader.Instance.InstantiateScriptableObject<ControllerProfile>(_CONTROLLER_PROFILE_ADDRESS);
            if (_profile == null)
            {
                GameLog.Error($"��ʼ��ControllerManagerʧ�ܣ� ---> δ��ȷ���������ļ�{_CONTROLLER_PROFILE_ADDRESS}��");
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
        /// �Ƿ����������
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
        /// ����������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="define"></param>
        /// <returns></returns>
        public T CreateController<T>(EControllerDefine define) where T : ControllerBase
        {
            T controller = default;
            if (_controllers.ContainsKey(define))
            {
                GameLog.Error($"����������ʧ�ܣ�---> �Ѵ��ڿ�������{define}�����޷��ظ�������");
                return controller;
            }

            // ��ȡ����
            if (!_profile.TryGetConfig(define, out ControllerConfig config))
            {
                GameLog.Error($"����������ʧ�ܣ�---> δ����Ŀ�������{define}��");
                return controller;
            }

            // ����������
            Type scripType = Assembly.Load("Assembly-CSharp").GetType(config.scriptType);
            if (scripType == null)
            {
                GameLog.Error($"����������ʧ�ܣ�---> δ����Ŀ������ű���{config.scriptType}��");
                return controller;
            }
            controller = Activator.CreateInstance(scripType) as T;
            if (controller == null)
            {
                GameLog.Error($"����������ʧ�ܣ�---> ������������ű���ƥ�䣺��{define}��,��{typeof(T).Name}��");
                return controller;
            }

            _controllers[define] = controller;
            return controller;
        }

        /// <summary>
        /// ��ȡ������
        /// </summary>
        public T GetController<T>(EControllerDefine define) where T : ControllerBase
        {
            T controller = default;
            if (!_controllers.TryGetValue(define, out ControllerBase controllerBase))
            {
                GameLog.Error($"��ȡ������ʧ�ܣ�---> δ������������{define}��!");
                return controller;
            }

            controller = controllerBase as T;
            if (controller == null)
            {
                GameLog.Error($"��ȡ������ʧ�ܣ�---> ��������������Ͳ�ƥ�䣺��{define}������{typeof(T).Name}��!");
                return controller;
            }

            return controller;
        }

        /// <summary>
        /// ���ٿ�����
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


