using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 图片倾斜效果组件，目前只支持Image组件，
/// </summary>
public class UISkewEffect: BaseMeshEffect
{
    public float skewDis = 0;

    public override void ModifyMesh(VertexHelper vh)
    {
        List<UIVertex> verts = new List<UIVertex>();
        vh.GetUIVertexStream(verts);

        for (int i = 0; i < verts.Count; i += 6)
        {
            if (skewDis > 0)
            {
                verts[i + 1] = SkewUIVertex(verts[i + 1], skewDis);
                verts[i + 2] = SkewUIVertex(verts[i + 2], skewDis);
                verts[i + 3] = SkewUIVertex(verts[i + 3], skewDis);
            }
            else
            {
                verts[i + 0] = SkewUIVertex(verts[i + 0], -skewDis);
                verts[i + 4] = SkewUIVertex(verts[i + 4], -skewDis);
                verts[i + 5] = SkewUIVertex(verts[i + 5], -skewDis);
            }
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(verts);
    }

    private UIVertex SkewUIVertex(UIVertex vert, float dis)
    {
        var newP = vert.position;
        newP.x = vert.position.x + dis;
        vert.position = newP;
        return vert;
    }
}