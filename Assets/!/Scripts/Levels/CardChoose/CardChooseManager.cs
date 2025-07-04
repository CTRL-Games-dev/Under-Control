using System;
using UnityEngine;
using DG.Tweening;

public class CardChooseManager : MonoBehaviour
{
    void Start() {
        Player.Instance.gameObject.SetActive(false);
        Player.Instance.SetPlayerPosition(new Vector3(0, 1, 0));
        Player.Instance.gameObject.SetActive(true);

        Player.UICanvas.ChangeUIBottomState(UIBottomState.NotVisible);
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.Choose);

        Invoke(nameof(sceneReady), 0.2f);

        EventBus.RunCardClickedEvent.AddListener(OnRunCardClickedEvent);
    }

    private void sceneReady() {
        EventBus.SceneReadyEvent?.Invoke();
    }
    
    private void OnRunCardClickedEvent(Card _) {
        GameManager.Instance.ChooseCard(_);
        GameManager.Instance.ResetCardChoice();
        Invoke(nameof(changeScene), 0.4f);
    }

    private void changeScene() {
        GameManager.Instance.ChangeDimension(Dimension.FOREST);
    }


    public void OnBtnPointerEnter(RectTransform rect) {
        rect.DOKill();
        rect.DOAnchorPosX(-25, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
        rect.DOScaleY(1.05f, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
    }

    public void OnBtnPointerExit(RectTransform rect) {
        rect.DOKill();
        rect.DOAnchorPosX(-50, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
        rect.DOScaleY(1f, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
    }

    public void OnSaveBtnClick(RectTransform rect) {
        SaveSystem.SaveGame();
        rect.DOKill();
        rect.DOScale(1.1f, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutBack).OnComplete(() => {
            rect.DOScale(1f, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutBack);
        });
        // Player.UICanvas.ShowMessage("Game saved!", 2f);
    }

    public void OnExitBtnClick(RectTransform rect) {
        GameManager.Instance.ChangeDimension(Dimension.HUB);
        rect.DOKill();
        rect.DOScale(1.1f, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutBack).OnComplete(() => {
            rect.DOScale(1f, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutBack);
        });
    }

}
