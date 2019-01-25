using UnityEngine;
using System.Collections;

namespace EmojiUI
{
    public struct EmojiUIVertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Color32 color;
        public Vector2 uv0;
        public Vector2 uv1;
        public Vector2 uv2;
        public Vector2 uv3;
        public Vector4 tangent;
        public Texture texture;

        public EmojiUIVertex(UIVertex vert)
        {
            position = vert.position;
            normal = vert.normal;
            color = vert.color;
            uv0 = vert.uv0;
            uv1 = vert.uv1;
            uv2 = vert.uv2;
            uv3 = vert.uv3;
            tangent = vert.tangent;
            texture = null;
        }

        public UIVertex ToUiVertex()
        {
            var uivert = new UIVertex();
            uivert.position = this.position;
            uivert.normal = this.normal;
            uivert.color = this.color;
            uivert.uv0 = this.uv0;
            uivert.uv1 = this.uv1;
            uivert.uv2 = this.uv2;
            uivert.uv3 = this.uv3;
            uivert.tangent = tangent;
            return uivert;
        }
    }
}
