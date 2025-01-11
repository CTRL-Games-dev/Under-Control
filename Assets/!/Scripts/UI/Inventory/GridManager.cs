using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [HideInInspector] public int InventoryWidth, InventoryHeight;
    [SerializeField] private InvTile _tilePrefab;

    private InvTile[,] _inventoryArray;

    public InvTile[,] InventoryArray => _inventoryArray;

    public InvTile SelectedTile { get; set; }

    [SerializeField] private SelectedItemUI _selectedItemUI;


    private static float _tileSize;
    public static float TileSize { 
        get {
            return _tileSize;
        } 
        private set {
            _tileSize = value;
        }
    }

    private GridLayoutGroup _gridLayoutGroup;

    private void Awake() {
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
    }


    public void SetupGrid() {
        TileSize = GetComponent<RectTransform>().rect.width / InventoryWidth;

        _gridLayoutGroup.cellSize = new Vector2(TileSize, TileSize);
        _inventoryArray = new InvTile[InventoryHeight, InventoryWidth];

        generateGrid();
    }


    public List<InvTile> PlaceItem(Vector2Int pos, Vector2Int size) {
        List<InvTile> tiles = new();

        for (int y = pos.y; y < pos.y + size.y; y++) {
            for (int x = pos.x; x < pos.x + size.x; x++) {
                _inventoryArray[y, x].IsEmpty = false;
                tiles.Add(_inventoryArray[y, x]);
            }
        }

        return tiles;
    }
    public void RemoveItem(ItemUI itemUI) {
        foreach (var tile in itemUI.OccupiedTiles) {
            tile.IsEmpty = true;
        }
        
    }

    public void MoveSelectedItem() {
        if (SelectedTile == null) {
            return;
        }

        SelectedTile.IsEmpty = false;

        RemoveItem(_selectedItemUI.ItemUI);
        _selectedItemUI.InventoryItem.RectTransform.anchoredPosition = new Vector2(SelectedTile.Pos.x, -SelectedTile.Pos.y) * TileSize;
        _selectedItemUI.ItemUI.OccupiedTiles = PlaceItem(SelectedTile.Pos, _selectedItemUI.InventoryItem.Size);
    }


    private void generateGrid() {
        for (int y = 0; y < InventoryHeight; y++) {
            for (int x = 0; x < InventoryWidth; x++) {
                var tile = Instantiate(_tilePrefab, transform);
                tile.name = $"Tile {x} {y}";

                InvTile invTile = tile.GetComponent<InvTile>();

                _inventoryArray[y, x] = invTile;
                
                invTile.GridManager = this;
                invTile.Pos = new Vector2Int(x, y);
            }
        }
    }
}
