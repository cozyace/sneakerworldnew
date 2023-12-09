using System;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgressBar : MonoBehaviour
{
    [SerializeField] private Transform following;
    [SerializeField] private Image waitingBar;

    private Camera _mainCamera;
    
    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 pos = _mainCamera.WorldToScreenPoint(following.position);
        transform.position = pos;
    }

    public void Setup(Transform parent)
    {
        following = parent;
        transform.SetParent(GameObject.FindGameObjectWithTag("Popups").transform);
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
    }

    public void SetFillAmount(float amount)
    {
        waitingBar.fillAmount = amount;
    }
}
