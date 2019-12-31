using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditorInternal;

public class NestedAssetAttribute : PropertyAttribute { }

[System.Serializable]
public class NestedAssetList<T> : List<T> where T : ScriptableObject { }

[CustomPropertyDrawer(typeof(NestedAssetAttribute))]
public class NestedAssetPropertyDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		//Debug.Log($"{property.propertyPath}");
		return base.GetPropertyHeight(property, label);
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		//Debug.Log($"{property.propertyPath}");

		base.OnGUI(position, property, label);
	}
}

[CustomPropertyDrawer(typeof(NestedAssetList<>))]
public class NestedAssetListPropertyDrawer : PropertyDrawer
{
	private ReorderableList _list;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		Debug.Log($"HAVE Array {property.FindPropertyRelative("Array")}");
		while (property.Next(true))
		{
			Debug.Log($"{property.propertyPath}");

		}
		base.OnGUI(position, property, label);
	}
	//public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	//{
	//	if(_list == null)
	//	{
	//		//Debug.Log($"H null");
	//		//CreateNewReorderableList(property);
	//	}
	//	return _list.GetHeight();
	//}

	//public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	//{
	//	if (_list == null)
	//	{
	//		//Debug.Log($"G null");
	//		//CreateNewReorderableList(property);
	//	}
	//	_list.DoList(position);
	//}

	private void CreateNewReorderableList(SerializedProperty property)
	{
		Debug.Log($"Prop is {property.isArray}, {property.propertyType}");
		_list = new ReorderableList(property.serializedObject, property);
	}
}

/*
public abstract class NestedAssetEditor<TMainAsset,TSubAsset> : Editor 
	where TMainAsset : ScriptableObject
	where TSubAsset : ScriptableObject
{
	private TMainAsset _targetAsset;

	private (Type type, string name)[] _derivedTypesOfSubAsset;

	private SerializedObject serObj;

	private Vector2 mouseScrollPos;

	private List<bool> itemsFoldouts = new List<bool>();

	private bool contextMenu;
	private int selected = 0;

	protected virtual void OnEnable()
	{
		_targetAsset = target as TMainAsset;

		_derivedTypesOfSubAsset = GetSubTypes().Select(t=>(t,t.ToString())).ToArray();

		AddFoldouts(_targetAsset.effects.Count);
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GUILayout.Space(10);

		ListHeader(_targetAsset.effects);

		DisplayListElements(_targetAsset.effects);
	}


	private void ListHeader<T>(List<T> serializedList) where T : ScriptableObject
	{
		GUILayout.BeginHorizontal("U2D.createRect");

		GUILayout.Label(typeof(T).ToString());

		//selected = EditorGUILayout.Popup("Add", selected, options);
		//GUILayout.Button("Add", "MiniPullDown");
		AddSubtypeButton(serializedList);

		GUILayout.EndHorizontal();
	}

	private void AddSubtypeButton<T>(List<T> serializedList) where T : ScriptableObject
	{
		GUILayout.BeginHorizontal();
		selected = EditorGUILayout.Popup(selected, _derivedTypesOfSubAssetNames);

		if (GUILayout.Button("Add"))
		{
			CreateSubAsset(serializedList, _derivedTypesOfSubAsset[selected]);
		}

		GUILayout.EndHorizontal();

	}

	private void DisplayListElements<T>(List<T> serializedList) where T : ScriptableObject
	{
		if (serializedList != null)
		{
			for (int i = 0; i < serializedList.Count; i++)
			{
				GUILayout.Space(3);
				GUILayout.BeginVertical();

				#region ItemLabel
				GUILayout.BeginHorizontal("TE NodeBox");

				EditorGUI.indentLevel++;
				itemsFoldouts[i] = EditorGUILayout.Foldout(itemsFoldouts[i], serializedList[i].ToString());
				EditorGUI.indentLevel--;
				if (GUILayout.Button("delete"))
				{
					serObj = null;
					DestroyImmediate(_targetAsset.effects[i], true);
					_targetAsset.effects.Remove(_targetAsset.effects[i]);
					itemsFoldouts.RemoveAt(i);
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_targetAsset));
					return;
				}

				GUILayout.EndHorizontal();
				#endregion

				if (itemsFoldouts[i])
				{
					#region Properties

					serObj = new SerializedObject(_targetAsset.effects[i]);

					SerializedProperty props = serObj.GetIterator();

					props.Reset();
					int iterator = 0;
					if (props.NextVisible(true))
					{
						do
						{
							if (props.name == "m_Script")
							{
								continue;
							}

							serObj.Update();
							GUILayout.BeginVertical("flow overlay box");
							EditorGUI.indentLevel++;

							EditorGUILayout.PropertyField(props, true);

							EditorGUI.indentLevel--;
							GUILayout.EndVertical();
							serObj.ApplyModifiedProperties();

							++iterator;
						}
						while (props.NextVisible(false));
					}

					#endregion
				}

				GUILayout.EndVertical();
			}
		}
	}


	private void CreateSubAsset<T>(List<T> serializedList, Type type) where T : ScriptableObject
	{
		ScriptableObject newObject = CreateInstance(type.ToString());
		newObject.name = string.Format("{0}_{1}", type.ToString(), _targetAsset.effects.Count);

		AssetDatabase.AddObjectToAsset(newObject, _targetAsset);

		serializedList.Add(newObject as T);
		itemsFoldouts.Add(false);

		AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_targetAsset));
	}

	private void AddFoldouts(int count)
	{
		for (int i = 0; i < count; i++)
		{
			itemsFoldouts.Add(false);
		}
	}

	private Type[] GetSubTypes()
	{
		return (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(TSubAsset))
				select assemblyType).ToArray();
	}
}
*/
