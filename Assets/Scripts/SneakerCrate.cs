using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class SneakerCrate : MonoBehaviour
{
    [SerializeField] private SneakerRarity rarity;
    [SerializeField] private TMP_Text price;
    public GameManager gameManager;

    public Sneaker Buy()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();

        if (gameManager.GetCash() <= int.Parse(price.text))
        {
            return new Sneaker()
            {
                name = null
            };
        }

        List<Sneaker> availableSneakers = new List<Sneaker>();
        for (int i = 0; i < gameManager._sneakers.Count; i++)
        {
            if (gameManager._sneakers[i].rarity == rarity)
            {
                availableSneakers.Add(gameManager._sneakers[i]);
            }
        }

        return availableSneakers[Random.Range(0, availableSneakers.Count)];

    }
}
