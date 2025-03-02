using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class InventoryCanvas : MonoBehaviour
{
    public enum InventoryTabs {
        Other,
        Armor,
        Abilities,
        Skills,
        Quests
    }

    [Header("Tab Buttons")]
    [SerializeField] private GameObject _otherTabGO;
    [SerializeField] private GameObject _armorTabGO;
    [SerializeField] private GameObject _abilitiesTabGO;
    [SerializeField] private GameObject _skillsTabGO;
    [SerializeField] private GameObject _questsTabGO;

    [Header("Panel Game Objects")]
    [SerializeField] private GameObject _otherInventoryPanelGO;
    [SerializeField] private GameObject _armorInventoryPanelGO;
    [SerializeField] private GameObject _playerInventoryPanelGO;

    private GameObject[] _tabButtonGameObjects;
    private Button[] _tabButtons;
    private Image[] _tabImages;
    private TextMeshProUGUI[] _tabTexts;
    private Dictionary<InventoryTabs, int> _tabPanelsIndex;

    private GameObject _currentTabPanel;
    private Button _currentTabButton;
    private Image _currentTabImage;
    private TextMeshProUGUI _currentTabText;
    private InventoryTabs _currentTab;

    private UICanvas _uiCanvas;

    private Color _defaultColor = new(102 / 255, 102 / 255, 102 / 255);

    #region Unity Methods

    private void Awake() {
    }

    void Start() {
        _uiCanvas = UICanvas.Instance;
        _tabButtonGameObjects = new GameObject[] {
            _otherTabGO,
            _armorTabGO,
            _abilitiesTabGO,
            _skillsTabGO,
            _questsTabGO
        };
        _tabButtons = new Button[] {
            _otherTabGO.GetComponent<Button>(),
            _armorTabGO.GetComponent<Button>(),
            _abilitiesTabGO.GetComponent<Button>(),
            _skillsTabGO.GetComponent<Button>(),
            _questsTabGO.GetComponent<Button>()
        };
        _tabImages = new Image[] {
            _otherTabGO.GetComponent<Image>(),
            _armorTabGO.GetComponent<Image>(),
            _abilitiesTabGO.GetComponent<Image>(),
            _skillsTabGO.GetComponent<Image>(),
            _questsTabGO.GetComponent<Image>()
        };
        _tabTexts = new TextMeshProUGUI[] {
            _otherTabGO.GetComponentInChildren<TextMeshProUGUI>(),
            _armorTabGO.GetComponentInChildren<TextMeshProUGUI>(),
            _abilitiesTabGO.GetComponentInChildren<TextMeshProUGUI>(),
            _skillsTabGO.GetComponentInChildren<TextMeshProUGUI>(),
            _questsTabGO.GetComponentInChildren<TextMeshProUGUI>()
        };

        _tabPanelsIndex = new Dictionary<InventoryTabs, int> {
            { InventoryTabs.Other, 0 },
            { InventoryTabs.Armor, 1 },
            { InventoryTabs.Abilities, 2 },
            { InventoryTabs.Skills, 3 },
            { InventoryTabs.Quests, 4 }
        };
    }

    #endregion

    #region Public Methods

    public void SetCurrentTab(InventoryTabs tab) {
        _currentTab = tab;
        _currentTabPanel = _tabButtonGameObjects[_tabPanelsIndex[_currentTab]];
        _currentTabButton = _tabButtons[_tabPanelsIndex[_currentTab]];
        _currentTabImage = _tabImages[_tabPanelsIndex[_currentTab]];
        _currentTabText = _tabTexts[_tabPanelsIndex[_currentTab]];

        foreach (InventoryTabs t in _tabPanelsIndex.Keys) {
            if (t == _currentTab) {
                highlightTab(t, true);
            } else {
                highlightTab(t, false);
            }
        }

        switch (_currentTab) {
            case InventoryTabs.Other:
                armorTabExit().OnComplete(() => otherTabEnter());
                playerTabEnter();
                break;
            case InventoryTabs.Armor:
                otherTabExit().OnComplete(() => armorTabEnter());
                playerTabEnter();
                break;
        }
    }

    public Tween OtherTabExit() {
        return otherTabExit();
    }
    public Tween OtherTabEnter() {
        return otherTabEnter();
    }

    public void OnBackgroundClick() {
        _uiCanvas.DropItem();
    }

    public void OnOpenOtherTab() { SetCurrentTab(InventoryTabs.Other); }

    public void OnOpenArmorTab() { SetCurrentTab(InventoryTabs.Armor); }

    public void OnOpenAbilitiesTab() { SetCurrentTab(InventoryTabs.Abilities); }

    public void OnOpenSkillsTab() { SetCurrentTab(InventoryTabs.Skills); }

    public void OnOpenQuestsTab() { SetCurrentTab(InventoryTabs.Quests); }
    

    #endregion

    #region Private Methods

    private void highlightTab(InventoryTabs tab, bool value) {
        if (value) {
            _tabImages[_tabPanelsIndex[tab]].DOColor(Color.white, 0.15f);
            _tabButtonGameObjects[_tabPanelsIndex[tab]].transform.DOScaleY(1.2f, 0.15f);
        } else {
            _tabImages[_tabPanelsIndex[tab]].DOColor(_defaultColor, 0.15f);
            _tabButtonGameObjects[_tabPanelsIndex[tab]].transform.DOScaleY(1f, 0.15f);
        }
    }

    private Tween playerTabEnter() {
        _playerInventoryPanelGO.SetActive(true);
        return _playerInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(-10, 0.25f);
    }

    private Tween playerTabExit() {
        return _playerInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(410, 0.25f).OnComplete(() => _playerInventoryPanelGO.SetActive(false));
    }

    private Tween otherTabEnter() {
        _otherInventoryPanelGO.SetActive(true);
        return _otherInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(10, 0.25f);
    }

    private Tween otherTabExit() {
        return _otherInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(-410, 0.25f).OnComplete(() => _otherInventoryPanelGO.SetActive(false));
    }

    private Tween armorTabEnter() {
        _armorInventoryPanelGO.SetActive(true);
        return _armorInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(10, 0.25f);
    }

    private Tween armorTabExit() {
        return _armorInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(-580, 0.25f).OnComplete(() => _armorInventoryPanelGO.SetActive(false));
    }

    #endregion
}
