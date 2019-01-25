using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace EmojiUI
{
    public class CollectAssetsSystem :IEmojiStructSystem<TextCotext>
    {
        public int DoStep(ref TextCotext context, EmojiEvent uievent)
        {
            return 0;
        }

        public int Init(ref TextCotext context)
        {
#if UNITY_EDITOR
            string[] result = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(SpriteAsset).FullName));

            if (result.Length > 0 )
            {
                for (int i = 0; i < result.Length; ++i)
                {
                    string path = AssetDatabase.GUIDToAssetPath(result[i]);
                    SpriteAsset asset = AssetDatabase.LoadAssetAtPath<SpriteAsset>(path);
                    if (asset)
                    {
                        context.Refercens.Add(asset);
                    }
                }
            }


#endif

            InlineManager[] managers = GameObject.FindObjectsOfType<InlineManager>();
            foreach (var manager in managers)
            {
                foreach (var asset in manager.SharedAssets)
                {
                    if (context.Refercens.Contains(asset) == false)
                    {
                        context.Refercens.Add(asset);
                    }
                }
            }

            Debug.LogFormat("find :{0} atlas resource", result.Length);

            foreach(var asset in context.Refercens)
            {
                foreach(var gp in asset.listSpriteGroup)
                {
                    context.GroupMapper[gp.tag] = gp;
                }
            }


            return 0;
        }
    }
}


