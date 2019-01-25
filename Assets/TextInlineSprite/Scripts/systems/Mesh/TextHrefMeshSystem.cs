using UnityEngine;
using System.Collections;

namespace EmojiUI
{
    public class TextHrefMeshSystem : IEmojiStructSystem<TextCotext>
    {
        private const float HrefLine = 1f;
        public struct HrefMesh
        {
            public EmojiList<EmojiUIVertex> HrefVerts;

            public void Init()
            {
                if (HrefVerts == null)
                {
                    HrefVerts = new EmojiList<EmojiUIVertex>();
                }
                else
                {
                    HrefVerts.Clear();
                }
            }
        }

        public int DoStep(ref TextCotext context, EmojiEvent uievent)
        {
            if (uievent == EmojiEvent.PreRender)
            {
                for (int i = 0; i < context.Results.Count; i++)
                {
                    var result = context.Results[i];
                    var text = context.AllTexts[i];
                    if(text == null)
                        continue;

                    var hrefmesh = result.MeshGroup.HrefMeshReult;
                    hrefmesh.Init();

                    for (int k = 0; k < result.MeshGroup.TextMeshResult.EmojiVerts.Count; k += 4)
                    {
                        int emojiindex = k / 4;
                        int arrayindex;
                        int emojitype = result.ParseGroup.GetIndexType(emojiindex, out arrayindex);

                        if (emojitype == 2 && arrayindex >= 0 && arrayindex < result.ParseGroup.HrefResult.Count)
                        {
                            var hrefcolor = result.ParseGroup.HrefResult.GetHrefCols()[arrayindex];

                            var p2 = result.MeshGroup.TextMeshResult.EmojiVerts[k+2];
                            var p3 = result.MeshGroup.TextMeshResult.EmojiVerts[k+3];
                            var width = p2.x - p3.x;
                            //
                            var botmid = (p3 + p2) / 2;

                            for (int m = 0; m < 4; m++)
                            {
                                var uivert = new EmojiUIVertex();
                                uivert.color = hrefcolor;
                                uivert.normal = Vector3.back;
                                uivert.tangent = new Vector4(1.0f, 0.0f, 0.0f, -1.0f);

                                float wc = ((m & 2) >> 1) == (m & 1) ? -0.5f : 0.5f;
                                float hc = (m & 2) == 0 ? -0.5f : 0.5f;
                                var pos = botmid + new Vector3(wc * width, (hc * HrefLine * text.fontSize) /10);//result.MeshGroup.TextMeshResult.EmojiVerts[k + m];
    
                                uivert.position = pos;
                                hrefmesh.HrefVerts.Add(uivert);
                            }
                        }

                    }

                    result.MeshGroup.HrefMeshReult = hrefmesh;
                    context.Results[i] = result;
                }
            }
            return 0;
        }

        public int Init(ref TextCotext context)
        {
            return 0;
        }
    }
}


