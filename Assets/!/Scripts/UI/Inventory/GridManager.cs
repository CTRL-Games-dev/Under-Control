using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [HideInInspector] public int InventoryWidth, InventoryHeight;
    [SerializeField] private InvTile _tilePrefab;

    private InvTile[,] _inventoryArray;

    public InvTile[,] InventoryArray => _inventoryArray;

    public InvTile SelectedTile { get; set; }


    public float TileSize { get; private set; }

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


    public bool TryPlaceItem(Vector2Int pos, Vector2Int size) {
        if (pos.x < 0 || pos.y < 0 || pos.x + size.x > InventoryWidth || pos.y + size.y > InventoryHeight) {
            return false;
        }

        for (int y = pos.y; y < pos.y + size.y; y++) {
            for (int x = pos.x; x < pos.x + size.x; x++) {
                if (!_inventoryArray[y, x].IsEmpty) {
                    return false;
                }
            }
        }

        for (int y = pos.y; y < pos.y + size.y; y++) {
            for (int x = pos.x; x < pos.x + size.x; x++) {
                _inventoryArray[y, x].IsEmpty = false;
            }
        }

        return true;

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
