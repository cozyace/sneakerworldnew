using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
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
    [Header("Main Inventory Editor Assets")]
    [SerializeField] private GameManager GameManager; 
    [SerializeField] private RectTransform InventoryGridLayout; //The layout where the inventory items are instantiated.
    [SerializeField] private TMP_InputField SearchInputField; //The search input field for querying item names.
    [SerializeField] private GameObject SneakerInventoryUIPrefab; //The prefab that is spawned for use as an inventory item.
    [SerializeField] private GameObject ChooseSneakerPanel; //The panel that allows you to choose a shoe when you don't have any. (start of game)
    
    [Space(10)]
    [Header("Trade Inventory Editor Assets")]
    
    [SerializeField] private RectTransform TradeInventoryGridLayout; //The layout where the trade inventory items are instantiated.
    [SerializeField] private Image TradeSelectedSneakerImage; //The image for the currently selected chosen trade shoe.
    [SerializeField] private GameObject TradeInventoryPanel; //The root of the trade inventory panel *for enabling/disabling*
    [SerializeField] private SelectedSneaker SelectedSneakerData;  //The data of the currently selected sneaker.
    
    [Space(10)]
    [Header("Runtime Data")]
    public List<SneakerInventoryItem> SneakerUIObjects; //All existing UI objects in the main inventory.
    public List<SneakerInventoryItem> TradeSneakerUIObjects;    //All existing UI objects in the trade inventory.
    
    public List<SneakersOwned> SneakersOwned;  //Every sneaker the player owns (has in their inventory), refreshed each time the UI items are spawned.

    public List<string> EnabledItems;
    
    

    private async void Start()
    {
        //Get the list of all of the player's sneakers from the database under /players/userId/sneakers
        List<SneakersOwned> loadedSneakers = await GameManager.firebase.GetSneakerAsync(GameManager.firebase.userId);
        
        //If the player actually has sneakers.
        if (loadedSneakers.Count > 0) 
        {
            //Make sure the 'choose shoe' panel turns off.
            ChooseSneakerPanel.SetActive(false);

            //For each sneaker that the player owns.
            CreateSneakerUIObjects(loadedSneakers);

            //Enable the AI controller.
            GameManager.aiManager.enabled = true;
        }
    }

    private IEnumerator BeginRefreshInventory()
    {
        yield return new WaitForSeconds(0.4f);
        ReloadInventory();
    }
    
    private async void ReloadInventory()
    {
        //Get the list of all of the player's sneakers from the database under /players/userId/sneakers
        List<SneakersOwned> loadedSneakers = await GameManager.firebase.GetSneakerAsync(GameManager.firebase.userId);
        CreateSneakerUIObjects(loadedSneakers);
    }

    public void AddShoesToCollection(SneakersOwned sneaker)
    {
        if (SneakersOwned.Any(x => x.name == sneaker.name))
        {
            SneakersOwned sneakerInstance = SneakersOwned[SneakersOwned.FindIndex(x => x.name == sneaker.name)];
            sneakerInstance.quantity += sneaker.quantity;
            SneakersOwned[SneakersOwned.FindIndex(x => x.name == sneaker.name)] = sneakerInstance;
        }
        else // if the player doesn't have the shoe.
            SneakersOwned.Add(sneaker);
        
        //Save to database either way.
        GameManager.SaveToDatabase();
        StartCoroutine(nameof(BeginRefreshInventory));
    }

    public bool RemoveShoeFromCollection(SneakersOwned sneaker)
    {
        //If the player owns the shoe.
        if (SneakersOwned.Any(x => x.name == sneaker.name))
        {
            //Store the player's specific current owning data of the shoe (e.g. how many the player has and such)
            SneakersOwned sneakerStoredInstance = SneakersOwned[SneakersOwned.FindIndex(x => x.name == sneaker.name)];
            
            //If the sneaker amount you currently have, is atleast more than, or the same than the amount you're trying to remove.
            if (sneakerStoredInstance.quantity >= sneaker.quantity)
            {
                sneakerStoredInstance.quantity -= sneaker.quantity;

                //If the player will have 0 of the shoe left.
                if (sneakerStoredInstance.quantity == 0)
                {
                    //If this is the last owned sneaker that's being deleted, or it's deleting the currently selected sneaker.
                    if (SneakersOwned.Count - 1 == 0 || SelectedSneakerData.GetName().ToUpper() == sneaker.name.ToUpper())
                    {
                        //Reset the Selected Sneaker UI data.
                        SelectedSneakerData.ResetElements();
                    }
                    //Remove the shoe from the list completely.
                    SneakersOwned.RemoveAt(SneakersOwned.FindIndex(x => x.name == sneaker.name));
                    RemoveShoeEntryFromDatabase(sneaker.name);
                    
                    StartCoroutine(nameof(BeginRefreshInventory));
                    return true;
                }
                //If the player still has some of the shoe left.
                if(sneakerStoredInstance.quantity > 0)
                {
                    //Overwrite the player's owned shoe data with the new lowered quantity.
                    SneakersOwned[SneakersOwned.FindIndex(x => x.name == sneaker.name)] = sneakerStoredInstance;
                    GameManager.SaveToDatabase();
                    StartCoroutine(nameof(BeginRefreshInventory));
                    return true;
                }
            }
            //If the player doesn't have enough of the shoe for this transaction.
            else
            {
                return false;
            }
                    
        }
        //If the player doesn't own the shoe.
        else
            return false;

        //This should never be hit.
        return false;
    }

    private async void RemoveShoeEntryFromDatabase(string shoeName)
    {
        await GameManager.firebase.RemoveSneakerFromUser(GameManager.firebase.userId, shoeName);
    }
    
    
    private void CreateSneakerUIObjects(List<SneakersOwned> sneakersToLoad)
    {
        foreach (SneakerInventoryItem existingItem in SneakerUIObjects)
        {
            Destroy(existingItem.gameObject);
        }
        SneakerUIObjects.Clear();
        SneakersOwned.Clear();

        //To store the items that were selected for selling previously, to re-enable them.
        List<string> ItemsToAddBackToEnabled = new List<string>();
        
        foreach (SneakersOwned sneakerLoad in sneakersToLoad)
        {
            //Create the UI Object.
            GameObject newSneaker = Instantiate(SneakerInventoryUIPrefab, InventoryGridLayout);

            SneakerInventoryItem sneakerInventoryItem = newSneaker.GetComponent<SneakerInventoryItem>();
            SneakerInformation sneakerFoundInData = GameManager.SneakerDatabase.Database.Find(x => string.Equals(x.Name, sneakerLoad.name, StringComparison.CurrentCultureIgnoreCase));

            //Assign the data to store on the UI object.
            sneakerInventoryItem.name = sneakerFoundInData.Name;
            sneakerInventoryItem.quantity = sneakerLoad.quantity;
            sneakerInventoryItem.rarity = sneakerFoundInData.Rarity;
            sneakerInventoryItem.purchasePrice = sneakerFoundInData.Value;
            sneakerInventoryItem.CanAIBuy = false;
            sneakerInventoryItem.ItemIconImage.sprite = sneakerFoundInData.Icon;
            sneakerInventoryItem.timestamp = DateTime.Now;
            sneakerInventoryItem.ItemNameText.text = sneakerInventoryItem.name;
            
            //If this item was previously enabled for selling before it was refreshed.
            if (EnabledItems.Contains(sneakerInventoryItem.name))
            {
                sneakerInventoryItem.ToggleSneaker(); //Re-enable it.
                ItemsToAddBackToEnabled.Add(sneakerInventoryItem.name);
            }

            //Adds this UI instance to the list of objects.
            SneakerUIObjects.Add(sneakerInventoryItem);
            
            //Add the sneakers to the list of sneakers you have owned.
            SneakersOwned.Add(sneakerLoad);
            
            //Adds the sneaker to the Friends Trading inventory list.
            foreach (SneakerInventoryItem sneaker in SneakerUIObjects)
            {
                SneakerInventoryItem tradeInventorySneaker = Instantiate(sneaker, TradeInventoryGridLayout);
                tradeInventorySneaker.IsATradeItem = true;
                
                TradeSneakerUIObjects.Add(tradeInventorySneaker);
            }

            //Makes the 'selected' shoe, the first one that gets spawned.
            OnSneakerClick(SneakerUIObjects[0]);
        }
        
        //Reset the list of enabled items.
        EnabledItems.Clear();
        //Add back the items that were already enabled for selling.
        EnabledItems = ItemsToAddBackToEnabled;
    }

    public void OnSneakerClick(SneakerInventoryItem sneakerInventoryItem)
    {
        SelectedSneakerData.UpdateDetails(sneakerInventoryItem);
    }

    public void OnSneakerSwapClick(SneakerInventoryItem sneakerInventoryItem)
    {
        TradeSelectedSneakerImage.sprite = sneakerInventoryItem.ItemIconImage.sprite;
        TradeInventoryPanel.SetActive(false);
    }
    
    
    #region Sorting Logic
    public void FilterBySearch()
    {
        foreach (Transform inventoryItem in InventoryGridLayout.transform)
        {
            SneakerInventoryItem sneaker = inventoryItem.GetComponent<SneakerInventoryItem>();
            inventoryItem.gameObject.SetActive(sneaker.name.ToLower().Contains(SearchInputField.text.ToLower()));
        }
    }

    public void FilterByRarity(TMP_Dropdown dropdown)
    {
        foreach (Transform inventoryItem in InventoryGridLayout.transform)
        {
            SneakerInventoryItem sneaker = inventoryItem.GetComponent<SneakerInventoryItem>();
            inventoryItem.gameObject.SetActive(dropdown.value == 0 || dropdown.value == (int)sneaker.rarity);
        }
    }
    
    public void FilterByBrand(TMP_Dropdown dropdown)
    {
        foreach (Transform inventoryItem in InventoryGridLayout.transform)
        {
            SneakerInventoryItem sneaker = inventoryItem.GetComponent<SneakerInventoryItem>();
            inventoryItem.gameObject.SetActive(dropdown.value == 0 ||
                                               sneaker.name.ToLower().Contains(dropdown.options[dropdown.value].text.ToLower()));
        }
    }

    public void SortByOption(TMP_Dropdown dropdown)
    {
        List<Transform> children = new List<Transform>();
        for (int i = InventoryGridLayout.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = InventoryGridLayout.transform.GetChild(i);
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
            sneaker.parent = InventoryGridLayout.transform;
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
    #endregion
    

    public void AddSneakerSlot()
    {
        print("Unfinished");
        //InitializeSneakers();
    }
    
}
