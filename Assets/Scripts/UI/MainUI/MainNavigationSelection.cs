using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainNavigationSelection : MonoBehaviour
{
    //This script exists in MainUI -> UserNavigation_Window -> SelectedButtonCover (GameObject)
    
    public Image SelectedWindowText;
    public Image SelectedWindowIcon;

    [Header("0 = Friends, 1 = Ranking, 2 = Inventory, 3 = Market, 4 = Store")]
    public Sprite[] WindowIconSprites; // ( 0 = Friends, 1 = Ranking, 2 = Inventory, 3 = Market, 4 = Store )
    public Sprite[] WindowTextSprites; // ( 0 = Friends, 1 = Ranking, 2 = Inventory, 3 = Market, 4 = Store )
    public GameObject[] ButtonObjects; // ( 0 = Friends, 1 = Ranking, 2 = Inventory, 3 = Market, 4 = Store )
    public GameObject[] GameWindows; //   ( 0 = Friends, 1 = Ranking, 2 = Inventory, 3 = Market, 4 = Store )

    public Animator MainUIAnimator;
    public Animator InventoryAnimator;


    private void Awake()
    {
        CloseAllWindows();
    }
    
    // ( 0 = Friends, 1 = Ranking, 2 = Inventory, 3 = Market, 4 = Store )
    public void SelectButton(int index)
    {
        MainUIAnimator.Play("SelectionPop");
        
        transform.position = new Vector3(ButtonObjects[index].transform.position.x, transform.position.y, transform.position.z);
        
        SelectedWindowText.sprite = WindowTextSprites[index];
        SelectedWindowIcon.sprite = WindowIconSprites[index];

        foreach(GameObject w in GameWindows)
            w.SetActive(Array.IndexOf(GameWindows, w) == index);

        switch (index)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                InventoryAnimator.Play("Inventory_FadeUp");
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }
    
    //Moves the selected button window out of view, so it appears to be disabled, and closes all windows.
    public void CloseAllWindows()
    {
        foreach (GameObject w in GameWindows)
        {
            if (w.activeSelf && Array.IndexOf(GameWindows, w) == 2)
            {
                InventoryAnimator.Play("Inventory_FadeDown");
            }
            
            //w.SetActive(false);

           
        }

        transform.position = new Vector3(-1000, transform.position.y, transform.position.z);
    }
    
}
