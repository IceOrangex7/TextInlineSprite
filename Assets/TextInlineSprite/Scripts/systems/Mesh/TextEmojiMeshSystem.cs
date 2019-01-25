using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EmojiUI
{
    public class TextEmojiMeshSystem : IEmojiStructSystem<TextCotext>
    {

        public struct EmojiMesh
        {
            public EmojiList<int> FrameIndex;

            public EmojiList<SpriteInfoGroup> RenderGroup;

            public EmojiList<EmojiUIVertex> EmojiVerts;

            public void Init()
            {
                if (FrameIndex == null)
                {
                    FrameIndex = new EmojiList<int>();
                }
                else
                {
                    FrameIndex.Clear();
                }

                if (RenderGroup == null)
                {
                    RenderGroup = new EmojiList<SpriteInfoGroup>();
                }
                else
                {
                    RenderGroup.Clear();
                }

                if (EmojiVerts == null)
                {
                    EmojiVerts = new EmojiList<EmojiUIVertex>();
                }
                else
                {
                    EmojiVerts.Clear();
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

                    var emojimesh = result.MeshGroup.EmojiMeshResult;
                    emojimesh.Init();


                    for (int k = 0; k < result.MeshGroup.TextMeshResult.EmojiVerts.Count; k += 4)
                    {
                        int emojiindex = k / 4;
                        int arrayindex;
                        int emojitype = result.ParseGroup.GetIndexType(emojiindex, out arrayindex);

                        if(emojitype == 1 && arrayindex >=0 && arrayindex < result.ParseGroup.EmojiResult.Count)
                        {
                            var group = result.ParseGroup.EmojiResult.GetGroups()[arrayindex];
                            emojimesh.FrameIndex.Add(0);
                            emojimesh.RenderGroup.Add(group);

                            var center = (result.MeshGroup.TextMeshResult.EmojiVerts[k] + result.MeshGroup.TextMeshResult.EmojiVerts[k+1]
                            + result.MeshGroup.TextMeshResult.EmojiVerts[k+2] + result.MeshGroup.TextMeshResult.EmojiVerts[k+3]) / 4;


                            for (int m = 0; m < 4; m++)
                            {
                                var uivert = new EmojiUIVertex();

                                var sprite = group.spritegroups[0];
                                uivert.uv0 = sprite.uv[3 - m];
                                uivert.uv1 = sprite.uv[3 - m];
                                uivert.uv2 = sprite.uv[3 - m];
                                uivert.uv3 = sprite.uv[3 - m];
                                uivert.color = Color.white;
                                uivert.normal = Vector3.back;
                                uivert.tangent = new Vector4(1.0f, 0.0f, 0.0f, -1.0f);
                                uivert.texture = sprite.sprite.texture;


                                float wc = ((m & 2) >> 1) == (m & 1) ? -0.5f : 0.5f;
                                float hc = (m & 2) == 0 ? -0.5f : 0.5f;

                                float emojisize = result.ParseGroup.EmojiResult.GetEmojiSize()[arrayindex];

                                var position = center + new Vector3(wc * emojisize, hc * emojisize, 0);
                                uivert.position = position;
                                emojimesh.EmojiVerts.Add(uivert);
                            }
                        }

                    }

                    result.MeshGroup.EmojiMeshResult = emojimesh;
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


