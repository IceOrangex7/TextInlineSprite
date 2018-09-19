using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EmojiUI
{
    public class EmojiRenderRect
    {
        public static void FillMesh(Graphic graphic, VertexHelper vh, UnitMeshInfo rendermesh)
        {
            if (rendermesh != null && rendermesh.getTexture() != null)
            {
                int vertcnt = rendermesh.VertCnt();
                int uvcnt = rendermesh.UVCnt();
                if (vertcnt != uvcnt)
                {
                    Debug.LogError("data error");
                }
                else
                {
                    for (int i = 0; i < vertcnt; ++i)
                    {
                        vh.AddVert(rendermesh.GetVert(i), graphic.color, rendermesh.GetUV(i));
                    }

                    int cnt = vertcnt / 4;
                    for (int i = 0; i < cnt; ++i)
                    {
                        int m = i * 4;

                        vh.AddTriangle(m, m + 1, m + 2);
                        vh.AddTriangle(m + 2, m + 3, m);
                    }

                    //vh.AddTriangle(0, 1, 2);
                    //vh.AddTriangle(2, 3, 0);
                }
            }
        }

    }

}

