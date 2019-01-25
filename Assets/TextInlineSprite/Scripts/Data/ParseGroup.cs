using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EmojiUI
{
    /// <summary>
    /// 不存成interface ，虽然coding 麻烦了，但是减少了boxing   emoji =1  href=2
    /// </summary>
    public struct ParseGroup
    {
        public EmojiList<int> fillplacemap;
        /// <summary>
        /// 文字解析结果
        /// </summary>
        public ParseTextSystem.ParseTextResult TextResult;
        /// <summary>
        /// Emoji解析结果
        /// </summary>
        public ParseEmojiSystem.EmojiResult EmojiResult;
        /// <summary>
        /// Href解析结果
        /// </summary>
        public ParseHrefSystem.ParseHrefResult HrefResult;

        public bool HasHref()
        {
            return HrefResult.Count > 0;
        }

        public bool HasEmoji()
        {
            return EmojiResult.Count > 0;
        }

        public bool HasData()
        {
            return HasEmoji() | HasHref();
        }

        public void AddExternFill(int idx, int cnt)
        {
            if (idx < fillplacemap.Count)
            {
                fillplacemap[idx] += cnt;
            }
            else
            {
                fillplacemap.Add(cnt);
            }
        }

        /// <summary>
        /// idx 表示第几个emoji
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool NeedFix(int idx)
        {
            if(HasEmoji() )
            {
                return EmojiResult.GetFixedIndex().Contains(idx);
            }

            return false;
        }

        /// <summary>
        /// idx 表示第几个emoji
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool TryGetSize(int idx, out float size)
        {
            size = 0;
            int arrayindex;
            if (HasEmoji() && TryFind(EmojiResult.GetEmojiPoints(), idx, out arrayindex))
            {
                size = EmojiResult.GetSize(arrayindex);
                return true;
            }

            if (HasHref() && TryFind(HrefResult.GetHrefPoints(), idx, out arrayindex))
            {
                size = HrefResult.GetSize(arrayindex);
                return true;
            }

            return false;
        }


        bool TryFind(EmojiList<EmojiIndex> list, int idx, out int arrayindex)
        {
            arrayindex = -1;
            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i];
                if (value.Emojiindex == idx)
                {
                    arrayindex = i;
                    return true;
                }
            }
            return false;
        }

        public bool IsInVert( int emojiidx, int textidx,out int type)
        {
            type = 0;
            if (HasEmoji() && EmojiResult.IsInVert(emojiidx,textidx))
            {
                type += 1;
            }

            if(HasHref() && HrefResult.IsInVert(emojiidx,textidx))
            {
                type += 2;
            }

            return type >0;
        }

        public int GetIndexType(int idx)
        {
            if (HasEmoji() && TryFind(EmojiResult.GetEmojiPoints(), idx))
            {
                return 1;
            }

            if (HasHref() && TryFind(HrefResult.GetHrefPoints(), idx))
            {
                return 2;
            }
            return 0;
        }


        public int GetIndexType(int idx,out int arrayindex)
        {
            if (HasEmoji() && TryFind(EmojiResult.GetEmojiPoints(), idx,out arrayindex))
            {
                return 1;
            }

            if (HasHref() && TryFind(HrefResult.GetHrefPoints(), idx,out arrayindex))
            {
                return 2;
            }
            arrayindex = -1;
            return 0;
        }

        bool TryFind(EmojiList<EmojiIndex> list, int idx)
        {
            int p;
            return TryFind(list,idx,out p);
        }
    }
}


