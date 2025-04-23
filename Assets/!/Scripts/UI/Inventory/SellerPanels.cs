using DG.Tweening;
using UnityEngine;

public class SellerPanels : MonoBehaviour
{

    [SerializeField] private InventoryPanel _buyPanel;
    [SerializeField] private InventoryPanel _sellPanel;
    [SerializeField] private CanvasGroup _buyCanvasGroup, _sellCanvasGroup;
    
    private Seller _seller;
    


    public void SetSeller(Seller seller) {
        _seller = seller;
        _buyPanel.SetTargetInventory(seller.BuyInventory);
        _sellPanel.SetTargetInventory(seller.SellInventory);
        OnBuyButton();
    }

    public void OnBuyButton() {
        CameraManager.Instance.SwitchCamera(_seller.BuyCamera);
        _buyCanvasGroup.alpha = 1f;
        _buyCanvasGroup.interactable = true;
        _buyCanvasGroup.blocksRaycasts = true;
        _sellCanvasGroup.alpha = 0f;
        _sellCanvasGroup.interactable = false;
        _sellCanvasGroup.blocksRaycasts = false;
        _seller.ShrinkPouch();
    }

    public void OnSellButton() {
        CameraManager.Instance.SwitchCamera(_seller.SellCamera);
        _buyCanvasGroup.alpha = 0f;
        _buyCanvasGroup.interactable = false;
        _buyCanvasGroup.blocksRaycasts = false;
        _seller.EnlargePouch().OnComplete(() => {
            _sellCanvasGroup.alpha = 1f;
            _sellCanvasGroup.interactable = true;
            _sellCanvasGroup.blocksRaycasts = true;
        });
    }

    public void OnCraftButton() {
        CameraManager.Instance.SwitchCamera(_seller.CraftCamera);
        _buyCanvasGroup.alpha = 0f;
        _buyCanvasGroup.interactable = false;
        _buyCanvasGroup.blocksRaycasts = false;
        _sellCanvasGroup.alpha = 0f;
        _sellCanvasGroup.interactable = false;
        _sellCanvasGroup.blocksRaycasts = false;
        _seller.ShrinkPouch();
    }
}
