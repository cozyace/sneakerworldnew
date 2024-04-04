using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text[] FeaturedTimeLabels;

    private float _timer = 100f;


    private void Update()
    {
        _timer -= Time.deltaTime;
        UpdateFeaturedTimers(_timer);
    }

    
    
    private void UpdateFeaturedTimers(float timeRemaining)
    {
        foreach (TMP_Text t in FeaturedTimeLabels)
        {
            TimeSpan time = TimeSpan.FromSeconds(timeRemaining);
            string formattedTime = time.ToString(@"hh\:mm\:ss");

            t.text = formattedTime;
        }
    }

}