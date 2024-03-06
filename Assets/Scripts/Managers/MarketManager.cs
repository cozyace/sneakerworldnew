using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
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

        public MarketListing(string d, string k, int lc, int lg, int q, string s, int sid)
        {
                description = d;
                key = k;
                listingPriceCash = lc;
                listingPriceGems = lg;
                quantity = q;
                sellerId = s;
                sneakerId = sid;
        }
}

public class MarketManager :MonoBehaviour
{
        private GameManager _GameManager;
        
        [Header("References/Assets")]
        public GameObject MarketListingPrefab;
        public GameObject InventoryItemPrefab;
        [SerializeField] private Transform MarketContentRoot;
        [SerializeField] private GameObject InventoryPanel;
        [SerializeField] private GameObject MyListingsPanel;
        [SerializeField] private GameObject CreateListingPanel;
        [SerializeField] private Image SelectedShoeIcon;
        [SerializeField] private GameObject DeleteListingConfirmationMenu;
        [SerializeField] private TMP_Text ListingCountLabel;
        [SerializeField] private GameObject InsufficientQuantityText;
        public Sprite[] RarityPanels;

        private SneakersOwned SelectedSneaker;
        
        [SerializeField] private TMP_InputField PriceField;
        [SerializeField] private TMP_InputField QuantityField;

        [SerializeField] private Button[] InventoryItemSlots;

        [SerializeField] private GameObject[] YourListingSlots;

        [SerializeField] private GameObject CashIcon;
        [SerializeField] private GameObject GemIcon;

        private List<MarketListing> _Listings = new List<MarketListing>();
        public List<MarketListing> _MyListings = new List<MarketListing>();
        
        private float _RefreshTimer = 0.25f;
        
        private void Awake() => _GameManager = GetComponent<GameManager>();

        private void Update()
        {
                _RefreshTimer -= Time.deltaTime;
                if (_RefreshTimer <= 0)
                {
                        GetListings();
                        RefreshInventory();
                        _RefreshTimer = 2.5f;
                }

                if (SelectedSneaker.name != "" && QuantityField.text != "")
                {
                        if (int.Parse(QuantityField.text) > SelectedSneaker.quantity)
                                InsufficientQuantityText.SetActive(true);
                        else
                                InsufficientQuantityText.SetActive(false);
                }
                else
                        InsufficientQuantityText.SetActive((false));
        }

        private async void GetListings()
        {
                DestroyAllListings();
                
                _Listings = await _GameManager.firebase.GetMarketplaceListingsAsync();
                
                _MyListings.Clear();
                
                foreach (MarketListing sneaker in _Listings)
                {
                        //Make sure this isn't this player's listing.
                        if(sneaker.sellerId != _GameManager.firebase.userId)
                                InstantiateMarketListing(sneaker);
                        else
                                _MyListings.Add(sneaker);
                        //Add these player-created listings to the listing tab.
                }

                ListingCountLabel.text = _MyListings.Count.ToString();

                print("Called");
                
                RefreshPlayerListings();
        }


        public void SwapCurrencyType()
        {
                CashIcon.SetActive(!CashIcon.activeSelf);
                GemIcon.SetActive(!GemIcon.activeSelf);
        }

        private void DestroyAllListings()
        {
                foreach (Transform t in MarketContentRoot)
                {
                        Destroy(t.gameObject);
                }
                
                _Listings.Clear();
        }
        

        private async void InstantiateMarketListing(MarketListing listing)
        {
                Transform listingInstance = Instantiate(MarketListingPrefab, MarketContentRoot).transform;

                //Find & store the data for the specific sneaker here, grab the data from the database.
                Sneaker sneaker = _GameManager._sneakers[listing.sneakerId];

                //Grab the references to the UI elements in the newly instantiated panel.
                Image listingImage = listingInstance.Find("Sneaker").GetComponent<Image>();
                Image rarityPanel = listingInstance.Find("Rarity").GetComponent<Image>();
                TMP_Text listingItemName = listingInstance.Find("ShoeName").GetComponent<TMP_Text>();
                TMP_Text listingUserName = listingInstance.Find("Username").GetComponent<TMP_Text>();
                TMP_Text listingPrice = listingInstance.Find("Price").GetChild(0).GetComponent<TMP_Text>();
                TMP_Text listingQuantity = listingInstance.Find("Quantity").GetChild(0).GetComponent<TMP_Text>();
                TMP_Text listingRarity = rarityPanel.transform.GetChild(0).GetComponent<TMP_Text>();
                Button buyButton = listingInstance.Find("BuyButton").GetComponent<Button>();
                
                //Assign all information to the UI elements.
                listingImage.sprite = Resources.Load<Sprite>($"Sneakers/{sneaker.imagePath[..^5]}");
                listingUserName.text = await _GameManager.firebase.GetUsernameFromUserIdAsync(listing.sellerId);
                listingItemName.text = sneaker.name;
                listingRarity.text = sneaker.rarity.ToString();
                listingQuantity.text = listing.quantity.ToString();
                rarityPanel.sprite = RarityPanels[(int)sneaker.rarity-1];
                
                buyButton.onClick.AddListener(() => PurchaseListing(listing));
                
                if (listing.listingPriceCash > 0)
                        listingPrice.text = "$" + listing.listingPriceCash;
                else if (listing.listingPriceGems > 0)
                { //Add signification that this is gems, maybe icon.
                        listingPrice.text = listing.listingPriceGems.ToString();
                        listingPrice.color = Color.cyan;
                }
        }




        private async void PurchaseListing(MarketListing listing)
        {
                int purchasePrice = 0;
                
                //Check if cost requirements are met.
                if (listing.listingPriceCash > 0)
                {
                        if (_GameManager.GetGems() < listing.listingPriceGems)
                        {
                                print("Insufficient Cash to Purchase!");
                                return;
                        }
                        
                        purchasePrice = listing.listingPriceCash;
                }
                else if (listing.listingPriceGems > 0)
                {
                        if (_GameManager.GetCash() < listing.listingPriceCash)
                        {
                                print("Insufficient Gems to Purchase!");
                                return;
                        }
                        
                        purchasePrice = listing.listingPriceGems;
                }
                        
                
                //Store the shoe as a Sneaker type.
                Sneaker sneaker = _GameManager._sneakers[listing.sneakerId];
                
                //Remove the shoe from the listing.
                await _GameManager.firebase.RemoveMarketListing(listing.key);
                
                _GameManager.inventoryManager.AddShoesToCollection(new SneakersOwned(sneaker.name, listing.quantity, 100, sneaker.rarity));
                        
                _GameManager.DeductCash(listing.listingPriceCash);
                _GameManager.DeductGems(listing.listingPriceGems);
                
                //Destroy & Refresh the listings.
                DestroyAllListings();
                GetListings();
                
                //Store a copy of the seller's current stats.
                PlayerStats modifiedSellerStats = await _GameManager.firebase.LoadDataAsync(listing.sellerId);
                
                
                //Check again what the currency type is, so that the right property can be adjusted.
                if (listing.listingPriceCash > 0)
                        modifiedSellerStats.cash += purchasePrice;
                else if (listing.listingPriceGems > 0)
                        modifiedSellerStats.gems += purchasePrice;
                //
                //Overwrite the seller's data with this new data.
                await _GameManager.firebase.SaveDataAsync(listing.sellerId, modifiedSellerStats);
                
                //need to verify that the other logged in user will actually recieve this money in real-time, or if they'll need to restart.
                //Might have to put a LoadAsync into that Invoked function, so it updates atleast after a bit.
        }
        
        //Populates the inventory within the marketplace.
        private void PopulateMarketInventory()
        {
                for (int i = 0; i < InventoryItemSlots.Length; i++)
                {
                        if (i == _GameManager.inventoryManager.sneakersOwned.Count)
                                return;
                        
                        Image inventoryShoeInstance = Instantiate(InventoryItemPrefab, InventoryItemSlots[i].transform).GetComponent<Image>();
                        TMP_Text shoeName = inventoryShoeInstance.transform.GetChild(0).GetComponent<TMP_Text>();
                        TMP_Text shoeQuantity = inventoryShoeInstance.transform.GetChild(1).GetComponent<TMP_Text>();

                        shoeName.text = _GameManager.inventoryManager.sneakersOwned[i].name;
                        shoeQuantity.text = _GameManager.inventoryManager.sneakersOwned[i].quantity.ToString();
                        string imagePath = _GameManager._sneakers.Find(x => x.name == _GameManager.inventoryManager.sneakersOwned[i].name).imagePath[..^5];
                        inventoryShoeInstance.sprite = Resources.Load<Sprite>($"Sneakers/{imagePath}");
                        
                        int i1 = i; //Used so they don't call have the same index.
                        InventoryItemSlots[i].onClick.AddListener(()=> SelectItemToList(_GameManager.inventoryManager.sneakersOwned[i1]));
                }
        }

        private void PopulateClientListings()
        {
                for (int i = 0; i < YourListingSlots.Length; i++)
                {
                        if (i == _MyListings.Count) 
                                return;
                        
                        Image inventoryShoeInstance = Instantiate(InventoryItemPrefab, YourListingSlots[i].transform).GetComponent<Image>();
                        TMP_Text shoeName = inventoryShoeInstance.transform.GetChild(0).GetComponent<TMP_Text>();
                        TMP_Text shoeQuantity = inventoryShoeInstance.transform.GetChild(1).GetComponent<TMP_Text>();
                        
                        
                        shoeName.text = _GameManager._sneakers[_MyListings[i].sneakerId].name;
                       shoeQuantity.text = _MyListings[i].quantity.ToString();
                        string imagePath = _GameManager._sneakers[_MyListings[i].sneakerId].imagePath[..^5];
                        inventoryShoeInstance.sprite = Resources.Load<Sprite>($"Sneakers/{imagePath}");

                        Button deleteButton = YourListingSlots[i].transform.GetChild(0).GetComponent<Button>();

                        int storedIndex = i;
                        
                        deleteButton.onClick.AddListener(()=> EnableDeleteConfirmationMenu(_MyListings[storedIndex]));
                }
        }

        private void EnableDeleteConfirmationMenu(MarketListing listing)
        {
                DeleteListingConfirmationMenu.SetActive(true);
                
                Button confirmButton = DeleteListingConfirmationMenu.transform.GetChild(0).GetChild(1).GetComponent<Button>();
                
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(()=> StartCoroutine(ConfirmRemoveListing(listing.key, listing)));
        }

        private async void RemoveListing(string key)
        {
                await _GameManager.firebase.RemoveMarketListing(key);
        }
        
        private IEnumerator ConfirmRemoveListing(string key, MarketListing listing)
        {
                _GameManager.inventoryManager.AddShoesToCollection(new SneakersOwned(_GameManager._sneakers[listing.sneakerId].name, listing.quantity, 100, _GameManager._sneakers[listing.sneakerId].rarity));
                DeleteListingConfirmationMenu.SetActive(false);
                RemoveListing(key);

                yield return new WaitForSeconds(0.5f);
                
                GetListings();
                RefreshInventory();
        }
        

        private void RefreshInventory()
        {
                for (int i = 0; i < InventoryItemSlots.Length; i++)
                {
                        if (InventoryItemSlots[i].transform.childCount == 0)
                                continue;

                        for (int x = 0; x < InventoryItemSlots[i].transform.childCount; x++)
                        {
                                Destroy(InventoryItemSlots[i].transform.GetChild(0).gameObject);
                        }
                }
                
                PopulateMarketInventory();
        }

        private void RefreshPlayerListings()
        {
                for (int i = 0; i < YourListingSlots.Length; i++)
                {
                        Button deleteButton = YourListingSlots[i].transform.GetChild(0).GetComponent<Button>();
                        deleteButton.enabled = YourListingSlots[i].transform.childCount != 1;
                        
                        if (YourListingSlots[i].transform.childCount == 1)
                                continue;
                        
                        Destroy(YourListingSlots[i].transform.GetChild(1).gameObject);
                }
                
                print("Refreshing Listings");
                PopulateClientListings();
        }

        private void SelectItemToList(SneakersOwned sneaker)
        {
                InventoryPanel.SetActive(false);
                SelectedShoeIcon.gameObject.SetActive(true);
                
                string imagePath = _GameManager._sneakers.Find(x => x.name == sneaker.name).imagePath[..^5];
                SelectedShoeIcon.sprite = Resources.Load<Sprite>($"Sneakers/{imagePath}");
                SelectedSneaker = sneaker;
                //Put sprite in the main box.
        }

        //Creates a new listing with the chosen item.
        public async void CreateNewListing()
        {
                if (QuantityField.text == "")
                        return;

                if (PriceField.text == "")
                        return;

                if (SelectedSneaker.quantity < int.Parse(QuantityField.text)) //Maybe add some red text somewhere for this.
                {
                        return;
                }

                int cashValue = 0;
                int gemValue = 0;

                if (CashIcon.activeSelf)
                        cashValue = int.Parse(PriceField.text);
                else if (GemIcon.activeSelf)
                        gemValue = int.Parse(PriceField.text);

                Guid key = Guid.NewGuid();
                
                await _GameManager.firebase.AddMarketListing(new MarketListing("Item Description...", key.ToString(), cashValue, gemValue, int.Parse(QuantityField.text), _GameManager.firebase.userId, _GameManager._sneakers.FindIndex(x => x.name == SelectedSneaker.name)));
                DestroyAllListings();
                GetListings();

                //Remove the quantity from the player's possession.
                SneakersOwned newData = _GameManager.inventoryManager.sneakersOwned[_GameManager.inventoryManager.sneakersOwned.FindIndex(x => x.name == SelectedSneaker.name)];
                newData.quantity -= int.Parse(QuantityField.text);
                _GameManager.inventoryManager.sneakersOwned[_GameManager.inventoryManager.sneakersOwned.FindIndex(x => x.name == SelectedSneaker.name)] = newData;
                
                RefreshInventory();

                //Reset fields
                QuantityField.text = "";
                PriceField.text = "";

                //Reset selected item.
                ClearSelectedSneaker();
                        CreateListingPanel.SetActive(false);
                MyListingsPanel.SetActive(true);
        }


        public void ClearSelectedSneaker()
        {
                SelectedSneaker = new SneakersOwned();
                SelectedShoeIcon.gameObject.SetActive(false);
        }
        
}

