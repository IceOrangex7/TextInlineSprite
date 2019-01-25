/// ========================================================
/// file：InlineText.cs
/// brief：
/// author： coding2233
/// date：
/// version：v1.0
/// ========================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

namespace EmojiUI
{
	public class InlineText : Text
	{
        private string _rendertext;
        public string EmojiText
        {
            get
            {
                if(_rendertext == null)
                {
                    return m_Text;
                }
                return _rendertext;
            }
        }

        public override float preferredWidth
        {
            get
            {
                var settings = GetGenerationSettings(Vector2.zero);
                float wid = cachedTextGeneratorForLayout.GetPreferredWidth(EmojiText, settings) / pixelsPerUnit;
                if (horizontalOverflow == HorizontalWrapMode.Overflow)
                {
                    return wid;
                }
                else
                {
                    return Mathf.Min(wid, rectTransform.rect.size.x);
                }

            }
        }

        private InlineManager _manager;
        public InlineManager Manager
        {
            get
            {
                if(_manager == null)
                {
                    _manager = GetComponentInParent<InlineManager>();
                }
                return _manager;
            }
        }

        public override float preferredHeight
		{
			get
			{
				var settings = GetGenerationSettings(new Vector2(GetPixelAdjustedRect().size.x, 0.0f));
				float height = cachedTextGeneratorForLayout.GetPreferredHeight(EmojiText, settings) / pixelsPerUnit;
                return height;
            }
		}

		void OnDrawGizmos()
		{
            var oldcol = Gizmos.color;
			Gizmos.color = Color.blue;

			var corners = new Vector3[4];
			rectTransform.GetWorldCorners(corners);

			Gizmos.DrawLine(corners[0], corners[1]);
			Gizmos.DrawLine(corners[1], corners[2]);
			Gizmos.DrawLine(corners[2], corners[3]);
			Gizmos.DrawLine(corners[3], corners[0]);

            if(Manager && Manager.Settings.OpenDrawPreferred)
            {
                Gizmos.color = Color.cyan;

                var wid = preferredWidth;
                var height = preferredHeight;
                Vector3 point = Vector3.zero;// transform.InverseTransformPoint(center);
                int alignH = (int)alignment % 3; // 左中右 0，1，2 // 0 => 1.5  1=>1 2 =>-1.5
                int alignV = (int)alignment / 3;// 上中下  0,1,2   //
                Vector3 p1, p2, p3, p4;
                p1 = p2 = p3 = p4 = Vector3.zero;

                //使用objectspace
                rectTransform.GetLocalCorners(corners);

                float deltax = -0.5f;
                float deltay = -0.5f;
                float deltarx = 0.5f;
                float deltary = 0.5f;

                point.x = alignH == 1 ? (corners[0] + corners[3]).x / 2 : (corners[alignH] + corners[(alignH + 1) % 4]).x / 2;
                point.y = alignV == 1 ? (corners[1] + corners[0]).y / 2 : (corners[(alignV + 1) % 4] + corners[(alignV + 2) % 4]).y / 2;
                if (alignH > 1) //2
                {
                    deltax = -1f;
                    deltarx = 0f;
                }
                else if (alignH < 1) //0
                {
                    deltax = 0f;
                    deltarx = 1f;
                }

                if (alignV > 1)
                {
                    deltay = 1f;
                    deltary = 0f;
                }
                else if (alignV < 1)
                {
                    deltay = 0f;
                    deltary = -1f;
                }


                p1 = transform.TransformPoint(point + new Vector3(deltax * wid, deltary * height));
                p2 = transform.TransformPoint(point + new Vector3(deltarx * wid, deltary * height));
                p3 = transform.TransformPoint(point + new Vector3(deltarx * wid, deltay * height));
                p4 = transform.TransformPoint(point + new Vector3(deltax * wid, deltay * height));

                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p4);
                Gizmos.DrawLine(p4, p1);

#if UNITY_EDITOR
                UnityEditor.Handles.Label((p1 + p2) / 2, "wid=" + wid);
                UnityEditor.Handles.Label((p3 + p2) / 2, "height=" + height);
#endif
            }

            Gizmos.color = oldcol;
#if UNITY_EDITOR
            DrawDebug();
#endif
        }

        protected override void Start()
		{
			base.Start();

			EmojiTools.AddUnityMemory(this);
		}

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public override void SetVerticesDirty()
		{
            if (font == null)
                return;

            if (Application.isPlaying && this.isActiveAndEnabled )
			{
                if(!m_DisableFontTextureRebuiltCallback )
                {
                    if (Manager!= null)
                    {
                        Manager.Rebuild(this);
                    }
                }
			    base.SetVerticesDirty();
            }
			else
			{
			    base.SetVerticesDirty();
            }
		}

        protected override void OnDestroy()
		{
			base.OnDestroy();

            StopAllCoroutines();


            EmojiTools.RemoveUnityMemory(this);
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			if (font == null)
				return;

            ParseGroup textresult;
            UIMeshGroup mesh;
            if (Application.isPlaying && isActiveAndEnabled && Manager != null
             && Manager.GetUIMeshGroup(this,out mesh) && Manager.GetParseGroup(this,out textresult))
            {
                m_DisableFontTextureRebuiltCallback = true;

                toFill.Clear();
                RenderText(toFill, ref mesh);
                _rendertext = textresult.TextResult.FormatString;

                m_DisableFontTextureRebuiltCallback = false;
            }
            else
            {
                _rendertext = null;
                base.OnPopulateMesh(toFill);
            }
		}

        void RenderText(VertexHelper toFill,ref UIMeshGroup meshgroup)
        {
            var mesh = meshgroup.TextMeshResult;
            if (mesh.TextVerts != null)
            {
                for (int i = 0; i < mesh.TextVerts.Count; i +=4)
                {
                    int vertcnt = toFill.currentVertCount;
                    toFill.AddVert(mesh.TextVerts[i]);
                    toFill.AddVert(mesh.TextVerts[i+1]);
                    toFill.AddVert(mesh.TextVerts[i +2]);
                    toFill.AddVert(mesh.TextVerts[i +3]);

                    toFill.AddTriangle(vertcnt, vertcnt + 1, vertcnt + 2);
                    toFill.AddTriangle(vertcnt + 2, vertcnt + 3, vertcnt);
                }
            }
        }

#if UNITY_EDITOR
        DrawGizmosSystem.DrawedGizmos? _dgData;
        public void DrawGizmos(ref DrawGizmosSystem.DrawedGizmos data)
        {
            _dgData = data;
        }

        void DrawDebug()
        {
            if(_dgData.HasValue && isActiveAndEnabled)
            {
                var col = Gizmos.color;
                if (_dgData.Value.Quads != null && _dgData.Value.Quads.Count > 1)
                {
                    Color whitecol = Color.white;
                    for (int i = 0; i < _dgData.Value.Quads.Count; i+=4)
                    {
                        Gizmos.color = new Color32((byte)((0 + i *10) %256), (byte)((0 + i * 10) % 256), 255, 255);
                        DrawLine(_dgData.Value.Quads[i], _dgData.Value.Quads[i + 1]);
                        DrawLine(_dgData.Value.Quads[i+1], _dgData.Value.Quads[i + 2]);
                        DrawLine(_dgData.Value.Quads[i+2], _dgData.Value.Quads[i + 3]);
                        DrawLine(_dgData.Value.Quads[i +3], _dgData.Value.Quads[i ]);
                    }
                }

                if(_dgData.Value.QuadLabel != null&& _dgData.Value.QuadLabel.Count >0)
                {
                    for (int i = 0; i < _dgData.Value.QuadLabel.Count; i++)
                    {
                        var data = _dgData.Value.QuadLabel[i];
                        UnityEditor.Handles.Label(data.Key, data.Value);
                    }
                }

                Gizmos.color = _dgData.Value.color;
            }
        }
#endif

        void DrawLine(Vector3 s1,Vector3 s2)
        {
            Gizmos.DrawLine(transform.TransformPoint(s1), transform.TransformPoint(s2));
        }
    }
}





