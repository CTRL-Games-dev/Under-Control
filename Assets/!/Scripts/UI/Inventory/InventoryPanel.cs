using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    #region Fields

    [Header("Assign if not player inventory")]
    [SerializeField] private bool _isPlayerInventory = false;
    public bool IsSellerInventory = false;
    public ItemContainer TargetEntityInventory; 

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
    private UICanvas _uiCanvas;
    private ItemContainer _currentEntityInventory;
    private GridLayoutGroup _gridLayoutGroup;
    private Image _layoutImage;

    // Inventory variables
    private static float _tileSize = 0;
    public static float TileSize { get {return _tileSize;} set {_tileSize = value;} }

    [SerializeField]
    private InvTile[,] _inventoryTileArray;
    private int _inventoryWidth, _inventoryHeight;

    public List<InventoryItem> _inventory => _currentEntityInventory.GetItems();
    private InventoryItem _selectedInventoryItem => _uiCanvas.SelectedItemUI.InventoryItem;

    // Grid variables
    public InvTile SelectedTile { get; set; }

    #endregion

    #region Unity Methods

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _layoutImage = GetComponent<Image>();
        _layoutImage.enabled = false;
        _gridLayoutGroup = _gridHolder.GetComponent<GridLayoutGroup>();
    }

    private void Start() {
        EventBus.InventoryItemChangedEvent.AddListener(UpdateItemUIS);
        EventBus.ItemUIClickEvent.AddListener(OnItemUIClick);
        EventBus.TileSizeSetEvent.AddListener(RegenerateInventory);
        // EventBus.ItemPlacedEvent.AddListener(() => SetImagesRaycastTarget(true));
        _uiCanvas = UICanvas.Instance;

        _currentEntityInventory = _isPlayerInventory ? _uiCanvas.PlayerInventory.ItemContainer : TargetEntityInventory; // If OtherEntityInventory is null, use PlayerInventory

        if (_isPlayerInventory) {
            TileSize = Mathf.Clamp(_rectTransform.rect.width / _currentEntityInventory.Size.x, 0, _rectTransform.rect.height / _currentEntityInventory.Size.y);
            EventBus.TileSizeSetEvent?.Invoke();
        } 
    }

    #endregion

    #region Grid Methods

    public void RegenerateInventory() {
        if ((_currentEntityInventory = _isPlayerInventory ? _uiCanvas.PlayerInventory.ItemContainer : TargetEntityInventory) == null) return; 
        setupGrid();
        UpdateItemUIS();
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
            }
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
        int startingY = pos.y;
        int endingY = startingY + size.y;

        if (inventoryItem.Size.x > 1 && inventoryItem.Rotated) {
            startingY = pos.y - inventoryItem.Size.x + 1;
            endingY = startingY + inventoryItem.Size.x;
        }

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
        if (_uiCanvas.SelectedItemUI.InventoryItem != null && invTile != null) {
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
        Debug.Log("Updating Item UIs");
        foreach (InventoryItem inventoryItem in _inventory) {
            destroyItemUI(inventoryItem);
            createItemUI(inventoryItem);
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

        _currentEntityInventory.RemoveInventoryItem(inventoryItem);
        // _currentEntityInventory.RemoveItemAt(inventoryItem.Position);
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
        if (_selectedInventoryItem.Size.x > 1 && _selectedInventoryItem.Rotated) {
            selectedTilePos = new Vector2Int(SelectedTile.Pos.x, SelectedTile.Pos.y - _selectedInventoryItem.Size.x + 1);
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

        _currentEntityInventory.AddItem(_selectedInventoryItem.ItemData, _selectedInventoryItem.Amount, selectedTilePos, _selectedInventoryItem.Rotated);
        GameObject item = createItemUI(_currentEntityInventory.GetInventoryItem(selectedTilePos));
        
        if (item.GetComponent<ItemUI>().CurrentInventoryPanel.IsSellerInventory) {
            _uiCanvas.PlayerController.Coins += (_selectedInventoryItem.ItemData.Value / 2) * _selectedInventoryItem.Amount;
        }

        _uiCanvas.SelectedItemUI.InventoryItem = null;
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

    public void SetTargetInventory(ItemContainer itemContainer) {
        TargetEntityInventory = itemContainer;
        RegenerateInventory();
    }

    public void SetImagesRaycastTarget(bool val) {
        foreach (Transform child in _itemHolder.transform) {
            child.GetComponent<ItemUI>().Image.raycastTarget = val;
        }
    }

    private IEnumerator redPanelShow() {
        _redPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        _redPanel.gameObject.SetActive(false);
    }

    #endregion

    #region Callbacks

    public void OnItemUIClick(ItemUI itemUI) {
        if (_inventory.Contains(itemUI.InventoryItem)) {
            if (IsSellerInventory) {
                _uiCanvas.PlayerController.Coins -= itemUI.InventoryItem.ItemData.Value * itemUI.InventoryItem.Amount;
            }  
            destroyItemUI(itemUI.InventoryItem);
        } 
        // SetImagesRaycastTarget(false);
    }

    public void OnPointerEnter() {
        _uiCanvas.ActiveInventoryPanel = this;
    }

    public void OnPointerExit() {
        _uiCanvas.ActiveInventoryPanel = null;
    }

    #endregion
}
