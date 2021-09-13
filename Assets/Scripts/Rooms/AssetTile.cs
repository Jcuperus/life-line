using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Rooms
{
    public class AssetTile : Tile
    {
        public GameObject tileAsset;

#if UNITY_EDITOR
        [MenuItem("Assets/Create/AssetTile")]
        public static void CreateAssetTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Asset Tile", "AssetTile", "Asset",
                "Save Road Tile", "Assets");
            if (path == "") return;
            AssetDatabase.CreateAsset(CreateInstance<AssetTile>(), path);
        }
#endif
    }
}