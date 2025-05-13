using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using System;
using NUnit.Framework.Internal;
using System.Collections;

public class InventoryCanvas : MonoBehaviour, IUICanvasState
{
    public enum InventoryTab {
        Other,
        Armor,
        Cards,
        Evo,
        Seller
    }

    private IInteractableInventory _lastInteractableInventory;
    private EntityInventory _currentOtherInventory;
    [SerializeField] private GameObject _otherInventoryHolder;
    [SerializeField] private RectTransform _underlineRect;
    [SerializeField] private GameObject _armorBtnTabGO, _cardsBtnTabGO, _evoBtnTabGO, _sellerBtnTabGO, _otherBtnTabGO;
    [SerializeField] private TextLocalizer _otherTabTextLocalizer;
    [SerializeField] private Image _bgImage;

    [Header("Panel Game Objects")]
    [SerializeField] private GameObject _playerInventoryPanelGO;
    [SerializeField] private GameObject _armorInventoryPanelGO, _cardsInventoryPanelGO, _evoInventoryPanelGO, _sellerInventoryPanelGO, _otherInventoryPanelGO;

    [Header("Armor panel parts")]
    [SerializeField] private GameObject _playerPreviewGO;
    [SerializeField] private GameObject[] _armorSlotsGO = new GameObject[7];
    private RectTransform[] _armorSlotsRects = new RectTransform[7];
    private CanvasGroup[] _armorSlotsCanvasGroups = new CanvasGroup[7];

    [Header("Evolution panel parts")]
    public EvoInfo EvoInfo;
    [SerializeField] private GameObject _evoPointsGO;
    [SerializeField] private RectTransform _pointsHolderRect;

    [Header("Other panel parts")]
    public CardsPanel CardsPanel;

    [Header("Seller panel parts")]
    public SellerPanels SellerPanels;

    private GameObject[] _tabButtonGameObjects;
    private TextMeshProUGUI[] _tabTexts;
    private RectTransform[] _tabRectTranforms;
    private Dictionary<InventoryTab, int> _tabPanelsIndex;

    private CanvasGroup _canvasGroup;
    private InventoryTab _currentTab = InventoryTab.Armor;
    public InventoryPanel PlayerInventoryPanel;



    #region Unity Methods

    void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
        _tabButtonGameObjects = new GameObject[] {
            _armorBtnTabGO,
            _cardsBtnTabGO,
            _evoBtnTabGO,
            _sellerBtnTabGO,
            _otherBtnTabGO
        };

        _tabPanelsIndex = new Dictionary<InventoryTab, int> {
            { InventoryTab.Armor, 0 },
            { InventoryTab.Cards, 1 },
            { InventoryTab.Evo, 2 },
            { InventoryTab.Seller, 3 },
            { InventoryTab.Other, 4 }
        };

        _tabTexts = new TextMeshProUGUI[] {
            _armorBtnTabGO.GetComponentInChildren<TextMeshProUGUI>(),
            _cardsBtnTabGO.GetComponentInChildren<TextMeshProUGUI>(),
            _evoBtnTabGO.GetComponentInChildren<TextMeshProUGUI>(),
            _sellerBtnTabGO.GetComponentInChildren<TextMeshProUGUI>(),
            _otherBtnTabGO.GetComponentInChildren<TextMeshProUGUI>()
        };

        _tabRectTranforms = new RectTransform[] {
            _armorBtnTabGO.GetComponent<RectTransform>(),
            _cardsBtnTabGO.GetComponent<RectTransform>(),
            _evoBtnTabGO.GetComponent<RectTransform>(),
            _sellerBtnTabGO.GetComponent<RectTransform>(),
            _otherBtnTabGO.GetComponent<RectTransform>()
        };
    }

    void Start() {
        _currentTab = InventoryTab.Armor;
        
        foreach (GameObject go in _armorSlotsGO) {
            _armorSlotsRects[Array.IndexOf(_armorSlotsGO, go)] = go.GetComponent<RectTransform>();
            _armorSlotsCanvasGroups[Array.IndexOf(_armorSlotsGO, go)] = go.GetComponent<CanvasGroup>();
        }
    }

    #endregion

    #region Public Methods

    public void HideUI() {
        CloseAllTabs();
        
        _canvasGroup.DOFade(0, 0.25f * Settings.AnimationSpeed);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        SetOtherInventory(null, null);
    }

    public void ShowUI() {
        gameObject.SetActive(true);
        _canvasGroup.DOComplete();

        _underlineRect.DOAnchorPos(_tabRectTranforms[_tabPanelsIndex[_currentTab]].anchoredPosition + new Vector2(0, -24), 0);
        SetCurrentTab(_currentTab);
        _canvasGroup.DOFade(1, 0.25f * Settings.AnimationSpeed);
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    public void SetCurrentTab(InventoryTab tab) {
        InventoryTab previousTab = _currentTab;
        _currentTab = tab;
        _bgImage.DOFade(_currentTab == InventoryTab.Seller ? 175f / 255f : 250f / 255f, 0.25f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
        _underlineRect.DOAnchorPos(_tabRectTranforms[_tabPanelsIndex[_currentTab]].anchoredPosition + new Vector2(0, -24), 0.25f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);

        foreach (InventoryTab t in _tabPanelsIndex.Keys) {
            if (t == _currentTab) {
                highlightTab(t, true);
            } else {
                highlightTab(t, false);
            }
        }

        // KillAllTweens(); TODO
        if (previousTab == _currentTab) {
            openTab(_currentTab);
            return;
        }
        switch (previousTab) {
            case InventoryTab.Other:
                if (!(_currentTab == InventoryTab.Armor || _currentTab == InventoryTab.Other || _currentTab == InventoryTab.Seller)) playerTabExit();
                otherTabExit().OnComplete(() => openTab(_currentTab));
                break;
            case InventoryTab.Armor:
                if (!(_currentTab == InventoryTab.Armor || _currentTab == InventoryTab.Other || _currentTab == InventoryTab.Seller)) playerTabExit();
                armorTabExit().OnComplete(() => openTab(_currentTab));
                break;
            case InventoryTab.Cards:
                cardsTabExit().OnComplete(() => openTab(_currentTab));
                break;
            case InventoryTab.Evo:
                evoTabExit().OnComplete(() => openTab(_currentTab));
                break;
            case InventoryTab.Seller:
                if (!(_currentTab == InventoryTab.Armor || _currentTab == InventoryTab.Other || _currentTab == InventoryTab.Seller)) playerTabExit();
                sellerTabExit().OnComplete(() => openTab(_currentTab));
                break;
        }
    }

    private void openTab(InventoryTab tab) {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClickSound, this.transform.position);
        switch (tab) {
            case InventoryTab.Other:
                otherTabEnter();
                playerTabEnter();
                break;
            case InventoryTab.Armor:
                armorTabEnter();
                playerTabEnter();
                break;
            case InventoryTab.Cards:
                cardsTabEnter();
                break;
            case InventoryTab.Evo:
                evoTabEnter();
                break;
            case InventoryTab.Seller:
                playerTabEnter();
                sellerTabEnter();
                break;
        }
    }

    public void OpenCurrentTab() {
        SetCurrentTab(_currentTab);
    }

    public void CloseAllTabs() {
        playerTabExit();
        armorTabExit();
        cardsTabExit();
        evoTabExit();
        sellerTabExit();
        otherTabExit();
    }

    public void KillAllTweens() {
        _otherInventoryPanelGO.GetComponent<RectTransform>().DOKill();
        _armorInventoryPanelGO.GetComponent<RectTransform>().DOKill();
        _cardsInventoryPanelGO.GetComponent<RectTransform>().DOKill();
        _evoInventoryPanelGO.GetComponent<RectTransform>().DOKill();
        _sellerInventoryPanelGO.GetComponent<RectTransform>().DOKill();
        _playerInventoryPanelGO.GetComponent<RectTransform>().DOKill();
    }

    public void SetOtherInventory(EntityInventory entityInventory, GameObject prefab, IInteractableInventory interactable = null, string title = null, bool changeAlpha = false, bool isChest = false) {
        if (_currentOtherInventory == entityInventory) return;
        _currentTab = entityInventory == null ? InventoryTab.Armor : InventoryTab.Other;

        _lastInteractableInventory?.EndInteract();
        _lastInteractableInventory = interactable;

        _currentOtherInventory = entityInventory;

        SetOtherTabTitle(title);

        if (entityInventory != null) { // animacja zmiany
            if (_otherInventoryHolder.transform.childCount > 0)
                Destroy(_otherInventoryHolder.transform.GetChild(0).gameObject);
            
            InventoryPanel inventoryPanel = Instantiate(prefab, _otherInventoryHolder.transform).GetComponentInChildren<InventoryPanel>();
            StartCoroutine(siur(inventoryPanel, entityInventory));
        } else {
            if (_otherInventoryHolder.transform.childCount > 0)
                Destroy(_otherInventoryHolder.transform.GetChild(0).gameObject);
        }
    }


    private IEnumerator siur(InventoryPanel inv, EntityInventory entInv) {
        yield return new WaitForEndOfFrame();

        inv.SetTargetInventory(entInv);
    }

    public void SetOtherTabTitle(string text) {
        _otherTabTextLocalizer.Key = text ?? "";
        _tabButtonGameObjects[_tabPanelsIndex[InventoryTab.Other]].GetComponent<Button>().interactable = text != null;
    }

    public void SetSellerTab(Seller seller) {
        SellerPanels.SetSeller(seller);
        if (seller == null) {
            _sellerBtnTabGO.SetActive(false);
            _currentTab = InventoryTab.Armor;
        } else {
            _sellerBtnTabGO.SetActive(true);
            _currentTab = InventoryTab.Seller;
        }

    }

    public Tween OtherTabExit() {
        return otherTabExit();
    }
    public Tween OtherTabEnter() {
        return otherTabEnter();
    }

    public void OnBackgroundClick() {
        Player.UICanvas.DropItem();
    }

    public void ChangeEvoPoints() {
        foreach(RectTransform rect in _pointsHolderRect) {
            rect.DOKill();
            rect.DOScale(0.9f, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic).OnComplete(() => {
                rect.DOScale(1, 0.2f * Settings.AnimationSpeed).SetDelay(0.4f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
            });
        }
        _pointsHolderRect.DOAnchorPosY(Player.Instance.EvolutionPoints * 57, 0.4f * Settings.AnimationSpeed).SetDelay(0.2f * Settings.AnimationSpeed).SetEase(Ease.InOutBack);
    }


    public void OnOpenOtherTab() { SetCurrentTab(InventoryTab.Other); }

    public void OnOpenArmorTab() { SetCurrentTab(InventoryTab.Armor); }

    public void OnOpenCardsTab() { SetCurrentTab(InventoryTab.Cards); }

    public void OnOpenEvoTab() { SetCurrentTab(InventoryTab.Evo); }

    public void OnOpenSellerTab() { SetCurrentTab(InventoryTab.Seller); }
    
    public void HighlightButton(int index) {
        if (index == _tabPanelsIndex[_currentTab]) return;
        _tabButtonGameObjects[index].transform.DOScale(1.1f, 0.75f * Settings.AnimationSpeed);
    }

    public void UnhighlightButton(int index) {
        if (index == _tabPanelsIndex[_currentTab]) return;
        _tabButtonGameObjects[index].transform.DOScale(1f, 0.75f * Settings.AnimationSpeed);
    }
    #endregion

    #region Private Methods

    private void highlightTab(InventoryTab tab, bool value) {
        _tabButtonGameObjects[_tabPanelsIndex[tab]].transform.DOKill();
        if (value) {
            _tabButtonGameObjects[_tabPanelsIndex[tab]].transform.DOScale(1.2f, 0.35f * Settings.AnimationSpeed);
            _tabTexts[_tabPanelsIndex[tab]].fontStyle = FontStyles.Bold;
        } else {
            _tabButtonGameObjects[_tabPanelsIndex[tab]].transform.DOScale(1, 0.35f * Settings.AnimationSpeed);
            _tabTexts[_tabPanelsIndex[tab]].fontStyle = FontStyles.Normal;
        } 
    }

    private Tween playerTabEnter() {
        _playerInventoryPanelGO.SetActive(true);
        return _playerInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(-180, 0.5f * Settings.AnimationSpeed).SetEase(Ease.OutExpo);
    }

    private Tween playerTabExit() {
        return _playerInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(470, 0.5f * Settings.AnimationSpeed).SetEase(Ease.OutExpo);
    }

    private Tween otherTabEnter() {
        _otherInventoryPanelGO.SetActive(true);
        return _otherInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(120, 0.5f * Settings.AnimationSpeed).SetEase(Ease.OutExpo);
    }

    private Tween otherTabExit() {
        _bgImage.DOFade(250f / 255f, 0.25f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
        return _otherInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(-760, 0.5f * Settings.AnimationSpeed).SetEase(Ease.OutExpo);
    }

    private Tween armorTabEnter() {
        float fadeSpeed = 0.05f;

        _playerInventoryPanelGO.GetComponent<RawImage>().DOComplete();
        for (int i = 0; i < _armorSlotsGO.Length; i++) {
            _armorSlotsCanvasGroups[i].DOKill();
            _armorSlotsRects[i].DOKill();
        }
        
        _armorInventoryPanelGO.SetActive(true);

        _playerPreviewGO.GetComponent<RawImage>().DOFade(1, 0.5f);

        float scaleSpeed = 0.2f;

        _armorSlotsRects[0].DOScale(1, scaleSpeed * Settings.AnimationSpeed).SetEase(Ease.OutBack);
        
        _armorSlotsCanvasGroups[0].DOFade(1, fadeSpeed * Settings.AnimationSpeed).OnComplete(() => {
            _armorSlotsRects[1].DOScale(1, scaleSpeed * Settings.AnimationSpeed).SetEase(Ease.OutBack);
            _armorSlotsCanvasGroups[1].DOFade(1, fadeSpeed * Settings.AnimationSpeed).OnComplete(() => {
                _armorSlotsRects[2].DOScale(1, scaleSpeed * Settings.AnimationSpeed).SetEase(Ease.OutBack);
                _armorSlotsCanvasGroups[2].DOFade(1, fadeSpeed * Settings.AnimationSpeed).OnComplete(() => {
                    _armorSlotsRects[3].DOScale(1, scaleSpeed * Settings.AnimationSpeed).SetEase(Ease.OutBack);
                    _armorSlotsCanvasGroups[3].DOFade(1, fadeSpeed * Settings.AnimationSpeed).OnComplete(() => {
                        _armorSlotsRects[4].DOScale(1, scaleSpeed * Settings.AnimationSpeed).SetEase(Ease.OutBack);
                        _armorSlotsCanvasGroups[4].DOFade(1, fadeSpeed * Settings.AnimationSpeed);
                    });
                });
            });
        });
        
        return _armorInventoryPanelGO.transform.DOScale(1, fadeSpeed * 5 * Settings.AnimationSpeed);
    }

    private Tween armorTabExit() {
        _playerInventoryPanelGO.GetComponent<RawImage>().DOComplete();
        for (int i = 0; i < _armorSlotsGO.Length; i++) {
            _armorSlotsCanvasGroups[i].DOKill();
            _armorSlotsRects[i].DOKill();
        }
        
        float fadeSpeed = 0.3f;
        float scaleSpeed = 0.05f;

        _playerPreviewGO.GetComponent<RawImage>().DOFade(0, 0.5f * Settings.AnimationSpeed);
        _armorSlotsCanvasGroups[0].DOFade(0, fadeSpeed * Settings.AnimationSpeed);
        _armorSlotsRects[0].DOScale(0, scaleSpeed * Settings.AnimationSpeed).SetEase(Ease.InBack).OnComplete(() => {
            _armorSlotsCanvasGroups[1].DOFade(0, fadeSpeed * Settings.AnimationSpeed);
            _armorSlotsRects[1].DOScale(0, scaleSpeed * Settings.AnimationSpeed).SetEase(Ease.InBack).OnComplete(() => {
                _armorSlotsCanvasGroups[2].DOFade(0, fadeSpeed * Settings.AnimationSpeed);
                _armorSlotsRects[2].DOScale(0, scaleSpeed * Settings.AnimationSpeed).SetEase(Ease.InBack).OnComplete(() => {
                    _armorSlotsCanvasGroups[3].DOFade(0, fadeSpeed * Settings.AnimationSpeed);
                    _armorSlotsRects[3].DOScale(0, scaleSpeed * Settings.AnimationSpeed).SetEase(Ease.InBack).OnComplete(() => {
                        _armorSlotsCanvasGroups[4].DOFade(0, fadeSpeed * Settings.AnimationSpeed);
                        _armorSlotsRects[4].DOScale(0, scaleSpeed * Settings.AnimationSpeed).SetEase(Ease.InBack);
                    });
                });
            });
        });        

        return _armorInventoryPanelGO.transform.DOScale(1, scaleSpeed * 5 * Settings.AnimationSpeed);
    }

    private Tween cardsTabEnter() {
        CanvasGroup c = _cardsInventoryPanelGO.GetComponent<CanvasGroup>();
        c.DOKill();
        c.interactable = true;
        c.blocksRaycasts = true;
        return c.DOFade(1, 0.25f * Settings.AnimationSpeed);
    }

    private Tween cardsTabExit() {
        CanvasGroup c = _cardsInventoryPanelGO.GetComponent<CanvasGroup>();
        c.DOKill();
        c.interactable = false;
        c.blocksRaycasts = false;
        return c.DOFade(0, 0.25f * Settings.AnimationSpeed);
    }

    private Tween evoTabEnter() {
        _evoInventoryPanelGO.SetActive(true);
        CanvasGroup c = _evoInventoryPanelGO.GetComponent<CanvasGroup>();
        c.DOKill();
        c.interactable = true;
        c.blocksRaycasts = true;
        ChangeEvoPoints();
        return c.DOFade(1, 0.25f * Settings.AnimationSpeed);
    }

    private Tween evoTabExit() {
        CanvasGroup c = _evoInventoryPanelGO.GetComponent<CanvasGroup>();
        c.DOKill();
        c.interactable = false;
        c.blocksRaycasts = false;
        return c.DOFade(0, 0.25f * Settings.AnimationSpeed).OnComplete(() => _evoInventoryPanelGO.SetActive(false));
    }

    private Tween sellerTabEnter() {
        _bgImage.DOFade(175f / 255f, 0.25f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
        SellerPanels.ShowButtons();
        CanvasGroup c = _sellerInventoryPanelGO.GetComponent<CanvasGroup>();
        c.DOKill();
        c.interactable = true;
        c.blocksRaycasts = true;
        return c.DOFade(1, 0.25f * Settings.AnimationSpeed);
    }

    private Tween sellerTabExit() {
        _bgImage.DOFade(250f / 255f, 0.25f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
        SellerPanels.HideButtons();
        CanvasGroup c = _sellerInventoryPanelGO.GetComponent<CanvasGroup>();
        c.DOKill();
        c.interactable = false;
        c.blocksRaycasts = false;
        return c.DOFade(0, 0.5f * Settings.AnimationSpeed);
    }

    #endregion
}
