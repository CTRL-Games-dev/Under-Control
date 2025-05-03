using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    #region Fields

    [Header("Assign if not player inventory")]
    [SerializeField] private bool _isPlayerInventory = false;
    public bool IsSellerInventory = false;

    public EntityInventory TargetEntityInventory; 
    public Sprite CustomInvTileSprite;

    [Header("Prefabs")]
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private GameObject _invTilePrefab;

    [Header("Inventory Stuff")]
    [SerializeField] private GameObject _gridHolder;
    [SerializeField] private GameObject _itemHolder;

    [Header("Interactions")]
    [SerializeField] private Image _redPanel;
    [SerializeField] private Image _bgImage;

    // References set in Awake or Start
    private RectTransform _rectTransform;
    private ItemContainer _currentEntityInventory;
    private GridLayoutGroup _gridLayoutGroup;
    private Image _layoutImage;

    // Inventory variables
    public static float TileSize = 94;

    [SerializeField]
    private InvTile[,] _inventoryTileArray;
    private int _inventoryWidth, _inventoryHeight;

    public List<InventoryItem> _inventory => _currentEntityInventory.GetItems();
    private InventoryItem _selectedInventoryItem => Player.UICanvas.SelectedItemUI.InventoryItem;
    [HideInInspector] public static bool IsItemJustBought = false;

    // Grid variables
    public InvTile SelectedTile { get; set; }

    #endregion

    #region Unity Methods

    public void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _layoutImage = GetComponent<Image>();
        _layoutImage.enabled = false;
        _gridLayoutGroup = _gridHolder.GetComponent<GridLayoutGroup>();
    }

    public void Start() {
        _currentEntityInventory = getCurrentEntityInventory();
        if (_currentEntityInventory == null) {
            // Debug.LogError("No inventory found!");
            disconnectSignals();
            return;
        }
        connectSignals();
        RegenerateInventory();
        EventBus.ItemPlacedEvent.AddListener(playInventorySound);
        EventBus.ItemUILeftClickEvent.AddListener(playInventorySound);
        EventBus.ItemUIRightClickEvent.AddListener(playInventorySound);
    }

    #endregion

    #region Grid Methods

    public void RegenerateInventory() {
        setupGrid();
        UpdateItemUIS();
    }

    private ItemContainer getCurrentEntityInventory() {
        if (_isPlayerInventory) return Player.Inventory.ItemContainer;
        if (TargetEntityInventory is SimpleInventory s) return s.ItemContainer;
        return null; 
    }

    private void setupGrid() {
        
        _inventoryWidth = _currentEntityInventory.Size.x;
        _inventoryHeight = _currentEntityInventory.Size.y;
        _gridLayoutGroup.constraintCount = _inventoryWidth;

        _gridLayoutGroup.cellSize = new Vector2(TileSize, TileSize);
        _inventoryTileArray = new InvTile[_inventoryHeight, _inventoryWidth];

        float xOffset = (_rectTransform.rect.width - _inventoryWidth * TileSize) / 2;
        _gridHolder.GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset, 0);
        _itemHolder.GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset, 0);

        generateGrid();
    }

    private void generateGrid() {
        for (int y = 0; y < _inventoryHeight; y++) {
            for (int x = 0; x < _inventoryWidth; x++) {
                var tile = Instantiate(_invTilePrefab, _gridHolder.transform);
                tile.name = $"Tile {x} {y}";

                InvTile invTile = tile.GetComponent<InvTile>();

                _inventoryTileArray[y, x] = invTile;
                
                invTile.InventoryPanel = this;
                invTile.Pos = new Vector2Int(x, y);
                
                if (CustomInvTileSprite != null) invTile.SetCustomImage(CustomInvTileSprite);
            }
        }
    }
    private void clearGrid(){
        foreach (RectTransform child in _gridHolder.GetComponentInChildren<RectTransform>()) {
            Destroy(child.gameObject);
        }
    }

    private List<InvTile> occupyTiles(InventoryItem inventoryItem) {
        List<InvTile> tiles = new();

        Vector2Int pos = inventoryItem.Position;
        Vector2Int size;

        if (inventoryItem.Rotated) {
            size = new Vector2Int(inventoryItem.Size.y, inventoryItem.Size.x);
        } else {
            size = inventoryItem.Size;
        }

        for (int y = pos.y; y < pos.y + size.y; y++) {
            for (int x = pos.x; x < pos.x + size.x; x++) {
                _inventoryTileArray[y, x].IsEmpty = false;
                tiles.Add(_inventoryTileArray[y, x]);
            }
        }
        return tiles;
    }

    private void clearHighlights() {
        foreach (InvTile tile in _inventoryTileArray) {
            tile.SetHighlight(false);
        }
        SelectedTile?.SetHighlight(false);
    }

    private void highlightNeighbours(Vector2Int pos, InventoryItem inventoryItem) {
        Vector2Int size = inventoryItem.Rotated ? new Vector2Int(inventoryItem.Size.y, inventoryItem.Size.x) : inventoryItem.Size;
        pos += inventoryItem.Rotated ?
          new Vector2Int(Player.UICanvas.SelectedItemUI.SelectedOffsetInv.y, -Player.UICanvas.SelectedItemUI.SelectedOffsetInv.x) :
          Player.UICanvas.SelectedItemUI.SelectedOffsetInv;
            
        int startingY = pos.y;
        int endingY = startingY + size.y;

        // if (inventoryItem.Size.x > 1 && inventoryItem.Rotated) {
        //     startingY = pos.y - inventoryItem.Size.x + 1;
        //     endingY = startingY + inventoryItem.Size.x;
        // }

        for(int y = startingY; y < endingY; y++) {
            for(int x = pos.x; x < pos.x + size.x; x++) {
                if (x < 0 || x >= _inventoryWidth || y < 0 || y >= _inventoryHeight) {
                    continue;
                }
                _inventoryTileArray[y, x].SetHighlight(true);
            }
        }
    }

    public void OnInvTileEnter(InvTile invTile) {
        SelectedTile = invTile;
        if (Player.UICanvas.SelectedItemUI.InventoryItem != null && invTile != null) {
            clearHighlights();
            highlightNeighbours(invTile.Pos, _selectedInventoryItem);
        } else {
            clearHighlights();
        }
    }

    public void OnInvTileExit(InvTile invTile) {
        if (SelectedTile == invTile) {
            SelectedTile = null;
        }
        clearHighlights();
    }

    #endregion

    #region ItemUI Methods

    public void UpdateItemUIS() {
        // _currentEntityInventory = _isPlayerInventory ? Player.UICanvas.PlayerInventory.ItemContainer : TargetEntityInventory;

        InventoryItem[] itemsArray = _inventory.ToArray();

        wipeAllItemUIS();
        foreach (InvTile tile in _inventoryTileArray) {
            tile.IsEmpty = true;
        }
        foreach (InventoryItem i in itemsArray) {
            destroyItemUI(i);
            createItemUI(i);
        }
    }

    private void wipeAllItemUIS() {
        foreach (Transform child in _itemHolder.transform) {
            Destroy(child.gameObject);
        }
    }

    private GameObject createItemUI(InventoryItem inventoryItem){
        GameObject itemGameObject = Instantiate(_itemPrefab, _itemHolder.transform);
        itemGameObject.name = inventoryItem.ItemData.DisplayName;

        inventoryItem.RectTransform = itemGameObject.GetComponent<RectTransform>();

        inventoryItem.ItemUI = itemGameObject.GetComponent<ItemUI>();
        inventoryItem.ItemUI.SetupItem(inventoryItem, TileSize, occupyTiles(inventoryItem), this);

        return itemGameObject;
    }

    private void destroyItemUI(InventoryItem inventoryItem) {
        if (inventoryItem.ItemUI == null) {
            return;
        }
        foreach (InvTile tile in inventoryItem.ItemUI.OccupiedTiles) {
            tile.IsEmpty = true;
        }
        foreach (Transform child in _itemHolder.transform) {
            if (child.GetComponent<ItemUI>().InventoryItem == inventoryItem) {
                Destroy(child.gameObject);
            }
        }

        // return _currentEntityInventory.RemoveInventoryItem(inventoryItem);
        // _currentEntityInventory.RemoveItemAt(inventoryItem.Position);
    }

    private void destroyAndRemoveItemUI(InventoryItem inventoryItem) {
        destroyItemUI(inventoryItem);
        _currentEntityInventory.RemoveInventoryItem(inventoryItem);
    }

    public void TryMoveSelectedItem() {
        clearHighlights();

        if (SelectedTile == null) {
            return;
        }

        if (_selectedInventoryItem == null) {
            return;
        }

        Vector2Int selectedTilePos = SelectedTile.Pos;
        if(_selectedInventoryItem != null) {
            selectedTilePos += _selectedInventoryItem.Rotated ?
                new Vector2Int(Player.UICanvas.SelectedItemUI.SelectedOffsetInv.y, -Player.UICanvas.SelectedItemUI.SelectedOffsetInv.x) :
                Player.UICanvas.SelectedItemUI.SelectedOffsetInv;
        }

        Vector2Int size = _selectedInventoryItem.Rotated ? new Vector2Int(_selectedInventoryItem.Size.y, _selectedInventoryItem.Size.x) : _selectedInventoryItem.Size;

        if (!_currentEntityInventory.FitsWithinBounds(selectedTilePos, size)) {
            StartCoroutine(redPanelShow());
            return;
        }

        if (!canBePlaced(selectedTilePos, size)) {
            StartCoroutine(redPanelShow());
            return;
        }

        _currentEntityInventory.AddItem(_selectedInventoryItem.ItemData, _selectedInventoryItem.Amount, selectedTilePos, _selectedInventoryItem.PowerScale, _selectedInventoryItem.Rotated);
        GameObject item = createItemUI(_currentEntityInventory.GetInventoryItem(selectedTilePos));
        
        if (item.GetComponent<ItemUI>().CurrentInventoryPanel.IsSellerInventory) {
            Player.Instance.Coins += (IsItemJustBought ? _selectedInventoryItem.ScaledValue : _selectedInventoryItem.ScaledValue / 2) * _selectedInventoryItem.Amount;
        }
        IsItemJustBought = false;

        Player.UICanvas.SelectedItemUI.InventoryItem = null;
        EventBus.ItemPlacedEvent?.Invoke();

        SelectedTile.SetHighlight(false);
    }

    private bool canBePlaced(Vector2Int pos, Vector2Int size) {
        for (int y = pos.y; y < pos.y + size.y; y++) {
            for (int x = pos.x; x < pos.x + size.x; x++) {
                if (x < 0 || x >= _inventoryWidth || y < 0 || y >= _inventoryHeight) {
                    return false;
                }
                if (!_inventoryTileArray[y, x].IsEmpty) {
                    return false;
                }
            }
        }
        return true;
    }

    #endregion

    #region Misc Methods
    private void connectSignals() {
        EventBus.InventoryItemChangedEvent.AddListener(UpdateItemUIS);
        EventBus.ItemUILeftClickEvent.AddListener(OnItemUILeftClick);
        EventBus.ItemUIRightClickEvent.AddListener(OnItemUIRightClick);
    }

    private void disconnectSignals() {
        EventBus.InventoryItemChangedEvent.RemoveListener(UpdateItemUIS);
        EventBus.ItemUILeftClickEvent.RemoveListener(OnItemUILeftClick);
        EventBus.ItemUIRightClickEvent.RemoveListener(OnItemUIRightClick);
    }

    public void SetTargetInventory(EntityInventory entityInventory) {
        TargetEntityInventory = entityInventory;
        foreach (Transform child in _itemHolder.transform) {
            Destroy(child.gameObject);
        }
        foreach (Transform child in _gridHolder.transform) {
            Destroy(child.gameObject);
        }
        Start();
    }

    public void SetImagesRaycastTarget(bool val) {
        foreach (Transform child in _itemHolder.transform) {
            child.GetComponent<ItemUI>().Image.raycastTarget = val;
        }
    }

    private IEnumerator redPanelShow() {
        _redPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f * Settings.AnimationSpeed);
        _redPanel.gameObject.SetActive(false);
    }
    
    private void playInventorySound(ItemUI arg){
        playInventorySound();
    }
    private void playInventorySound(){
        AudioClip InvClickClip = Resources.Load("SFX/click") as AudioClip;
        SoundFXManager.Instance.PlaySoundFXClip(InvClickClip,transform);
    }
    public void ChangeCurrentInventory(ItemContainer newInventory){
        _currentEntityInventory = newInventory;
        clearGrid();
        RegenerateInventory();
    }

    #endregion

    #region Callbacks

    public void OnItemUIClick(ItemUI itemUI) {
        // if (Player.UICanvas.SelectedItemUI.InventoryItem != null) return; 
        if (_inventory.Contains(itemUI.InventoryItem)) {
            if (Player.UICanvas.SelectedItemUI.InventoryItem != null) return; 
            if (Player.Instance.Coins - itemUI.InventoryItem.ScaledValue * itemUI.InventoryItem.Amount < 0 && IsSellerInventory) return;
            Player.UICanvas.SetSelectedItemUI(itemUI);
            if (IsSellerInventory) {
                Player.Instance.Coins -= itemUI.InventoryItem.ScaledValue * itemUI.InventoryItem.Amount;
                IsItemJustBought = true;
            }
            destroyAndRemoveItemUI(itemUI.InventoryItem);
        }
        // SetImagesRaycastTarget(false);
    }

    public void OnItemUILeftClick(ItemUI itemUI) {
        if (itemUI == null) return;

        if (_inventory.Contains(itemUI.InventoryItem)) {
            if (Player.UICanvas.SelectedItemUI.InventoryItem == null) {
                if (Player.Instance.Coins - itemUI.InventoryItem.ScaledValue * itemUI.InventoryItem.Amount < 0 && IsSellerInventory) return;
                Player.UICanvas.SetSelectedItemUI(itemUI);
                if (IsSellerInventory) {
                    if (Player.Instance.Coins - itemUI.InventoryItem.ScaledValue * itemUI.InventoryItem.Amount < 0) return;
                    Player.Instance.Coins -= itemUI.InventoryItem.ScaledValue * itemUI.InventoryItem.Amount;
                    IsItemJustBought = true;
                }
                destroyAndRemoveItemUI(itemUI.InventoryItem);
            } else {
                if (Player.UICanvas.SelectedItemUI.InventoryItem.ItemData == itemUI.InventoryItem.ItemData) {
                    if (itemUI.InventoryItem.ItemData.MaxQuantity - itemUI.InventoryItem.Amount >= Player.UICanvas.SelectedItemUI.InventoryItem.Amount) {
                        if (Player.Instance.Coins - itemUI.InventoryItem.ScaledValue * itemUI.InventoryItem.Amount < 0 && IsSellerInventory) return;
                        itemUI.InventoryItem.Amount += Player.UICanvas.SelectedItemUI.InventoryItem.Amount;
                        if (IsSellerInventory) {
                            Player.Instance.Coins -= itemUI.InventoryItem.ScaledValue * Player.UICanvas.SelectedItemUI.InventoryItem.Amount;
                            IsItemJustBought = true;
                        }
                        itemUI.UpdateAmount();
                        Player.UICanvas.SelectedItemUI.InventoryItem = null;
                    } else {
                        if (Player.Instance.Coins - itemUI.InventoryItem.ScaledValue * itemUI.InventoryItem.Amount < 0 && IsSellerInventory) return;
                        Player.UICanvas.SelectedItemUI.InventoryItem.Amount = Player.UICanvas.SelectedItemUI.InventoryItem.Amount - (itemUI.InventoryItem.ItemData.MaxQuantity - itemUI.InventoryItem.Amount);
                        itemUI.InventoryItem.Amount = itemUI.InventoryItem.ItemData.MaxQuantity;
                        itemUI.UpdateAmount();
                        Player.UICanvas.SelectedItemUI.UpdateAmount();
                        if (IsSellerInventory) {
                            Player.Instance.Coins -= itemUI.InventoryItem.ScaledValue * (itemUI.InventoryItem.ItemData.MaxQuantity - itemUI.InventoryItem.Amount);
                            IsItemJustBought = true;
                        }
                    }
                }
            }
        }
    }

    public void OnItemUIRightClick(ItemUI itemUI) {
        if (itemUI == null) return;
        if (_inventory.Contains(itemUI.InventoryItem)) {
            if (Player.UICanvas.SelectedItemUI.InventoryItem == null) {
                if (itemUI.InventoryItem.Amount > 1) {
                    int half = itemUI.InventoryItem.Amount / 2;
                    if (Player.Instance.Coins - itemUI.InventoryItem.ScaledValue * (half + itemUI.InventoryItem.Amount % 2) < 0 && IsSellerInventory) return;
                    
                    InventoryItem inventoryItem = new(){
                        ItemData = itemUI.InventoryItem.ItemData,
                        Amount = half + itemUI.InventoryItem.Amount % 2,
                        Position = itemUI.InventoryItem.Position,
                        Rotated = itemUI.InventoryItem.Rotated
                    };
                    Player.UICanvas.SetSelectedInventoryItem(inventoryItem);
                    Player.UICanvas.SelectedItemUI.UpdateAmount();

                    itemUI.InventoryItem.Amount = half;
                    itemUI.UpdateAmount();
                    if (IsSellerInventory) {
                        Player.Instance.Coins -= itemUI.InventoryItem.ScaledValue * (half + itemUI.InventoryItem.Amount % 2);
                        IsItemJustBought = true;
                    }
                } else {
                    if (Player.Instance.Coins - itemUI.InventoryItem.ScaledValue * itemUI.InventoryItem.Amount < 0 && IsSellerInventory) return;
                    Player.UICanvas.SetSelectedItemUI(itemUI);
                    if (IsSellerInventory) {
                        Player.Instance.Coins -= itemUI.InventoryItem.ScaledValue * itemUI.InventoryItem.Amount;
                        IsItemJustBought = true;
                    }
                    destroyAndRemoveItemUI(itemUI.InventoryItem);
                }
            } else {
                if (Player.UICanvas.SelectedItemUI.InventoryItem.ItemData == itemUI.InventoryItem.ItemData) {
                    if (itemUI.InventoryItem.Amount + 1 <= itemUI.InventoryItem.ItemData.MaxQuantity && Player.UICanvas.SelectedItemUI.InventoryItem.Amount - 1 >= 0) {
                        itemUI.InventoryItem.Amount = itemUI.InventoryItem.Amount + 1;
                        itemUI.UpdateAmount();
                        Player.UICanvas.SelectedItemUI.InventoryItem.Amount = Player.UICanvas.SelectedItemUI.InventoryItem.Amount - 1;
                        Player.UICanvas.SelectedItemUI.UpdateAmount();
                        if (IsSellerInventory) {
                            Player.Instance.Coins += itemUI.InventoryItem.ScaledValue;
                            IsItemJustBought = true;
                        }
                    }
                }
            }
        }
    }


    public void OnPointerEnter() {
        Player.UICanvas.ActiveInventoryPanel = this;
    }

    public void OnPointerExit() {
        Player.UICanvas.ActiveInventoryPanel = null;
    }

    #endregion
}
