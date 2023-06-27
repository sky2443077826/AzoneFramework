using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzoneFramework
{
    /// <summary>
    /// ����¼�
    /// </summary>
    public enum EFrameEventID
    {
        Invalid     = 0,

        /* �����¼� 1 - 99 */
        OnLoadSceneStart = 1,       //��ʼ���س���
        OnSceneLoading,             //����������
        OnSceneLoadEnd,             //�����������
        AfterSceneLoad,             //�������غ�

        /* UI�¼� 101 - 199 */
        UIModuleInitSuccess = 101,      // UIģ���ʼ���ɹ�
    }

    public class FrameEvent : EventDispatcher<EFrameEventID> { }
}
