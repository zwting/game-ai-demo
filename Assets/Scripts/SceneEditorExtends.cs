using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class GUIContentPool
{
    private static List<GUIContent> _pools = new List<GUIContent>();

    public static GUIContent Get(string content)
    {
        if (_pools.Count <= 0)
        {
            _pools.Add(new GUIContent());
        }

        var ret = _pools[_pools.Count - 1];
        _pools.Remove(ret);
        ret.text = content;
        return ret;
    }

    public static void Release(GUIContent guiContent)
    {
        if (!_pools.Contains(guiContent))
        {
            guiContent.text = default(string);
            guiContent.tooltip = default(string);
            _pools.Add(guiContent);
        }
    }

    public static void Cleanup()
    {
        _pools.Clear();
    }
}


public class SceneEditorExtends
{
    [DidReloadScripts]
    public static void Init()
    {
        

        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    private static bool _isExpandWin = true;
    private static RectTransform UIRoot = null;

    private static void GetUIRoot()
    {
        if (UIRoot != null)
        {
            return;
        }
        var curStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (curStage != null)
        {
            UIRoot = curStage.prefabContentsRoot.GetComponent<RectTransform>();
        }
        else
        {
            var go = GameObject.Find("UIRootCanvas");
            if (go != null)
            {
                UIRoot = go.GetComponent<RectTransform>();
            }
        }
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        DrawObjects();
    }


    private static void DrawObjects()
    {
        var selObjs = Selection.gameObjects;
        bool isHadUIRect = false;
        if (selObjs == null || selObjs.Length <= 0)
        {
        }

        GetUIRoot();

        if (UIRoot == null)
        {
            return;
        }

        Handles.BeginGUI();
        {
            float VEC2_WIDTH = 160;

            Vector2 size = new Vector2(0, 60);
            for (int i = 0; i < selObjs.Length; ++i)
            {
                if (selObjs[i].transform is RectTransform rectTransform)
                {
                    isHadUIRect = true;
                    var guiContent = GUIContentPool.Get(selObjs[i].name);
                    var s = GUI.skin.label.CalcSize(guiContent);
                    GUIContentPool.Release(guiContent);
                    if (s.x + VEC2_WIDTH > size.x)
                    {
                        size.x = s.x + VEC2_WIDTH;
                    }

                    size.y += s.y;
                }
            }

            size.x += 16;
            size.x = Mathf.Max(200, size.x);

            var prevColor = GUI.color;
            GUI.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            GUILayout.BeginArea(new Rect(5, 5, size.x, size.y));
            {
                _isExpandWin = GUILayout.Toggle(_isExpandWin, "选中物体的坐标", (GUIStyle) "ShurikenModuleTitle");
                if (_isExpandWin)
                {
                    GUI.color = prevColor;
                    GUILayout.BeginArea(new Rect(0, 16, size.x, size.y - 16), GUIContent.none, "Badge");
                    {
                        GUILayout.BeginVertical();
                        {
                            if (isHadUIRect)
                            {
                                for (int i = 0; i < selObjs.Length; ++i)
                                {
                                    if (selObjs[i].transform is RectTransform rectTransform)
                                    {
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label(selObjs[i].name, GUILayout.ExpandWidth(false));
                                        var pos = RectTransformPosToLTPos(rectTransform);
                                        var pos2 = EditorGUILayout.Vector2Field(GUIContent.none,HandlePrecision(pos),
                                            GUILayout.Width(VEC2_WIDTH));
                                        var delta = pos2 - pos;
                                        if (delta != Vector2.zero)
                                        {
                                            delta.y *= -1;
                                            rectTransform.anchoredPosition += delta;
                                        }

                                        GUILayout.EndHorizontal();
                                    }
                                }
                            }
                            else
                            {
                                GUILayout.Label("请选中一个UI对象", "AM MixerHeader");
                            }

                            GUILayout.EndVertical();
                        }
                        GUILayout.EndArea();
                    }
                }

                GUILayout.EndArea();
            }
            Handles.EndGUI();
            GUI.color = prevColor;
        }
    }

    private static Vector2 HandlePrecision(Vector2 v)
    {
        string strX = v.x.ToString("F3");
        string strY = v.y.ToString("F3");

        Vector2 ret = v;

        if (float.TryParse(strX, out var outX))
        {
            ret.x = outX;
        }

        if (float.TryParse(strY, out var outY))
        {
            ret.y = outY;
        }

        return ret;

    }

    private static Vector2 RectTransformPosToLTPos(RectTransform r)
    {
        var pos = new Vector2(r.rect.xMin, r.rect.yMax);
        var offset = new Vector2(-UIRoot.sizeDelta.x * 0.5f, UIRoot.sizeDelta.y * 0.5f);
        var worldPos = r.TransformPoint(pos);
        pos = UIRoot.InverseTransformPoint(worldPos);
        var ret = Vector2.zero;
        ret.x = pos.x - offset.x;
        ret.y = -pos.y + offset.y;

        return ret;
    }

    private static Vector2 LTPosToRectTransformPos(RectTransform r, Vector2 p)
    {
        var pos = Vector2.zero;
        var offset = new Vector2(-UIRoot.sizeDelta.x * 0.5f, UIRoot.sizeDelta.y * 0.5f);

        pos.x = p.x + offset.x;
        pos.y = offset.y - p.y;

        var worldPos = UIRoot.TransformPoint(pos);
        pos = r.InverseTransformPoint(worldPos);
        pos.x += r.sizeDelta.x * r.pivot.x;
        pos.y -= r.sizeDelta.y * (1 - r.pivot.y);
        return pos;
    }
}