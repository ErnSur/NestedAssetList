using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
using UnityEditorInternal;

[CustomPropertyDrawer(typeof(NestedAssetList<>),true)]
public class NestedAssetListPropertyDrawer : PropertyDrawer
{
    private const string _targetListFieldName = "_list";

    private static readonly ReorderableList.Defaults _defaultListDrawer = new ReorderableList.Defaults();

    private static Dictionary<Type, Type[]> _derivedTypes = new Dictionary<Type, Type[]>();
    private Type _subAssetType;
    private ReorderableList _list;

    private int _selected;

    public override bool CanCacheInspectorGUI(SerializedProperty property)
    {
        TryInit(property);
        Debug.Log($"Can Cashe");
        return true;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        TryInit(property);
        return _list.GetHeight();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        TryInit(property);
        _list.DoList(position);
    }

    private void TryInit(SerializedProperty property)
    {
        if (_subAssetType == null)
        {
            _subAssetType = fieldInfo.FieldType
            .GetField(_targetListFieldName, BindingFlags.Instance | BindingFlags.NonPublic)
            .FieldType
            .GetGenericArguments()[0];

            _derivedTypes[_subAssetType] = TypeCache.GetTypesDerivedFrom(_subAssetType).ToArray();
        }

        if (_list == null)
        {
            CreateNewReorderableList(property);
        }
    }

    private void CreateNewReorderableList(SerializedProperty property)
    {
        var listProperty = property.FindPropertyRelative("_list");
        _list = new ReorderableList(listProperty.serializedObject, listProperty);
        _list.drawHeaderCallback = DrawHeader;
        _list.drawElementCallback = DrawElement;
        _list.drawFooterCallback += DrawFooter;

        _list.onReorderCallbackWithDetails += OnReorder;
        _list.onAddCallback += OnAdd;
        _list.onRemoveCallback += OnRemove;
        _list.onCanAddCallback += OnCanAdd;

        void DrawHeader(Rect rect)
        {
            GUI.Label(rect, property.displayName);
        }

        void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.PropertyField(rect, listProperty.GetArrayElementAtIndex(index));
        }

        void DrawFooter(Rect rect)
        {
            var popupRect = rect;
            popupRect.xMax -= 100;
            _selected = EditorGUI.Popup(popupRect, _selected, _derivedTypes[_subAssetType].Select(t => t.ToString()).ToArray());

            _defaultListDrawer.DrawFooter(rect, _list);
        }
    }

    private bool OnCanAdd(ReorderableList list)
    {
        return _derivedTypes[_subAssetType].Length > 0;
    }

    private void OnReorder(ReorderableList list, int oldIndex, int newIndex)
    {
        UpdateSubAssetNames();
    }

    private void OnRemove(ReorderableList list)
    {
        var asset = GetMainAsset();
        asset.RemoveNestedAsset(list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue);
        list.serializedProperty.DeleteArrayElementAtIndex(_list.index);
        list.serializedProperty.DeleteArrayElementAtIndex(_list.index);

        if (list.index >= list.serializedProperty.arraySize - 1)
            list.index = list.serializedProperty.arraySize - 1;

        UpdateSubAssetNames();
    }


    private void OnAdd(ReorderableList list)
    {
        var asset = GetMainAsset();

        var index = list.index == -1 ? list.count : list.index;

        if (asset.TryCreateNestedAsset(_derivedTypes[_subAssetType][_selected], out var subAsset))
        {
            list.serializedProperty.InsertArrayElementAtIndex(index);
            list.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue = subAsset;
        }
        UpdateSubAssetNames();
    }

    private void UpdateSubAssetNames()
    {
        for (int i = 0; i < _list.count; i++)
        {
            _list.serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue.name = GetSubAssetName(i);
        }

        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(GetMainAsset()));

        string GetSubAssetName(int index) => $"{fieldInfo.Name}_{index}";
    }

    private ScriptableObject GetMainAsset()
    {
        var asset = _list.serializedProperty.serializedObject.targetObject as ScriptableObject;
        if (asset == null)
        {
            throw new Exception($"{_list.serializedProperty.serializedObject.targetObject.name} is not a {nameof(ScriptableObject)}");
        }
        return asset;
    }

    private UnityEngine.Object GetSubAsset(int index)
    {
        return _list.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue;
    }
}
