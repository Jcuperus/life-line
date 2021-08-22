using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rooms
{
    [CreateAssetMenu(fileName = "TileDictionary", menuName = "ScriptableObjects/TileDictionary", order = 1)]
    public class TileDictionaryScriptableObject : ScriptableObject
    {
        [Serializable]
        public class TileAsset
        {
            public TileBase tile;
            public GameObject asset;
        }
        
        [SerializeField] private TileAsset[] tileAssets;
        
        private Dictionary<TileBase, GameObject> tileDictionary;
        private Dictionary<TileBase, GameObject> TileDictionary => tileDictionary ??= GetTileDictionary();

        private Dictionary<TileBase, GameObject> GetTileDictionary()
        {
            var dictionary = new Dictionary<TileBase, GameObject>();

            foreach (TileAsset tileAsset in tileAssets)
            {
                dictionary.Add(tileAsset.tile, tileAsset.asset);
            }

            return dictionary;
        }

        public GameObject this[TileBase key] => TileDictionary[key];

        public bool ContainsKey(TileBase key)
        {
            return TileDictionary.ContainsKey(key);
        }
    }
}