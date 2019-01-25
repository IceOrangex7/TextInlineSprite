using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EmojiUI
{
    public class EmojiImageTaskSystem : IEmojiStructSystem<TextCotext>
    {

        public struct EmojiImagePool
        {
            public GameObject _pool;

            public void RemoveChildFromTarget(EmojiImage emojiimg)
            {
                InitPool(ref TextCotext.Pool);

                if (emojiimg != null)
                {
                    emojiimg.transform.SetParent(TextCotext.Pool._pool.transform);
                }
            }

            public EmojiImage AddChild2Target( Transform transform)
            {
                InitPool(ref TextCotext.Pool);
                if (transform != null)
                {
                    Transform pooltrans = TextCotext.Pool._pool.transform;
                    if (pooltrans.childCount > 0)
                    {
                        var trans = pooltrans.GetChild(0);
                        EmojiImage emojiimg = trans.GetComponent<EmojiImage>();
                        trans.SetParent(transform);
                        return emojiimg;
                    }
                    else
                    {
                        GameObject gameobject = new GameObject();
                        EmojiImage emojiimg = gameobject.AddComponent<EmojiImage>();
                        emojiimg.raycastTarget = false;
                        gameobject.transform.SetParent(transform);
                        return emojiimg;
                    }
                }
                return null;
            }

            public void InitPool(ref EmojiImagePool pool)
            {
                if (pool._pool == null)
                {
                    pool._pool = new GameObject("EmojiPool");

                    for (int i = 0; i < 10; i++)
                    {
                        GameObject gameobject = new GameObject("EmojiImg");
                        EmojiImage  img =gameobject.AddComponent<EmojiImage>();
                        img.raycastTarget = false;
                        gameobject.transform.SetParent(pool._pool.transform);
                    }
                }
            }
        }

        public struct EmojiImageInfo
        {
            public List<EmojiImage> Images;

            public bool NeedUpdate;
        }

        public int DoStep(ref TextCotext context, EmojiEvent uievent)
        {
            if (uievent == EmojiEvent.PreRender)
            {
                for (int i = 0; i < context.AllTexts.Count; i++)
                {
                    var text = context.AllTexts[i];
                    if (text == null)
                        continue;

                    var result = context.Results[i];

                    var task = result.EmojiImgeTasks;
                    task.NeedUpdate = context.RebuildTexts.Contains(text);
                    result.EmojiImgeTasks = task;

                    context.Results[i] = result;
                }

                //clear
                context.RebuildTexts.Clear();
            }
            else if (uievent == EmojiEvent.LateUpdate)
            {
                for (int i = 0; i < context.AllTexts.Count; i++)
                {
                    var text = context.AllTexts[i];
                    if(text == null)
                        continue;

                    var result = context.Results[i];
                    var task = result.EmojiImgeTasks;
                    bool canrender = result.MeshGroup.CanRender();

                    if (canrender)
                    {
                        if (task.Images == null)
                        {
                            task.Images = new EmojiList<EmojiImage>();
                        }

                        if (text.isActiveAndEnabled && task.NeedUpdate)
                        {
                            var list =  ListPool<int>.Get();
                            for (int j = 0; j < task.Images.Count; j++)
                            {
                                if (j < list.Count)
                                {
                                    list[j] = 1;
                                }
                                else
                                {
                                    list.Add(1);
                                }
                            }

                            for (int j = 0; j < result.MeshGroup.EmojiMeshResult.EmojiVerts.Count; j+=4)
                            {
                                var vert = result.MeshGroup.EmojiMeshResult.EmojiVerts[j];
                                var texture = vert.texture;
                                
                                if (texture != null )
                                {
                                    int idx = GetRenderIndex(texture, ref task);
                                    if (idx >= 0)
                                    {
                                        EmojiImage emojimg = task.Images[idx];
                                        emojimg.RenderEmoji(text, texture, ref result.MeshGroup);

                                        list[idx] = 0;
                                    }
                                    else
                                    {
                                        int freeidx = GetNullRenderIndex(texture, ref task);
                                        if (freeidx >= 0)
                                        {
                                            list[freeidx] = 0;
                                            EmojiImage emojimg = task.Images[freeidx];
                                            emojimg.RenderEmoji(text, texture, ref result.MeshGroup);
                                        }
                                        else
                                        {
                                            EmojiImage emojimg = TextCotext.Pool.AddChild2Target(text.transform);
                                            emojimg.RenderEmoji(text, texture, ref result.MeshGroup);
                                            task.Images.Add(emojimg);
                                        }
                                    }
                                }
                            }

                            for (int j = 0; j < list.Count; j++)
                            {
                                if (list[j] == 1)
                                {
                                    task.Images[j].UnRenderEmoji();
                                }
                            }

                            ListPool<int>.Release(list);
                        }
                        else if(!text.isActiveAndEnabled)
                        {
                            for (int j = 0; j < task.Images.Count; j++)
                            {
                                task.Images[j].UnRenderEmoji();
                            }
                        }

                        task.NeedUpdate = false;
                    }
                    else if(task.Images != null && !canrender)
                    {
                        for (int j = task.Images.Count -1; j >=0 ; j--)
                        {
                            TextCotext.Pool.RemoveChildFromTarget(task.Images[j]);
                            task.Images.RemoveAt(j);
                        }
                    }

                    result.EmojiImgeTasks = task;
                    context.Results[i] = result;
                }
            }
            return 0;
        }

        public int Init(ref TextCotext context)
        {
            return 0;
        }

        int GetRenderIndex(Texture texture, ref EmojiImageInfo task)
        {
            for (int j = 0; j < task.Images.Count; j++)
            {
                if (task.Images[j].GetRenderTexture() == texture)
                {
                    return j;
                }
            }
            return -1;
        }

        int GetNullRenderIndex(Texture texture, ref EmojiImageInfo task)
        {
            for (int j = 0; j < task.Images.Count; j++)
            {
                if (task.Images[j].GetRenderTexture() == null)
                {
                    return j;
                }
            }
            return -1;
        }

    }
}


