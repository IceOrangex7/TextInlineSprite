using UnityEngine;
using System.Collections;
using System;

namespace EmojiUI
{
    public class ReadSettingsSystem : IEmojiStructSystem<TextCotext>
    {
        [System.Serializable]
        public struct Settings:IEquatable<Settings>
        {
            [SerializeField]
            public bool OverDrawTest;
            [SerializeField]
            public bool DoubleRender;
            [SerializeField]
            public bool OpenDebug;
            [SerializeField]
            public int AnimationSpeed ;
            [SerializeField]
            public bool OpenDrawPreferred;

            public bool Equals(Settings other)
            {
                if (OverDrawTest != other.OverDrawTest) return false;
                if (DoubleRender != other.DoubleRender) return false;
                if (OpenDebug != other.OpenDebug) return false;
                if (AnimationSpeed != other.AnimationSpeed) return false;
                if (OpenDrawPreferred != other.OpenDrawPreferred) return false;
                return true;
            }
        }

        private Settings? _last;

        public int DoStep(ref TextCotext context, EmojiEvent uievent)
        {
            if (uievent == EmojiEvent.LateUpdate)
            {
                context.Settings = context.Owner.Settings;
                if(context.Settings.AnimationSpeed <1)
                {
                    context.Settings.AnimationSpeed = 30;
                }

                if(_last.HasValue)
                {
                    if(_last.Value.Equals(context.Settings) == false)
                    {
                        for (int i = 0; i < context.AllTexts.Count; i++)
                        {
                            if (context.AllTexts[i] == null)
                                continue;
                            context.AllTexts[i].SetVerticesDirty();
                        }

                        _last = context.Settings;
                    }
                }
                else
                {
                    _last = context.Settings;
                }
            }

            return 0;
        }

        public int Init(ref TextCotext context)
        {
            return 0;
        }


    }
}
