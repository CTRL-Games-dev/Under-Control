using System.Diagnostics;
using DG.Tweening;
using Unity.AppUI.UI;
using UnityEngine;

public class SellerPanels : MonoBehaviour
{
    public enum PanelType
    {
        Buy,
        Sell,
        Craft,
        None
    }


    [SerializeField] private InventoryPanel _buyPanel;
    [SerializeField] private InventoryPanel _sellPanel;
    [SerializeField] private CanvasGroup _buyCanvasGroup, _sellCanvasGroup, _craftCanvasGroup;
    [SerializeField] private RectTransform _buyPanelRect, _sellPanelRect, _craftPanelRect;
    [SerializeField] private CanvasGroup _buyBtnCanvasGroup, _sellBtnCanvasGroup, _craftBtnCanvasGroup;
    [SerializeField] private RectTransform _buyBtnRect, _sellBtnRect, _craftBtnRect;
    [SerializeField] private CraftPanel _craftPanel;
    
    private Seller _seller;
    private PanelType _currentPanelType = PanelType.None;

    private float _highlitedX = 25f, _selectedX = 50f;


    public void SetSeller(Seller seller) {
        if (seller == null) {
            SwitchSellerPanel(PanelType.None);
            _seller = seller;
        } else {
            _seller = seller;
            SwitchSellerPanel(PanelType.Buy);
        }
        _buyPanel.SetTargetInventory(seller == null ? null : seller.BuyInventory);
        _sellPanel.SetTargetInventory(seller == null ? null : seller.SellInventory);
    }

    public void ShowButtons() {
        _buyBtnCanvasGroup.DOKill();
        _sellBtnCanvasGroup.DOKill();
        _craftBtnCanvasGroup.DOKill();
        _buyBtnRect.DOKill();
        _sellBtnRect.DOKill();
        _craftBtnRect.DOKill();
        
        _buyBtnRect.DOAnchorPosX(0, 0.5f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
        _buyBtnCanvasGroup.DOFade(1f, 0.2f * Settings.AnimationSpeed).OnComplete(() => {
            _buyBtnCanvasGroup.interactable = true;
            _buyBtnCanvasGroup.blocksRaycasts = true;

            _sellBtnRect.DOAnchorPosX(0, 0.5f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
            _sellBtnCanvasGroup.DOFade(1f, 0.2f * Settings.AnimationSpeed).OnComplete(() => {
                _sellBtnCanvasGroup.interactable = true;
                _sellBtnCanvasGroup.blocksRaycasts = true;

                _craftBtnRect.DOAnchorPosX(0, 0.5f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
                _craftBtnCanvasGroup.DOFade(1f, 0.2f * Settings.AnimationSpeed).OnComplete(() => {
                    _craftBtnCanvasGroup.interactable = true;
                    _craftBtnCanvasGroup.blocksRaycasts = true;
                    
                    RectTransform rect = getPanelBtnRect(_currentPanelType);
                    rect.DOKill();
                    rect.DOAnchorPosX(_selectedX, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
                });
            });
        });

        
    }

    public void HideButtons() {
        _buyBtnCanvasGroup.DOKill();
        _sellBtnCanvasGroup.DOKill();
        _craftBtnCanvasGroup.DOKill();
        _buyBtnRect.DOKill();
        _sellBtnRect.DOKill();
        _craftBtnRect.DOKill();
        _buyBtnCanvasGroup.interactable = false;
        _buyBtnCanvasGroup.blocksRaycasts = false;
        _sellBtnCanvasGroup.interactable = false;
        _sellBtnCanvasGroup.blocksRaycasts = false;
        _craftBtnCanvasGroup.interactable = false;
        _craftBtnCanvasGroup.blocksRaycasts = false;
        
        _buyBtnRect.DOAnchorPosX(-300, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
        _buyBtnCanvasGroup.DOFade(0f, 0.1f * Settings.AnimationSpeed).OnComplete(() => {
            _sellBtnRect.DOAnchorPosX(-300, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
            _sellBtnCanvasGroup.DOFade(0f, 0.1f * Settings.AnimationSpeed).OnComplete(() => {
                _craftBtnRect.DOAnchorPosX(-300, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
                _craftBtnCanvasGroup.DOFade(0f, 0.1f * Settings.AnimationSpeed);
            });
        });
    }

    public void OnBtnPointerEnter(int index) {
        PanelType panel = (PanelType)index;
        if (panel == _currentPanelType) return;
        
        RectTransform rect = getPanelBtnRect(panel);
        rect.DOKill();
        rect.DOScaleY(1.05f, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
        rect.DOAnchorPosX(_highlitedX, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
    }

    public void OnBtnPointerExit(int index) {
        PanelType panel = (PanelType)index;
        if (panel == _currentPanelType) return;
        
        RectTransform rect = getPanelBtnRect(panel);
        rect.DOKill();
        rect.DOAnchorPosX(0, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
        rect.DOScaleY(1f, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
    }


    public void SwitchSellerPanel(PanelType panelType) {
        if (panelType == _currentPanelType) {
            return;
        }
        closeSellerPanel(_currentPanelType);
        _currentPanelType = panelType;
        openSellerPanel(panelType);
    }

    private void closeSellerPanel(PanelType panelType) {
        switch (panelType) {
            case PanelType.Buy:
                _buyCanvasGroup.DOKill();
                _buyPanelRect.DOKill();
                _buyBtnRect.DOKill();
                _buyBtnRect.DOAnchorPosX(0, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
                _buyCanvasGroup.DOFade(0f, 0.25f * Settings.AnimationSpeed);
                _buyCanvasGroup.interactable = false;
                _buyCanvasGroup.blocksRaycasts = false;
                _buyPanelRect.DOAnchorPosY(-564, 0.25f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
                break;
            case PanelType.Sell:
                _seller.EnsmallPouch();
                _sellCanvasGroup.DOKill();
                _sellPanelRect.DOKill();
                _sellBtnRect.DOKill();
                _sellBtnRect.DOAnchorPosX(0, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);_sellPanelRect.DOScale(Vector3.zero, 0.25f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
                _sellCanvasGroup.DOFade(0f, 0.25f * Settings.AnimationSpeed);
                _sellCanvasGroup.interactable = false;
                _sellCanvasGroup.blocksRaycasts = false;
                break;
            case PanelType.Craft:
                _craftBtnRect.DOKill();
                _craftBtnRect.DOAnchorPosX(0, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
                _craftPanelRect.DOAnchorPosY(-869f, 0.5f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
                _craftCanvasGroup.DOFade(0f, 0.25f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
                _craftCanvasGroup.interactable = true;
                _craftCanvasGroup.blocksRaycasts = true;
                break;
        }
    }

    private void openSellerPanel(PanelType panelType) {
        switch (panelType) {
            case PanelType.Buy:
                _buyCanvasGroup.DOKill();
                _buyPanelRect.DOKill();
                _buyPanelRect.DOAnchorPosY(-564, 0);
                _buyBtnRect.DOKill();
                _buyBtnRect.DOAnchorPosX(_selectedX, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
                float delayBuy = 0;
                if (!CameraManager.IsCameraActive(_seller.BuyCamera)) {
                    CameraManager.SwitchCamera(_seller.BuyCamera);
                    delayBuy = 2f;
                }
                _buyPanelRect.DOAnchorPosY(0, 0.5f * Settings.AnimationSpeed).SetEase(Ease.OutCubic).SetDelay(delayBuy);
                _buyCanvasGroup.DOFade(1f, 0.25f * Settings.AnimationSpeed).SetEase(Ease.OutCubic).SetDelay(delayBuy);
                _buyCanvasGroup.interactable = true;
                _buyCanvasGroup.blocksRaycasts = true;
                break;
            case PanelType.Sell:
                _sellCanvasGroup.DOKill();
                _sellPanelRect.DOKill();
                _sellBtnRect.DOKill();
                _sellBtnRect.DOAnchorPosX(_selectedX, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
                float delaySell = 0;
                if (!CameraManager.IsCameraActive(_seller.SellCamera)) {
                    _seller.EnlargePouch();
                    CameraManager.SwitchCamera(_seller.SellCamera);
                    delaySell = 2.1f;
                }
                _sellPanelRect.DOScale(Vector3.one, 0.5f * Settings.AnimationSpeed).SetEase(Ease.Linear).SetDelay(delaySell);
                _sellCanvasGroup.DOFade(1f, 0.25f * Settings.AnimationSpeed).SetEase(Ease.OutCubic).SetDelay(delaySell);
                _sellCanvasGroup.alpha = 1f;
                _sellCanvasGroup.interactable = true;
                _sellCanvasGroup.blocksRaycasts = true;
                break;
            case PanelType.Craft:
                _craftPanel.UpdateChildren();
                _craftCanvasGroup.DOKill();
                _craftPanelRect.DOKill();
                _craftBtnRect.DOKill();
                _craftBtnRect.DOAnchorPosX(_selectedX, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
                float delayCraft = 0;
                if (!CameraManager.IsCameraActive(_seller.CraftCamera)) {
                    CameraManager.SwitchCamera(_seller.CraftCamera);
                    delayCraft = 2f;
                }
                _craftPanelRect.DOAnchorPosY(0, 0.5f * Settings.AnimationSpeed).SetEase(Ease.OutCubic).SetDelay(delayCraft);
                _craftCanvasGroup.DOFade(1f, 0.25f * Settings.AnimationSpeed).SetEase(Ease.OutCubic).SetDelay(delayCraft);
                _craftCanvasGroup.interactable = true;
                _craftCanvasGroup.blocksRaycasts = true;
                break;
        }
    }


    public void OnBuyButton() {
        SwitchSellerPanel(PanelType.Buy);
    }

    public void OnSellButton() {
        SwitchSellerPanel(PanelType.Sell);
    }

    public void OnCraftButton() {
        SwitchSellerPanel(PanelType.Craft);
    }

    private RectTransform getPanelBtnRect(PanelType panelType) {
        switch (panelType) {
            case PanelType.Buy:
                return _buyBtnRect;
            case PanelType.Sell:
                return _sellBtnRect;
            case PanelType.Craft:
                return _craftBtnRect;
            default:
                return null;
        }
    }
}
