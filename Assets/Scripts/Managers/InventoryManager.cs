using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public struct SneakersOwned
{
    public string name;
    public int quantity;
    public int purchasePrice;
    public SneakerRarity rarity;

    public SneakersOwned(string n, int q, int p, SneakerRarity r)
    {
        name = n;
        quantity = q;
        purchasePrice = p;
        rarity = r;
    }
}

public class InventoryManager : MonoBehaviour
{
    public GameManager gameManager;
    public List<SneakersOwned> sneakersOwned;
    public SelectedSneaker selectedSneaker;
    public SneakerInventoryItem currentSneakerSelected, sneakerToSwap;
    public RectTransform gridLayout;
    public TMP_InputField search;
    public GameObject chooseSneakerPanel;
    public GameObject sneakerInventoryItemPrefab, swapInventoryItemPrefab;
    public Sprite[] sprites;
    public RectTransform swapSneakersInventoryTransform;
    [SerializeField] private Image sneakerToSwapImage;
    public GameObject swapInventoryPanel;
    public List<SneakerInventoryItem> sneakers;
    private bool checkboxActive = true;
    private int sneakerCount; 
    private int sneakerRarity;
    private int rarityLevel;

    private async void Start()
    {
        List<SneakersOwned> sneakers = await gameManager.firebase.GetSneakerAsync(gameManager.firebase.userId);
        
        if (sneakers.Count > 0) 
        {
            chooseSneakerPanel.SetActive(false);

            foreach (SneakersOwned sneaker in sneakers)
            {
                InstantiateSneakers(sneaker);
            }

            gameManager.aiManager.enabled = true;
        }
    }

    private void InitializeSneakers()
    {
        var names = new string[]
        {
            "Sneaker World I", "Sneaker World I High", "Sneaker World I High Sky", "Jordan 1 Chicago", "Jordan 3 Cement", 
            "Jordan 4 Bred", "Jordan 5 Supreme", "Jordan 6 Infrared"
        };

        var newSneaker = Instantiate(sneakerInventoryItemPrefab, gridLayout);
        var sneakerInventoryItem = newSneaker.GetComponent<SneakerInventoryItem>();
        sneakerInventoryItem.name = names[rarityLevel];
        sneakerInventoryItem.quantity = sneakerCount;
        sneakerInventoryItem.rarity = (SneakerRarity)sneakerRarity;
        sneakerInventoryItem.purchasePrice = Random.Range(120, 150) * (int)sneakerInventoryItem.rarity;
        sneakerInventoryItem.aiCanBuy = false;
        sneakerInventoryItem.sneakerImage.sprite = sprites[rarityLevel];
        sneakerInventoryItem.timestamp = DateTime.Now;
        sneakerInventoryItem.nameText.text = sneakerInventoryItem.name;
        sneakers.Add(sneakerInventoryItem);

        foreach (SneakerInventoryItem sneaker in sneakers)
        {
            SneakerInventoryItem _sneaker = Instantiate(sneaker, swapSneakersInventoryTransform);
            _sneaker.isSwapItem = true;

            SneakersOwned _sneakers = new()
            {
                name = sneaker.name,
                quantity = sneaker.quantity,
                purchasePrice = sneaker.purchasePrice,
                rarity = sneaker.rarity
            };

            sneakersOwned.Add(_sneakers);
        }

        OnSneakerClick(sneakers[0]);
    }

    public void AddShoesToCollection(SneakersOwned sneaker)
    {
        if (sneakersOwned.Any(x => x.name == sneaker.name))
        {
            SneakersOwned sneakerInstance = sneakersOwned[sneakersOwned.FindIndex(x => x.name == sneaker.name)];
            sneakerInstance.quantity += sneaker.quantity;
            sneakersOwned[sneakersOwned.FindIndex(x => x.name == sneaker.name)] = sneakerInstance;
        }
        else // if the player doesn't have the shoe.
            sneakersOwned.Add(sneaker);
    }

    private void InstantiateSneakers(SneakersOwned _sneakersOwned)
    {
        var newSneaker = Instantiate(sneakerInventoryItemPrefab, gridLayout);
        var sneakerInventoryItem = newSneaker.GetComponent<SneakerInventoryItem>();
        sneakerInventoryItem.name = _sneakersOwned.name;
        sneakerInventoryItem.quantity = _sneakersOwned.quantity;
        sneakerInventoryItem.rarity = _sneakersOwned.rarity;
        sneakerInventoryItem.purchasePrice = _sneakersOwned.purchasePrice;
        sneakerInventoryItem.aiCanBuy = false;
        sneakerInventoryItem.sneakerImage.sprite = sprites[rarityLevel];
        sneakerInventoryItem.timestamp = DateTime.Now;
        sneakerInventoryItem.nameText.text = sneakerInventoryItem.name;
        sneakers.Add(sneakerInventoryItem);

        foreach (SneakerInventoryItem sneaker in sneakers)
        {
            SneakerInventoryItem _sneaker = Instantiate(sneaker, swapSneakersInventoryTransform);
            _sneaker.isSwapItem = true;

            SneakersOwned _sneakers = new()
            {
                name = sneaker.name,
                quantity = sneaker.quantity,
                purchasePrice = sneaker.purchasePrice,
                rarity = sneaker.rarity
            };

            sneakersOwned.Add(_sneakers);
        }

        OnSneakerClick(sneakers[0]);
    }

    public void OnSneakerClick(SneakerInventoryItem sneakerInventoryItem)
    {
        selectedSneaker.UpdateDetails(sneakerInventoryItem);
        currentSneakerSelected = sneakerInventoryItem;
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
            sneakers.Sort((Transform t1, Transform t2) => t1.GetComponent<SneakerInventoryItem>().purchasePrice
                .CompareTo(t2.GetComponent<SneakerInventoryItem>().purchasePrice));
        }
        else
        {
            sneakers.Sort((Transform t1, Transform t2) => t2.GetComponent<SneakerInventoryItem>().purchasePrice
                .CompareTo(t1.GetComponent<SneakerInventoryItem>().purchasePrice));
        }

        return sneakers;
    }

    public void BuySneaker(SneakerInventoryItem sneakerInventoryItem)
    {
        foreach (SneakerInventoryItem sneaker in sneakers.Where(sneaker => sneaker == sneakerInventoryItem))
        {
            if (sneaker.quantity > 0)
                sneaker.quantity--;
            else
            {
                print("Tried to remove sneaker while none are left!");
                return;
            }
            
            if (currentSneakerSelected.Equals(sneaker))
            {
                selectedSneaker.UpdateDetails(sneaker);
            }
        }
    }

    public void SwapSneakers(SneakerInventoryItem sneakerInventoryItem)
    {
        foreach (SneakerInventoryItem sneaker in sneakers.Where(sneaker => sneaker == sneakerInventoryItem))
        {
            sneaker.quantity--;

            if (sneakerToSwap.Equals(sneaker))
            {
                selectedSneaker.UpdateDetails(sneaker);
            }
        }
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
        InitializeSneakers();
    }

    public async void CommonSneakersButton()
    {
        sneakerCount = 50;
        sneakerRarity = 1;
        rarityLevel = 0;
        //InitializeSneakers();
       // await gameManager.firebase.ChooseSneakerAsync(gameManager.firebase.userId, sneakersOwned[0]);
        gameManager.aiManager.enabled = true;
    }

    public async void UncommonSneakersButton()
    {
        sneakerCount = 25;
        sneakerRarity = 2;
        rarityLevel = 1;
       // InitializeSneakers();
       // await gameManager.firebase.ChooseSneakerAsync(gameManager.firebase.userId, sneakersOwned[0]);
        gameManager.aiManager.enabled = true;
    }
}
