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
    
        //Makes a list of all available sneakers
        List<Sneaker> availableSneakers = new List<Sneaker>();
        for (int i = 0; i < gameManager.SneakerDatabase.Database.Count; i++)
        {
            //Find all the shoes with the matching rarity for this crate.
            if (gameManager.SneakerDatabase.Database[i].Rarity == rarity)
            {
                Sneaker sneaker = new Sneaker
                {
                    name = gameManager.SneakerDatabase.Database[i].Name,
                    imagePath = gameManager.SneakerDatabase.Database[i].Icon.name,
                    rarity = gameManager.SneakerDatabase.Database[i].Rarity
                };
                
                //Add all of the sneakers to the list.
                availableSneakers.Add(sneaker);
            }
        }

        //Return a random shoe from the available shoes.
        return availableSneakers[Random.Range(0, availableSneakers.Count)];

    }
}
