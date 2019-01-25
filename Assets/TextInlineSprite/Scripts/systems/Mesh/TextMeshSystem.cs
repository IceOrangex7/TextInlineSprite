using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EmojiUI
{
    //text 顶点顺序是 0 1
    //                3 2

    public class TextMeshSystem : IEmojiStructSystem<TextCotext>
    {
        private static UIVertex[] m_TempVerts = new UIVertex[4];
        private static UIVertex[] m_TempEmojiVerts = new UIVertex[4];
        private static Dictionary<int, int> _cache = new Dictionary<int, int>();


        public struct UITextMesh
        {
            /// <summary>
            /// 结构上的代价，额外的内存
            /// </summary>
            public EmojiList<UIVertex> TextVerts;

            public EmojiList<Vector3> EmojiVerts;

            public void Init()
            {
                if (TextVerts == null)
                {
                    TextVerts = new EmojiList<UIVertex>(64);
                }
                else
                {
                    TextVerts.Clear();
                }

                if (EmojiVerts == null)
                {
                    EmojiVerts = new EmojiList<Vector3>(64);
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
                int errcode = 0;
                for (int i = 0; i < context.AllTexts.Count; i++)
                {
                    InlineText text = context.AllTexts[i];
                    if (text == null)
                        continue;

                    if (!context.RebuildTexts.Contains(text))
                    {
                        continue;
                    }

                    TextElementResult result = context.Results[i];

                    result.MeshGroup.TextMeshResult.Init();
                    result.GizmosResult.Clear();

                    //text
                    string currenttext = null;
                    if (result.ParseGroup.TextResult.FormatElements != null && result.ParseGroup.TextResult.FormatElements.Count > 0)
                    {
                        currenttext = result.ParseGroup.TextResult.FormatString;
                    }
                    else
                    {
                        currenttext = text.text;
                    }
                    errcode = FillText(text, currenttext, ref context, ref result);

                    context.Results[i] = result;
                }

                return errcode;
            }

            return 0;
        }

        bool IsVisiableType(int type)
        {
            return (type & 2) == 2;
        }

        int FillText(InlineText text, string currenttext, ref TextCotext context, ref TextElementResult result)
        {
            int errcode = 0;
            //text generate
            Vector2 extents = text.rectTransform.rect.size;
            var settings = text.GetGenerationSettings(extents);
            text.cachedTextGenerator.PopulateWithErrors(currenttext, settings, text.gameObject);

            // Apply the offset to the vertices
            IList<UIVertex> verts = text.cachedTextGenerator.verts;
            float unitsPerPixel = 1 / text.pixelsPerUnit;
            //Last 4 verts are always a new line... (\n)
            int vertCount = verts.Count - 4;
            Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
            roundingOffset = text.PixelAdjustPoint(roundingOffset) - roundingOffset;

            bool rdeq = roundingOffset != Vector2.zero;

            int emojiindex = 0;

            _cache.Clear();

            for (int vertidx = 0; vertidx < vertCount; ++vertidx)
            {
                int tempVertsIndex = vertidx & 3;
                int textidx = vertidx / 4;
                int ctype;
                bool isinvert = GetIsInVert(ref result, emojiindex, textidx, out ctype);

                bool isvisiable = IsVisiableType(ctype);

                ; if (context.Settings.DoubleRender || isvisiable || !isinvert)
                {
                    FillTextVerts(verts, vertidx, unitsPerPixel, roundingOffset, rdeq);
                    if (tempVertsIndex == 3)
                    {
                        for (int m = 0; m < 4; m++)
                        {
                            result.MeshGroup.TextMeshResult.TextVerts.Add(m_TempVerts[m]);
                        }
                    }
                }

                if (isinvert)
                {
                    int nexttype;
                    int lasttype;
                    bool nextisinvert = GetIsInVert(ref result, emojiindex, textidx + 1, out nexttype);
                    bool lastisinvert = GetIsInVert(ref result, emojiindex, textidx - 1, out lasttype);

                    bool next = nextisinvert && nexttype == ctype;
                    bool last = lastisinvert && lasttype == ctype;

                    if (tempVertsIndex == 0)
                    {
                        if (!last)//qi点
                        {
                            FillEmojiTextVerts(verts, vertidx, unitsPerPixel, roundingOffset, rdeq);
                        }
                        //else
                        //{
                        //    //skip
                        //}
                    }
                    else if (tempVertsIndex == 1)
                    {
                        if (!next)
                        {
                            FillEmojiTextVerts(verts, vertidx, unitsPerPixel, roundingOffset, rdeq);
                        }
                        //else
                        //{
                        //    //skip
                        //}
                    }
                    else if (tempVertsIndex == 2)
                    {
                        if (!next)
                        {
                            FillEmojiTextVerts(verts, vertidx, unitsPerPixel, roundingOffset, rdeq);
                            if (last)//终点
                            {
                                bool needfix = result.ParseGroup.NeedFix(emojiindex);
                                errcode = Render(ref result, isvisiable, emojiindex, needfix);
                            }
                        }
                        //else
                        //{
                        //    //skip
                        //}
                    }
                    else if (tempVertsIndex == 3)
                    {
                        if (!last)
                        {
                            FillEmojiTextVerts(verts, vertidx, unitsPerPixel, roundingOffset, rdeq);

                            if (!next)//终点
                            {
                                bool needfix = result.ParseGroup.NeedFix(emojiindex);
                                errcode = Render(ref result, isvisiable, emojiindex, needfix);
                                emojiindex++;
                            }
                        }
                        else if (last && !next)
                        {
                            emojiindex++;
                        }
                        //else
                        //{
                        //    //skip
                        //}
                    }
                }
            }
            return errcode;
        }

        bool GetIsInVert(ref TextElementResult result, int emojiidx, int textidx, out int type)
        {

            bool isinvert = false;
            int cacheval;
            if (!_cache.TryGetValue(emojiidx + textidx * 100, out cacheval))
            {
                isinvert = result.ParseGroup.IsInVert(emojiidx, textidx, out type);
                _cache.Add(emojiidx + textidx * 100, (isinvert ? 200 : 100) + type);
            }
            else
            {
                int c = cacheval / 100;
                isinvert = (c == 2);
                type = cacheval % 100;
            }
            return isinvert;
        }

        void FillTextVerts(IList<UIVertex> verts, int vertidx, float unitsPerPixel, Vector2 roundingOffset, bool roundequal)
        {
            int tempVertsIndex = vertidx & 3;
            if (roundequal)
            {
                m_TempVerts[tempVertsIndex] = verts[vertidx];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
            }
            else
            {
                m_TempVerts[tempVertsIndex] = verts[vertidx];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
            }
        }

        void FillEmojiTextVerts(IList<UIVertex> verts, int vertidx, float unitsPerPixel, Vector2 roundingOffset, bool roundequal)
        {
            int tempVertsIndex = vertidx & 3;
            if (roundequal)
            {
                m_TempEmojiVerts[tempVertsIndex] = verts[vertidx];
                m_TempEmojiVerts[tempVertsIndex].position *= unitsPerPixel;
                m_TempEmojiVerts[tempVertsIndex].position.x += roundingOffset.x;
                m_TempEmojiVerts[tempVertsIndex].position.y += roundingOffset.y;
            }
            else
            {
                m_TempEmojiVerts[tempVertsIndex] = verts[vertidx];
                m_TempEmojiVerts[tempVertsIndex].position *= unitsPerPixel;
            }
        }


        int Render(ref TextElementResult result, bool isvisiable, int emojiindex, bool fix = false)
        {
            float groupsize;
            if (!result.ParseGroup.TryGetSize(emojiindex, out groupsize))
            {
                return (int)EmojiErrorCode.GetSizeError;
            }

            if (fix)
            {
                m_TempEmojiVerts[1].position = m_TempEmojiVerts[0].position + new Vector3(groupsize, 0, 0);
                m_TempEmojiVerts[2].position = m_TempEmojiVerts[3].position + new Vector3(groupsize, 0, 0);
            }


            for (int i = 0; i < m_TempEmojiVerts.Length; i++)
            {
                result.MeshGroup.TextMeshResult.EmojiVerts.Add(m_TempEmojiVerts[i].position);
            }
            return 0;
        }

        public int Init(ref TextCotext context)
        {
            return 0;
        }
    }
}
