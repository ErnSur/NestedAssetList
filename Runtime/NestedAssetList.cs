using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// List of asset files that are nested inside ScriptableObject
/// Special property drawer allows you to easily add and remove nested assets 
/// Use only in ScriptableObjects classes
/// </summary>
/// <typeparam name="T">Nested asset type</typeparam>
/// 
[Serializable]
public class NestedAssetList<T> where T : ScriptableObject
{
    // Expose the _list the way you want
    // it can be made public but keep in mind that
    // for each list element corresponding asset should exist.
    [SerializeField]
    private List<T> _list = new List<T>();
}
