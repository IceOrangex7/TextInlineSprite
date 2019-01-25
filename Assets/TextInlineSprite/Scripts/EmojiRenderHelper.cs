using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace  EmojiUI
{

    public class EmojiRenderHelper 
    {
        private static Material _uimaterial;

        public static Material GetDefaultMaterial()
        {
            if (_uimaterial == null)
            {
                _uimaterial = new Material(Shader.Find("Unlit/EmojiUI"));
            }
            return _uimaterial;
        }

    }
}


