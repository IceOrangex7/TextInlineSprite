#define EMOJI_RUNTIME
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EmojiUI
{
    [RequireComponent(typeof(Canvas))]
	public class InlineManager : MonoBehaviour,ICanvasElement
    {
        [SerializeField]
        private List<int> _systemsid;

        [SerializeField]
        public ReadSettingsSystem.Settings Settings;

        [SerializeField]
        [Tooltip("SpriteAsset References")]
        public List<SpriteAsset> SharedAssets ;

#if UNITY_EDITOR
        [SerializeField]
		private bool _openDebug;
		public bool OpenDebug
		{
			get
			{
				return _openDebug;
			}
			set
			{
				if (_openDebug != value)
				{
					_openDebug = value;
					if (Application.isPlaying)
					{
						if (value)
						{
							EmojiTools.StartDumpGUI();
						}
						else
						{
							EmojiTools.EndDumpGUI();
						}
					}
				}
			}
		}

#endif
        private SystemFeature _feature = new SystemFeature();

		void Awake()
		{
            EmojiTools.AddUnityMemory(this);

#if UNITY_EDITOR
            if (OpenDebug)
            {
                EmojiTools.StartDumpGUI();
            }
#endif

            Initialize();
        }

        void Initialize()
		{
            _feature.InitSystem(this,_systemsid);
        }

		private void OnDestroy()
		{
            if(_feature != null)
            {
                _feature.Destroy();
            }
            EmojiTools.RemoveUnityMemory(this);
		}

        internal bool GetParseGroup(InlineText text, out ParseGroup mesh)
        {
            if (_feature != null)
            {
                var resultlist = _feature.Context.Results;
                for (int i = 0; i < resultlist.Count; i++)
                {
                    var info = resultlist[i];
                    if (info.RenderId == text.GetHashCode())
                    {
                        mesh = info.ParseGroup;
                        return true;
                    }
                }
            }
            mesh = default(ParseGroup);
            return false;
        }

        internal bool GetUIMeshGroup(InlineText text, out UIMeshGroup mesh)
        {
            if (_feature != null)
            {
                var resultlist = _feature.Context.Results;
                for (int i = 0; i < resultlist.Count; i++)
                {
                    var info = resultlist[i];
                    if (info.RenderId == text.GetHashCode())
                    {
                        mesh = info.MeshGroup;
                        return true;
                    }
                }
            }
            mesh = default(UIMeshGroup);
            return false;
        }

        internal void Rebuild(InlineText text)
        {
            if(_feature != null &&_feature.Context.RebuildTexts.Contains(text) == false)
            {
                _feature.Context.RebuildTexts.Add(text);
                CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
            }
        }

        internal void CancelRebuild(InlineText text)
        {
            if (_feature != null &&_feature.Context.RebuildTexts.Remove(text) && _feature.Context.RebuildTexts.Count == 0)
            {
                CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
            }
        }

        internal void Remove(InlineText text)
        {
            if (_feature != null )
            {
                CancelRebuild(text);
                int idx = _feature.Context.AllTexts.IndexOf(text);
                if (idx != -1)
                {
                    _feature.Context.AllTexts.RemoveAt(idx);
                    _feature.Context.Results.RemoveAt(idx);
                }
            }
        }

        private void LateUpdate()
        {
            if (_feature != null)
            {
                _feature.DoUpdate(EmojiEvent.LateUpdate);
            }
        }

	    private void OnDrawGizmos()
	    {
            if (_feature != null)
            {
                _feature.DoUpdate(EmojiEvent.DrawGizmos);
            }
        }


        private void Update()
        {
            if (_feature != null)
            {
                _feature.DoUpdate(EmojiEvent.Update);
            }
        }

        public void Rebuild(CanvasUpdate executing)
        {
            if (_feature != null)
            {
                if(executing == CanvasUpdate.PreRender)
                    _feature.DoUpdate(EmojiEvent.PreRender);
                else if(executing == CanvasUpdate.LatePreRender)
                    _feature.DoUpdate(EmojiEvent.LatePreRender);
            }
        }

        public void LayoutComplete()
        {

        }

        public void GraphicUpdateComplete()
        {
            if (_feature != null)
            {
                _feature.DoUpdate(EmojiEvent.GraphicComplete);
            }
        }

        public bool IsDestroyed()
        {
            return this == null;
        }
    }
}


