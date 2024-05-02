using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class FrontDeskUpgradeInteract : MonoBehaviour
{
    private GameObject _DeskCanvas;
    [SerializeField] private float TimeRemaining;

    private void Start() => _DeskCanvas = transform.GetChild(1).gameObject;
    
    
    private void Update()
    {
        if (!_DeskCanvas)
            return;
        
        if (TimeRemaining >= 0)
        {
            TimeRemaining -= Time.deltaTime;
        }

        _DeskCanvas.SetActive(TimeRemaining > 0);
    }
    
    private void OnMouseUpAsButton()
    {
        //Makes sure that you don't have any other menu open that could conflict with clicking on the front desk.
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        TimeRemaining = 3f;
    }

    public void PurchaseUpgrade()
    {
        
    }
}
