using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class InvTileEquipment : InvTile {
    private enum TileType {
        Armor,
        Amulet,
        Weapon,
        Consumeable1,
        Consumeable2,
    }

    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private TileType _tileType;
    [SerializeField] private GameObject _hintImgGO;
    [SerializeField] private Image _bgImg, _highlightImg, _iconImg;
    [SerializeField] private Sprite _singleBg, _doubleBg, _tripleBg;
    [SerializeField] private Sprite _singleHighlight, _doubleHighlight, _tripleHighlight;
    [SerializeField] private Color _defaultColor, _highlightColor;
    private RectTransform _rectTransform;
    private ItemUI _itemUI;
    
    private InventoryItem SelectedInventoryItem => Player.UICanvas.SelectedItemUI.InventoryItem;

    private void Start() {
        EventBus.TileSizeSetEvent.AddListener(OnTileSizeSetEvent);
        EventBus.ItemUILeftClickEvent.AddListener(OnItemUIClickEvent);
        EventBus.ItemUIRightClickEvent.AddListener(OnItemUIClickEvent);

        EventBus.SelectedItemSet.AddListener(OnItemPickedUpEvent);

        
        Player.LivingEntity.OnDeath.AddListener(resetToDefault);
        

        _rectTransform = GetComponent<RectTransform>();
        // EventBus.InvTileClickEvent.AddListener(OnTileClick);
        OnTileSizeSetEvent();

        /*
            Checking by ItemData, because SerializeField serializes into
            concrete value and not into reference, therefore null is not possible.
            We should move to structs for items...
        */
        switch (_tileType) {
            case TileType.Armor:
                if (Player.Inventory.Armor.ItemData != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem = Player.Inventory.Armor;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break;

            case TileType.Amulet:
                if (Player.Inventory.Amulet.ItemData != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem = Player.Inventory.Amulet;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break;

            case TileType.Weapon:
                if (Player.Inventory.Weapon.ItemData != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem = Player.Inventory.Weapon;
                    inventoryItem.Rotated = false;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break; 
            
            case TileType.Consumeable1:
                Player.Instance.UpdateConsumablesEvent.AddListener(OnConsumablesUpdate);
                if (Player.Instance.ConsumableItemOne != null) {
                    InventoryItem inventoryItem = ((InventoryItem) Player.Instance.ConsumableItemOne).CloneViaSerialization();
                    inventoryItem.Rotated = false;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break;
            
            case TileType.Consumeable2:
                Player.Instance.UpdateConsumablesEvent.AddListener(OnConsumablesUpdate);
                if (Player.Instance.ConsumableItemTwo != null) {
                    InventoryItem inventoryItem = ((InventoryItem) Player.Instance.ConsumableItemTwo).CloneViaSerialization();
                    inventoryItem.Rotated = false;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break;
        }
    }

    private void OnItemPickedUpEvent(ItemData itemData) {
        if (!_hintImgGO.activeSelf) return;
        if (itemData == null) {
            _iconImg.DOKill();
            _iconImg.DOColor(_defaultColor, 0.3f * Settings.AnimationSpeed);
            return;
        }

        switch (_tileType) {
            case TileType.Armor:
                if (itemData is not ArmorItemData) return;
                break;
            case TileType.Amulet:
                if (itemData is not AmuletItemData) return;
                break;
            case TileType.Weapon:
                if (itemData is not WeaponItemData) return;
                break;
            case TileType.Consumeable1:
            case TileType.Consumeable2:
                if (itemData is not ConsumableItemData) return;
                break;
        }

        _iconImg.DOKill();
        _iconImg.DOColor(_highlightColor, 0.3f * Settings.AnimationSpeed);
    }


    private void OnTileSizeSetEvent() {
        _rectTransform.sizeDelta = new Vector2(InventoryPanel.TileSize, InventoryPanel.TileSize * (_tileType == TileType.Weapon ? 3 : 1));
    }

    public void UpdateInvTile() {
        if (_itemUI?.InventoryItem.Amount <= 0) {
            Destroy(_itemUI.gameObject);
            _itemUI = null;
            IsEmpty = true;
        } 
    }

    private void OnItemUIClickEvent(ItemUI itemUI) {        
        if (itemUI == null || Player.UICanvas.SelectedItemUI.InventoryItem != null) return;
        
        if (itemUI != _itemUI) return;
        PickUpItem();
    }

    public void PickUpItem(){
        switch (_tileType) {
            case TileType.Armor:
                Player.Inventory.Armor = null;
                break;
            case TileType.Amulet:
                Player.Inventory.Amulet = null;
                break;
            case TileType.Weapon:
                Player.Inventory.Weapon = null;
                break;
            case TileType.Consumeable1:
                Player.Instance.ConsumableItemOne = null;
                Player.UICanvas.HUDCanvas.OnUpdateConsumables();
                break;
            case TileType.Consumeable2:
                Player.Instance.ConsumableItemTwo = null;
                Player.UICanvas.HUDCanvas.OnUpdateConsumables();
                break;
        }
        
        if (_itemUI != null) Player.UICanvas.SetSelectedItemUI(_itemUI);
        resetToDefault();
    }

    private void resetToDefault() {
        IsEmpty = true;
        _hintImgGO.SetActive(true);
    
        switch (_tileType) {
                case TileType.Armor:
                    Player.Inventory.Armor = null;
                    break;
                case TileType.Amulet:
                    Player.Inventory.Amulet = null;
                    break;
                case TileType.Weapon:
                    Player.Inventory.Weapon = null;
                    break;
                case TileType.Consumeable1:
                    Player.Instance.ConsumableItemOne = null;
                    Player.UICanvas.HUDCanvas.OnUpdateConsumables();
                    break;
                case TileType.Consumeable2:
                    Player.Instance.ConsumableItemTwo = null;
                    Player.UICanvas.HUDCanvas.OnUpdateConsumables();
                    break;
            }


        if (_tileType == TileType.Weapon) {
            _rectTransform.DOKill();
            _rectTransform.DOSizeDelta(new Vector2(InventoryPanel.TileSize, InventoryPanel.TileSize * 3), 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutBack).OnComplete(() => {
                _bgImg.sprite = _tripleBg;
                _highlightImg.sprite = _tripleHighlight;
            });
        }

        if (_itemUI != null) {
            Destroy(_itemUI.gameObject);
            _itemUI = null;
        }
    }

    public void OnPointerClickEquipment() {
        InventoryItem item = SelectedInventoryItem;
        if (item == null && _itemUI == null) return;        
        if (item != null && _itemUI != null) return;
        if(!IsEmpty) return;
        PlaceItem(item);
    }

    public void PlaceItem(InventoryItem item){
        if (_tileType == TileType.Armor) {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.EquipArmor, this.transform.position);
            if(!item.TryAs(out InventoryItem<ArmorItemData> armorItem)) {
                return;
            }

            Player.Inventory.Armor = armorItem;
        } else if (_tileType == TileType.Amulet) {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.EquipAmulet, this.transform.position);
            if(!item.TryAs(out InventoryItem<AmuletItemData> amuletItem)) {
                return;
            }

            Player.Inventory.Amulet = amuletItem;
        } else if (_tileType == TileType.Weapon) {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.EquipWeapon, this.transform.position);
            if(!item.TryAs(out InventoryItem<WeaponItemData> weaponItem)) {
                return;
            }
            if (weaponItem.ItemData.WeaponType == WeaponType.Fishingrod) {
                Player.Instance.EquipFishingRod(true);
                // return;
            }

            AudioClip EquipWeaponClip = Resources.Load("SFX/bron/wyjmowaniebroni") as AudioClip;
            SoundFXManager.Instance.PlaySoundFXClip(EquipWeaponClip,transform);
            Player.Inventory.Weapon = weaponItem;
        } else if (_tileType == TileType.Consumeable1) {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.EquipItem, this.transform.position);
            if(!item.TryAs(out InventoryItem<ConsumableItemData> consumableItem)) {
                return;
            }

            Player.Instance.ConsumableItemOne = consumableItem;
            Player.UICanvas.HUDCanvas.OnUpdateConsumables();
        } else if (_tileType == TileType.Consumeable2) {
            if(!item.TryAs(out InventoryItem<ConsumableItemData> consumableItem)) {
                return;
            }

            Player.Instance.ConsumableItemTwo = consumableItem;
            Player.UICanvas.HUDCanvas.OnUpdateConsumables();
        }


        _hintImgGO.SetActive(false);
        
        
        item.Rotated = _tileType == TileType.Weapon ? false : item.Rotated;

        if (_tileType == TileType.Weapon) {

            Sprite goalSprite;

            if (item.Size.y == 1) {
                goalSprite = _singleBg;
                _highlightImg.sprite = _singleHighlight;
            } else if (item.Size.y == 2) {
                goalSprite = _doubleBg;
                _highlightImg.sprite = _doubleHighlight;
            } else {
                goalSprite = _tripleBg;
                _highlightImg.sprite = _tripleHighlight;
            }

            
            _rectTransform.DOKill();
            _rectTransform.DOSizeDelta(new Vector2(InventoryPanel.TileSize, InventoryPanel.TileSize * item.Size.y), 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutBack).OnComplete(() => {
                _bgImg.sprite = goalSprite;
            });
        }

        createItemUI(item);

        EventBus.ItemPlacedEvent?.Invoke();
        IsEmpty = false;
        Player.UICanvas.SelectedItemUI.InventoryItem = null;
    }

    private void OnConsumablesUpdate() {
        // Check if we have anything to update
        if(_itemUI == null) return;

        InventoryItem consumable = _tileType == TileType.Consumeable1 ? Player.Instance.ConsumableItemOne : Player.Instance.ConsumableItemTwo;

        if (consumable == null) {
            resetToDefault();
        }  else {
            // Londek: wyglada na niepotrzebne bo item jest ustawiany przy
            // tworzeniu itemui i taki chyba powinien pozostac, nie?
            _itemUI.InventoryItem = consumable;
            
            _itemUI.UpdateAmount();
        }
    }

    private GameObject createItemUI(InventoryItem inventoryItem){
        if (inventoryItem.ItemData == null) return null;
        GameObject itemGameObject = Instantiate(_itemPrefab, _rectTransform);
        itemGameObject.name = inventoryItem.ItemData.DisplayName;

        inventoryItem.Position = new Vector2Int(0, 0);
        inventoryItem.RectTransform = itemGameObject.GetComponent<RectTransform>();
        inventoryItem.ItemUI = itemGameObject.GetComponent<ItemUI>();
        inventoryItem.ItemUI.SetupItem(inventoryItem, InventoryPanel.TileSize, null, null);

        _itemUI = inventoryItem.ItemUI;
        return itemGameObject;
    }
    #region Save and load methods
    public void removeItem(){
        switch (_tileType) {
                case TileType.Armor:
                    Player.Inventory.Armor = null;
                    break;
                case TileType.Amulet:
                    Player.Inventory.Amulet = null;
                    break;
                case TileType.Weapon:
                    Player.Instance.EquipFishingRod(false);
                    Player.Inventory.Weapon = null;
                    break;
                case TileType.Consumeable1:
                    Player.Instance.ConsumableItemOne = null;
                    Player.UICanvas.HUDCanvas.OnUpdateConsumables();
                    break;
                case TileType.Consumeable2:
                    Player.Instance.ConsumableItemTwo = null;
                    Player.UICanvas.HUDCanvas.OnUpdateConsumables();
                    break;
            }
            
            IsEmpty = true;
            _hintImgGO.SetActive(true);
            if (_tileType == TileType.Weapon) {
                _rectTransform.DOKill();
                _rectTransform.DOSizeDelta(new Vector2(InventoryPanel.TileSize, InventoryPanel.TileSize * 3), 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutBack).OnComplete(() => {
                    _bgImg  .sprite = _tripleBg;
                    _highlightImg.sprite = _tripleHighlight;
                });
            }

            if (_itemUI != null) {
                Player.UICanvas.SetSelectedItemUI(_itemUI);
                Destroy(_itemUI.gameObject);
                _itemUI = null;
            }
    }
    #endregion
}
