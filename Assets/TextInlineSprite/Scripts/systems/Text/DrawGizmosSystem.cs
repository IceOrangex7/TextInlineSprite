using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EmojiUI
{
    public class DrawGizmosSystem : IEmojiStructSystem<TextCotext>
    {
        public struct DrawedGizmos
        {
            public Color color;
            public List<Vector3> Quads;
            public List<KeyValuePair<Vector3, string>> QuadLabel;

            public void Clear()
            {
                if (Quads != null)
                    Quads.Clear();


                if (QuadLabel != null)
                    QuadLabel.Clear();
            }

            public void DrawVert(Vector3 pos)
            {
                if (Quads == null)
                    Quads = new List<Vector3>();
                Quads.Add(pos);
            }

            public void DrawLabel(string lbl,Vector3 pos)
            {
                if (QuadLabel == null)
                    QuadLabel = new List<KeyValuePair<Vector3, string>>();

                QuadLabel.Add(new KeyValuePair<Vector3, string>(pos,lbl));
            }
        }

        public int DoStep(ref TextCotext context, EmojiEvent uievent)
        {
            if (uievent == EmojiEvent.DrawGizmos)
            {
#if UNITY_EDITOR
                if (context.AllTexts.Count > 0 && context.Results.Count >0)
                {
                    for (int i = 0; i < context.Results.Count; i++)
                    {
                        var gdata = context.Results[i].GizmosResult;
                        if(i < context.AllTexts.Count)
                        {
                            context.AllTexts[i].DrawGizmos(ref gdata);
                        }
                    }
                }
#endif
            }

            return 0;
        }

        public int Init(ref TextCotext context)
        {
            return 0;
        }


    }
}
