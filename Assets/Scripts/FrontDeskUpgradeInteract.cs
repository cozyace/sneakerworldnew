using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class FrontDeskUpgradeInteract : MonoBehaviour
{
    private UIManager _UIManager;

    private void Awake() => _UIManager = FindObjectOfType<UIManager>();

    
    private void OnMouseUpAsButton()
    {
        //Makes sure that you don't have any other menu open that could conflict with clicking on the front desk.
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        _UIManager.ShowUpgrades();
    }
}
