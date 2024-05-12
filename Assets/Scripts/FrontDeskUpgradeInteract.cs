using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class FrontDeskUpgradeInteract : MonoBehaviour
{

    
    

    
    private void OnMouseUpAsButton()
    {
        //Makes sure that you don't have any other menu open that could conflict with clicking on the front desk.
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        print("Clicked on Desk! Active Upgrade Menu!");
        
        FindAnyObjectByType<UpgradesUIWindow>().TriggerUpgradesWindow();
    }
    
}
