using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EmojiUI
{
    public enum EmojiEvent
    {
        LateUpdate,
        GraphicComplete,
        PreRender,
        LatePreRender,
        Update,
        DrawGizmos,
    }

    public class SystemFeature
    {
        private List<IEmojiSystem> _systems;

        internal TextCotext Context;

        public void InitSystem(InlineManager manager, List<int> systems)
        {
            if(_systems == null)
            {
                Context = new TextCotext(manager,12);
                _systems = new List<IEmojiSystem>();

                for (int i = 0; i < systems.Count; i++)
                {
                    AddSystem(systems[i]);
                }
            }
        }

        public void Restart(InlineManager manager, List<int> systems)
        {
            Destroy();
            InitSystem(manager,systems);
        }

        public void AddSystem(int systemid)
        {
            var system = SystemFactory.mIns.Create(systemid);
            if (system != null)
            {
                if (system is IEmojiStructSystem<TextCotext>)
                {
                    IEmojiStructSystem<TextCotext> essystem = system as IEmojiStructSystem<TextCotext>;
                    int code = essystem.Init(ref Context);
                    if(code != 0)
                    {
#if UNITY_EDITOR
                        Debug.LogErrorFormat("Init Code {0} in {1}", (EmojiErrorCode)code, essystem);
#endif
                    }
                }

                _systems.Add(system);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("missing {0}", system);
#endif
            }
        }

        public void DoUpdate(EmojiEvent uievent)
        {
            if(_systems != null)
            {
                for (int i = 0; i < _systems.Count; i++)
                {
                    var sy = _systems[i];
                    if(sy is IEmojiStructSystem<TextCotext>)
                    {
                        IEmojiStructSystem<TextCotext> essystem = sy as IEmojiStructSystem<TextCotext>;
                        int code =essystem.DoStep(ref Context ,uievent);
                        if (code != 0)
                        {
#if UNITY_EDITOR
                            Debug.LogErrorFormat("DoStep Code {0} in {1}",(EmojiErrorCode)code, essystem);
#endif
                        }
                    }
                }
            }
        }

        public void Destroy()
        {
            Context.Destroy();
            _systems = null;
        }
    }
}

