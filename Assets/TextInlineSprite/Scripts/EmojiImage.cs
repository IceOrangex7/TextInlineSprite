using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EmojiUI
{
    public class EmojiImage : Image
    {

        private bool _rendering;
        private InlineText _cachetext;
        private Texture _texture;

        public override Texture mainTexture
        {
            get
            {
                if (_rendering && _texture != null)
                {
                    return _texture;
                }
                return base.mainTexture;
            }
        }

        public override Material defaultMaterial
        {
            get
            {
                return EmojiRenderHelper.GetDefaultMaterial();
            }
        }

        private UIMeshGroup _meshgroup;

        public void RenderEmoji(InlineText text,Texture texture, ref UIMeshGroup mesh)
        {
            _texture = texture;
            _cachetext = text;
            _meshgroup = mesh;
            _rendering = true;

            SetVerticesDirty();
        }

        public Texture GetRenderTexture()
        {
            return _texture;
        }

        public void UnRenderEmoji()
        {
            if (_rendering)
            {
                _meshgroup = default(UIMeshGroup);
                SetVerticesDirty();
            }
            _texture = null;
            _rendering = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _texture = null;
            _cachetext = null;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {

            if (IsActive() && _rendering && _cachetext != null && _meshgroup.CanRender())
            {
                toFill.Clear();

                if (_meshgroup.CanRenderEmoji())
                {
                    RenderEmoji(toFill);
                }

                if (_meshgroup.CanRenderHref())
                {
                    RenderHref(toFill);
                }
            }
            else
            {
                toFill.Clear();
            }
            //else
            //{
            //    base.OnPopulateMesh(toFill);
            //}
        }

        void RenderEmoji(VertexHelper toFill)
        {
            for (int i = 0; i < _meshgroup.EmojiMeshResult.EmojiVerts.Count; i += 4)
            {
                int vertcnt = toFill.currentVertCount;
                var vert = _meshgroup.EmojiMeshResult.EmojiVerts[i];
                if (_texture != null && vert.texture == _texture)
                {
                    toFill.AddVert(GetUiVert(vert, 50));
                    toFill.AddVert(GetUiVert(_meshgroup.EmojiMeshResult.EmojiVerts[i + 1], 50));
                    toFill.AddVert(GetUiVert(_meshgroup.EmojiMeshResult.EmojiVerts[i + 2], 50));
                    toFill.AddVert(GetUiVert(_meshgroup.EmojiMeshResult.EmojiVerts[i + 3], 50));

                    toFill.AddTriangle(vertcnt + 2, vertcnt + 1, vertcnt);
                    toFill.AddTriangle(vertcnt, vertcnt + 3, vertcnt + 2);
                }
            }
        }

        void RenderHref(VertexHelper toFill)
        {
            for (int i = 0; i < _meshgroup.HrefMeshReult.HrefVerts.Count; i += 4)
            {
                int vertcnt = toFill.currentVertCount;
                toFill.AddVert(GetUiVert(_meshgroup.HrefMeshReult.HrefVerts[i],-5));
                toFill.AddVert(GetUiVert(_meshgroup.HrefMeshReult.HrefVerts[i + 1], -5));
                toFill.AddVert(GetUiVert(_meshgroup.HrefMeshReult.HrefVerts[i + 2], -5));
                toFill.AddVert(GetUiVert(_meshgroup.HrefMeshReult.HrefVerts[i + 3], -5));


                toFill.AddTriangle(vertcnt + 2, vertcnt + 1, vertcnt);
                toFill.AddTriangle(vertcnt, vertcnt + 3, vertcnt + 2);
            }
        }

        UIVertex GetUiVert(EmojiUIVertex vert)
        {
            UIVertex uivert = new UIVertex();
            Vector3 worldpos = _cachetext.transform.TransformPoint(vert.position);
            Vector3 localpos = transform.InverseTransformPoint(worldpos);
            uivert.position = localpos;
            uivert.normal = vert.normal;
            uivert.color = vert.color;
            uivert.tangent = vert.tangent;
            uivert.uv0 = vert.uv0;
            uivert.uv1 = vert.uv1;
            uivert.uv2 = vert.uv2;
            uivert.uv3 = vert.uv3;

            return uivert;
        }

        UIVertex GetUiVert(EmojiUIVertex vert,float fixz)
        {
            UIVertex uivert = new UIVertex();
            Vector3 worldpos = _cachetext.transform.TransformPoint(vert.position);
            Vector3 localpos = transform.InverseTransformPoint(worldpos);
            uivert.position = new Vector3(localpos.x,localpos.y, fixz);
            uivert.normal = vert.normal;
            uivert.color = vert.color;
            uivert.tangent = vert.tangent;
            uivert.uv0 = vert.uv0;
            uivert.uv1 = vert.uv1;
            uivert.uv2 = vert.uv2;
            uivert.uv3 = vert.uv3;

            return uivert;
        }

        UIVertex GetUiWorldVert(EmojiUIVertex vert)
        {
            UIVertex uivert = new UIVertex();
            Vector3 worldpos = _cachetext.transform.TransformPoint(vert.position);
            uivert.position = worldpos;
            uivert.normal = vert.normal;
            uivert.color = vert.color;
            uivert.tangent = vert.tangent;
            uivert.uv0 = vert.uv0;
            uivert.uv1 = vert.uv1;
            uivert.uv2 = vert.uv2;
            uivert.uv3 = vert.uv3;

            return uivert;
        }

        void OnDrawGizmos()
        {
            if ( _cachetext != null)
            {
                var oldcol = Gizmos.color;

                if (_meshgroup.CanRenderEmoji())
                {
                    Gizmos.color = Color.red;
                    for (int i = 0; i < _meshgroup.EmojiMeshResult.EmojiVerts.Count; i += 4)
                    {
                        var p1 = GetUiWorldVert(_meshgroup.EmojiMeshResult.EmojiVerts[i]).position;
                        var p2 = GetUiWorldVert(_meshgroup.EmojiMeshResult.EmojiVerts[i + 1]).position;
                        var p3 = GetUiWorldVert(_meshgroup.EmojiMeshResult.EmojiVerts[i + 2]).position;
                        var p4 = GetUiWorldVert(_meshgroup.EmojiMeshResult.EmojiVerts[i + 3]).position;

                        Gizmos.DrawLine(p1, p2);
                        Gizmos.DrawLine(p2, p3);
                        Gizmos.DrawLine(p3, p4);
                        Gizmos.DrawLine(p4, p1);

#if UNITY_EDITOR
                        UnityEditor.Handles.Label((p1 + p2 + p3 + p4) / 4, (i / 4).ToString());
#endif
                    }
                }

                if (_meshgroup.CanRenderHref())
                {
                    Gizmos.color = Color.green;
                    for (int i = 0; i < _meshgroup.HrefMeshReult.HrefVerts.Count; i += 4)
                    {
                        var p1 = GetUiWorldVert(_meshgroup.HrefMeshReult.HrefVerts[i]).position;
                        var p2 = GetUiWorldVert(_meshgroup.HrefMeshReult.HrefVerts[i + 1]).position;
                        var p3 = GetUiWorldVert(_meshgroup.HrefMeshReult.HrefVerts[i + 2]).position;
                        var p4 = GetUiWorldVert(_meshgroup.HrefMeshReult.HrefVerts[i + 3]).position;

                        Gizmos.DrawLine(p1, p2);
                        Gizmos.DrawLine(p2, p3);
                        Gizmos.DrawLine(p3, p4);
                        Gizmos.DrawLine(p4, p1);
                    }
                }
                Gizmos.color = oldcol;
            }
        }
    }

}

