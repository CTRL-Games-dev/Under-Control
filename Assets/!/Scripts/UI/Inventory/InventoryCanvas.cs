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
    [SerializeField] private GameObject _abilitiesInventoryPanelGO;
    [SerializeField] private GameObject _skillsInventoryPanelGO;
    [SerializeField] private GameObject _questsInventoryPanelGO;
    [SerializeField] private GameObject _playerInventoryPanelGO;

    private GameObject[] _tabButtonGameObjects;
    private Button[] _tabButtons;
    private Image[] _tabImages;
    private TextMeshProUGUI[] _tabTexts;
    private RectTransform[] _tabRectTranforms;
    private Dictionary<InventoryTabs, int> _tabPanelsIndex;

    private GameObject _currentTabPanel;
    private Button _currentTabButton;
    private Image _currentTabImage;
    private TextMeshProUGUI _currentTabText;
    private InventoryTabs _currentTab;

    private UICanvas _uiCanvas;

    private Color _defaultColor = new(102 / 255, 102 / 255, 102 / 255);

    #region Unity Methods


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
        _tabRectTranforms = new RectTransform[] {
            _otherTabGO.GetComponent<RectTransform>(),
            _armorTabGO.GetComponent<RectTransform>(),
            _abilitiesTabGO.GetComponent<RectTransform>(),
            _skillsTabGO.GetComponent<RectTransform>(),
            _questsTabGO.GetComponent<RectTransform>()
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
        // KillAllTweens(); TODO
 

        switch (_currentTab) {
            case InventoryTabs.Other:
                abilitiesTabExit();
                skillsTabExit();
                questsTabExit();
                armorTabExit().OnComplete(() => otherTabEnter());
                playerTabEnter();
                break;
            case InventoryTabs.Armor:
                abilitiesTabExit();
                skillsTabExit();
                questsTabExit();
                otherTabExit().OnComplete(() => armorTabEnter());
                playerTabEnter();
                break;
            case InventoryTabs.Abilities:
                playerTabExit();
                armorTabExit();
                skillsTabExit();
                questsTabExit();
                otherTabExit().OnComplete(() => abilitiesTabEnter());
                break;
            case InventoryTabs.Skills:
                playerTabExit();
                armorTabExit();
                abilitiesTabExit();
                questsTabExit();
                otherTabExit().OnComplete(() => skillsTabEnter());
                break;
            case InventoryTabs.Quests:
                playerTabExit();
                armorTabExit();
                abilitiesTabExit();
                skillsTabExit();
                otherTabExit().OnComplete(() => questsTabEnter());
                break;
        }
    }

    public void OpenCurrentTab() {
        SetCurrentTab(_currentTab);
    }

    public void CloseAllTabs() {
        playerTabExit();
        armorTabExit();
        abilitiesTabExit();
        skillsTabExit();
        questsTabExit();
        otherTabExit();
    }

    public void KillAllTweens() {
        _otherInventoryPanelGO.GetComponent<RectTransform>().DOKill();
        _armorInventoryPanelGO.GetComponent<RectTransform>().DOKill();
        _abilitiesInventoryPanelGO.GetComponent<RectTransform>().DOKill();
        _skillsInventoryPanelGO.GetComponent<RectTransform>().DOKill();
        _questsInventoryPanelGO.GetComponent<RectTransform>().DOKill();
        _playerInventoryPanelGO.GetComponent<RectTransform>().DOKill();
    }

    public void SetOtherTabTitle(string text) {
        _tabTexts[_tabPanelsIndex[InventoryTabs.Other]].text = text ?? "-";
        _tabTexts[_tabPanelsIndex[InventoryTabs.Other]].DOColor(text == null ? Color.gray : Color.white, 0.15f);
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
        // if (_otherInventoryPanelGO.GetComponent<RectTransform>().anchoredPosition.x < 0) return DOTween.Sequence().AppendInterval(0).AppendCallback(() => { });
        return _otherInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(-410, 0.25f).OnComplete(() => _otherInventoryPanelGO.SetActive(false));
    }

    private Tween armorTabEnter() {
        _armorInventoryPanelGO.SetActive(true);
        return _armorInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(10, 0.25f);
    }

    private Tween armorTabExit() {
        // if (_armorInventoryPanelGO.GetComponent<RectTransform>().anchoredPosition.x < 0) return DOTween.Sequence().AppendInterval(0).AppendCallback(() => { });
        return _armorInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DX(-580, 0.25f).OnComplete(() => _armorInventoryPanelGO.SetActive(false));
    }

    private Tween abilitiesTabEnter() {
        _abilitiesInventoryPanelGO.SetActive(true);
        return _abilitiesInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DY(-630, 0.25f);
    }

    private Tween abilitiesTabExit() {
        return _abilitiesInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DY(0, 0.25f).OnComplete(() => _abilitiesInventoryPanelGO.SetActive(false));
    }

    private Tween skillsTabEnter() {
        _skillsInventoryPanelGO.SetActive(true);
        return _skillsInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DY(-630, 0.25f);
    }

    private Tween skillsTabExit() {
        return _skillsInventoryPanelGO.GetComponent<RectTransform>().DOAnchorPos3DY(0, 0.25f).OnComplete(() => _skillsInventoryPanelGO.SetActive(false));
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
