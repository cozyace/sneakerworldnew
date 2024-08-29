// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

namespace SneakerWorld.UI {

    /// <summary>
    /// Listens to the store and updates the UI accordingly. 
    /// </summary>
    [ExecuteInEditMode]
    public class UIScrollTiling : MonoBehaviour {

        // The bounds within which we must display the tiles. 
        [SerializeField]
        private RectTransform bounds = null;

        // The space from the edges.
        public int columns = 2;

        // If we want to get the size ratio from a demo component.
        public RectTransform placeholder;

        // The space from the edges.
        public float rowToColumnRatio = 1f;

        // The space from the edges.
        public Vector2 edgeBuffer = new Vector2(0f, 0f);

        // The space between each tile.
        public Vector2 tileBuffer = new Vector2(0f, 0f);

        // The list of stuff that it is meant to tile.
        public List<RectTransform> rts;

        void Update() {
            if (!Application.isPlaying) {
                UpdateWhileInEditMode();
            }
        } 

        void UpdateWhileInEditMode() {
            if (bounds == null) {
                bounds = GetComponent<RectTransform>();
            }

            rts = new List<RectTransform>();
            foreach (Transform child in transform) {
                RectTransform rt = child.GetComponent<RectTransform>();
                if (rt != null && rt != placeholder) {
                    rts.Add(rt);
                }
            }

            if (placeholder != null) {
                rowToColumnRatio = placeholder.rect.height / placeholder.rect.width;
            }

            ResizeRects();
            RepositionRects();

        }

        // Conforms all the elements to the correct size. 
        void ResizeRects() {

            // Calculate the width and height of the tiles.
            Vector2 tileDim = GetTileDimensions();

            // Itterate and check.
            for (int i = 0; i < rts.Count; i++) {
                Vector2 tmp = rts[i].sizeDelta;
                if (tmp.x != tileDim.x || tmp.y != tileDim.y) {
                    rts[i].sizeDelta = tileDim;
                }
            }

        }

         // Places all the elements to the correct position. 
        void RepositionRects() {

            // Calculate the width and height of the tiles.
            Vector2 tileDim = GetTileDimensions();

            float initX = (-bounds.rect.width / 2f + edgeBuffer.x + tileBuffer.x + tileDim.x / 2f);
            float initY = (bounds.rect.height / 2f - edgeBuffer.y - tileBuffer.y - tileDim.y / 2f);

            // Itterate and check.
            for (int i = 0; i < rts.Count; i++) {
                rts[i].anchoredPosition = new Vector2(0.5f, 0.5f);
                rts[i].anchorMin = new Vector2(0.5f, 0.5f);
                rts[i].anchorMax = new Vector2(0.5f, 0.5f);

                int xIndex = i % columns;
                float xPos = initX + (xIndex * (2 * tileBuffer.x + tileDim.x));

                int yIndex = (int)Mathf.Floor((float)i / (float)columns);
                float yPos = initY - (yIndex * (2 * tileBuffer.y + tileDim.y));

                rts[i].anchoredPosition = new Vector2(xPos, yPos);

            }

        }

        // Get the tile dimensions based on the bounds and columns.
        public Vector2 GetTileDimensions() {
            // Calculate the width.
            float widthPerTile = (bounds.rect.width - edgeBuffer.x * 2f) / (float)columns;
            widthPerTile = (widthPerTile - tileBuffer.x * 2f);
            // Calculate the height.
            float heightPerTile = widthPerTile * rowToColumnRatio;
            return new Vector2(widthPerTile, heightPerTile);
        }

    }

}
