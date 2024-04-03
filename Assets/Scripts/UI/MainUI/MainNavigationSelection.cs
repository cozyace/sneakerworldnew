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

    private Animator _Animator;

    private UIWindowManager _UIWindowManager;

    public bool _IsOnCooldown;




    private void Awake()
    {
        _Animator = GetComponent<Animator>();
        _UIWindowManager = FindObjectOfType<UIWindowManager>();

        CloseAllWindows();
    }


    // ( 0 = Friends, 1 = Ranking, 2 = Inventory, 3 = Market, 4 = Store )
    public void SelectButton(int index)
    {
        if (_IsOnCooldown)
            return;

        StartCoroutine(nameof(BeginCooldown));

        _Animator.Play("SelectionPop");

        transform.position = new Vector3(ButtonObjects[index].transform.position.x, transform.position.y, transform.position.z);

        SelectedWindowText.sprite = WindowTextSprites[index];
        SelectedWindowIcon.sprite = WindowIconSprites[index];

        _UIWindowManager.OpenWindow(index);
    }

    //Moves the selected button window out of view, so it appears to be disabled, and closes all windows.
    public void CloseAllWindows()
    {
        _UIWindowManager.CloseAllWindows();

        transform.position = new Vector3(-1000, transform.position.y, transform.position.z);
    }

    
    //This method is intended to be used on the Main Navigation Selection button. (blue highlighted button)
    public void CloseAllWindowsWithCooldown()
    {
        if (_IsOnCooldown)
            return;
        
        CloseAllWindows();
    }
    

    private IEnumerator BeginCooldown()
    {
        _IsOnCooldown = true;
        yield return new WaitForSeconds(0.65f);
        _IsOnCooldown = false;
    }
}
