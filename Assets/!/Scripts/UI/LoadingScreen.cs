using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using JetBrains.Annotations;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;
    [SerializeField] private RectTransform _topImgRect, _bottomImgRect;
    public static bool IsLoading = false;
    // private static string _currentSceneName = string.Empty;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start() {
        EventBus.SceneReadyEvent.RemoveListener(OnSceneReadyEvent);
        EventBus.SceneReadyEvent.AddListener(OnSceneReadyEvent);
    }

    public static void LoadScene(string sceneName) {
        if (IsLoading) return;
        Debug.Log($"Loading scene: {sceneName}");
        // if (sceneName == _currentSceneName) return;
        // _currentSceneName = sceneName;
        IsLoading = true;

        Instance._topImgRect.DOKill();
        Instance._bottomImgRect.DOKill();

        Instance._topImgRect.DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.InOutSine).SetUpdate(true).SetDelay(0.5f);
        Instance._bottomImgRect.DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.InOutSine).SetUpdate(true).SetDelay(0.5f).OnComplete(() => {

            Instance.StartCoroutine(Instance.loadSceneAsync(sceneName)); 
        });
    }


    private IEnumerator loadSceneAsync(string sceneName) {
        yield return new WaitForSecondsRealtime(2f);

        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        Player.UICanvas.ChangeUIBottomState(UIBottomState.NotVisible);
    }

    private void OnSceneReadyEvent() {
        Instance._topImgRect.DOComplete();
        Instance._bottomImgRect.DOComplete();
        Instance._topImgRect.DOKill();
        Instance._bottomImgRect.DOKill();

        Instance._topImgRect.DOAnchorPos(new Vector2(0, 540), 0.5f).SetEase(Ease.InOutSine).SetUpdate(true);
        Instance._bottomImgRect.DOAnchorPos(new Vector2(0, -540), 0.5f).SetEase(Ease.InOutSine).SetUpdate(true).OnComplete(() => {
            IsLoading = false;
        });
    }
}
