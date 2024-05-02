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
        Gradient.SetActive(true);
        Window.SetActive(true);
        _Animator.Play("Open");

        CashEarnedText.text = (minutesGone * 10).ToString("n0");//TEMP
        
        if (minutesGone > 60)
            TimeGoneText.text = (minutesGone / 60).ToString("n1") + " HRS.";
        else
            TimeGoneText.text = (minutesGone).ToString("n0") + " MIN.";
    }
    
    public void CloseWelcomeWindow()
    {
        Gradient.SetActive(false);
        Window.SetActive(false);
        
        FindFirstObjectByType<MainNavigationSelection>().SelectButton(1);
    }
}
