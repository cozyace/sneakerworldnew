using System;
using UnityEngine;

public class CashPopup : MonoBehaviour
{
    [SerializeField] private int cash = 0;
    [SerializeField] private float speed = 15;

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.parent.position, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, transform.parent.position) < speed * Time.deltaTime)
        {
            GameManager.instance.AddCash(cash);
            Destroy(gameObject);
        }
    }

    public void SetPopup(int cashAmount)
    {
        cash = cashAmount;
    }
}