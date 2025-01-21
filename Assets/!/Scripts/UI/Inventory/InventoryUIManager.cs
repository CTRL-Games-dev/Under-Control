using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static InventorySystem;

public class InventoryUIManager : MonoBehaviour
{

    [Header("Prefabs")]
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private GameObject _invTilePrefab;

    [Header("Inventory Stuff")]
    [SerializeField] private RectTransform _gridBoundsRectTransform;
    [SerializeField] private GameObject _fullContainer;
    [SerializeField] private GameObject _gridHolder;
    [SerializeField] private GameObject _itemHolder;

    [Header("Interactions")]
    [SerializeField] private GameObject _dropArea;
    [SerializeField] private ItemEntityManager _itemEntityManager;
    [SerializeField] private SelectedItemUI _selectedItemUI;
    [SerializeField] private Image _redPanel;
    
    [Header("Item Info Panel")]
    [SerializeField] private GameObject _itemInfoPanel;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemDescription;

    
    // References set in Awake
    private UICanvas _uiCanvasParent;
    private InventorySystem _playerInventory;
    private GridLayoutGroup _gridLayoutGroup;

    private Animator _animator;
    private readonly int _slideDownAnimation = Animator.StringToHash("SlideDown");
    private readonly int _slideUpAnimation = Animator.StringToHash("SlideUp");
    private bool _isInventoryOpen = false;
    private bool _isItemInfoOpen = false;

    // Inventory stuff
    private static float _tileSize;
    public static float TileSize { get { return _tileSize; } private set { _tileSize = value; } }


    private int _inventoryWidth, _inventoryHeight;
    public int InventoryHeight { get { return _inventoryHeight; } }
    public int InventoryWidth { get { return _inventoryWidth; } }
    private InvTile[,] _inventoryTileArray;
    public InvTile SelectedTile;
    private InventoryItem _selectedInventoryItem;
    public InventoryItem SelectedInventoryItem { 
        get { return _selectedInventoryItem; } 
        private set { 
            _dropArea.SetActive(value != null);
            _selectedItemUI.InventoryItem = value; 
            _selectedInventoryItem = value;
            ClearHighlights();
            if (value != null && SelectedTile != null) {
                HighlightNeighbours(SelectedTile.Pos, value);
            }
        }
    }


    private List<InventoryItem> _inventory;


    // Lifetime methods
    private void Awake() {
        _uiCanvasParent = gameObject.GetComponentInParent<UICanvas>(); 
        _gridLayoutGroup = _gridHolder.GetComponent<GridLayoutGroup>();
        _animator = GetComponent<Animator>();
    }

    private void Start() {
        EventBus.OnInventoryItemChanged.AddListener(UpdateItemUIS);

        _playerInventory = _uiCanvasParent.PlayerInventory;
        _uiCanvasParent.PlayerController.OnInventoryToggleEvent.AddListener(OnToggleInventory);
        _uiCanvasParent.PlayerController.OnUICancelEvent.AddListener(OnUICancel);
        _uiCanvasParent.PlayerController.OnItemRotateEvent.AddListener(OnItemRotate);

        setupGrid();
        _inventory = _playerInventory.GetItems();
        UpdateItemUIS();
    }

    private void LateUpdate() {
        if (_isItemInfoOpen) {
            _itemInfoPanel.transform.position = new Vector2(
                Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - _itemInfoPanel.GetComponent<RectTransform>().rect.width), 
                Mathf.Clamp(Input.mousePosition.y, _itemInfoPanel.GetComponent<RectTransform>().rect.height, Screen.height)
            );
        }
    }


    // ItemUI methods
    public void UpdateItemUIS() {
        foreach (InventoryItem inventoryItem in _inventory) {
            destroyItemUI(inventoryItem);
            createItemUI(inventoryItem);
        }
    }

    private void createItemUI(InventoryItem inventoryItem){
        var itemGameObject = Instantiate(_itemPrefab, _itemHolder.transform);
        itemGameObject.name = inventoryItem.Item.DisplayName;

        inventoryItem.RectTransform = itemGameObject.GetComponent<RectTransform>();

        inventoryItem.ItemUI = itemGameObject.GetComponent<ItemUI>();
        inventoryItem.ItemUI.InventoryUIManager = this;
        inventoryItem.ItemUI.SetupItem(inventoryItem, TileSize, OccupyTiles(inventoryItem));
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
    }
    
    public void TryMoveSelectedItem() {
        ClearHighlights();

        if (SelectedTile == null) {
            Debug.Log("No tile selected");
            return;
        }

        if (_selectedInventoryItem == null) {
            Debug.Log("No item selected");
            return;
        }

        Vector2Int selectedTilePos = SelectedTile.Pos;
        if (SelectedInventoryItem.Size.x > 1 && SelectedInventoryItem.Rotated) {
            selectedTilePos = new Vector2Int(SelectedTile.Pos.x, SelectedTile.Pos.y - SelectedInventoryItem.Size.x + 1);
        }


        Vector2Int size = SelectedInventoryItem.Rotated ? new Vector2Int(SelectedInventoryItem.Size.y, SelectedInventoryItem.Size.x) : SelectedInventoryItem.Size;

        if (!_playerInventory.FitsWithinBounds(selectedTilePos, size)) {
            Debug.Log("Doesn't fit within");
            StartCoroutine(redPanelShow());
            return;
        }


        if (!canBePlaced(selectedTilePos, size)) {
            Debug.Log("Can't be placed");
            StartCoroutine(redPanelShow());
            return;
        }

        // moveItemUI(_selectedInventoryItem, SelectedTile.Pos);
        // moveItemUI(_selectedInventoryItem, SelectedTile.Pos);
        // destroyItemUI(SelectedInventoryItem);
        SelectedInventoryItem.Position = selectedTilePos;
        createItemUI(SelectedInventoryItem);
        SelectedInventoryItem = null;
        SetImagesRaycastTarget(true);


        SelectedTile.SetHighlight(false);
    }



    // Inventory grid methods
    private void setupGrid() {
        _inventoryWidth = _playerInventory.InventorySize.x;
        _inventoryHeight = _playerInventory.InventorySize.y;

        TileSize = Mathf.Clamp(_gridBoundsRectTransform.rect.width /_inventoryWidth, 0, _gridBoundsRectTransform.rect.height / _inventoryHeight);
        _gridLayoutGroup.constraintCount = _inventoryWidth;

        _gridLayoutGroup.cellSize = new Vector2(TileSize, TileSize);
        _inventoryTileArray = new InvTile[_inventoryHeight, _inventoryWidth];

        float xOffset = (_gridBoundsRectTransform.rect.width - _inventoryWidth * TileSize) / 2;
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
                
                invTile.InventoryUIManager = this;
                invTile.Pos = new Vector2Int(x, y);
            }
        }
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

    public List<InvTile> OccupyTiles(InventoryItem inventoryItem) {
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

    public bool HighlightNeighbours(Vector2Int pos, InventoryItem inventoryItem) {
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

        return false;
    }

    public void ClearHighlights() {
        for (int y = 0; y < _inventoryHeight; y++) {
            for (int x = 0; x < _inventoryWidth; x++) {
                _inventoryTileArray[y, x].SetHighlight(false);
            }
        }
        SelectedTile?.SetHighlight(false);
    }

    public void SetImagesRaycastTarget(bool val) {
        foreach (Transform child in _itemHolder.transform) {
            child.GetComponent<ItemUI>().Image.raycastTarget = val;
        }
    }


    // Inventory interaction methods
    public bool SetSelectedInventoryItem(InventoryItem inventoryItem) {
        if (_selectedInventoryItem != null && inventoryItem != null && !(_selectedInventoryItem == inventoryItem)) {
                StartCoroutine(redPanelShow());
                return false;
        }
        _itemInfoPanel.SetActive(false);
        SelectedInventoryItem = inventoryItem;
        _selectedItemUI.InventoryItem = _selectedInventoryItem;
        destroyItemUI(inventoryItem);
        SetImagesRaycastTarget(false);
        return true;
    }

    public void DisplayItemInfo(InventoryItem inventoryItem) {
        _isItemInfoOpen = inventoryItem != null;
        if (SelectedInventoryItem != null) {
            return;
        }
        if (inventoryItem == null) {
            _itemInfoPanel.SetActive(false);
            return;
        }

        
        _itemName.text = inventoryItem.Item.DisplayName;
        _itemDescription.text = inventoryItem.Item.Description;
        _itemInfoPanel.SetActive(true);
    }

    public void ClearSelectedItem() {
        if (SelectedInventoryItem != null) {
            if (SelectedInventoryItem.ItemUI != null) {
                SelectedInventoryItem.ItemUI.Image.raycastTarget = true;
            }   
            SelectedInventoryItem = null;
        }
        SetImagesRaycastTarget(true);
    }

    public void DropItem() {
        if (SelectedInventoryItem == null) {
            return;
        }
        _itemEntityManager.SpawnItemEntity(SelectedInventoryItem.Item, SelectedInventoryItem.Amount, _uiCanvasParent.PlayerController.transform.position + new Vector3(Random.Range(1f, 5f), 1, Random.Range(1f, 5f)));
        destroyItemUI(SelectedInventoryItem);
        _playerInventory.RemoveInventoryItem(SelectedInventoryItem);
        // SelectedInventoryItem.ItemUI.OccupiedTiles.ForEach(tile => tile.IsEmpty = true);

        ClearSelectedItem();
    }

    private IEnumerator redPanelShow() {
        _redPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        _redPanel.gameObject.SetActive(false);
    }


    // Callbacks
    private void OnUICancel() {
        if (SelectedInventoryItem != null) {
            createItemUI(SelectedInventoryItem);
            ClearSelectedItem();
        }
    }

    private void OnToggleInventory() {
        _isInventoryOpen = !_isInventoryOpen;
        if (_isInventoryOpen) {
            _animator.SetTrigger(_slideDownAnimation);
        } else {
            _animator.SetTrigger(_slideUpAnimation);
            if (SelectedInventoryItem != null) {
                createItemUI(SelectedInventoryItem);
                ClearSelectedItem();
            }
        }
    }

    private void OnItemRotate() {
        if (SelectedInventoryItem == null) {
            return;
        }
        SelectedInventoryItem.Rotated = !SelectedInventoryItem.Rotated;

        ClearHighlights();
        if (SelectedTile != null) HighlightNeighbours(SelectedTile.Pos, SelectedInventoryItem);

        _selectedItemUI.Rotate(SelectedInventoryItem.Rotated);
    }
}
