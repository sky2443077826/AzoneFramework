using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI工具类
/// </summary>
public class UIUtility
{
    /// <summary>
    /// 设置RecTransform组件的锚点
    /// </summary>
    /// <param name="recTransform"></param>
    /// <param name="textAnchor"></param>
    public static void SetRecTransformAnchor(RectTransform recTransform, TextAnchor textAnchor)
    {
        if (recTransform == null)
        {
            return;
        }

        switch (textAnchor)
        {
            case TextAnchor.UpperLeft:
                recTransform.anchorMax = new Vector2(0, 1);
                recTransform.anchorMin = new Vector2(0, 1);
                break;
            case TextAnchor.UpperCenter:
                recTransform.anchorMax = new Vector2(0.5f, 1);
                recTransform.anchorMin = new Vector2(0.5f, 1);
                break;
            case TextAnchor.UpperRight:
                recTransform.anchorMax = new Vector2(0, 0.5f);
                recTransform.anchorMin = new Vector2(0, 0.5f);
                break;
            case TextAnchor.MiddleLeft:
                recTransform.anchorMax = new Vector2(0.5f, 0.5f);
                recTransform.anchorMin = new Vector2(0.5f, 0.5f);
                break;
            case TextAnchor.MiddleCenter:
                recTransform.anchorMax = new Vector2(0.5f, 0.5f);
                recTransform.anchorMin = new Vector2(0.5f, 0.5f);
                break;
            case TextAnchor.MiddleRight:
                recTransform.anchorMax = new Vector2(1, 0.5f);
                recTransform.anchorMin = new Vector2(1, 0.5f);
                break;
            case TextAnchor.LowerLeft:
                recTransform.anchorMax = new Vector2(0, 0);
                recTransform.anchorMin = new Vector2(0, 0);
                break;
            case TextAnchor.LowerCenter:
                recTransform.anchorMax = new Vector2(0.5f, 0);
                recTransform.anchorMin = new Vector2(0.5f, 0);
                break;
            case TextAnchor.LowerRight:
                recTransform.anchorMax = new Vector2(1, 0);
                recTransform.anchorMin = new Vector2(1, 0);
                break;
        }
    }
}
