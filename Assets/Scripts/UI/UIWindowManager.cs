using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindowManager : MonoBehaviour
{
    [Header("Window Animator")]
    public Animator FriendsAnimator;
    public Animator RankingAnimator;
    public Animator InventoryAnimator;
    public Animator MarketAnimator;
    public Animator StoreAnimator;
    [Space(10)]
    [Header("Window Activity")]
    public int ActiveIndex = -1;
    private int QueuedIndex = -1;



    private void Awake()
    {
        ActiveIndex = -1;
    }

    public void CloseAllWindows()
    {
        if (ActiveIndex == -1)
            return;
            
        StartCoroutine(CloseWindow(ActiveIndex));
    }
    
    public void OpenWindow(int index)
    {
        QueuedIndex = index;
        
        //Check if a previous window is open; if so-- close it and wait for the animation to finish.
        CloseAllWindows();

        if (ActiveIndex != -1)
            return;
        //Open the new window.
            
        Animator selectedAnimator = GetAnimatorFromIndex(index);
        selectedAnimator.gameObject.SetActive(true);
        ActiveIndex = index;
        selectedAnimator!.Play("OpenWindow");
        QueuedIndex = -1;
    }

    private IEnumerator CloseWindow(int index)
    {
        GetAnimatorFromIndex(index).Play("CloseWindow");
        yield return new WaitForSeconds(0.5f);
        GetAnimatorFromIndex(index).gameObject.SetActive(false);
        ActiveIndex = -1;
        
        if(QueuedIndex != -1)
            OpenWindow(QueuedIndex);
    }


    private Animator GetAnimatorFromIndex(int index)
    {
        return index switch
        {
            0 => FriendsAnimator,
            1 => RankingAnimator,
            2 => InventoryAnimator,
            3 => MarketAnimator,
            4 => StoreAnimator,
            _ => null
        };
    }
    

  
}
