using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EmojiUI;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;
using System.Linq;

[CanEditMultipleObjects]
[CustomEditor(typeof(InlineManager),true)]
public class InlineManagerEditor : Editor
{

    private SerializedProperty _systemsid;
    private SerializedProperty _openDebug;
    private SerializedProperty SharedAssets;
    private SerializedProperty Settings;

    private Dictionary<int, string> _systemmap;
    private List<int> removelist = new  List<int>();

    Vector2 _scrollpos;
    int _addval;

    private void OnDisable()
    {

    }

    private void OnEnable()
    {
        _systemsid = serializedObject.FindProperty("_systemsid");
        _openDebug = serializedObject.FindProperty("_openDebug");
        SharedAssets = serializedObject.FindProperty("SharedAssets");
        Settings = serializedObject.FindProperty("Settings");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(_openDebug,new GUIContent("Debug Mode"));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(Settings,true);
        EditorGUILayout.PropertyField(SharedAssets,true);

        InitMap();
        DrawSystems();

        serializedObject.ApplyModifiedProperties();
    }


    void InitMap()
    {
        if(_systemmap == null)
        {
            _systemmap = new Dictionary<int, string>();

            int size = SystemFactory.mIns.GetDefualts().Count;
            for (int i = 0; i <size; i++)
            {
                var info = SystemFactory.mIns.GetDefualts()[i];
                _systemmap[info.Key] = info.Value;
                AddDefault(info.Key);
            }


            size = SystemFactory.mIns.GetSystems().Count;
            for (int i = 0; i < size; i++)
            {
                var info = SystemFactory.mIns.GetSystems()[i];
                _systemmap[info.Key] = info.Value;
            }

        }
    }

    void AddDefault(int defaultid)
    {
        int size = _systemsid.arraySize;
        for (int i = size - 1; i >= 0; i--)
        {
            var id = _systemsid.GetArrayElementAtIndex(i).intValue;
            if(id == defaultid)
            {
                return;
            }
        }

        _systemsid.arraySize++;
        _systemsid.GetArrayElementAtIndex(size).intValue = defaultid;
    }

    void AddSystem(int newid)
    {
        if (newid <= 0)
            return;

        if (_systemmap.ContainsKey(newid) == false)
            return;

        int size = _systemsid.arraySize;
        int insertpoint = SystemFactory.mIns.GetDefualts().Count;

        for (int i = insertpoint; i < _systemsid.arraySize; i++)
        {
            var id = _systemsid.GetArrayElementAtIndex(i).intValue;
            if(id == newid)
            {
                insertpoint = -1;
                break;
            }
            else if(id > newid)
            {
                break;
            }
            insertpoint++;
        }

        if(insertpoint >0)
        {

            _systemsid.InsertArrayElementAtIndex(insertpoint);
            _systemsid.GetArrayElementAtIndex(insertpoint).intValue = newid;
        }

    }

    void DrawSystems()
    {
        if(_systemsid != null && _systemsid.arraySize >0)
        {
            EditorGUILayout.HelpBox("越大越先执行。", MessageType.Info);

            _scrollpos = EditorGUILayout.BeginScrollView(_scrollpos,GUILayout.MinHeight(100));

            int size = _systemsid.arraySize;
            for (int i = 0;  i < size; i++)
            {
                var id = _systemsid.GetArrayElementAtIndex(i).intValue;
                DrawSystem(id, i);
            }

            for (int i = removelist.Count-1; i >=0; i--)
            {
                _systemsid.DeleteArrayElementAtIndex(removelist[i]);
            }
            removelist.Clear();

            EditorGUILayout.EndScrollView();

            GUILayout.Space(2);

            DrawAddText();
        }

    }

    void Swap(int left,int right)
    {
        if(left >= 0 && left < SystemFactory.mIns.GetDefualts().Count)
        {
            return;
        }

        if(right >=0 && right < SystemFactory.mIns.GetDefualts().Count)
        {
            return;
        }

        if(left < _systemsid.arraySize && right < _systemsid.arraySize)
        {
            int ltval = _systemsid.GetArrayElementAtIndex(left).intValue;
            int rtval = _systemsid.GetArrayElementAtIndex(right).intValue;

            _systemsid.GetArrayElementAtIndex(left).intValue = rtval;
            _systemsid.GetArrayElementAtIndex(right).intValue = ltval;
        }
    }


    void DrawSystem(int id,int idx)
    {
        string empty = null;
        if(!_systemmap.TryGetValue(id,out empty))
        {
            empty = "Unknown System";
        }

        EditorGUILayout.BeginHorizontal(GUILayout.Width(400));
        GUILayout.Label(idx.ToString(), GUILayout.Width(30));
        GUILayout.Label(empty, GUILayout.Width(120));

        if(GUILayout.Button("", "OL Minus",GUILayout.Width(50)))
        {
            if (idx >= SystemFactory.mIns.GetDefualts().Count)
                removelist.Add(idx);
        }

        if (GUILayout.Button("Up", GUILayout.Width(50)))
        {
            Swap(idx, idx -1);
        }

        if (GUILayout.Button("Down", GUILayout.Width(50)))
        {
            Swap(idx, idx +1);
        }


        EditorGUILayout.EndHorizontal();
    }

    void DrawAddText()
    {
        GUILayout.BeginHorizontal();

        _addval = EditorGUILayout.IntField(_addval, GUILayout.Width(200));

        if(GUILayout.Button("add",GUILayout.Width(100)) && _addval >= 0)
        {
            AddSystem(_addval);
        }
        GUILayout.EndVertical();

        if (GUILayout.Button("add all default ", GUILayout.Width(100)))
        {
            foreach (var sys in SystemFactory.mIns.GetDefaultExternSystems() )
            {
                AddSystem(sys.Key);
            }
        }
    }
}
