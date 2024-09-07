// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

namespace SneakerWorld.Decoration {

    [ExecuteInEditMode]
    public class FloorGenerator : MonoBehaviour {

        public Tilemap tilemap;
        public TileBase tilebase;

        public int baseWidth;
        public int baseHeight;

        public int widthPerLevel;
        public int heightPerLevel;

        public int level;

        void Update() {
            if (!Application.isPlaying) {

                tilemap.ClearAllTiles();
                for (int i = -baseHeight; i < heightPerLevel * level; i++) {
                    for (int j = -baseWidth; j < widthPerLevel * level; j++) {
                        Vector3Int pos = new Vector3Int(j, i, 0);
                        tilemap.SetTile(pos, tilebase);
                    }
                }

            }
        }

        
    }

}
