
    using UnityEngine;
    using UnityEngine.Serialization;
    public class GridTile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer Sprite; //Exterior border of the sprite.
        [SerializeField] private SpriteRenderer InsideSprite; //Interior shading of the sprite.
        
        public Vector2 GridPosition; // The theoretical 'position' of this tile on the grid layout. (Represents a local position within the grid)
        public GameObject OccupyingObject; //The object occupying this space.

        public bool IsBlocked = false;
        
        private void Start()
        {
            Invoke(nameof(CheckIfDeletionNeeded), 0.15f);
        }


        private void CheckIfDeletionNeeded()
        {
            if (!IsSpriteMasked(Sprite, transform.parent.GetComponent<SpriteRenderer>()))
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            //If an object is occupying this square.
            if (!IsBlocked)
            {
                InsideSprite.color = OccupyingObject ? Color.green : Color.white;
                Sprite.color = Color.black;
            }
            else if (IsBlocked)
            {
                InsideSprite.color = Color.red;
                Sprite.color = Color.red;
            }
        }
        
        private bool IsSpriteMasked(SpriteRenderer sprite, SpriteRenderer mask)
        {
            if (sprite == null || mask == null)
                return false;
            
            // Get the bounds of the sprite
            Bounds spriteBounds = sprite.bounds;

            // Get the bounds of the mask
            Bounds maskBounds = mask.bounds;
            // Check if the sprite bounds intersect with the mask bounds
            return spriteBounds.Intersects(maskBounds);
        }
        
        public void UpdateColor( Color color) => Sprite.color = color;

        public void OnTriggerEnter2D(Collider2D col)
        {
            if(col.transform.CompareTag("GridBlocker"))
                IsBlocked = true;
        }
    }

