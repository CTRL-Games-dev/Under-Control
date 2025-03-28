using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using System;

public class InventoryCanvas : MonoBehaviour
{
    public enum InventoryTabs {
        Other,
        Armor,
        Cards,
        Evo,
        Quests
    }

    private IInteractableInventory _lastInteractableInventory;
    private ItemContainer _currentOtherInventory;
    [SerializeField] private GameObject _otherInventoryHolder;
    [SerializeField] private RectTransform _underlineRect;
    [SerializeField] private GameObject _armorBtnTabGO, _cardsBtnTabGO, _evoBtnTabGO, _questsBtnTabGO, _otherBtnTabGO;

    [Header("Panel Game Objects")]
    [SerializeField] private GameObject _playerInventoryPanelGO;
    [SerializeField] private GameObject _armorInventoryPanelGO, _cardsInventoryPanelGO, _evoInventoryPanelGO, _questsInventoryPanelGO, _otherInventoryPanelGO;

    [Header("Armor panel parts")]
    [SerializeField] private GameObject _playerPreviewGO;
    [SerializeField] private GameObject[] _armorSlotsGO = new GameObject[7];
    private RectTransform[] _armorSlotsRects = new RectTransform[7];
    private CanvasGroup[] _armorSlotsCanvasGroups = new CanvasGroup[7];

    [Header("Evolution panel parts")]
    [SerializeField] private GameObject _evoPointsGO;
    [SerializeField] private RectTransform _pointsHolderRect;


    private GameObject[] _tabButtonGameObjects;
    private Button[] _tabButtons;
    private Image[] _tabImages;
    private TextMeshProUGUI[] _tabTexts;
    private RectTransform[] _tabRectTranforms;
    private Dictionary<InventoryTabs, int> _tabPanelsIndex;

    private CanvasGroup _canvasGroup;
    private GameObject _currentTabPanel;
    private Button _currentTabButton;
    private Image _currentTabImage;
    private TextMeshProUGUI _currentTabText;
    private InventoryTabs _currentTab = InventoryTabs.Armor;

    private Color _defaultColor = Color.white;

    #region Unity Methods

    void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
        _tabButtonGameObjects = new GameObject[] {
            _armorBtnTabGO,
            _cardsBtnTabGO,
            _evoBtnTabGO,
            _questsBtnTabGO,
            _otherBtnTabGO
        };
        _tabButtons = new Button[] {
            _armorBtnTabGO.GetComponent<Button>(),
            _cardsBtnTabGO.GetComponent<Button>(),
            _evoBtnTabGO.GetComponent<Button>(),
            _questsBtnTabGO.GetComponent<Button>(),
            _otherBtnTabGO.GetComponent<Button>()
        };
        _tabImages = new Image[] {
            _armorBtnTabGO.GetComponent<Image>(),
            _cardsBtnTabGO.GetComponent<Image>(),
            _evoBtnTabGO.GetComponent<Image>(),
            _questsBtnTabGO.GetComponent<Image>(),
            _otherBtnTabGO.GetComponent<Image>(),
        };
        _tabTexts = new TextMeshProUGUI[] {
            _armorBtnTabGO.GetComponentInChildren<TextMeshProUGUI>(),
            _cardsBtnTabGO.GetComponentInChildren<TextMeshProUGUI>(),
            _evoBtnTabGO.GetComponentInChildren<TextMeshProUGUI>(),
            _questsBtnTabGO.GetComponentInChildren<TextMeshProUGUI>(),
            _otherBtnTabGO.GetComponentInChildren<TextMeshProUGUI>()
        };
        _tabRectTranforms = new RectTransform[] {
            _armorBtnTabGO.GetComponent<RectTransform>(),
            _cardsBtnTabGO.GetComponent<RectTransform>(),
            _evoBtnTabGO.GetComponent<RectTransform>(),
            _questsBtnTabGO.GetComponent<RectTransform>(),
            _otherBtnTabGO.GetComponent<RectTransform>(),
        };

        _tabPanelsIndex = new Dictionary<InventoryTabs, int> {
            { InventoryTabs.Armor, 0 },
            { InventoryTabs.Cards, 1 },
            { InventoryTabs.Evo, 2 },
            { InventoryTabs.Quests, 3 },
            { InventoryTabs.Other, 4 }
        };
    }

    void Start() {
        _currentTab = InventoryTabs.Armor;
        
        foreach (GameObject go in _armorSlotsGO) {
            _armorSlotsRects[Array.IndexOf(_armorSlotsGO, go)] = go.GetComponent<RectTransform>();
            _armorSlotsCanvasGroups[Array.IndexOf(_armorSlotsGO, go)] = go.GetComponent<CanvasGroup>();
        }
    }

    #endregion

    #region Public Methods

    public void HideUI() {
        CloseAllTabs();
        
        _canvasGroup.DOFade(0, 0.25f);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        SetOtherInventory(null, null);
    }

    public void ShowUI() {
        gameObject.SetActive(true);
        _canvasGroup.DOComplete();
        _underlineRect.DOAnchorPosX(_tabRectTranforms[_tabPanelsIndex[_currentTab]].anchoredPosition.x, 0);
        SetCurrentTab(_currentTab);
        _canvasGroup.DOFade(1, 0.25f);
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    public void SetCurrentTab(InventoryTabs tab) {
        InventoryTabs previousTab = _currentTab;
        _currentTab = tab;
        _currentTabPanel = _tabButtonGameObjects[_tabPanelsIndex[_currentTab]];
        _currentTabButton = _tabButtons[_tabPanelsIndex[_currentTab]];
        _currentTabImage = _tabImages[_tabPanelsIndex[_currentTab]];
        _currentTabText = _tabTexts[_tabPanelsIndex[_currentTab]];
        _underlineRect.DOAnchorPosX(_tabRectTranforms[_tabPanelsIndex[_currentTab]].anchoredPosition.x, 0.25f).SetEase(Ease.OutCubic);

        foreach (InventoryTabs t in _tabPanelsIndex.Keys) {
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
            case InventoryTabs.Other:
                if (!(_currentTab == InventoryTabs.Other || _currentTab == InventoryTabs.Armor)) playerTabExit();
                otherTabExit().OnComplete(() => openTab(_currentTab));
                break;
            case InventoryTabs.Armor:
                if (!(_currentTab == InventoryTabs.Other || _currentTab == InventoryTabs.Armor)) playerTabExit();
                armorTabExit().OnComplete(() => openTab(_currentTab));
                break;
            case InventoryTabs.Cards:
                cardsTabExit().OnComplete(() => openTab(_currentTab));
                break;
            case InventoryTabs.Evo:
                evoTabExit().OnComplete(() => openTab(_currentTab));
                break;
            case InventoryTabs.Quests:
                questsTabExit().OnComplete(() => openTab(_currentTab));
                break;
        }
    }

    private void openTab(InventoryTabs tab) {
        switch (tab) {
            case InventoryTabs.Other:
                otherTabEnter();
                playerTabEnter();
                break;
            case InventoryTabs.Armor:
                armorTabEnter();
                playerTabEnter();
                break;
            case InventoryTabs.Cards:
                cardsTabEnter();
                break;
            case InventoryTabs.Evo:
                evoTabEnter();
                break;
            case InventoryTabs.Quests:
                questsTabEnter();
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
        questsTabExit();
        otherTabExit();
    }

    public void KillAllTweens() {
        _otherInventoryPanelGO.GetComponent<RectTransform>().DOKill();
        _armorInventoryPanelGO.GetComponent<RectTransform>().DOKill();
        _cardsInventoryPanelGO.GetComponent<RectTransform>().DOKill();
        _evoInventoryPanelGO.GetComponent<RectTransform>().DOKill();
        _questsInventoryPanelGO.GetComponent<RectTransform>().DOKill();
        _playerInventoryPanelGO.GetComponent<RectTransform>().DOKill();
    }

    public void SetOtherInventory(ItemContainer itemContainer, GameObject prefab, IInteractableInventory interactable = null, string title = null) {
        if (_currentOtherInventory == itemContainer) return;
        _currentTab = itemContainer == null ? InventoryTabs.Armor : InventoryTabs.Other;

        _lastInteractableInventory?.EndInteract();
        _lastInteractableInventory = interactable;

        _currentOtherInventory = itemContainer;
        SetOtherTabTitle(title);

        if (itemContainer != null) { // animacja zmiany
            if (_otherInventoryHolder.transform.childCount > 0)
                Destroy(_otherInventoryHolder.transform.GetChild(0).gameObject);
            
            InventoryPanel inventoryPanel = Instantiate(prefab, _otherInventoryHolder.transform).GetComponentInChildren<InventoryPanel>();
            inventoryPanel.SetTargetInventory(itemContainer);
        } else {
            if (_otherInventoryHolder.transform.childCount > 0)
                Destroy(_otherInventoryHolder.transform.GetChild(0).gameObject);
        }
    }

    public void SetOtherTabTitle(string text) {
        _tabTexts[_tabPanelsIndex[InventoryTabs.Other]].text = text ?? "";
        _tabTexts[_tabPanelsIndex[InventoryTabs.Other]].DOColor(text == null ? Color.gray : Color.white, 0.15f);
        _tabButtonGameObjects[_tabPanelsIndex[InventoryTabs.Other]].GetComponent<Button>().interactable = text != null;
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
            rect.DOScale(0.9f, 0.2f).SetEase(Ease.OutCubic).OnComplete(() => {
                rect.DOScale(1, 0.2f).SetDelay(0.4f).SetEase(Ease.OutCubic);
            });
        }
        _pointsHolderRect.DOAnchorPosY(Player.Instance.EvolutionPoints * 57, 0.4f).SetDelay(0.2f).SetEase(Ease.InOutBack);
    }


    public void OnOpenOtherTab() { SetCurrentTab(InventoryTabs.Other); }

    public void OnOpenArmorTab() { SetCurrentTab(InventoryTabs.Armor); }

    public void OnOpenCardsTab() { SetCurrentTab(InventoryTabs.Cards); }

    public void OnOpenEvoTab() { SetCurrentTab(InventoryTabs.Evo); }

    public void OnOpenQuestsTab() { SetCurrentTab(InventoryTabs.Quests); }
    
    public void HighlightButton(int index) {
        if (index == _tabPanelsIndex[_currentTab]) return;
        _tabButtonGameObjects[index].transform.DOScale(1.1f, 0.75f);
    }

    public void UnhighlightButton(int index) {
        if (index == _tabPanelsIndex[_currentTab]) return;
        _tabButtonGameObjects[index].transform.DOScale(1f, 0.75f);
    }
    #endregion

    #region Private Methods

    private void highlightTab(InventoryTabs tab, bool value) {
        _tabButtonGameObjects[_tabPanelsIndex[tab]].transform.DOKill();
        if (value) {
            _tabButtonGameObjects[_tabPanelsIndex[tab]].transform.DOScale(1.2f, 0.35f);
            _tabTexts[_tabPanelsIndex[tab]].fontStyle = FontStyles.Bold;
        } else {
            _tabButtonGameObjects[_tabPanelsIndex[tab]].transform.DOScale(1, 0.35f);
            _tabTexts[_tabPanelsIndex[tab]].fontStyle = FontStyles.Normal;
        } 
    }

    private Tween playerTabEnter() {
        _playerInventoryPanelGO.SetActive(true);
        return _playerInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(-180, 0.5f).SetEase(Ease.OutExpo);
    }

    private Tween playerTabExit() {
        return _playerInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(470, 0.5f).SetEase(Ease.OutExpo);
    }

    private Tween otherTabEnter() {
        _otherInventoryPanelGO.SetActive(true);
        return _otherInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(120, 0.5f).SetEase(Ease.OutExpo);
    }

    private Tween otherTabExit() {
        return _otherInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(-760, 0.5f).SetEase(Ease.OutExpo);
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

        _armorSlotsRects[0].DOScale(1, scaleSpeed).SetEase(Ease.OutBack);
        
        _armorSlotsCanvasGroups[0].DOFade(1, fadeSpeed).OnComplete(() => {
            _armorSlotsRects[1].DOScale(1, scaleSpeed).SetEase(Ease.OutBack);
            _armorSlotsCanvasGroups[1].DOFade(1, fadeSpeed).OnComplete(() => {
                _armorSlotsRects[2].DOScale(1, scaleSpeed).SetEase(Ease.OutBack);
                _armorSlotsCanvasGroups[2].DOFade(1, fadeSpeed).OnComplete(() => {
                    _armorSlotsRects[3].DOScale(1, scaleSpeed).SetEase(Ease.OutBack);
                    _armorSlotsCanvasGroups[3].DOFade(1, fadeSpeed).OnComplete(() => {
                        _armorSlotsRects[4].DOScale(1, scaleSpeed).SetEase(Ease.OutBack);
                        _armorSlotsCanvasGroups[4].DOFade(1, fadeSpeed).OnComplete(() => {
                            _armorSlotsRects[5].DOScale(1, scaleSpeed).SetEase(Ease.OutBack);
                            _armorSlotsCanvasGroups[5].DOFade(1, fadeSpeed).OnComplete(() => {
                                _armorSlotsRects[6].DOScale(1, scaleSpeed).SetEase(Ease.OutBack);
                                _armorSlotsCanvasGroups[6].DOFade(1, fadeSpeed);
                            });
                        });
                    });
                });
            });
        });
        
        return _armorInventoryPanelGO.transform.DOScale(1, fadeSpeed * 7);
    }

    private Tween armorTabExit() {
        _playerInventoryPanelGO.GetComponent<RawImage>().DOComplete();
        for (int i = 0; i < _armorSlotsGO.Length; i++) {
            _armorSlotsCanvasGroups[i].DOKill();
            _armorSlotsRects[i].DOKill();
        }
        
        float fadeSpeed = 0.3f;
        float scaleSpeed = 0.05f;

        _playerPreviewGO.GetComponent<RawImage>().DOFade(0, 0.5f);
        _armorSlotsCanvasGroups[0].DOFade(0, fadeSpeed);
        _armorSlotsRects[0].DOScale(0, scaleSpeed).SetEase(Ease.InBack).OnComplete(() => {
            _armorSlotsCanvasGroups[1].DOFade(0, fadeSpeed);
            _armorSlotsRects[1].DOScale(0, scaleSpeed).SetEase(Ease.InBack).OnComplete(() => {
                _armorSlotsCanvasGroups[2].DOFade(0, fadeSpeed);
                _armorSlotsRects[2].DOScale(0, scaleSpeed).SetEase(Ease.InBack).OnComplete(() => {
                    _armorSlotsCanvasGroups[3].DOFade(0, fadeSpeed);
                    _armorSlotsRects[3].DOScale(0, scaleSpeed).SetEase(Ease.InBack).OnComplete(() => {
                        _armorSlotsCanvasGroups[4].DOFade(0, fadeSpeed);
                        _armorSlotsRects[4].DOScale(0, scaleSpeed).SetEase(Ease.InBack).OnComplete(() => {
                            _armorSlotsCanvasGroups[5].DOFade(0, fadeSpeed);
                            _armorSlotsRects[5].DOScale(0, scaleSpeed).SetEase(Ease.InBack).OnComplete(() => {
                                _armorSlotsCanvasGroups[6].DOFade(0, fadeSpeed);
                                _armorSlotsRects[6].DOScale(0, scaleSpeed).SetEase(Ease.InBack);
                            });
                        });
                    });
                });
            });
        });        

        return _armorInventoryPanelGO.transform.DOScale(1, scaleSpeed * 7);
    }

    private Tween cardsTabEnter() {
        _cardsInventoryPanelGO.SetActive(true);
        return _cardsInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DY(-1000, 0.25f);
    }

    private Tween cardsTabExit() {
        return _cardsInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DY(0, 0.25f).OnComplete(() => _cardsInventoryPanelGO.SetActive(false));
    }

    private Tween evoTabEnter() {
        _evoInventoryPanelGO.SetActive(true);
        return _evoInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DY(-1000, 0.25f);
    }

    private Tween evoTabExit() {
        return _evoInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DY(0, 0.25f).OnComplete(() => _evoInventoryPanelGO.SetActive(false));
    }

    private Tween questsTabEnter() {
        _questsInventoryPanelGO.SetActive(true);
        return _questsInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DY(-630, 0.25f);
    }

    private Tween questsTabExit() {
        return _questsInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DY(0, 0.25f).OnComplete(() => _questsInventoryPanelGO.SetActive(false));
    }

    #endregion
}
