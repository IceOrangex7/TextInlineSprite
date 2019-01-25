using UnityEngine;
using System.Collections;

namespace  EmojiUI
{

    public enum EmojiErrorCode 
    {
        Success =0,
        TooMuchChars=1,
        ParseFailed=2,
        ParseColFailed=3,
        GetSizeError=4,
        VertTooLarge =5,
    }
}

