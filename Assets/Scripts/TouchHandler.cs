using UnityEngine;

public class TouchHandler : MonoBehaviour
{
    
    private void OnMouseUpAsButton()
    {
        UIManager.instance.ShowUpgrades();
    }
}
