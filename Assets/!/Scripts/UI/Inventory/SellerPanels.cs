using System.Diagnostics;
using DG.Tweening;
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
    
    private Seller _seller;
    private PanelType _currentPanelType = PanelType.None;


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
                _buyCanvasGroup.DOFade(0f, 0.25f * Settings.AnimationSpeed);
                _buyCanvasGroup.interactable = false;
                _buyCanvasGroup.blocksRaycasts = false;
                _buyPanelRect.DOAnchorPosY(-564, 0.25f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
                break;
            case PanelType.Sell:
                _seller.EnsmallPouch();
                _sellCanvasGroup.DOKill();
                _sellPanelRect.DOKill();
                _sellPanelRect.DOScale(Vector3.zero, 0.25f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
                _sellCanvasGroup.DOFade(0f, 0.25f * Settings.AnimationSpeed);
                _sellCanvasGroup.interactable = false;
                _sellCanvasGroup.blocksRaycasts = false;
                break;
        }
    }

    private void openSellerPanel(PanelType panelType) {
        switch (panelType) {
            case PanelType.Buy:
                _buyCanvasGroup.DOKill();
                _buyPanelRect.DOKill();
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
                _craftCanvasGroup.DOKill();
                _craftPanelRect.DOKill();
                if (!CameraManager.IsCameraActive(_seller.CraftCamera)) {
                    CameraManager.SwitchCamera(_seller.CraftCamera);
                }
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
}
