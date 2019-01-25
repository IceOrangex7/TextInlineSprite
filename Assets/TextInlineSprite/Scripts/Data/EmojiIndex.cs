using UnityEngine;
using System.Collections;

namespace EmojiUI
{
    public struct EmojiIndex
    {
        public int Emojiindex;

        public int TextIndex;

        public int Length;

        public EmojiIndex(int idx, int textidx, int len)
        {
            Emojiindex = idx;
            TextIndex = textidx;
            Length = len;
        }

        public bool InTextRange(int textidx)
        {
            return textidx >= TextIndex && textidx < TextIndex + Length;
        }
    }
}

