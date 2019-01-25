using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EmojiUI
{
    public class ParseTextSystem : IEmojiStructSystem<TextCotext>
    {
        private static StringBuilder _sb;
        public struct ParseTextResult
        {
            public string FormatString;
            /// <summary>
            /// format 元素
            /// </summary>
            public EmojiList<string> FormatElements;
            /// <summary>
            /// 字符填充索引信息
            /// </summary>
            public EmojiList<int> Points;
            /// <summary>
            /// 每个部分的大小
            /// </summary>
            public EmojiList<float> PartSize;

            public EmojiList<string> GetElements()
            {
                if (FormatElements == null)
                    FormatElements = new EmojiList<string>(8);
                return FormatElements;
            }


            public EmojiList<int> GetPoints()
            {
                if (Points == null)
                    Points = new EmojiList<int>(8);
                return Points;
            }

            public EmojiList<float> GetPartSize()
            {
                if (PartSize == null)
                    PartSize = new EmojiList<float>(8);
                return PartSize;
            }

            public void Clear()
            {
                if (FormatElements != null)
                    FormatElements.Clear();

                if (Points != null)
                    Points.Clear();

                if (PartSize != null)
                    PartSize.Clear();
            }
        }

        private const char ParsetLeft = '[';

        private const char ParsetRight = ']';

        public int DoStep(ref TextCotext context, EmojiEvent uievent)
        {
            if (uievent == EmojiEvent.PreRender)
            {
                int errcode = 0;
                foreach (var target in context.RebuildTexts)
                {
                    if (target == null)
                        continue;

                    string text = target.text;

                    if (text.Length > 16381) //65535/4-1
                    {
                        errcode = (int)EmojiErrorCode.TooMuchChars;
                        continue;
                    }

                    ParseTextResult result = new ParseTextResult();
                    int index = context.AllTexts.IndexOf(target);
                    if(index != -1)
                    {
                        result = context.Results[index].ParseGroup.TextResult;
                        result.Clear();
                    }

                    if (_sb == null)
                        _sb = new StringBuilder();

                    _sb.Capacity = text.Length;

                    int textstart = -1;
                    int start = 0;
                    int flag = 0;
                    for (int i = 0; i < text.Length; i++)
                    {
                        char subchar = text[i];
                        if (subchar == ParsetLeft)
                        {
                            if(flag == 0)
                            {
                                start = i;
                                result.GetPoints().Add(_sb.Length);
                            }

                            flag++;
                        }
                        else if(subchar == ParsetRight)
                        {
                            flag--;
                            if(flag == 0)
                            {
                                string substr = text.Substring(start, i - start +1);
                                
                                result.GetElements().Add(substr);
                                //
                                textstart = -1;
                            }
                        }
                        else if(flag == 0)//正常显示内容
                        {
                            if(textstart <0)
                            {
                                textstart = i;
                            }
                            _sb.Append(subchar);
                        }
                    }

                    if (flag >0)
                    {
                        string substr = text.Substring(start);
                        _sb.Append(substr);
                    }

                    result.FormatString = _sb.ToString();

                    FillPart(target, result.FormatString, 0, result.FormatString.Length-1, ref result);

                    _sb.Length = 0;

                    //assign
                    if(index == -1)
                    {
                        TextElementResult elementresult = new TextElementResult();
                        elementresult.RenderId = target.GetHashCode();
                        elementresult.ParseGroup.TextResult = result;
                        elementresult.ParseGroup.fillplacemap = new EmojiList<int>(64);
                        context.AllTexts.Add(target);
                        context.Results.Add(elementresult);
                    }
                    else
                    {
                        TextElementResult elementresult = context.Results[index];
                        elementresult.ParseGroup.TextResult = result;
                        elementresult.ParseGroup.fillplacemap.Clear();

                        context.Results[index] = elementresult;
                    }
                }

                return errcode;
            }

            return 0;
        }

        void FillPart(InlineText text, string context, int start,int end,ref ParseTextResult result)
        {
            string substr = context.Substring(start, end - start + 1);

            //生成字符 拿到字符大小
            Vector2 extents = text.rectTransform.rect.size;
            var settings = text.GetGenerationSettings(extents);
            text.cachedTextGenerator.PopulateWithErrors(substr, settings, text.gameObject);

            //最后4个点是空行
            var charlist = text.cachedTextGenerator.characters;

            //插入点之后文字所占用的宽度
            for (int i = 0; i < text.cachedTextGenerator.characterCountVisible; i++)
            {
                var chardata = charlist[i];
                result.GetPartSize().Add(chardata.charWidth);
            }


            if (text.Manager != null && text.Manager.OpenDebug)
            {
#if UNITY_EDITOR
                Debug.LogFormat("{0}  point ={1}", substr,start);
#endif
            }

        }

        public int Init(ref TextCotext context)
        {
            return 0;
        }
    }
}


