using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public struct LineInfo
{
    public Bounds bounds;
    public int vertexCount;

    public LineInfo(Bounds bounds, int lineCharCount)
    {
        this.bounds = bounds;
        vertexCount = lineCharCount * 6;
    }
}

[RequireComponent(typeof(Text))]
public class UnderlineText : BaseMeshEffect
{
    public float Padding = 1.0f;
    public float LineWidth = 1.0f;
    public bool IsShowBox = true;
    public Color LineColor = Color.white;
    public float lineHeight = 18f;

    List<LineInfo> lines = new List<LineInfo>();

    private Text _text;


    public override void ModifyMesh(VertexHelper vh)
    {
        CalcLineBounds(vh);
        CalcAndApplyLineHeight(vh);
#if UNITY_EDITOR
        if (IsShowBox)
        {
            CalcLineBounds(vh);
        }
#endif
    }


    /// <summary>
    /// 计算并应用行高
    /// </summary>
    /// <param name="vh"></param>
    public void CalcAndApplyLineHeight(VertexHelper vh)
    {
        //计算行起始值
        float offsetY = 0;
        if (lines.Count > 0)
        {
            offsetY = lines[0].bounds.center.y;
        }
        else
        {
            return;
        }

        List<UIVertex> verts = new List<UIVertex>();
        vh.GetUIVertexStream(verts);
        int vertexIdx = 0;
        List<UIVertex> newVerts = new List<UIVertex>();
        for (int i = 0; i < lines.Count; ++i)
        {
            float offsetLine = -offsetY + lineHeight * i + lines[i].bounds.center.y;
            for (int j = 0; j < lines[i].vertexCount; ++j, vertexIdx++)
            {
                UIVertex vert = verts[vertexIdx];
                Vector3 pos = verts[vertexIdx].position;
                pos.y += offsetLine;
                vert.position = pos;
                newVerts.Add(vert);
            }
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(newVerts);
    }

    /// <summary>
    /// 通过判断面积是否大于0来决定bounds是否是有效的
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    private bool IsBoundValid(Bounds bounds)
    {
        return bounds.size.x * bounds.size.y > 0;
    }

    /// <summary>
    /// 计算每一行的包围盒列表
    /// </summary>
    /// <param name="vh"></param>
    private void CalcLineBounds(VertexHelper vh)
    {
        _text = GetComponent<Text>();
        lines.Clear();
        List<UIVertex> verts = new List<UIVertex>();
        vh.GetUIVertexStream(verts);

        Vector2 prevPos = Vector2.one * float.MinValue;
        Bounds lineBounds = new Bounds(Vector3.zero, Vector3.zero);
        bool isNewBound = true;
        int lineFontCount = 0;
        UnityEngine.Debug.Log("count=" + verts.Count);
        for (int i = 0; i < verts.Count; i += 6)
        {
            var lt = verts[i].position;
            var rb = verts[i + 2].position;

            if (isNewBound)
            {
                prevPos = lt;
                lineFontCount = 0;
                lineBounds = new Bounds(Vector3.zero, Vector3.zero);
                lineBounds.center = (lt + rb) / 2.0f;
                lineBounds.size = new Vector2(Mathf.Abs(lt.x - rb.x),
                    Mathf.Abs(lt.y - rb.y));
                isNewBound = false;
            }

            if (lt.x < prevPos.x && lt.y < prevPos.y) //新行出现了
            {
                if (IsBoundValid(lineBounds))
                {
                    lines.Add(new LineInfo(lineBounds, lineFontCount));
                }

                prevPos = lt;
                lineFontCount = 0;
                lineBounds = new Bounds(Vector3.zero, Vector3.zero);
                lineBounds.center = (lt + rb) / 2.0f;
                lineBounds.size = new Vector2(Mathf.Abs(lt.x - rb.x),
                    Mathf.Abs(lt.y - rb.y));

                isNewBound = true;
            }
            else
            {
                lineBounds.Encapsulate(lt);
                lineBounds.Encapsulate(rb);
                prevPos = lt;
            }
            lineFontCount++;
        }

        if (IsBoundValid(lineBounds))
        {
            lines.Add(new LineInfo(lineBounds, lineFontCount));
        }

        DisplayBoxSize();

        Debug.Log("bounds Count=" + lines.Count);
    }

    private void DisplayBoxSize()
    {
        string str = "";
        float aveHeight = 0;
        for (int i = 0; i < lines.Count; ++i)
        {
            aveHeight += lines[i].bounds.size.y;
            str += lines[i].bounds.size.ToString() + "\n";
        }

        str += (aveHeight / lines.Count).ToString() + "\n";

        UnityEngine.Debug.Log(str);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!IsShowBox) return;

        for (int i = 0; i < lines.Count; ++i)
        {
            var lineInfo = lines[i];
            Handles.color = Color.green;
            var a = transform.TransformPoint(lineInfo.bounds.min);
            var b = transform.TransformPoint(lineInfo.bounds.max);
            var size = b - a;
            Handles.DrawLine(a, a + Vector3.up * size.y);
            Handles.DrawLine(a + Vector3.up * size.y, b);
            Handles.DrawLine(b, b + Vector3.down * size.y);
            Handles.DrawLine(b + Vector3.down * size.y, a);
        }
    }
#endif
}