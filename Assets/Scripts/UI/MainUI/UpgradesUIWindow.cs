using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesUIWindow : MonoBehaviour
{
    [SerializeField] private GameObject Gradient;
    [SerializeField] private GameObject Window;
    
    private Animator _Animator;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        _Animator = Window.GetComponent<Animator>();
        
    }

    public void TriggerUpgradesWindow()
    {
        Gradient.SetActive(true);
        Window.SetActive(true);
        _Animator.Play("Open");
    }
    
    public void CloseUpgradesWindow()
    {
        Gradient.SetActive(false);
        Window.SetActive(false);
    }
}
