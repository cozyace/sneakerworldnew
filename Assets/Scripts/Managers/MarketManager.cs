using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


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
    public long postedDate; //When the listing was created (UNIX TIME)
    public long expirationDate; // UNIX TIME
    
    public MarketListing(string d, string k, int lc, int lg, int q, string s, int sid, long timestamp, long expirationdate)
    {
        description = d;
        key = k;
        listingPriceCash = lc;
        listingPriceGems = lg;
        quantity = q;
        sellerId = s;
        sneakerId = sid;
        postedDate = timestamp;
        expirationDate = expirationdate;
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
    public GameManager GameManager;

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
    [SerializeField] private Button RefreshCircleButton;
    
    //Asset Reference.
    public Sprite[] RarityPanels;

    //All listings on the market.
    public List<PhysicalMarketListing> Listings;
    //The listings loaded that were made by the logged-in user.
    public List<PhysicalMarketListing> MyListings;
    //The sneaker that you have selected in the 'choose item' box.
    private SneakersOwned _SelectedSneaker;


    private bool _IsReloading;
    private float _ReloadTime;







  



    private void Awake() => GameManager = GetComponent<GameManager>();

    private void Start()
    {
        //This will listen for any changes in the market data, and then update the UI accordingly.
        DatabaseReference snapshot = GameManager.firebase.dbReference.Child($"users/{GameManager.firebase.userId}/listings");
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
        RefreshCircleButton.interactable = !_IsReloading;
        
        if (PriceField.text == "" || QuantityField.text == "" || _SelectedSneaker.name == "")
        {
            ListingCost.text = "";
            ListingCostIcon.color = new Color(1, 1, 1, 0);
        }

        if (_IsReloading)
        {
            _ReloadTime -= Time.deltaTime;
            
            if (_ReloadTime <= 0)
                _IsReloading = false;
        }
        
       
    }


    public void RefreshButton()
    {
        if (_IsReloading)
            return;

        _IsReloading = true;
        _ReloadTime = 1.5f;
        print("Refreshing");
        GetListings(false);
        RefreshInventory();
    }


//This method was created with the intention of checking if there was an error spawning a listing, then automatically refreshing to fix. (Not currently used)
private IEnumerator CheckIfNeedPublicReload()
{
    List<MarketListing> allMarketListings = GameManager.firebase.GetMarketplaceListingsAsync().Result;
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

    List<MarketListing> marketListings = await GameManager.firebase.GetMarketplaceListingsAsync();
  
    //Insert the listing data into each slot of the dictionary
    foreach (MarketListing listing in marketListings)
    {
        if(listing.sellerId == GameManager.firebase.userId)
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
    bool isPlayerListing = listing.sellerId == GameManager.firebase.userId;
    Transform listingParent = isPlayerListing ? OwnMarketListingsParent : PublicMarketListingsParent;

    MarketListingItem listingInstance = Instantiate(MarketListingPrefab, listingParent).GetComponent<MarketListingItem>();

    //Find & store the data for the specific sneaker here, grab the data from the database.
    Sneaker sneaker = new Sneaker(GameManager.SneakerDatabase.Database[listing.sneakerId].Name,
        GameManager.SneakerDatabase.Database[listing.sneakerId].Rarity,
        GameManager.SneakerDatabase.Database[listing.sneakerId].Icon != null ? GameManager.SneakerDatabase.Database[listing.sneakerId].Icon.name : "");

    listingInstance.UpdateUIComponents(
        this,
        isPlayerListing,
        sneaker.name,
        listing.listingPriceCash,
        listing.listingPriceGems,
        listing.quantity,
        DateTimeOffset.FromUnixTimeSeconds(listing.expirationDate).LocalDateTime, //REPLACE THIS
        sneaker.rarity,
        Resources.Load<Sprite>($"Sneakers/{sneaker.imagePath}"),
        await GameManager.firebase.GetUsernameFromUserIdAsync(listing.sellerId),
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

private (bool canPurchase, string errorMsg) CanPurchaseListing(MarketListing listing)
{
    //Makes sure it exists, and hasn't expired.
    bool doesListingExist = GameManager.firebase.DoesMarketListingExist(listing.key).Result && DateTimeOffset.UtcNow > DateTimeOffset.FromUnixTimeSeconds(listing.expirationDate);
    bool hasSufficientCurrency = GameManager.GetGems() >= listing.listingPriceGems && GameManager.GetCash() >= listing.listingPriceCash;
    bool hasInventorySpace = GameManager.inventoryManager.GetTotalShoeCountCumulative() + listing.quantity <= 50 + 5 * (GameManager.playerStats.level - 1);

    if (doesListingExist && hasSufficientCurrency && hasInventorySpace)
        return (true, default);

    return (false, $"DoesExist - {doesListingExist} || HasCurrency - {hasSufficientCurrency} || HasInvSpace - {hasInventorySpace}");
}

//Purchases the listing.
public async void PurchaseListing(MarketListing listing)
{
    int purchasePrice = 0;
    string notificationString = "";

    (bool canPurchase, string errorMsg) canPurchase = CanPurchaseListing(listing);

    if (!canPurchase.canPurchase)
    {
        Debug.LogError(canPurchase.errorMsg);
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
        name = GameManager.SneakerDatabase.Database[listing.sneakerId].Name,
        imagePath = GameManager.SneakerDatabase.Database[listing.sneakerId].Icon.name,
        rarity = GameManager.SneakerDatabase.Database[listing.sneakerId].Rarity
    };

    //Remove the shoe from the listing.
    await GameManager.firebase.RemoveMarketListing(listing.key);

    GameManager.inventoryManager.AddShoesToCollection(new SneakersOwned(sneaker.name, listing.quantity, GameManager.SneakerDatabase.Database.Find(x => x.Name == sneaker.name).Value, sneaker.rarity));

    GameManager.DeductCash(listing.listingPriceCash);
    GameManager.DeductGems(listing.listingPriceGems);

    //Destroy & Refresh the listings.
    GetListings(false);

    //Update the seller's gold in the database.
    await GameManager.firebase.UpdateGoldAsync(listing.sellerId, listing.listingPriceCash);
    await GameManager.firebase.UpdateGemsAsync(listing.sellerId, listing.listingPriceGems);

    //Inform the seller that their listing has been sold. (This is for saving data, to override the auto-saving, so it instead READS from the database)
    await GameManager.firebase.AddNotificationToUser(listing.sellerId, $"YOUR LISTING SOLD {listing.quantity}x {GameManager.SneakerDatabase.Database[listing.sneakerId].Name} has sold for {notificationString}");

    await GameManager.firebase.RemoveListingFromUser(GameManager.firebase.userId, listing.key);
}

//All this does is remove the market listing data from the Firebase database.
private async void RemoveListing(string key) => await GameManager.firebase.RemoveMarketListing(key);


public async void Demo_ForceCreateNewListing()
{
    //Add the listing to the database.
    await GameManager.firebase.AddMarketListing(new MarketListing("Item Description...", Guid.NewGuid().ToString(), 0, 15, 5, "JZlewLe0kodO5feIDqFLlBP1TVj1", 1, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds(), new DateTimeOffset(new DateTime(2024,5,12)).ToUnixTimeSeconds()));

    GetListings(false);
    RefreshInventory();
    ResetCreateListingElements();
}

public async void Demo_AddNotificationToUser()
{
    await GameManager.firebase.AddNotificationToUser(GameManager.firebase.userId, $"Test MSG {Random.Range(0,100000)}");
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
        GameManager.DeductCash(Mathf.RoundToInt(Mathf.Clamp(Mathf.Round(cashValue * percentage), 1, 10000)));
    }
    else if (GemIcon.activeSelf)
    {
        gemValue = int.Parse(PriceField.text);
        GameManager.DeductGems(Mathf.RoundToInt(Mathf.Clamp(Mathf.Round(cashValue * percentage), 1, 10000)));
    }

    //Generate a new GUID to act as the key for the listing. (Very very very very low probability that two of the same will exist at the same time ever)
    Guid key = Guid.NewGuid();
    
    DateTime expirationDate = DateTime.Now.AddHours(24*(TimeDropdown.value + 1)); 
    
    //Add the listing to the database.
    await GameManager.firebase.AddMarketListing(new MarketListing("Item Description...", key.ToString(), cashValue, gemValue, int.Parse(QuantityField.text), GameManager.firebase.userId, GameManager.SneakerDatabase.Database.FindIndex(x => x.Name == _SelectedSneaker.name), new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds(), new DateTimeOffset(expirationDate).ToUnixTimeSeconds()));

    //Name of the listing doesn't really matter, just needs to detect changes.
    await GameManager.firebase.AddListingDataToUser(GameManager.firebase.userId, key.ToString());

    SneakersOwned newData = GameManager.inventoryManager.SneakersOwned[GameManager.inventoryManager.SneakersOwned.FindIndex(x => x.name == _SelectedSneaker.name)];
    newData.quantity = int.Parse(QuantityField.text);

    //Remove the quantity from the player's possession.
    GameManager.inventoryManager.RemoveShoeFromCollection(newData);

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
        return (false, "Must enter a price!");

    if (_SelectedSneaker.quantity < int.Parse(QuantityField.text))
        return (false, "You don't have enough of this item!");
    
    //If you're selling with cash, and you've entered a price.
    if (PriceField.text != "" && CashIcon.activeSelf)
    {
        int cashValue = int.Parse(PriceField.text);

        //Calculates the cost of posting the listing, 1% for 24hr, 1.75% for 48hr, 2.25% for 72hr.
        int costValue = Mathf.RoundToInt(Mathf.Clamp(Mathf.Round(cashValue * (0.01f * (TimeDropdown.value + 1))), 1, 10000));

        ListingCost.text = costValue.ToString();
        ListingCost.color = Color.green;
        ListingCostIcon.sprite = Resources.Load<Sprite>("Cash");
        ListingCostIcon.color = new Color(1, 1, 1, 1);
        
        if (GameManager.playerStats.cash < costValue) return(false, "You don't have enough cash to post this!");
        
    }

    //If you're selling with gems, and you've entered a price.
    if (PriceField.text != "" && GemIcon.activeSelf)
    {
        int gemValue = int.Parse(PriceField.text);

        //Calculates the cost of posting the listing, 1% for 24hr, 1.75% for 48hr, 2.25% for 72hr.
        int costValue = Mathf.RoundToInt(Mathf.Clamp(Mathf.Round(gemValue * (0.01f * (TimeDropdown.value + 1))), 1, 10000));

        ListingCost.text = costValue.ToString();
        ListingCost.color = Color.cyan;
        ListingCostIcon.sprite = Resources.Load<Sprite>("Gem");
        ListingCostIcon.color = new Color(1, 1, 1, 1);
        
        if (GameManager.playerStats.gems < costValue) return(false, "You don't have enough gems to post this!");
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
    for (int i = 0; i < GameManager.inventoryManager.SneakersOwned.Count; i++)
    {
        //Grab the UI Components.
        SneakerInventoryItem inventoryShoeInstance = Instantiate(InventoryItemPrefab, PlayerInventoryParent);

        inventoryShoeInstance.ItemNameText.text = GameManager.inventoryManager.SneakersOwned[i].name;
        inventoryShoeInstance.ItemRarityText.text = GameManager.inventoryManager.SneakersOwned[i].rarity.ToString();
        inventoryShoeInstance.ItemQuantityText.text = "QTY:" + GameManager.inventoryManager.SneakersOwned[i].quantity;
        inventoryShoeInstance.RarityPanel.sprite = inventoryShoeInstance.RarityPanelSprites[(int)GameManager.inventoryManager.SneakersOwned[i].rarity - 1];

        //Assign the data to the UI Components.
        inventoryShoeInstance.Initialize(false, true, GameManager.inventoryManager.SneakersOwned[i].name);
        string imagePath = GameManager.SneakerDatabase.Database.Find(x => x.Name == GameManager.inventoryManager.SneakersOwned[i].name).Icon.name;
        inventoryShoeInstance.ItemIconImage.sprite = Resources.Load<Sprite>($"Sneakers/{imagePath}");
    }
}


//This method is attached to each of the UI elements instantiated in the inventory, and make you select the item when you click.
public void SelectItemToList(SneakersOwned sneaker)
{
    InventoryPanel.SetActive(false);
    SelectedShoeIcon.gameObject.SetActive(true);

    string imagePath = GameManager.SneakerDatabase.Database.Find(x => x.Name == sneaker.name).Icon.name;
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
            query = GameManager.SneakerDatabase.Database[listing.Data.sneakerId].Name;

        listing.UI.SetActive(GameManager.SneakerDatabase.Database[listing.Data.sneakerId].Name.ToUpper().Contains(query.ToUpper()));
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
    if (!GameManager.inventoryManager.WillShoeFitInInventory(listing.quantity))
    {
        print("Can't remove listing, inventory will be full once you get the shoe(s) back ; implement UI that says this!!!");
        DeleteListingConfirmationMenu.SetActive(false);
        yield break;
    }

    //Give the shoe back to the lister.
    GameManager.inventoryManager.AddShoesToCollection(new SneakersOwned(GameManager.SneakerDatabase.Database[listing.sneakerId].Name, listing.quantity, GameManager.SneakerDatabase.Database.Find(x => x.Name == GameManager.SneakerDatabase.Database[listing.sneakerId].Name).Value, GameManager.SneakerDatabase.Database[listing.sneakerId].Rarity));
    
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
            DateTime.Compare(DateTimeOffset.FromUnixTimeSeconds(t1.GetComponent<MarketListingItem>().ListingData.postedDate).DateTime,
                DateTimeOffset.FromUnixTimeSeconds(t2.GetComponent<MarketListingItem>().ListingData.postedDate).DateTime));
    }
    else
    {
        sneakers.Sort((Transform t1, Transform t2) =>
            DateTime.Compare(DateTimeOffset.FromUnixTimeSeconds(t2.GetComponent<MarketListingItem>().ListingData.postedDate).DateTime,
                DateTimeOffset.FromUnixTimeSeconds(t1.GetComponent<MarketListingItem>().ListingData.postedDate).DateTime));
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
