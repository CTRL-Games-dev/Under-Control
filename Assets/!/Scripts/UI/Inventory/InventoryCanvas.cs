using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryCanvas : MonoBehaviour
{
    private enum InventoryTabs {
        Other,
        Armor,
        Abilities,
        Skills,
        Quests
    }

    [SerializeField] private GameObject _otherTabGO;
    [SerializeField] private GameObject _armorTabGO;
    [SerializeField] private GameObject _abilitiesTabGO;
    [SerializeField] private GameObject _skillsTabGO;
    [SerializeField] private GameObject _questsTabGO;

    private GameObject[] _tabButtonGameObjects;
    private Button[] _tabButtons;
    private Image[] _tabImages;
    private TextMeshProUGUI[] _tabTexts;
    private Dictionary<InventoryTabs, int> _tabPanelsIndex;

    [SerializeField] private InventoryTabs _currentPage;
    [SerializeField] private GameObject _currentPagePanel;
    [SerializeField] private Button _currentPageButton;
    [SerializeField] private Image _currentPageImage;
    [SerializeField] private TextMeshProUGUI _currentPageText;

    private UICanvas _uiCanvas;

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

    private void SetCurrentTab(InventoryTabs tab) {
        _currentPage = tab;
        _currentPagePanel = _tabButtonGameObjects[_tabPanelsIndex[_currentPage]];
        _currentPageButton = _tabButtons[_tabPanelsIndex[_currentPage]];
        _currentPageImage = _tabImages[_tabPanelsIndex[_currentPage]];
        _currentPageText = _tabTexts[_tabPanelsIndex[_currentPage]];
    }

    public void OnBackgroundClick() {
        _uiCanvas.DropItem();
    }

    public void OnOpenOtherTab() {
        _currentPage = InventoryTabs.Other;
        
    }

    public void OnOpenArmorTab() {
        _currentPage = InventoryTabs.Armor;
        // _currentPageButton = _armorTabGO;
    }

    public void OnOpenAbilitiesTab() {
        _currentPage = InventoryTabs.Abilities;
        // _currentPageButton = _abilitiesTabGO;
    }

    #endregion

    #region Private Methods
    


    private void highlightCurrentTab() {
        foreach (Button button in _tabButtons) {
            button.interactable = true;
        }

        _currentPageButton.interactable = false;
    }

    #endregion
}
