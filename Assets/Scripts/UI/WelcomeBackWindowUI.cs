using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class WelcomeBackWindowUI : MonoBehaviour
{
    [SerializeField] private GameObject Gradient;
    [SerializeField] private GameObject Window;
    
    private Animator _Animator;

    [SerializeField] private TMP_Text CashEarnedText;
    [SerializeField] private TMP_Text TimeGoneText;
    
    // Start is called before the first frame update
    private void Start()
    {
        _Animator = Window.GetComponent<Animator>();
        
    }

    public void TriggerWelcomeWindow(double minutesGone)
    {
        if (minutesGone < 15)
            return;
        
        Gradient.SetActive(true);
        Window.SetActive(true);
        _Animator.Play("Open");

        CashEarnedText.text = (minutesGone * 10).ToString("n0");//TEMP
        
        if (minutesGone > 60)
            TimeGoneText.text = (minutesGone / 60).ToString("n1") + " HRS.";
        else
            TimeGoneText.text = (minutesGone).ToString("n0") + " MIN.";
    }


    private int CalculateMoneyEarned(int minutesGone)
    {
        SneakersOwned[] SneakersBeingSold; //The sneakers you've selected to sell.
        int employeeCount = 1; //How many employees are active in the store.
        float customersPerMinute = 3; //How many customers are entering on average per minute.
        float transactionsPerMinute = 3f; //How many transactions an employee CAN get done per minute. 
        
        //Do a loop of however many transactions are possible, and pick a random shoe that's active each time.
        
        return 0;
    }
    
    public void CloseWelcomeWindow()
    {
        Gradient.SetActive(false);
        Window.SetActive(false);
        
        FindFirstObjectByType<MainNavigationSelection>().SelectButton(1);
    }
}
