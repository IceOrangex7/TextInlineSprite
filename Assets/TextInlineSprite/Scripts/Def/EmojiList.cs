using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace EmojiUI
{

    public class EmojiList<T>:List<T> 
    {

        public EmojiList() : base()
        {

        }

        public EmojiList(int cap):base(cap)
        {

        }
    }


    public interface IBuffList
    {
        int GetElementSize();

        void AskNewArea();
    }

    public class BufferArea
    {
        private byte[] _buffer;

        private int datalen;

        private AreaNode Head;

        private AreaNode Tail;
        /// <summary>
        /// 4+4+4  +4 =16
        /// </summary>
        private struct AreaNode
        {
            public int AreaId;
            public int Head;
            public int Tail;
            /// <summary>
            /// 暂时占位
            /// </summary>
            public int Place;
            public int Capacity
            {
                get
                {
                    return Tail - Head + 1;
                }
            }
        }
        // 16 * 4 *2 = 64  *2
        private List<AreaNode> _nodelist = new List<AreaNode>(8);
        // 64  *4
        public BufferArea():this(256)
        {

        }

        public BufferArea(int cap)
        {
            _buffer = new byte[cap];
        }


        void Changeapcaptity(int newcap)
        {
            if(newcap < _buffer.Length)
            {

            }
            else if(newcap > _buffer.Length)
            {
                var newbuffer = new byte[newcap];
            }
        }

        int GetNextSize(int size,int needsize)
        {
            int externsize = 0;
            if(size < 256)
            {
                externsize = Mathf.Max(needsize,size);
            }
            else if(size < 512)
            {
                externsize = Mathf.Max(needsize, size /2);
            }
            else
            {
                int part = size / 10;
                while (externsize < needsize)
                {
                    externsize += part;
                }
            }

            return size + externsize;
        }

        public void StartInsert(int idx)
        {

        }
    }

}

