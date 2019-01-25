using UnityEngine;
using System.Collections;

namespace  EmojiUI
{
    public struct UIMeshGroup 
    {
        public TextMeshSystem.UITextMesh TextMeshResult;

        public TextEmojiMeshSystem.EmojiMesh EmojiMeshResult;

        public TextHrefMeshSystem.HrefMesh HrefMeshReult;

        public bool CanRenderEmoji()
        {
            return EmojiMeshResult.EmojiVerts != null && EmojiMeshResult.EmojiVerts.Count > 0;
        }

        public bool CanRenderHref()
        {
            return HrefMeshReult.HrefVerts != null && HrefMeshReult.HrefVerts.Count > 0;
        }

        public bool CanRender()
        {
            return CanRenderEmoji() || CanRenderHref();
        }
    }
}
