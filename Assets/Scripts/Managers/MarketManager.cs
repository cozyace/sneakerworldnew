﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


[Serializable]
public struct MarketListing
{
    public string description;
    public string key;
    public int listingPriceCash;
    public int listingPriceGems;
    public int quantity;
    public string sellerId;
    public int sneakerId;
    public DateTime timeStamp;

    public MarketListing(string d, string k, int lc, int lg, int q, string s, int sid, DateTime timestamp)
    {
        description = d;
        key = k;
        listingPriceCash = lc;
        listingPriceGems = lg;
        quantity = q;
        sellerId = s;
        sneakerId = sid;
        timeStamp = timestamp;
    }
}


[Serializable]
public struct PhysicalMarketListing
{
    public GameObject UI;
    public MarketListing Data;

    public PhysicalMarketListing(GameObject ui, MarketListing data)
    {
        UI = ui;
        Data = data;
    }
}

public class MarketManager : MonoBehaviour
{
    private GameManager _GameManager;

    [Header("References/Assets")]
    public GameObject MarketListingPrefab;
    public SneakerInventoryItem InventoryItemPrefab;
    [SerializeField] private Transform PublicMarketListingsParent;
    [SerializeField] private Transform OwnMarketListingsParent;
    [SerializeField] private Transform PlayerInventoryParent;
    [SerializeField] private GameObject InventoryPanel;
    [SerializeField] private GameObject CreateListingPanel;
    [SerializeField] private GameObject DeleteListingConfirmationMenu;

    //Smaller-Importance UI References.
    [SerializeField] private Button CreateListingButton;
    [SerializeField] private Image SelectedShoeIcon;
    [SerializeField] private TMP_Text ErrorText;
    [SerializeField] private GameObject CashIcon;
    [SerializeField] private GameObject GemIcon;
    [SerializeField] private TMP_InputField PriceField;
    [SerializeField] private TMP_InputField QuantityField;
    [SerializeField] private TMP_Dropdown TimeDropdown; //Time selection (24,48,72)
    [SerializeField] private TMP_Text ListingCost; //How much it'll cost the player to post the listing.
    [SerializeField] private Image ListingCostIcon;
    
    //Asset Reference.
    public Sprite[] RarityPanels;

    //All listings on the market.
    public List<PhysicalMarketListing> Listings;
    //The listings loaded that were made by the logged-in user.
    public List<PhysicalMarketListing> MyListings;
    //The sneaker that you have selected in the 'choose item' box.
    private SneakersOwned _SelectedSneaker;







  



    private void Awake() => _GameManager = GetComponent<GameManager>();

    private void Start()
    {
        //This will listen for any changes in the market data, and then update the UI accordingly.
        DatabaseReference snapshot = _GameManager.firebase.dbReference.Child($"users/{_GameManager.firebase.userId}/listings");
        snapshot.ChildAdded += ListenForNewListing;
        snapshot.ChildRemoved += ListenForNewListing;
        
        Invoke(nameof(RefreshInventory), 0.95f);
        Invoke(nameof(RefreshButton), 1f);
    }
    
    //Called when a  child is added / removed from the listings
    private void ListenForNewListing(object sender, ChildChangedEventArgs args)
    {
        //If a player's listing is changed, reset player listings.
        GetListings(true);
        //Refresh player's inventory.
        RefreshInventory();
    }

    private void Update()
    {
        CreateListingButton.interactable = MyListings.Count < 8;
        ErrorText.text = CanListSelectedItem().Item2;
        if (PriceField.text == "" || QuantityField.text == "" || _SelectedSneaker.name == "")
        {
            ListingCost.text = "";
            ListingCostIcon.color = new Color(1, 1, 1, 0);
        }
    }


    public void RefreshButton()
    {
        print("Refreshing");
        GetListings(false);
        RefreshInventory();
    }


//This method was created with the intention of checking if there was an error spawning a listing, then automatically refreshing to fix. (Not currently used)
private IEnumerator CheckIfNeedPublicReload()
{
    List<MarketListing> allMarketListings = _GameManager.firebase.GetMarketplaceListingsAsync().Result;
    yield return new WaitForSeconds(0.8f);

    if (PublicMarketListingsParent.childCount == allMarketListings.Count)
        yield break;

    Debug.LogWarning("Needed Reload!");

    //If some of the listings are missing, refresh.
    RefreshButton();
}

//Populates either the global & player listings, or only player listings.
private async void GetListings(bool reloadPlayerListingsOnly)
{
    print("Getting Listings!!");
    //If you're not only reloading the player's
    if (!reloadPlayerListingsOnly)
    {
        //Delete all existing global market items.
        foreach (Transform t in PublicMarketListingsParent)
            Destroy(t.gameObject);
    }

    MyListings.Clear();

    if (!reloadPlayerListingsOnly)
        Listings.Clear();

    //Store every listing in a temporary list.

    List<MarketListing> marketListings = await _GameManager.firebase.GetMarketplaceListingsAsync();
  
    //Insert the listing data into each slot of the dictionary
    foreach (MarketListing listing in marketListings)
    {
        if(listing.sellerId == _GameManager.firebase.userId)
            MyListings.Add(new PhysicalMarketListing(null, listing));
        else
        {
            if (!reloadPlayerListingsOnly)
                Listings.Add(new PhysicalMarketListing(null, listing));
        }
    }

    //Go through all of the global listing (not the player's) and instantiate them individually.
    for (int listingIndex = 0; listingIndex < Listings.Count; listingIndex++)
    {
        //Create the physical representation of another user's market listing.
        if (!reloadPlayerListingsOnly)
            InstantiateMarketListingUI(Listings[listingIndex].Data);

     //   StartCoroutine(nameof(CheckIfNeedPublicReload));
    }

    //Refreshes the player's listings.
    RefreshPlayerListings();
}

//Creates a single UI object representing a listing.
private async void InstantiateMarketListingUI(MarketListing listing)
{
    print("INSTANTIATING LISTING - " + listing.key);
    bool isPlayerListing = listing.sellerId == _GameManager.firebase.userId;
    Transform listingParent = isPlayerListing ? OwnMarketListingsParent : PublicMarketListingsParent;

    MarketListingItem listingInstance = Instantiate(MarketListingPrefab, listingParent).GetComponent<MarketListingItem>();

    //Find & store the data for the specific sneaker here, grab the data from the database.
    Sneaker sneaker = new Sneaker(_GameManager.SneakerDatabase.Database[listing.sneakerId].Name,
        _GameManager.SneakerDatabase.Database[listing.sneakerId].Rarity,
        _GameManager.SneakerDatabase.Database[listing.sneakerId].Icon != null ? _GameManager.SneakerDatabase.Database[listing.sneakerId].Icon.name : "");

    listingInstance.UpdateUIComponents(
        this,
        isPlayerListing,
        sneaker.name,
        listing.listingPriceCash,
        listing.listingPriceGems,
        listing.quantity,
        sneaker.rarity,
        Resources.Load<Sprite>($"Sneakers/{sneaker.imagePath}"),
        await _GameManager.firebase.GetUsernameFromUserIdAsync(listing.sellerId),
        RarityPanels[(int)sneaker.rarity - 1],
        listing
    );

    List<PhysicalMarketListing> listingList = isPlayerListing ? MyListings : Listings;

    //Stores where the entry is on the list.
    int physicalMarketListingEntryIndex = listingList.FindIndex(x => x.Data.Equals(listing));

    //Update the data with the newly created gameobject.
    if (physicalMarketListingEntryIndex != -1)
        listingList[physicalMarketListingEntryIndex] = new PhysicalMarketListing(listingInstance.gameObject, listingList[physicalMarketListingEntryIndex].Data);
    else
        print("DATA WAS NOT FOUND IN LISTINGS");

}

//Purchases the listing.
public async void PurchaseListing(MarketListing listing)
{
    int purchasePrice = 0;
    string notificationString = "";


    if (_GameManager.GetGems() < listing.listingPriceGems || _GameManager.GetCash() < listing.listingPriceCash)
    {
        print("Insufficient Currency to Purchase!");
        return;
    }

    //If purchasing this shoe would put you over your quantity limit for your level.
    if (_GameManager.inventoryManager.GetTotalShoeCountCumulative().Result + listing.quantity > 50 + 5 * (_GameManager.firebase.playerStats.level - 1))
    {
        print("Insufficient Inventory Space to Purchase!");
        return;
    }

    //Check if cost requirements are met.
    if (listing.listingPriceCash > 0)
    {
        purchasePrice = listing.listingPriceCash;
        notificationString = purchasePrice + " Cash";
    }
    else if (listing.listingPriceGems > 0)
    {
        purchasePrice = listing.listingPriceGems;
        notificationString = purchasePrice + " Gems";
    }


    //Store the shoe as a Sneaker type.
    Sneaker sneaker = new Sneaker
    {
        name = _GameManager.SneakerDatabase.Database[listing.sneakerId].Name,
        imagePath = _GameManager.SneakerDatabase.Database[listing.sneakerId].Icon.name,
        rarity = _GameManager.SneakerDatabase.Database[listing.sneakerId].Rarity
    };

    //Remove the shoe from the listing.
    await _GameManager.firebase.RemoveMarketListing(listing.key);

    _GameManager.inventoryManager.AddShoesToCollection(new SneakersOwned(sneaker.name, listing.quantity, _GameManager.SneakerDatabase.Database.Find(x => x.Name == sneaker.name).Value, sneaker.rarity));

    _GameManager.DeductCash(listing.listingPriceCash);
    _GameManager.DeductGems(listing.listingPriceGems);

    //Destroy & Refresh the listings.
    GetListings(false);

    //Update the seller's gold in the database.
    await _GameManager.firebase.UpdateGoldAsync(listing.sellerId, listing.listingPriceCash);
    await _GameManager.firebase.UpdateGemsAsync(listing.sellerId, listing.listingPriceGems);

    //Inform the seller that their listing has been sold. (This is for saving data, to override the auto-saving, so it instead READS from the database)
    await _GameManager.firebase.AddNotificationToUser(listing.sellerId, $"Your listing of {listing.quantity}x {_GameManager.SneakerDatabase.Database[listing.sneakerId].Name} has sold for {notificationString}");

    await _GameManager.firebase.RemoveListingFromUser(_GameManager.firebase.userId, listing.key);
}

//All this does is remove the market listing data from the Firebase database.
private async void RemoveListing(string key) => await _GameManager.firebase.RemoveMarketListing(key);


public async void Demo_ForceCreateNewListing()
{
    //Add the listing to the database.
    await _GameManager.firebase.AddMarketListing(new MarketListing("Item Description...", Guid.NewGuid().ToString(), 0, 15, 5, "JZlewLe0kodO5feIDqFLlBP1TVj1", 1, DateTime.Now));

    GetListings(false);
    RefreshInventory();
    ResetCreateListingElements();
}

//Creates a new listing with the chosen item. (Called via UI button in game)
public async void CreateNewListing()
{
    if (!CanListSelectedItem().Item1)
        return;
    
    //Do a check here for if, the player's gems/cash is less than 1-2% of the listing price, then don't list.

    int cashValue = 0;
    int gemValue = 0;
    
    float percentage = 0.01f * (TimeDropdown.value + 1); //What percentage of the listing price is going to apply to the lister?

    if (CashIcon.activeSelf)
    {
        cashValue = int.Parse(PriceField.text);
        //Calculates the cost of posting the listing, 1% for 24hr, 1.75% for 48hr, 2.25% for 72hr.
        _GameManager.DeductCash(Mathf.RoundToInt(Mathf.Clamp(Mathf.Round(cashValue * percentage), 1, 10000)));
    }
    else if (GemIcon.activeSelf)
    {
        gemValue = int.Parse(PriceField.text);
        _GameManager.DeductGems(Mathf.RoundToInt(Mathf.Clamp(Mathf.Round(cashValue * percentage), 1, 10000)));
    }

    //Generate a new GUID to act as the key for the listing. (Very very very very low probability that two of the same will exist at the same time ever)
    Guid key = Guid.NewGuid();

    //Add the listing to the database.
    await _GameManager.firebase.AddMarketListing(new MarketListing("Item Description...", key.ToString(), cashValue, gemValue, int.Parse(QuantityField.text), _GameManager.firebase.userId, _GameManager.SneakerDatabase.Database.FindIndex(x => x.Name == _SelectedSneaker.name), DateTime.Now));

    //Name of the listing doesn't really matter, just needs to detect changes.
    await _GameManager.firebase.AddListingDataToUser(_GameManager.firebase.userId, key.ToString());

    SneakersOwned newData = _GameManager.inventoryManager.SneakersOwned[_GameManager.inventoryManager.SneakersOwned.FindIndex(x => x.name == _SelectedSneaker.name)];
    newData.quantity = int.Parse(QuantityField.text);

    //Remove the quantity from the player's possession.
    _GameManager.inventoryManager.RemoveShoeFromCollection(newData);

    GetListings(false);
    RefreshInventory();
    ResetCreateListingElements();

    CreateListingPanel.SetActive(false);
}

//Does the current entered listing meet all the requirements for posting. (RETURNS STATUS & ERROR LOG)
private (bool, string) CanListSelectedItem()
{
    if (_SelectedSneaker.name == "")
        return (false, "Must select a shoe!");

    if (QuantityField.text == "")
        return (false, "Must enter a quantity!");

    if (PriceField.text == "")
        return (false, "Must enter a price to create listing!");

    if (_SelectedSneaker.quantity < int.Parse(QuantityField.text))
        return (false, "You don't have enough of this item!");
    
    //If you're selling with cash, and you've entered a price.
    if (PriceField.text != "" && CashIcon.activeSelf)
    {
        int cashValue = int.Parse(PriceField.text);
        float percentage = 0.01f * (TimeDropdown.value + 1); //What percentage of the listing price is going to apply to the lister?

        //Calculates the cost of posting the listing, 1% for 24hr, 1.75% for 48hr, 2.25% for 72hr.
        int costValue = Mathf.RoundToInt(Mathf.Clamp(Mathf.Round(cashValue * percentage), 1, 10000));

        ListingCost.text = costValue.ToString();
        ListingCost.color = Color.green;
        ListingCostIcon.sprite = Resources.Load<Sprite>("Cash");
        ListingCostIcon.color = new Color(1, 1, 1, 1);
        
        if (_GameManager.firebase.playerStats.cash < costValue)
        {
            return(false, "You don't have enough cash to post this!");
        }
    }

    //If you're selling with gems, and you've entered a price.
    if (PriceField.text != "" && GemIcon.activeSelf)
    {
        int gemValue = int.Parse(PriceField.text);
        float percentage = 0.01f * (TimeDropdown.value + 1); //What percentage of the listing price is going to apply to the lister?
        
        
        //Calculates the cost of posting the listing, 1% for 24hr, 1.75% for 48hr, 2.25% for 72hr.
        int costValue = Mathf.RoundToInt(Mathf.Clamp(Mathf.Round(gemValue * percentage), 1, 10000));

        ListingCost.text = costValue.ToString();
        ListingCost.color = Color.cyan;
        ListingCostIcon.sprite = Resources.Load<Sprite>("Gem");
        ListingCostIcon.color = new Color(1, 1, 1, 1);
        
        if (_GameManager.firebase.playerStats.gems < costValue)
        {
            return(false, "You don't have enough gems to post this!");
        }
    }
    
    
    return (true, "");
}


//Deletes all existing inventory UI items, and then respawns them.
private void RefreshInventory()
{
    List<GameObject> invItems = new List<GameObject>();

    for (int i = 0; i < PlayerInventoryParent.childCount; i++)
        invItems.Add(PlayerInventoryParent.GetChild(i).gameObject);

    foreach (GameObject item in invItems)
        Destroy(item);

    PopulateMarketInventory();
}

//Destroys all existing 'My Listings', enables/disables trash button, and spawns new 'My Listings'.
private void RefreshPlayerListings()
{
    //Delete all existing global market items.
    foreach (Transform t in OwnMarketListingsParent)
        Destroy(t.gameObject);

    //Go through all of the global listing (not the player's) and instantiate them individually.
    for (int listingIndex = 0; listingIndex < MyListings.Count; listingIndex++)
    {
        //Create the physical representation of another user's market listing.
        InstantiateMarketListingUI(MyListings[listingIndex].Data);
    }

    print($"<size=14><color=green>MARKET</color> | Player listings refreshed. </size>");
}

//Populates the inventory within the marketplace.
private void PopulateMarketInventory()
{
    for (int i = 0; i < _GameManager.inventoryManager.SneakersOwned.Count; i++)
    {
        //Grab the UI Components.
        SneakerInventoryItem inventoryShoeInstance = Instantiate(InventoryItemPrefab, PlayerInventoryParent);

        inventoryShoeInstance.ItemNameText.text = _GameManager.inventoryManager.SneakersOwned[i].name;
        inventoryShoeInstance.ItemRarityText.text = _GameManager.inventoryManager.SneakersOwned[i].rarity.ToString();
        inventoryShoeInstance.ItemQuantityText.text = "QTY:" + _GameManager.inventoryManager.SneakersOwned[i].quantity;
        inventoryShoeInstance.RarityPanel.sprite = inventoryShoeInstance.RarityPanelSprites[(int)_GameManager.inventoryManager.SneakersOwned[i].rarity - 1];

        //Assign the data to the UI Components.
        inventoryShoeInstance.Initialize(false, true, _GameManager.inventoryManager.SneakersOwned[i].name);
        string imagePath = _GameManager.SneakerDatabase.Database.Find(x => x.Name == _GameManager.inventoryManager.SneakersOwned[i].name).Icon.name;
        inventoryShoeInstance.ItemIconImage.sprite = Resources.Load<Sprite>($"Sneakers/{imagePath}");
    }
}


//This method is attached to each of the UI elements instantiated in the inventory, and make you select the item when you click.
public void SelectItemToList(SneakersOwned sneaker)
{
    InventoryPanel.SetActive(false);
    SelectedShoeIcon.gameObject.SetActive(true);

    string imagePath = _GameManager.SneakerDatabase.Database.Find(x => x.Name == sneaker.name).Icon.name;
    SelectedShoeIcon.sprite = Resources.Load<Sprite>($"Sneakers/{imagePath}");
    _SelectedSneaker = sneaker;
    //Put sprite in the main box.
}



//Attached to the Search box via Dynamic event.
public void SetMarketListingActiveByName(string query)
{
    foreach (PhysicalMarketListing listing in Listings)
    {
        if (query == "") //If the query is empty, then turn them all on by setting the query to each name.
            query = _GameManager.SneakerDatabase.Database[listing.Data.sneakerId].Name;

        listing.UI.SetActive(_GameManager.SneakerDatabase.Database[listing.Data.sneakerId].Name.ToUpper().Contains(query.ToUpper()));
    }
}



public void EnableDeleteConfirmationMenu(MarketListing listing)
{
    //Enable the confirmation UI element.
    DeleteListingConfirmationMenu.SetActive(true);

    //Get the confirmation button.
    Button confirmButton = DeleteListingConfirmationMenu.transform.GetChild(0).GetChild(1).GetComponent<Button>();

    //Attach the hook to remove the specific listing.
    confirmButton.onClick.RemoveAllListeners();
    confirmButton.onClick.AddListener(() => StartCoroutine(ConfirmRemoveListing(listing.key, listing)));
}


//Removes listing, and returns the shoe to the seller.
private IEnumerator ConfirmRemoveListing(string key, MarketListing listing)
{
    //Give the shoe back to the lister.
    _GameManager.inventoryManager.AddShoesToCollection(new SneakersOwned(_GameManager.SneakerDatabase.Database[listing.sneakerId].Name, listing.quantity, _GameManager.SneakerDatabase.Database.Find(x => x.Name == _GameManager.SneakerDatabase.Database[listing.sneakerId].Name).Value, _GameManager.SneakerDatabase.Database[listing.sneakerId].Rarity));

    //Disable the confirmation menu.
    DeleteListingConfirmationMenu.SetActive(false);

    //Remove the listing from the Firebase Database.
    RemoveListing(key);

    //Wait a second.
    yield return new WaitForSeconds(0.35f);

    //Refresh everything.
    RefreshPlayerListings();
    GetListings(true);
    RefreshInventory();
}


//Finds if the listing already exists, and returns the index. (If it doesn't exist, returns -1)
private int GetMarketListingIndexByData(MarketListing listing, IReadOnlyList<PhysicalMarketListing> list)
{
    for (int i = 0; i < list.Count; i++)
    {
        if (list[i].Data.Equals(listing))
        {
            return i;
        }
    }
    //Do a check here, go through each of the existing listings in the list, and check if it still exists.
    return -1;
}




//Resets the UI data for the current listing.
private void ResetCreateListingElements()
{
    _SelectedSneaker = new SneakersOwned();
    SelectedShoeIcon.gameObject.SetActive(false);
    SelectedShoeIcon.sprite = null;
    QuantityField.text = "";
    PriceField.text = "";
    TimeDropdown.value = 0;
    ListingCost.text = "";
    
}

public void SwapCurrencyType()
{
    CashIcon.SetActive(!CashIcon.activeSelf);
    GemIcon.SetActive(!GemIcon.activeSelf);
}





public void FilterByRarity(TMP_Dropdown dropdown)
{
    foreach (Transform marketItem in PublicMarketListingsParent.transform)
    {
        MarketListingItem sneaker = marketItem.GetComponent<MarketListingItem>();
        marketItem.gameObject.SetActive(dropdown.value == 0 || dropdown.value == (int)sneaker.Rarity);
    }
}

public void FilterByBrand(TMP_Dropdown dropdown)
{
    foreach (Transform marketItem in PublicMarketListingsParent.transform)
    {
        MarketListingItem sneaker = marketItem.GetComponent<MarketListingItem>();
        marketItem.gameObject.SetActive(dropdown.value == 0 ||
            sneaker.Name.ToLower().Contains(dropdown.options[dropdown.value].text.ToLower()));
    }
}

public void SortByOption(TMP_Dropdown dropdown)
{
    List<Transform> children = new List<Transform>();
    for (int i = PublicMarketListingsParent.transform.childCount - 1; i >= 0; i--)
    {
        Transform child = PublicMarketListingsParent.transform.GetChild(i);
        children.Add(child);
        child.parent = null;
    }

    List<Transform> sortedList = new List<Transform>();
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
        sneaker.parent = PublicMarketListingsParent.transform;
    }
}


private List<Transform> SortByTime(List<Transform> sneakers, bool newestFirst)
{
    if (newestFirst)
    {
        sneakers.Sort((t1, t2) =>
            DateTime.Compare(t1.GetComponent<MarketListingItem>().ListingData.timeStamp,
                t2.GetComponent<MarketListingItem>().ListingData.timeStamp));
    }
    else
    {
        sneakers.Sort((Transform t1, Transform t2) =>
            DateTime.Compare(t2.GetComponent<MarketListingItem>().ListingData.timeStamp,
                t1.GetComponent<MarketListingItem>().ListingData.timeStamp));
    }

    return sneakers;
}

private List<Transform> SortByName(List<Transform> sneakers, bool ascending)
{
    if (ascending)
    {
        sneakers.Sort((t1, t2) => string.Compare(t1.GetComponent<MarketListingItem>().Name, t2.GetComponent<MarketListingItem>().Name, StringComparison.Ordinal));
    }
    else
    {
        sneakers.Sort((t1, t2) => string.Compare(t2.GetComponent<MarketListingItem>().Name, t1.GetComponent<MarketListingItem>().Name, StringComparison.Ordinal));
    }

    return sneakers;
}

private List<Transform> SortByRarity(List<Transform> sneakers, bool ascending)
{
    if (ascending)
    {
        sneakers.Sort((t1, t2) => t1.GetComponent<MarketListingItem>().Rarity
            .CompareTo(t2.GetComponent<MarketListingItem>().Rarity));
    }
    else
    {
        sneakers.Sort((t1, t2) => t2.GetComponent<MarketListingItem>().Rarity
            .CompareTo(t1.GetComponent<MarketListingItem>().Rarity));
    }

    return sneakers;
}

private List<Transform> SortByPrice(List<Transform> sneakers, bool ascending)
{
    if (ascending)
    {
        sneakers.Sort((t1, t2) => t1.GetComponent<MarketListingItem>().CashPrice
            .CompareTo(t2.GetComponent<MarketListingItem>().CashPrice));
    }
    else
    {
        sneakers.Sort((t1, t2) => t2.GetComponent<MarketListingItem>().CashPrice
            .CompareTo(t1.GetComponent<MarketListingItem>().CashPrice));
    }

    return sneakers;
}









}
