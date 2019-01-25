using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EmojiUI
{
    public class ParseHrefSystem : IEmojiStructSystem<TextCotext>
    {
        public struct ParseHrefResult: ParseRuleData
        {
            public EmojiList<Color> HrefCols;

            public EmojiList<EmojiIndex> HrefPoints;

            public int Count
            {
                get
                {
                    return GetHrefCols().Count;
                }
            }

            public EmojiList<Color> GetHrefCols()
            {
                if (HrefCols == null)
                    HrefCols = new EmojiList<Color>();
                return HrefCols;
            }

            public EmojiList<EmojiIndex> GetHrefPoints()
            {
                if (HrefPoints == null)
                    HrefPoints = new EmojiList<EmojiIndex>();
                return HrefPoints;
            }

            public void Clear()
            {
                if (HrefCols != null)
                    HrefCols.Clear();

                if (HrefPoints != null)
                    HrefPoints.Clear();
            }

            public EmojiIndex GetPoints(int idx)
            {
                return GetHrefPoints()[idx];
            }

            public float GetSize(int idx)
            {
                return 0;
            }

            public bool IsInVert(int emojiidx, int textidx)
            {
                if(HrefPoints != null)
                {
                    for (int i = 0; i < HrefPoints.Count; i++)
                    {
                        var href = HrefPoints[i];

                        if (href.Emojiindex == emojiidx)
                        {
                            if (href.InTextRange((textidx)))
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

                    if(result.ParseGroup.TextResult.FormatElements == null)
                        continue;

                    result.ParseGroup.HrefResult.Clear();

                    for (int m = 0; m < result.ParseGroup.TextResult.FormatElements.Count; m++)
                    {
                        string info = result.ParseGroup.TextResult.FormatElements[m];
                        int point = result.ParseGroup.TextResult.GetPoints()[m];

                        int len = 0;
                        string tagKey = null;
                        if (!ParseElement(text, info, ref context, out len, out tagKey))
                        {
                            errcode = (int)EmojiErrorCode.ParseFailed;
                            break;
                        }

                        if(len >=0)
                        {
                            continue;
                        }

                        Color col;
                        if(!ColorUtility.TryParseHtmlString(tagKey, out col))
                        {
                            errcode = (int)EmojiErrorCode.ParseColFailed;
                            break;
                        }

                        len = Mathf.Abs(len);
                        //int left = result.ParseGroup.TextResult.FormatString.Length -1 -(point+ len);
                        //if (left < len)
                        //{
                        //    len = Mathf.Max(0, left);
                        //}

                        result.ParseGroup.HrefResult.GetHrefCols().Add(col);
                        result.ParseGroup.HrefResult.GetHrefPoints().Add(new EmojiIndex(m, point , len));
                    }

                    context.Results[index] = result;
                }

                return errcode;
            }

            return 0;
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

                tagKey = info.Substring(index , info.Length - index - 2 +1);
            }
            else
            {
                tagKey = info.Substring(1, info.Length - 2);
            }
            return true;
        }

    }
}
