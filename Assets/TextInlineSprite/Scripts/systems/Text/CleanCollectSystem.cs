using UnityEngine;
using System.Collections;

namespace EmojiUI
{
    public class CleanCollectSystem: IEmojiStructSystem<TextCotext>
    {
        public int DoStep(ref TextCotext context, EmojiEvent uievent)
        {
            if(uievent == EmojiEvent.LateUpdate)
            {
                for (int i = context.AllTexts.Count-1; i>=0;i--)
                {
                    if (context.AllTexts[i] == null)
                    {
                        context.AllTexts.RemoveAt(i);
                        context.Results.RemoveAt(i);
                    }
                }

                for (int i = 0; i < context.FailedText.Count; i++)
                {
                    int idx = context.AllTexts.IndexOf(context.FailedText[i]);
                    if(idx != -1)
                    {
                        context.AllTexts.RemoveAt(idx);
                        context.Results.RemoveAt(idx);
                    }
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

