using UnityEngine;

public class TouchHandler : MonoBehaviour
{
    public UIManager uiManager;

    private void OnMouseUpAsButton()
    {
        if (uiManager != null)
            uiManager = FindAnyObjectByType<UIManager>();

        uiManager.ShowUpgrades();
    }
}
