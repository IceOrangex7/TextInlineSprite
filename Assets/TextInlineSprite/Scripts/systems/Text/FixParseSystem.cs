using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EmojiUI
{
    public class FixParseSystem : IEmojiStructSystem<TextCotext>
    {
        private Dictionary<int, int> _cache = new Dictionary<int, int>();
        public int DoStep(ref TextCotext context, EmojiEvent uievent)
        {
            if (uievent == EmojiEvent.PreRender)
            {
                for (int i = 0; i < context.AllTexts.Count; i++)
                {
                    InlineText text = context.AllTexts[i];
                    if (text == null)
                        continue;

                    if (!context.RebuildTexts.Contains(text))
                    {
                        continue;
                    }

                    _cache.Clear();

                    TextElementResult result = context.Results[i];

                    //if (result.ParseGroup.emojitypeMap == null)
                    //    result.ParseGroup.emojitypeMap = new Dictionary<int, int>();
                    //else
                        //result.ParseGroup.emojitypeMap.Clear();

                    int val = 0;
                    for (int k = 0; k < result.ParseGroup.fillplacemap.Count; k++)
                    {
                        val += result.ParseGroup.fillplacemap[k];
                        _cache[k] = val;
                    }
                    //emoji
                    FixPoints(result.ParseGroup.EmojiResult.GetEmojiPoints(),1, ref result);
 
                    //href

                    FixPoints(result.ParseGroup.HrefResult.GetHrefPoints(),2, ref result);

                    context.Results[i] = result;
                }
            }

            return 0;
        }

        public int Init(ref TextCotext context)
        {
            return 0;
        }

        void FixPoints(EmojiList<EmojiIndex> list ,int type,ref TextElementResult result)
        {
            for (int k = 0; k < list.Count; k++)
            {
                var pointval = list[k];

                int cacheval = 0;
                if (_cache.TryGetValue(pointval.Emojiindex - 1, out cacheval))
                {
                    pointval.TextIndex += cacheval;
                }
                //result.ParseGroup.emojitypeMap[emojiidx] = type + k *100;
                list[k] = pointval;
            }
        }
    }
}


