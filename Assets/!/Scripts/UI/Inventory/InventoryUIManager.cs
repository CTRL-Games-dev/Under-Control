using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

using static InventorySystem;


public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private InventorySystem _playerInventory;

    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private GameObject _invTilePrefab;
    [SerializeField] private GameObject _gridHolder;
    [SerializeField] private GameObject _itemHolder;
    [SerializeField] private SelectedItemUI _selectedItemUI;
    [SerializeField] private Image _redPanel;
    [SerializeField] private GameObject _itemInfoPanel;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemDescription;


    private RectTransform _rectTransform;
    private GridLayoutGroup _gridLayoutGroup;

    private static float _tileSize;
    public static float TileSize { get { return _tileSize; } private set { _tileSize = value; } }


    private int _inventoryWidth, _inventoryHeight;
    private InvTile[,] _inventoryTileArray;
    public InvTile SelectedTile;
    private InventoryItem _selectedInventoryItem;
    public InventoryItem SelectedInventoryItem { 
        get { return _selectedInventoryItem; } 
        private set { 
            _selectedItemUI.InventoryItem = value; 
            _selectedInventoryItem = value;
        }
    }


    private List<InventoryItem> _inventory;


    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _gridLayoutGroup = _gridHolder.GetComponent<GridLayoutGroup>();
    }

    private void Start() {        
        setupGrid();
        _inventory = _playerInventory.GetItems();
        setupItems();
    }


    private IEnumerator redPanelShow() {
        _redPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        _redPanel.gameObject.SetActive(false);
    }

    public void DisplayItemInfo(InventoryItem inventoryItem) {
        if (SelectedInventoryItem != null) {
            return;
        }
        if (inventoryItem == null) {
            _itemInfoPanel.SetActive(false);
            return;
        }

        _itemInfoPanel.transform.position = new Vector2(
            Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - _itemInfoPanel.GetComponent<RectTransform>().rect.width), 
            Mathf.Clamp(Input.mousePosition.y, _itemInfoPanel.GetComponent<RectTransform>().rect.height, Screen.height)
        );
        
        _itemInfoPanel.SetActive(true);
        _itemName.text = inventoryItem.Item.DisplayName;
        _itemDescription.text = inventoryItem.Item.Description;
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

        if (!_playerInventory.FitsWithinBounds(SelectedTile.Pos, _selectedInventoryItem.Size)) {
            Debug.Log("Doesn't fit within");
            StartCoroutine(redPanelShow());
            return;
        }

        List<InvTile> occupiedBeforeTiles = _selectedInventoryItem.ItemUI.OccupiedTiles;
        _selectedInventoryItem.ItemUI.OccupiedTiles.ForEach(tile => tile.IsEmpty = true);

        if (!canBePlaced(SelectedTile.Pos, _selectedInventoryItem.Size)) {
            Debug.Log("Can't be placed");
            _selectedInventoryItem.ItemUI.OccupiedTiles = occupiedBeforeTiles;
            _selectedInventoryItem.ItemUI.OccupiedTiles.ForEach(tile => tile.IsEmpty = false);
            StartCoroutine(redPanelShow());
            return;
        }

        _selectedInventoryItem.Position = SelectedTile.Pos;
        _selectedInventoryItem.ItemUI.Image.raycastTarget = true;
        _selectedInventoryItem.ItemUI.OccupiedTiles = PlaceItem(SelectedTile.Pos, _selectedInventoryItem.Size);
        _selectedInventoryItem.RectTransform.anchoredPosition = new Vector2(SelectedTile.Pos.x, -SelectedTile.Pos.y) * TileSize;
        SelectedInventoryItem = null;

        SelectedTile.SetHighlight(false);
    }


    private void setupItems() {
        foreach (InventoryItem inventoryItem in _inventory) {
            var itemGameObject = Instantiate(_itemPrefab, _itemHolder.transform);
            itemGameObject.name = inventoryItem.Item.DisplayName;

            inventoryItem.RectTransform = itemGameObject.GetComponent<RectTransform>();

            inventoryItem.ItemUI = itemGameObject.GetComponent<ItemUI>();
            inventoryItem.ItemUI.InventoryUIManager = this;
            inventoryItem.ItemUI.SetupItem(inventoryItem, TileSize, PlaceItem(inventoryItem.Position, inventoryItem.Size));
        }
    }


    private void setupGrid() {
        _inventoryWidth = _playerInventory.InventorySize.x;
        _inventoryHeight = _playerInventory.InventorySize.y;

        TileSize = Mathf.Clamp(_rectTransform.rect.width /_inventoryWidth, 0, _rectTransform.rect.height / _inventoryHeight);
        _gridLayoutGroup.constraintCount = _inventoryWidth;

        _gridLayoutGroup.cellSize = new Vector2(TileSize, TileSize);
        _inventoryTileArray = new InvTile[_inventoryHeight, _inventoryWidth];

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

    public List<InvTile> PlaceItem(Vector2Int pos, Vector2Int size) {
        List<InvTile> tiles = new();

        for (int y = pos.y; y < pos.y + size.y; y++) {
            for (int x = pos.x; x < pos.x + size.x; x++) {
                _inventoryTileArray[y, x].IsEmpty = false;
                tiles.Add(_inventoryTileArray[y, x]);
            }
        }

        return tiles;
    }

    private bool canBePlaced(Vector2Int pos, Vector2Int size) {
        for (int y = pos.y; y < pos.y + size.y; y++) {
            for (int x = pos.x; x < pos.x + size.x; x++) {
                if (!_inventoryTileArray[y, x].IsEmpty) {
                    return false;
                }
            }
        }

        return true;
    }

    public bool SetSelectedInventoryItem(InventoryItem inventoryItem) {
        if (_selectedInventoryItem != null && inventoryItem != null && !(_selectedInventoryItem == inventoryItem)) {
                StartCoroutine(redPanelShow());
                return false;
        }
        _itemInfoPanel.SetActive(false);
        _selectedInventoryItem = inventoryItem;
        _selectedItemUI.InventoryItem = _selectedInventoryItem;
        return true;
    }

    public bool HighlightNeighbours(Vector2Int pos, Vector2Int size) {
        for (int y = pos.y; y < pos.y + size.y; y++) {
            for (int x = pos.x; x < pos.x + size.x; x++) {
                if (x < 0 || x >= _inventoryWidth || y < 0 || y >= _inventoryHeight) {
                    return false;
                }
                _inventoryTileArray[y, x].SetHighlight(true);
            }
        }

        return true;
    }

    public void ClearHighlights() {
        for (int y = 0; y < _inventoryHeight; y++) {
            for (int x = 0; x < _inventoryWidth; x++) {
                _inventoryTileArray[y, x].SetHighlight(false);
            }
        }
        SelectedTile?.SetHighlight(false);
    }
}
