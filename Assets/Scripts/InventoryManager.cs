using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InventoryManager : MonoBehaviour
{
    public InventoryStats inventoryStats;
    public SelectedSneaker selectedSneaker;
    public SneakerInventoryItem currentSneakerSelected, sneakerToSwap;
    public GridLayoutGroup gridLayout;
    public TMP_InputField search;
    public GameObject sneakerInventoryItemPrefab, swapInventoryItemPrefab;
    public Sprite[] sprites;
    public RectTransform swapSneakersInventoryTransform;
    [SerializeField] private Image sneakerToSwapImage;
    public GameObject swapInventoryPanel;

    public GameObject stopButton;
    public GameObject sellButton;

    public List<SneakerInventoryItem> sneakers;

    private bool checkboxActive = true;

    private void Start()
    {
        sneakers = new List<SneakerInventoryItem>();

        var names = new string[]
        {
            "Air forces", "Ballet shoe", "Bast shoe", "Blucher shoe", "Boat shoe", "Brogan", "Brogue shoe",
            "Brothel creeper", "Bucks", "Cantabrian", "Chelsea boot", "Chopine", "Chukka boot", "Climbing shoe", "Clog",
            "Court shoe", "Cross country running shoe", "Derby shoe", "Desert Boot", "Diabetic shoe", "Dress shoe",
            "Driving moccasins", "Duckbill shoe", "Earth shoe", "Elevator shoes", "Espadrille", "Fashion boot",
            "Galesh", "Geta", "Giveh", "High-heeled footwear", "Hiking shoes", "Huarache", "Jazz shoe", "Jelly shoes",
            "Jika-tabi", "Jutti", "Kitten heel", "Kolhapuri Chappal", "Kung fu shoe", "Loafers", "Lotus shoes",
            "Mary Jane", "Moccasin", "Mojari", "Monk shoe", "Mule", "Okobo", "Opanak", "Opinga", "Organ shoes",
            "Orthopaedic footwear", "Over-the-knee boot", "Oxford shoe", "Pampootie", "Peep-toe shoe",
            "Peranakan beaded slippers", "Peshawari chappal", "Platform shoe", "Plimsoll", "Pointe shoe",
            "Pointed shoe", "Pointinini", "Riding boots", "Rocker bottom shoe", "Rope-soled shoe", "Russian boot",
            "Saddle shoe", "Sailing boots", "Sandal", "Silver Shoes", "Slingback", "Slip-on shoe", "Slipper",
            "Sneakers", "Snow boot", "Spectator shoe", "Spool heel", "Steel-toe boot", "Stiletto heel", "T-bar sandal",
            "Tiger-head shoes", "Toe shoe", "Toe shoe", "Trail running shoes", "Tsarouhi", "Turnshoe",
            "Venetian-style shoe", "Walk-Over shoes", "Wedge", "Wellington boot", "Winklepicker", "Wörishofer", "Zori"
        };

        for (int i = 0; i < inventoryStats.numSneakers; i++)
        {
            var newSneaker = Instantiate(sneakerInventoryItemPrefab, gridLayout.transform);
            var sneakerInventoryItem = newSneaker.GetComponent<SneakerInventoryItem>();
            sneakerInventoryItem.name = names[Random.Range(0, names.Length)];
            sneakerInventoryItem.quantity = Random.Range(50, 700);
            sneakerInventoryItem.rarity = (SneakerRarity)Random.Range(1, 5);
            sneakerInventoryItem.purchasedPrice = Random.Range(120, 200) * (int)sneakerInventoryItem.rarity;
            sneakerInventoryItem.aiCanBuy = false;
            sneakerInventoryItem.sneakerImage.sprite = sprites[Random.Range(0, sprites.Length)];
            sneakerInventoryItem.timestamp = DateTime.Now;
            sneakers.Add(sneakerInventoryItem);
        }

        OnSneakerClick(sneakers[0]);

        foreach (SneakerInventoryItem sneaker in sneakers)
        {
            SneakerInventoryItem _sneaker = Instantiate(sneaker, swapSneakersInventoryTransform);
            _sneaker.isSwapItem = true;
        }
    }

    private void InstantiateSneakers()
    {
        var names = new string[]
        {
            "Air forces", "Ballet shoe", "Bast shoe", "Blucher shoe", "Boat shoe", "Brogan", "Brogue shoe",
            "Brothel creeper", "Bucks", "Cantabrian", "Chelsea boot", "Chopine", "Chukka boot", "Climbing shoe", "Clog",
            "Court shoe", "Cross country running shoe", "Derby shoe", "Desert Boot", "Diabetic shoe", "Dress shoe",
            "Driving moccasins", "Duckbill shoe", "Earth shoe", "Elevator shoes", "Espadrille", "Fashion boot",
            "Galesh", "Geta", "Giveh", "High-heeled footwear", "Hiking shoes", "Huarache", "Jazz shoe", "Jelly shoes",
            "Jika-tabi", "Jutti", "Kitten heel", "Kolhapuri Chappal", "Kung fu shoe", "Loafers", "Lotus shoes",
            "Mary Jane", "Moccasin", "Mojari", "Monk shoe", "Mule", "Okobo", "Opanak", "Opinga", "Organ shoes",
            "Orthopaedic footwear", "Over-the-knee boot", "Oxford shoe", "Pampootie", "Peep-toe shoe",
            "Peranakan beaded slippers", "Peshawari chappal", "Platform shoe", "Plimsoll", "Pointe shoe",
            "Pointed shoe", "Pointinini", "Riding boots", "Rocker bottom shoe", "Rope-soled shoe", "Russian boot",
            "Saddle shoe", "Sailing boots", "Sandal", "Silver Shoes", "Slingback", "Slip-on shoe", "Slipper",
            "Sneakers", "Snow boot", "Spectator shoe", "Spool heel", "Steel-toe boot", "Stiletto heel", "T-bar sandal",
            "Tiger-head shoes", "Toe shoe", "Toe shoe", "Trail running shoes", "Tsarouhi", "Turnshoe",
            "Venetian-style shoe", "Walk-Over shoes", "Wedge", "Wellington boot", "Winklepicker", "Wörishofer", "Zori"
        };

        var newSneaker = Instantiate(sneakerInventoryItemPrefab, gridLayout.transform);
        var sneakerInventoryItem = newSneaker.GetComponent<SneakerInventoryItem>();
        sneakerInventoryItem.name = names[Random.Range(0, names.Length)];
        sneakerInventoryItem.quantity = Random.Range(50, 700);
        sneakerInventoryItem.rarity = (SneakerRarity)Random.Range(1, 5);
        sneakerInventoryItem.purchasedPrice = Random.Range(120, 200) * (int)sneakerInventoryItem.rarity;
        sneakerInventoryItem.aiCanBuy = false;
        sneakerInventoryItem.sneakerImage.sprite = sprites[Random.Range(0, sprites.Length)];
        sneakerInventoryItem.timestamp = DateTime.Now;
        sneakers.Add(sneakerInventoryItem);
    }

    public void OnSneakerClick(SneakerInventoryItem sneakerInventoryItem)
    {
        selectedSneaker.UpdateDetails(sneakerInventoryItem);
        currentSneakerSelected = sneakerInventoryItem;
        SetSellButtonState(sneakerInventoryItem.aiCanBuy);
    }

    public void OnSneakerSwapClick(SneakerInventoryItem sneakerInventoryItem)
    {
        sneakerToSwap = sneakerInventoryItem;
        sneakerToSwapImage.sprite = sneakerInventoryItem.sneakerImage.sprite;
        swapInventoryPanel.SetActive(false);
    }

    public void ToggleCheckboxes()
    {
        foreach (Transform inventoryItem in gridLayout.transform)
        {
            inventoryItem.GetChild(0).gameObject.SetActive(!checkboxActive);
        }

        checkboxActive = !checkboxActive;
    }
    
    public void FilterBySearch()
    {
        foreach (Transform inventoryItem in gridLayout.transform)
        {
            SneakerInventoryItem sneaker = inventoryItem.GetComponent<SneakerInventoryItem>();
            inventoryItem.gameObject.SetActive(sneaker.name.ToLower().Contains(search.text.ToLower()));
        }
    }

    public void FilterByRarity(TMP_Dropdown dropdown)
    {
        foreach (Transform inventoryItem in gridLayout.transform)
        {
            SneakerInventoryItem sneaker = inventoryItem.GetComponent<SneakerInventoryItem>();
            inventoryItem.gameObject.SetActive(dropdown.value == 0 || dropdown.value == (int)sneaker.rarity);
        }
    }
    
    public void FilterByBrand(TMP_Dropdown dropdown)
    {
        foreach (Transform inventoryItem in gridLayout.transform)
        {
            SneakerInventoryItem sneaker = inventoryItem.GetComponent<SneakerInventoryItem>();
            inventoryItem.gameObject.SetActive(dropdown.value == 0 ||
                                               sneaker.name.ToLower().Contains(dropdown.options[dropdown.value].text.ToLower()));
        }
    }

    public void SortByOption(TMP_Dropdown dropdown)
    {
        List<Transform> children = new List<Transform>();
        for (int i = gridLayout.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = gridLayout.transform.GetChild(i);
            children.Add(child);
            child.parent = null;
        }

        var sortedList = new List<Transform>();
        switch (dropdown.value)
        {
            case 0:
                sortedList = SortByTime(children, true);
                break;
            case 1:
                sortedList = SortByTime(children, false);
                break;
            case 2:
                sortedList = SortByName(children, true);
                break;
            case 3:
                sortedList = SortByName(children, false);
                break;
            case 4:
                sortedList = SortByRarity(children, true);
                break;
            case 5:
                sortedList = SortByRarity(children, false);
                break;
            case 6:
                sortedList = SortByPrice(children, true);
                break;
            case 7:
                sortedList = SortByPrice(children, false);
                break;
        }
        
        foreach (var sneaker in sortedList)
        {
            sneaker.parent = gridLayout.transform;
        }
    }

    private List<Transform> SortByTime(List<Transform> sneakers,bool newestFirst)
    {
        if (newestFirst)
        {
            sneakers.Sort((Transform t1, Transform t2) =>
                DateTime.Compare(t1.GetComponent<SneakerInventoryItem>().timestamp,
                    t2.GetComponent<SneakerInventoryItem>().timestamp));
        }
        else
        {
            sneakers.Sort((Transform t1, Transform t2) =>
                DateTime.Compare(t2.GetComponent<SneakerInventoryItem>().timestamp,
                    t1.GetComponent<SneakerInventoryItem>().timestamp));
        }

        return sneakers;
    }
    
    private List<Transform> SortByName(List<Transform> sneakers,bool ascending)
    {
        if (ascending)
        {
            sneakers.Sort((Transform t1, Transform t2) => string.Compare(t1.GetComponent<SneakerInventoryItem>().name, t2.GetComponent<SneakerInventoryItem>().name, StringComparison.Ordinal));
        }
        else
        {
            sneakers.Sort((Transform t1, Transform t2) => string.Compare(t2.GetComponent<SneakerInventoryItem>().name, t1.GetComponent<SneakerInventoryItem>().name, StringComparison.Ordinal));
        }

        return sneakers;
    }
    
    private List<Transform> SortByRarity(List<Transform> sneakers,bool ascending)
    {
        if (ascending)
        {
            sneakers.Sort((Transform t1, Transform t2) => t1.GetComponent<SneakerInventoryItem>().rarity
                .CompareTo(t2.GetComponent<SneakerInventoryItem>().rarity));
        }
        else
        {
            sneakers.Sort((Transform t1, Transform t2) => t2.GetComponent<SneakerInventoryItem>().rarity
                .CompareTo(t1.GetComponent<SneakerInventoryItem>().rarity));
        }

        return sneakers;
    }
    
    private List<Transform> SortByPrice(List<Transform> sneakers,bool ascending)
    {
        if (ascending)
        {
            sneakers.Sort((Transform t1, Transform t2) => t1.GetComponent<SneakerInventoryItem>().purchasedPrice
                .CompareTo(t2.GetComponent<SneakerInventoryItem>().purchasedPrice));
        }
        else
        {
            sneakers.Sort((Transform t1, Transform t2) => t2.GetComponent<SneakerInventoryItem>().purchasedPrice
                .CompareTo(t1.GetComponent<SneakerInventoryItem>().purchasedPrice));
        }

        return sneakers;
    }

    public void BuySneaker(SneakerInventoryItem sneakerInventoryItem)
    {
        foreach (var sneaker in sneakers.Where(sneaker => sneaker == sneakerInventoryItem))
        {
            sneaker.quantity--;
            if (currentSneakerSelected.Equals(sneaker))
            {
                selectedSneaker.UpdateDetails(sneaker);
            }
        }
    }

    public void SwapSneakers(SneakerInventoryItem sneakerInventoryItem)
    {
        foreach (var sneaker in sneakers.Where(sneaker => sneaker == sneakerInventoryItem))
        {
            sneaker.quantity--;

            if (sneakerToSwap.Equals(sneaker))
            {
                selectedSneaker.UpdateDetails(sneaker);
            }
        }
    }

    public void SetSellButtonState(bool aiCanBuy)
    {
        sellButton.SetActive(!aiCanBuy);
        stopButton.SetActive(aiCanBuy);
    }

    public void SetSellState()
    {
        if (currentSneakerSelected.aiCanBuy)
        {
            currentSneakerSelected.aiCanBuy = false;
            currentSneakerSelected.toggle.isOn = false;
        }
        else
        {
            currentSneakerSelected.aiCanBuy = true;
            currentSneakerSelected.toggle.isOn = true;
        }
    }

    public void AddSneakerSlot()
    {
        inventoryStats.numSneakers++;
        InstantiateSneakers();
    }
}
