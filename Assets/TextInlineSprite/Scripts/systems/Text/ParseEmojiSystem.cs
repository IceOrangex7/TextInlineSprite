using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

namespace EmojiUI
{
    public class ParseEmojiSystem : IEmojiStructSystem<TextCotext>
    {
        private const string Placeholder = "1";
        private const string DoublePlaceholder = "11";

        private InlineText _lasttext;
        private Vector2 _placesize;
        private float _space;

        private static StringBuilder _sb;
        public struct EmojiResult : ParseRuleData
        {
            /// <summary>
            /// 1 = 在text内容的新索引 2 所属的组
            /// </summary>
            public EmojiList<SpriteInfoGroup> Groups;

            public EmojiList<float> EmojiSize;

            public EmojiList<EmojiIndex> EmojiPoints;

            public EmojiList<int> FixedIndex;

            public int Count
            {
                get
                {
                    return GetGroups().Count;
                }
            }

            public EmojiList<int> GetFixedIndex()
            {
                if (FixedIndex == null)
                    FixedIndex = new EmojiList<int>();
                return FixedIndex;
            }

            public EmojiList<SpriteInfoGroup> GetGroups()
            {
                if (Groups == null)
                    Groups = new EmojiList<SpriteInfoGroup>();
                return Groups;
            }

            public EmojiList<float> GetEmojiSize()
            {
                if (EmojiSize == null)
                    EmojiSize = new EmojiList<float>();
                return EmojiSize;
            }

            public EmojiList<EmojiIndex> GetEmojiPoints()
            {
                if (EmojiPoints == null)
                    EmojiPoints = new EmojiList<EmojiIndex>();
                return EmojiPoints;
            }

            public void Clear()
            {
                if (Groups != null)
                {
                    Groups.Clear();
                }

                if (EmojiSize != null)
                {
                    EmojiSize.Clear();
                }

                if (EmojiPoints != null)
                {
                    EmojiPoints.Clear();
                }

                if (FixedIndex != null)
                {
                    FixedIndex.Clear();
                }
            }

            public EmojiIndex GetPoints(int idx)
            {
                return GetEmojiPoints()[idx];
            }

            public float GetSize(int idx)
            {
                return GetEmojiSize()[idx];
            }

            public bool IsInVert(int emojiidx,int textidx)
            {
                if (EmojiPoints != null)
                {
                    for (int i = 0; i < EmojiPoints.Count; i++)
                    {
                        var emoji = EmojiPoints[i];
 
                        if (emoji.Emojiindex == emojiidx)
                        {
                            if (emoji.InTextRange((textidx)))
                            {
                                return true;
                            }
                            return false;
                        }
                    }
                }
                return false;
            }
        }

        public int DoStep(ref TextCotext context, EmojiEvent uievent)
        {
            if (uievent == EmojiEvent.PreRender)
            {
                int errcode = 0;
                for (int i = 0; i < context.RebuildTexts.Count; i++)
                {
                    var text = context.RebuildTexts[i];
                    if (text == null)
                        continue;

                    int index = context.AllTexts.IndexOf(text);
                    if (index == -1)
                        continue;

                    var result = context.Results[index];
                    result.ParseGroup.EmojiResult.Clear();

                    if (result.ParseGroup.TextResult.FormatElements != null && result.ParseGroup.TextResult.FormatElements.Count > 0)
                    {
                        if (_sb == null)
                            _sb = new StringBuilder(result.ParseGroup.TextResult.FormatString);
                        else
                        {
                            _sb.Length = 0;
                            _sb.Append(result.ParseGroup.TextResult.FormatString);
                        }

                        int addedcnt = 0;
                        float linesize = 0;
                        int emojistart = -1;
                        int emojiidx = 0;
                        if (emojiidx < result.ParseGroup.TextResult.GetPoints().Count)
                        {
                            emojistart = result.ParseGroup.TextResult.GetPoints()[emojiidx];
                            //不相等 刷新缓存
                            if (_lasttext != text)
                            {
                                Vector2 extents = text.rectTransform.rect.size;
                                TextGenerationSettings settings = text.GetGenerationSettings(extents);
                                text.cachedTextGenerator.Populate(DoublePlaceholder, settings);

                                if (text.cachedTextGenerator.vertexCount < 8)
                                {
                                    errcode = (int) EmojiErrorCode.VertTooLarge;
                                    break;
                                }

                                IList<UIVertex> spaceverts = text.cachedTextGenerator.verts;
                                float spacewid = spaceverts[1].position.x - spaceverts[0].position.x;
                                float spaceheight = spaceverts[0].position.y - spaceverts[3].position.y;

                                _space = spaceverts[4].position.x - spaceverts[1].position.x;
                                _placesize = new Vector2(spacewid, spaceheight);
                                _lasttext = text;
                            }
                        }

                        float textsize = text.rectTransform.rect.width;

                        for (int charindex = 0; charindex < result.ParseGroup.TextResult.GetPartSize().Count; charindex++)
                        {

                            while (emojistart == charindex
    && InsertEmoji(text, ref result, ref emojiidx, ref context, ref linesize, ref emojistart, ref addedcnt))
                            {
                                //fix line
                                if (text.horizontalOverflow != HorizontalWrapMode.Overflow && linesize > textsize - _space - _placesize.x)//这个字符自动跳到下一行
                                {
                                    linesize = 0;
                                }
                            }

                            if (emojistart < 0)
                            {
                                break;
                            }

                            float w = result.ParseGroup.TextResult.GetPartSize()[charindex];
                            linesize += w;
   
                            //fix line
                            if (text.horizontalOverflow != HorizontalWrapMode.Overflow && linesize > textsize - _space - _placesize.x)//这个字符自动跳到下一行
                            {
                                linesize = 0;
                            }
                        }

                        //fill last
                        if (emojistart >= 0)
                        {
                            while (emojistart == result.ParseGroup.TextResult.GetPartSize().Count
                                && InsertEmoji(text, ref result, ref emojiidx, ref context, ref linesize, ref emojistart, ref addedcnt))
                            {
                                //fix line
                                if (text.horizontalOverflow != HorizontalWrapMode.Overflow && linesize > textsize - _space - _placesize.x)//这个字符自动跳到下一行
                                {
                                    linesize = 0;
                                }
                            }
                        }

                        result.ParseGroup.TextResult.FormatString = _sb.ToString();
                    }

                    //assign
                    context.Results[index] = result;
                }

                _lasttext = null;
                return errcode;
            }

            return 0;
        }

        bool InsertEmoji(InlineText text, ref TextElementResult result, ref int emojiidx, ref TextCotext context, ref float linesize, ref int emojistart, ref int addedcnt)
        {
            if (emojiidx >= result.ParseGroup.TextResult.FormatElements.Count)
                return false;

            string info = result.ParseGroup.TextResult.FormatElements[emojiidx];

            int atlasId = 0;
            string tagKey = null;
            if (!ParseElement(text, info, ref context, out atlasId, out tagKey))
            {
                return false;
            }

            int fillplacecnt = 0;
            //Fill
            SpriteInfoGroup group;
            bool canrender = CanRender(atlasId, tagKey, ref context, out group);
            if (canrender && atlasId >= 0)
            {
                Fill(text, group, emojiidx, ref linesize, ref result, out fillplacecnt);
                //Calculate
                result.ParseGroup.EmojiResult.GetEmojiPoints().Add(new EmojiIndex(emojiidx, emojistart, fillplacecnt));

                //Fill placeholder
                if (canrender && fillplacecnt > 0)
                {
                    for (int k = 0; k < fillplacecnt; k++)
                    {
                        _sb.Insert(emojistart + k + addedcnt, Placeholder);
                    }
                }

                addedcnt += fillplacecnt;
                emojiidx++;
                if (emojiidx < result.ParseGroup.TextResult.GetPoints().Count)
                {
                    emojistart = result.ParseGroup.TextResult.GetPoints()[emojiidx];
                }
                else
                {
                    emojistart = -1;
                }

                //
                result.ParseGroup.AddExternFill(emojiidx-1, fillplacecnt);
            }
            else
            {
                emojiidx++;
                if (emojiidx < result.ParseGroup.TextResult.GetPoints().Count)
                {
                    emojistart = result.ParseGroup.TextResult.GetPoints()[emojiidx];
                }
                else
                {
                    emojistart = -1;
                }
                result.ParseGroup.AddExternFill(emojiidx - 1, 0);
            }

            return true;
        }

        void Fill(InlineText target, SpriteInfoGroup group, int elementidx, ref float linesize, ref TextElementResult textresult, out int fillsize)
        {
            fillsize = 0;

            float spacesize = Mathf.Max(_placesize.x, _placesize.y);
            float minsize = Mathf.Min(group.size, Mathf.Min(_placesize.x, _placesize.y));

            if (minsize > 0.1f)
            {
                float autosize = Mathf.Min(group.size, spacesize);
                int fillspacecnt = Mathf.CeilToInt(autosize / minsize); ;

                if (target.horizontalOverflow != HorizontalWrapMode.Overflow && fillspacecnt > 0)
                {
                    float wid = target.rectTransform.rect.width;
                    float fillwidth = fillspacecnt * (minsize + _space);

                    //填充字符之间穿插了换行的情况 进行检测
                    if (wid - linesize < fillwidth)
                    {
                        fillspacecnt = 1;
                        linesize = 0;
                        textresult.ParseGroup.EmojiResult.GetFixedIndex().Add(elementidx);
                    }
                    else
                    {
                        linesize += fillwidth;
                    }
                }

                fillsize = fillspacecnt;

                textresult.ParseGroup.EmojiResult.GetEmojiSize().Add(autosize);
                textresult.ParseGroup.EmojiResult.GetGroups().Add(group);
            }
        }

        public int Init(ref TextCotext context)
        {
            return 0;
        }

        bool ParseElement(InlineText text, string info, ref TextCotext context, out int atlasId, out string tagKey)
        {
            atlasId = -1;
            tagKey = null;

            int index = info.IndexOf('#');
            //parse element
            if (index != -1)
            {
                string subId = info.Substring(1, index - 1);
                if (subId.Length > 0 && !int.TryParse(subId, out atlasId))
                {
#if UNITY_EDITOR
                    Debug.LogErrorFormat("{0} convert failed ", subId);
#endif
                    context.FailedText.Add(text);
                    return false;
                }
                else if (subId.Length == 0)
                {
                    atlasId = 0;
                }

                tagKey = info.Substring(index + 1, info.Length - index - 2);
            }
            else
            {
                tagKey = info.Substring(1, info.Length - 2);
            }
            return true;
        }

        bool CanRender(int atlasid, string key, ref TextCotext context, out SpriteInfoGroup group)
        {
            bool canrender = false;
            group = null;
            for (int i = 0; i < context.Refercens.Count; i++)
            {
                var refasset = context.Refercens[i];
                if (refasset.ID == atlasid && context.GroupMapper.TryGetValue(key, out group))
                {
                    canrender = true;
                    break;
                }
            }
            return canrender;
        }
    }
}



