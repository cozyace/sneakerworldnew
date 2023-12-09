using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform contentPanel;

    public void SnapTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        var contentPanelAnchoredPosition = contentPanel.anchoredPosition;
        contentPanelAnchoredPosition.y = ((Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
                                          - (Vector2)scrollRect.transform.InverseTransformPoint(target.position)).y;
        contentPanel.anchoredPosition = contentPanelAnchoredPosition;
    }

    public void UpdateFeaturedSneakers()
    {
        
    }
}