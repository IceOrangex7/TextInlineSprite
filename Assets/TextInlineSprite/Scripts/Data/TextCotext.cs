using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EmojiUI
{
    public struct TextCotext 
    {
        public InlineManager Owner;

        public ReadSettingsSystem.Settings Settings;

        public EmojiList<InlineText> RebuildTexts;

        public EmojiList<InlineText> AllTexts ;
        /// <summary>
        /// Be carefully if use contains
        /// </summary>
        public EmojiList<TextElementResult> Results ;

        public EmojiList<InlineText> FailedText;

        public EmojiList<SpriteAsset> Refercens;

        public Dictionary<string, SpriteInfoGroup> GroupMapper;

        public static EmojiImageTaskSystem.EmojiImagePool Pool;

        public TextCotext(InlineManager manager, int size)
        {
            Owner = manager;

            Settings = manager.Settings;
            //cl 64-16 48/4=12
            RebuildTexts = new EmojiList<InlineText>(size);
            //cl
            AllTexts = new EmojiList<InlineText>(size);

            Results = new EmojiList< TextElementResult>(size);

            Refercens = new EmojiList<SpriteAsset>();

            GroupMapper = new Dictionary<string, SpriteInfoGroup>();

            FailedText = new EmojiList<InlineText>();
        }


        public void Destroy()
        {
            Owner = null;
            RebuildTexts = null;
            AllTexts = null;
            Results = null;
            Refercens = null;
            GroupMapper = null;
            FailedText = null;
            Pool._pool = null;
        }
    }

    public struct TextElementResult
    {
        /// <summary>
        /// ref to text
        /// </summary>
        public int RenderId;
        /// <summary>
        /// parsed text result
        /// </summary>
  
        public ParseGroup ParseGroup;

        public UIMeshGroup MeshGroup;

        public EmojiImageTaskSystem.EmojiImageInfo EmojiImgeTasks;

        public DrawGizmosSystem.DrawedGizmos GizmosResult;
    }

}
