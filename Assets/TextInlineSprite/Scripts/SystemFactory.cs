using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EmojiUI
{
    public class SystemFactory
    {
        private static SystemFactory _mins;
        public static SystemFactory mIns
        {
            get
            {
                if(_mins == null)
                {
                    _mins = new SystemFactory();
                }
                return _mins;
            }
        }

        private Dictionary<int, Type> _dict = new Dictionary<int, Type>();
        private  Dictionary<int,Type> _defaultexterndict = new Dictionary<int, Type>();
        private List<KeyValuePair<int, string>> _defaults;
        private List<KeyValuePair<int, string>> _systems;
        private List<KeyValuePair<int, string>> _defualtexternsystems;

        private int _did;
        private int _sid;
        private SystemFactory()
        {
            _did = -1;
            AddDefault(typeof(ParseEmojiSystem));
            AddDefault(typeof(ParseTextSystem));
            AddDefault(typeof(CollectAssetsSystem));
            AddDefault(typeof(ReadSettingsSystem));
            _sid = 1;
            AddSystem(typeof(ParseHrefSystem),true);
            AddSystem(typeof(FixParseSystem), true);

            _sid = 100000;
            AddSystem(typeof(TextMeshSystem), true);
            AddSystem(typeof(TextEmojiMeshSystem), true);
            AddSystem(typeof(EmojiImageTaskSystem), true);
            AddSystem(typeof(TextHrefMeshSystem), true);
            AddSystem(typeof(CleanCollectSystem), true);
            AddSystem(typeof(DrawGizmosSystem), true);

        }

        void AddDefault(Type type)
        {
            _dict[_did--] = type;
        }

        void AddSystem(Type type,bool defaultextern =false)
        {
            _dict[_sid++] = type;
            if (defaultextern)
            {
                _defaultexterndict[_sid] = type;
            }
        }

        public List<KeyValuePair<int,string>> GetDefualts()
        {
            if(_defaults == null)
            {
                _defaults = new List<KeyValuePair<int, string>>();

                foreach(var kv in _dict)
                {
                    if(kv.Key < 0)
                    {
                        _defaults.Add(new KeyValuePair<int, string>(kv.Key,kv.Value.Name));
                    }
                }
                _defaults.Sort(Sort);
            }

            return _defaults;
        }

        public List<KeyValuePair<int, string>> GetSystems()
        {
            if (_systems == null)
            {
                _systems = new List<KeyValuePair<int, string>>();

                foreach (var kv in _dict)
                {
                    if (kv.Key > 0)
                    {
                        _systems.Add(new KeyValuePair<int, string>(kv.Key, kv.Value.Name));
                    }
                }

                _systems.Sort(Sort);
            }

            return _systems;
        }

        public List<KeyValuePair<int, string>> GetDefaultExternSystems()
        {
            if (_defualtexternsystems == null)
            {
                _defualtexternsystems = new List<KeyValuePair<int, string>>();

                foreach (var kv in _dict)
                {
                    if (kv.Key > 0)
                    {
                        _defualtexternsystems.Add(new KeyValuePair<int, string>(kv.Key, kv.Value.Name));
                    }
                }

                _defualtexternsystems.Sort(Sort);
            }

            return _defualtexternsystems;
        }

        int Sort(KeyValuePair<int, string> left, KeyValuePair<int, string> right)
        {
            return left.Key - right.Key;
        }

        public IEmojiSystem Create(int id)
        {
            IEmojiSystem system = null;

            Type type;
            if(_dict.TryGetValue(id,out type))
            {
                system = (IEmojiSystem)Activator.CreateInstance(type);
            }

            return system;
        }
    }
}


