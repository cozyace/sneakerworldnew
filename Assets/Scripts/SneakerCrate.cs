using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class SneakerCrate : MonoBehaviour
{
    [SerializeField] private SneakerRarity rarity;
    [SerializeField] private TMP_Text price;

    public Sneaker Buy()
    {
        if (GameManager.instance.GetCash() <= int.Parse(price.text))
        {
            return new Sneaker()
            {
                name = null
            };
        }

        List<Sneaker> availableSneakers = new List<Sneaker>();
        for (int i = 0; i < GameManager.instance._sneakers.Count; i++)
        {
            if (GameManager.instance._sneakers[i].rarity == rarity)
            {
                availableSneakers.Add(GameManager.instance._sneakers[i]);
            }
        }

        return availableSneakers[Random.Range(0, availableSneakers.Count)];

    }
}
