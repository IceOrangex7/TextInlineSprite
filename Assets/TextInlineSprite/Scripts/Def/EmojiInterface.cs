using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmojiUI
{

    public interface IEmojiSystem
    {

    }

    public interface IEmojiStructSystem<T>:IEmojiSystem where T:struct
    {
        int Init(ref T context);

        int DoStep(ref T context,EmojiEvent uievent);
    }


    public interface ParseRuleData
    {
        int Count { get; }
        EmojiIndex GetPoints(int idx);
        float GetSize(int idx);
        bool IsInVert(int emojiidx,int textidx);
    }

}

