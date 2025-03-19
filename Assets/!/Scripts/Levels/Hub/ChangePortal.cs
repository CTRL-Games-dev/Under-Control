using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine.UI;
using TMPro;

public class ChangePortal : MonoBehaviour, IInteractable
{
    [Header("Portal things")]
    [SerializeField] private Portal _portal;
    [SerializeField] private Renderer _ballRenderer;
    [SerializeField] private CinemachineCamera _ballCamera;
    [SerializeField] private Material _portalMaterial;

    [Header("UI things")]
    [SerializeField] private GameObject _ui;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _dimensionHolder;
    [SerializeField] private RectTransform _spaceBGRect;

    [Header("Right Panel")]
    [SerializeField] private GameObject _rightPanel;
    [SerializeField] private RectTransform _rightPanelBGRect;
    [SerializeField] private GameObject _whitePanelGO;
    [SerializeField] private GameObject _itemDisplayPrefab;
    [SerializeField] private Transform _itemDisplayParent;
    [SerializeField] private GameObject _chooseBtn;
    [SerializeField] private TextMeshProUGUI _dimensionName, _dimensionDescription, _dimensionType, _dimensionDifficulty, _dimensionVekhtarControl, _availableItems;

    private CanvasGroup _rightPanelCanvasGroup;


    private bool _opened;
    public bool Locked = false;
    private UIDimension _currentDimension;
    public static ChangePortal Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        _rightPanelCanvasGroup = _rightPanel.GetComponent<CanvasGroup>();
    }

    private void Start() {
        UICanvas.Instance.PlayerController.UICancelEvent.AddListener(CloseUI);   
    }

    void FixedUpdate() {
        float xOffset = Mathf.Sin(Time.time) * 0.2f;
        float yOffset = Mathf.Cos(Time.time) * 0.2f;
        _ballRenderer.material.mainTextureOffset = new Vector2(xOffset, yOffset);
        _spaceBGRect.rotation = Quaternion.Euler(0, 0, Mathf.PingPong(Time.time, 10));
    }

    public void Interact(PlayerController player)
    {
        if(_opened) CloseUI();
        else OpenUI();
    }
    public void OpenUI()
    {
        UICanvas.Instance.IsOtherUIOpen = true;
        CameraManager.Instance.SwitchCamera(_ballCamera);
        _ui.SetActive(true);
        _canvasGroup.DOFade(1, 1).SetDelay(1);
        _opened = true;
    }

    public void CloseUI()
    {
        CameraManager.Instance.SwitchCamera(UICanvas.Instance.PlayerController.PlayerTopDownCamera);
        _canvasGroup.DOFade(0, 1).OnComplete(() => {
            UICanvas.Instance.IsOtherUIOpen = false;
            _ui.SetActive(false);
        });
        _opened = false;
    }

    public void SetPortal()
    {
        Invoke(nameof(CloseUI), 1f);
    }

    public void SetDimensionInfo(UIDimension dimension) {
        if (Locked) return;
        _rightPanelCanvasGroup.DOComplete();
        if (dimension == null) {
            _whitePanelGO.SetActive(false);
            _rightPanelCanvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutSine);
            return;    
        }
        // if (_currentDimension == dimension) return;
        _currentDimension = dimension;
        _portal.SetDimensionAndActivate(_currentDimension.WhatDimension);
        _portalMaterial.DOColor(_currentDimension.Color, 0.3f);

        _whitePanelGO.SetActive(false);
        setupRightPanel(_currentDimension);
        _rightPanelCanvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutSine).OnComplete(() => {
            _whitePanelGO.SetActive(true);
        });
    
    }

    private void setupRightPanel(UIDimension dimension) {
        _dimensionName.text = dimension.Name;
        _dimensionDescription.text = dimension.Description;
        _dimensionType.text = dimension.Type.ToString() + " Dimension";
        _dimensionDifficulty.text = "Difficulty: " + dimension.Difficulty.ToString();
        _dimensionVekhtarControl.text = "Vekhtar Control: " + dimension.VekhtarControl.ToString();
        _availableItems.text = dimension.AvaliableItems.Count == 0 ? "" : "Available Items:";
        foreach (Transform child in _itemDisplayParent) {
            Destroy(child.gameObject);
        }
        foreach (ItemData item in dimension.AvaliableItems) {
            GameObject itemDisplay = Instantiate(_itemDisplayPrefab, _itemDisplayParent);
            itemDisplay.GetComponentInChildren<TextMeshProUGUI>().text = item.DisplayName;
            itemDisplay.GetComponentInChildren<Image>().sprite = item.Icon;
        }
    }


    public void LockDimension(UIDimension dimension) {
        Locked = !Locked;
        Vector3 newSize = Locked ? _rightPanelBGRect.sizeDelta - new Vector2(12, 12) : _rightPanelBGRect.sizeDelta + new Vector2(12, 12);
        _rightPanelBGRect.DOSizeDelta(newSize, 0.3f).SetEase(Ease.OutSine);
        SetDimensionInfo(dimension);
    }


    public void OnTravelButtonClick() {
        Debug.Log("Traveling to " + _currentDimension.Name);
    }
    
}