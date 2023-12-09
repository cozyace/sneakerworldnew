using UnityEngine;
using UnityEngine.UI;

public class ItemPopup : MonoBehaviour
{
    [SerializeField] private Transform following;
    [SerializeField] private Image image;
    [SerializeField] private Image shadowImage;
    
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (following == null)
        {
            Destroy(gameObject);
        }
        else
        {
            Vector3 pos = _mainCamera.WorldToScreenPoint(following.position);
            transform.position = pos;
        }
    }

    public void SetPopup(Transform parent, Sprite sprite)
    {
        following = parent;
        if (sprite != null)
        {
            image.sprite = sprite;
            shadowImage.sprite = sprite; 
        }
        transform.SetParent(GameObject.FindGameObjectWithTag("Popups").transform);
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
    }
}