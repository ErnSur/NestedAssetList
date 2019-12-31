using UnityEngine;
using UnityEditor;
using System;

public static class NestedAssetExtensions
{
    public static bool TryCreateNestedAsset(this ScriptableObject so, Type type, out UnityEngine.Object asset)
    {
        if (!typeof(ScriptableObject).IsAssignableFrom(type))
        {
            Debug.Log($"{type}: does not derive from {nameof(ScriptableObject)}");
            asset = null;
            return false;
        }

        asset = ScriptableObject.CreateInstance(type);

        AssetDatabase.AddObjectToAsset(asset, so);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(so));
        return true;
    }

    public static void RemoveNestedAsset(this ScriptableObject so, UnityEngine.Object asset)
    {
        var soPath = AssetDatabase.GetAssetPath(so);
        var assetIsNested = asset != so && soPath == AssetDatabase.GetAssetPath(asset);

        if (assetIsNested)
        {
            AssetDatabase.RemoveObjectFromAsset(asset);
            AssetDatabase.ImportAsset(soPath);
        }
    }
}
