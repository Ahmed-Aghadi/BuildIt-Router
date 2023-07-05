using Nethereum.Web3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thirdweb.Contracts.DirectListingsLogic.ContractDefinition;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform buildingMenuRect;
    public Button editButton;
    public Button confirmButton;
    public Button cancelButton;
    public Prefab_ConnectWallet prefab_connectWallet;
    public UIController uiController;
    public GameObject buildingMenu;
    public GameObject faucetButton;
    public Button marketplaceButton;
    public Text marketplaceButtonText;
    bool isMarketplaceOpen = false;
    public InputManager inputManager;
    public PlacementManager placementManager;
    public GameObject lightBlueHighlightPrefab;
    public GameObject redHighlightPrefab;
    List<GameObject> highlightPrefabs;

    [Header("For Marketplace Panel")]
    public GameObject marketplacePanel;
    public GameObject marketplaceLoadingPanel;
    public TMP_Text errorText;
    public TMP_Dropdown typeDropdown;
    public TMP_InputField priceInput;
    public TMP_Text priceInputLabel;
    public Toggle usdToggle;
    public TMP_InputField durationInput;
    public TMP_InputField linkAmountInput;
    public GameObject buttonsContainer;
    public Button createListingButton;
    public Button cancelListingButton;

    [Header("For Listing Panel")]
    public GameObject listingsPanel;
    public GameObject listingsContainer;
    public GameObject listingPrefab;
    List<GameObject> listingPrefabs;

    [Header("For Listing Handle Panel")]
    public GameObject listingHandlePanel;
    public TMP_InputField bidInput;
    public TMP_Text priceText;
    public Button confirmListingButton;
    public TMP_Text confirmListingButtonText;

    [Header("For Transfer Panel (cross chain)")]
    public Button transferUtilsButton; // to activate transferPanel
    public GameObject transferPanel;
    public TMP_Dropdown chainDropdown;
    public TMP_Dropdown tokenDropdown;
    public TMP_InputField amountInput;
    public Button transferButton;
    public Button cancelTransferButton;


    // for marketplace panel
    bool isError = false;
    string errorTextString = "ERROR!!!";
    float timePassed = 0;
    float errorDuration = 5f;
    int xIndex, yIndex;

    // Start is called before the first frame update
    void Start()
    {
        highlightPrefabs = new List<GameObject>();
        marketplacePanel.SetActive(false);
        marketplaceLoadingPanel.SetActive(false);
        errorText.gameObject.SetActive(false);
        durationInput.gameObject.SetActive(false);
        linkAmountInput.gameObject.SetActive(false);
        typeDropdown.onValueChanged.AddListener(delegate
        {
            OnTypeDropdownValueChanged(typeDropdown);
        });
        createListingButton.onClick.AddListener(delegate
        {
            OnCreateListing();
        });
        cancelListingButton.onClick.AddListener(delegate
        {
            OnCancelListing();
        });

        listingPrefabs = new List<GameObject>();
        listingsPanel.SetActive(false);

        transferPanel.SetActive(false);
        transferUtilsButton.onClick.AddListener(delegate
        {
            ShowTransferPanel();
        });
        transferButton.onClick.AddListener(delegate
        {
            OnTransfer();
        });
        cancelTransferButton.onClick.AddListener(delegate
        {
            HideTransferPanel();
        });

        listingHandlePanel.SetActive(false);
        // SellLand();
    }

    async void OnTransfer()
    {
        string sourceChain = "", destinationChain = "";
        bool success = false;
        if (chainDropdown.options[chainDropdown.value].text == "Mantle Testnet")
        {
            sourceChain = "80001";
            destinationChain = "5001";
        }
        else if (chainDropdown.options[chainDropdown.value].text == "Polygon Mumbai")
        {
            sourceChain = "5001";
            destinationChain = "80001";
        }
        if (sourceChain != "" && destinationChain != "")
        {
            int tokenId = ContractManager.EMPTY;
            if(tokenDropdown.value == 0)
            {
                tokenId = ContractManager.ROAD;
            } else if (tokenDropdown.value == 1)
            {
                tokenId = ContractManager.HOUSE;
            } else if (tokenDropdown.value == 2)
            {
                tokenId = ContractManager.SPECIAL;
            }

            if(tokenId == ContractManager.EMPTY)
            {
                return;
            }

            success = await ContractManager.Instance.TransferUtilsCrossChain(sourceChain, destinationChain, tokenId, amountInput.text);
        }
        if (success)
        {
            HideTransferPanel();
            await ContractManager.Instance.setItemBalances();
        }
    }

    async void ShowTransferPanel()
    {
        var chainId = await ThirdwebManager.Instance.SDK.wallet.GetChainId();
        if (chainId != 80001 && chainId != 5001)
        {
            return;
        }
        transferPanel.SetActive(true);
        chainDropdown.ClearOptions();
        chainDropdown.AddOptions(new List<TMP_Dropdown.OptionData>
        {
            new TMP_Dropdown.OptionData(){text= (chainId == 80001 ? "Mantle Testnet" : "Polygon Mumbai")},
        });
    }

    void HideTransferPanel()
    {
        transferPanel.SetActive(false);
        chainDropdown.ClearOptions();
    }

    public void ShowTransferUtilsButton()
    {
        transferUtilsButton.gameObject.SetActive(true);
    }

    public void HideTransferUtilsButton()
    {
        transferUtilsButton.gameObject.SetActive(false);
    }

    void OnTypeDropdownValueChanged(TMP_Dropdown dropdown)
    {
        if (dropdown.value == 0)
        {
            durationInput.gameObject.SetActive(false);
            linkAmountInput.gameObject.SetActive(false);
            buttonsContainer.transform.localPosition = new Vector3(0, 0, 0);
            priceInputLabel.text = "Price";
        }
        else if (dropdown.value == 1)
        {
            durationInput.gameObject.SetActive(true);
            linkAmountInput.gameObject.SetActive(true);
            buttonsContainer.transform.localPosition = new Vector3(0, -80, 0);
            priceInputLabel.text = "Minimum Bid";
        }
    }

    // Update is called once per frame
    void Update()
    {

        // For adjust the scaling of Prefab Connect Wallet
        /*var canvasHeight = canvas.pixelRect.height;
        var canvasWidth = canvas.pixelRect.width;
        var canvasScale = canvas.scaleFactor;*/
        var canvasRect = canvas.GetComponent<RectTransform>().rect;
        /*var canvasRectWidth = canvasRect.width;*/
        var canvasRectHeight = canvasRect.height;
        var scaleCalc = canvasRectHeight / 540;
        prefab_connectWallet.transform.localScale = new Vector3(scaleCalc, scaleCalc, scaleCalc);

        // For handling timing for Error Text of Marketplace panel
        if (isError)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= errorDuration)
            {
                isError = false;
                timePassed = 0;
                // errorTextString = "ERROR!!!";
                errorText.gameObject.SetActive(false);
            }
        }

        // For highlight
        if (isMarketplaceOpen && !marketplacePanel.activeSelf && !listingsPanel.activeSelf)
        {
            var position = inputManager.RaycastGround();
            if (position != null)
            {
                if (placementManager.CheckIfPositionInBound(position.Value) == true)
                {
                    if (ContractManager.Instance.userOwnsIndex(position.Value.x, position.Value.z))
                    {
                        highlightLand(position.Value.x, position.Value.z, lightBlueHighlightPrefab);
                    }
                    else
                    {
                        highlightLand(position.Value.x, position.Value.z, redHighlightPrefab);
                    }
                }
            }
            else
            {
                if (highlightPrefabs.Count != 0)
                {
                    ResetHighlightPrefabsList();
                }
            }

        }

        if (isMarketplaceOpen && Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false && !marketplacePanel.activeSelf && !listingsPanel.activeSelf)
        {
            var position = inputManager.RaycastGround();
            if (position != null)
            {
                if (placementManager.CheckIfPositionInBound(position.Value) == true)
                {
                    Debug.Log("Before Pos" + position.Value.x + ", " + position.Value.z);
                    int perSize = ContractManager.Instance.perSize;
                    if (perSize == 0)
                    {
                        return;
                    }
                    int xIndex = position.Value.x / perSize;
                    int yIndex = position.Value.z / perSize;
                    Debug.Log("After Pos" + xIndex + ", " + yIndex);

                    if (ContractManager.Instance.landListedIndex(xIndex, yIndex))
                    {
                        Debug.Log("Buy Land");
                        ShowBuyLandPanel();
                    }
                    else if (ContractManager.Instance.userOwnsIndex(position.Value.x, position.Value.z))
                    {
                        Debug.Log("Sell Land");
                        SellLand();
                    }
                }
            }
        }
    }

    void ShowError(string text)
    {
        errorTextString = text;
        isError = true;
        timePassed = 0;
        errorText.gameObject.SetActive(true);
        errorText.text = errorTextString;
    }
    void highlightLand(int x, int y, GameObject prefab)
    {
        if (highlightPrefabs.Count != 0)
        {
            ResetHighlightPrefabsList();
        }
        int perSize = ContractManager.Instance.perSize;
        if (perSize == 0)
        {
            return;
        }
        int x1 = (int)(x / perSize) * perSize;
        int y1 = (int)(y / perSize) * perSize;
        int x2 = x1 + perSize - 1;
        int y2 = y1 + perSize - 1;
        int tmpY1 = y1;
        while (x1 <= x2)
        {
            while (y1 <= y2)
            {

                var current = GameObject.Instantiate(prefab);
                current.transform.localPosition = new Vector3(x1, 0.002f, y1);
                highlightPrefabs.Add(current);
                y1++;
            }
            y1 = tmpY1;
            x1++;
        }
    }

    void SellLand()
    {
        marketplacePanel.SetActive(true);
        ResetMarketplacePanel();
    }

    void ShowBuyLandPanel()
    {
        List<Listing> listingsList = ContractManager.Instance.GetListingsIndex(xIndex, yIndex);
        if (listingsList == null) return;
        Listing[] listings = listingsList.ToArray();
        listingsPanel.SetActive(true);
        int i = 1;
        float height = 0;
        for (int j = 1; j <= listings.Length; j++)
        {
            Debug.Log("Listing: " + i);
            Listing listing = listings[j - 1];
            if (listing.isValid)
            {
                Debug.Log("Listing Valid");
                Debug.Log("Instantiating: " + i);
                var listingPanel = GameObject.Instantiate(listingPrefab, listingsContainer.transform);
                listingPrefabs.Add(listingPanel);
                Debug.Log("Instantiated!!!");
                var rectListingPanel = listingPanel.GetComponent<RectTransform>().rect;
                if (rectListingPanel == null) return;
                float currentHeight = (25 * i) + ((i - 1) * rectListingPanel.height);
                rectListingPanel.position = new Vector2(rectListingPanel.x, currentHeight);
                height = currentHeight + rectListingPanel.height;

                Debug.Log("1");
                var headerPanelTransform = listingPanel.transform.Find("HeaderPanel");
                if (headerPanelTransform == null) return;
                Debug.Log("2");
                var priceLabelTransform = headerPanelTransform.Find("PriceLabel");
                if (priceLabelTransform == null) return;
                Debug.Log("3");
                TMP_Text priceLabel = priceLabelTransform.gameObject.GetComponent<TMP_Text>();
                if (priceLabel == null) return;

                Debug.Log("4");
                var valuePanelTransform = listingPanel.transform.Find("ValuePanel");
                if (valuePanelTransform == null) return;
                Debug.Log("5");
                var priceValueTransform = valuePanelTransform.Find("PriceValue");
                if (priceValueTransform == null) return;
                Debug.Log("6");
                TMP_Text priceValue = priceValueTransform.gameObject.GetComponent<TMP_Text>();
                if (priceValue == null) return;
                Debug.Log("7");
                var typeValueTransform = valuePanelTransform.Find("TypeValue");
                if (typeValueTransform == null) return;
                Debug.Log("8");
                TMP_Text typeValue = typeValueTransform.gameObject.GetComponent<TMP_Text>();
                if (typeValue == null) return;
                Debug.Log("9");
                var buttonTransform = valuePanelTransform.Find("Button");
                if (buttonTransform == null) return;
                Debug.Log("10");
                Button button = buttonTransform.gameObject.GetComponent<Button>();
                if (button == null) return;
                Debug.Log("11");
                var buttonLabelTransform = buttonTransform.Find("ButtonLabel");
                if (buttonLabelTransform == null) return;
                Debug.Log("12");
                TMP_Text buttonLabel = buttonLabelTransform.gameObject.GetComponent<TMP_Text>();
                if (buttonLabel == null) return;

                Debug.Log("13");
                Debug.Log("Price: " + listing.price);
                var price = Web3.Convert.FromWei(System.Numerics.BigInteger.Parse(listing.price));
                Debug.Log("Price Converted: " + price);
                priceValue.text = listing.inUSD ? price.ToString() + " USD" : price.ToString();

                if (listing.isAuction)
                {
                    Debug.Log("Auction");
                    priceLabel.text = "Minimum Bid";
                    typeValue.text = "Auction";
                    buttonLabel.text = "Bid";
                    button.onClick.AddListener(delegate
                    {
                        BidLand(listing.id);
                    });
                }
                else
                {
                    Debug.Log("Direct");
                    priceLabel.text = "Price";
                    typeValue.text = "Direct";
                    buttonLabel.text = "Buy";
                    button.onClick.AddListener(delegate
                    {
                        BuyLand(listing.id);
                    });
                }
            }
            i++;
        }
        if (height != 0)
        {
            var rectListingsContainer = listingsContainer.GetComponent<RectTransform>().rect;
            rectListingsContainer.height = height;
        }

    }

    async void BuyLand(int listingId)
    {
        listingHandlePanel.SetActive(true);
        bidInput.gameObject.SetActive(false);
        string price = await ContractManager.Instance.GetPrice(listingId);
        priceText.text = "Price: " + Web3.Convert.FromWei(System.Numerics.BigInteger.Parse(price));
        confirmListingButton.onClick.AddListener(delegate
        {
            ConfirmBuy(listingId, price);
        });
    }

    async void ConfirmBuy(int listingId, string price)
    {
        bool success = await ContractManager.Instance.BuyListing(listingId, price);
        OnCancelListingHandle();
        ResetListingPanel();
        if (success)
        {
            ToggleMarketplace();
        }
        ContractManager.Instance.OnSwitchNetwork();
    }
    async void BidLand(int listingId)
    {
        listingHandlePanel.SetActive(true);
        bidInput.gameObject.SetActive(true);
        string highestBid = await ContractManager.Instance.GetHighestBid(listingId);
        priceText.text = "Highest Bid: " + Web3.Convert.FromWei(System.Numerics.BigInteger.Parse(highestBid));
        confirmListingButton.onClick.AddListener(delegate
        {
            ConfirmBid(listingId, highestBid);
        });
    }
    async void ConfirmBid(int listingId, string highestBid)
    {
        bool success = await ContractManager.Instance.BidListing(listingId, highestBid, bidInput.text);
        OnCancelListingHandle();
        ResetListingPanel();
        if (success)
        {
            ToggleMarketplace();
        }
        ContractManager.Instance.OnSwitchNetwork();
    }

    public void OnCancelListingHandle()
    {
        listingHandlePanel.SetActive(false);
        bidInput.text = "";
        bidInput.gameObject.SetActive(false);
        confirmListingButton.onClick.RemoveAllListeners();
    }

    async void OnCreateListing()
    {
        if (priceInput.text == "" || Convert.ToDouble(priceInput.text) <= 0)
        {
            ShowError("Invalid " + priceInputLabel.text);
            return;
        }
        if (typeDropdown.value == 1)
        {
            if (durationInput.text == "" || Convert.ToDouble(durationInput.text) <= 0)
            {
                ShowError("Invalid Duration");
                return;
            }

            if (linkAmountInput.text == "" || Convert.ToDouble(linkAmountInput.text) <= 0)
            {
                ShowError("Invalid Amount of Link");
                return;
            }
        }
        marketplaceLoadingPanel.SetActive(true);
        Debug.Log("TRYING...");
        Debug.Log("Details1: " + usdToggle.isOn);
        Debug.Log("Details2: " + priceInput.text);
        Debug.Log("Details3: " + (typeDropdown.value == 1).ToString());
        Debug.Log("Details4: " + durationInput.text);
        Debug.Log("Details5: " + linkAmountInput.text);
        // await ContractManager.Instance.CreateListing(usdToggle.isOn, xIndex, yIndex, priceInput.text, typeDropdown.value == 1, durationInput.text, linkAmountInput.text);
        if (typeDropdown.value == 1)
        {
            await ContractManager.Instance.CreateListing(usdToggle.isOn, xIndex, yIndex, priceInput.text, typeDropdown.value == 1, durationInput.text, linkAmountInput.text);
        }
        else
        {
            await ContractManager.Instance.CreateListing(usdToggle.isOn, xIndex, yIndex, priceInput.text, typeDropdown.value == 1, "0", "0");
        }
        // await ContractManager.Instance.CreateListing(usdToggle.isOn, 0, 0, priceInput.text, typeDropdown.value == 1, durationInput.text, linkAmountInput.text);
        OnCancelListing();
        ToggleMarketplace();
    }

    void OnCancelListing()
    {
        marketplacePanel.SetActive(false);
        marketplaceLoadingPanel.SetActive(false);
        ResetMarketplacePanel();
    }

    void ResetMarketplacePanel()
    {
        isError = false;
        typeDropdown.value = 0;
        priceInput.text = "";
        usdToggle.isOn = false;
        durationInput.text = "";
        linkAmountInput.text = "";
    }

    public void ResetListingPanel()
    {
        listingsPanel.SetActive(false);
        ResetListingPrefabsList();
    }

    void ResetHighlightPrefabsList()
    {
        foreach (var item in highlightPrefabs)
        {
            Destroy(item);
        }
        highlightPrefabs.Clear();
        highlightPrefabs = new List<GameObject>();
    }

    void ResetListingPrefabsList()
    {
        foreach (var item in listingPrefabs)
        {
            Destroy(item);
        }
        listingPrefabs.Clear();
        listingPrefabs = new List<GameObject>();
    }

    public void AttachClickListeners()
    {
        editButton.onClick.AddListener(OnEditClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
        confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    public void RemoveClickListeners()
    {
        uiController.RemoveEventListeners();
        editButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        confirmButton.onClick.RemoveAllListeners();
    }

    void OnEditClicked()
    {
        uiController.AddEventListeners();
        buildingMenuRect.sizeDelta = new Vector2(180f, 240f);
        editButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
    }

    void OnCancelClicked()
    {
        uiController.RemoveEventListeners();
        buildingMenuRect.sizeDelta = new Vector2(180f, 210f);
        editButton.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        ContractManager.Instance.CancelClicked();
    }

    async void OnConfirmClicked()
    {
        Debug.Log("updating...");
        await ContractManager.Instance.confirmMapUpdates();
        Debug.Log("updated!!!");
        OnCancelClicked();
    }

    public async void ToggleMarketplace()
    {
        if (!isMarketplaceOpen)
        {
            bool isSuccess = await ContractManager.Instance.SetMarketplaceData();
            if (!isSuccess)
            {
                return;
            }
        }
        isMarketplaceOpen = !isMarketplaceOpen;
        buildingMenu.SetActive(!isMarketplaceOpen);
        faucetButton.SetActive(!isMarketplaceOpen);
        marketplaceButtonText.text = !isMarketplaceOpen ? "Marketplace" : "Go Back";
        if (!isMarketplaceOpen)
        {
            if (highlightPrefabs.Count != 0)
            {
                ResetHighlightPrefabsList();
            }
            ContractManager.Instance.ResetListingHighlights();
        }
    }
}
