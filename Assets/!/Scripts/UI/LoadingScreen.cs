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
    [SerializeField] private GameObject _imageGO;
    [SerializeField] private Image _image;
    [SerializeField] private List<Sprite> _sprites;
    private CanvasGroup _canvasGroup;
    public static bool IsLoading = false;
    // private static string _currentSceneName = string.Empty;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    private void Start() {
        _canvasGroup = _image.GetComponent<CanvasGroup>();
        StopCoroutine(animateImages());
    }


    public static void LoadScene(string sceneName) {
        if (IsLoading) return;
        // if (sceneName == _currentSceneName) return;
        // _currentSceneName = sceneName;
        
        Instance._canvasGroup.alpha = 0;
        Instance._imageGO.SetActive(true);
        IsLoading = true;
        Instance.StartCoroutine(Instance.animateImages());
        Instance._canvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed).SetUpdate(true).OnComplete(() => Instance.StartCoroutine(Instance.loadSceneAsync(sceneName))); 
        Player.UICanvas.ChangeUIBottomState(UIBottomState.NotVisible);
    }


    private IEnumerator loadSceneAsync(string sceneName) {
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone) {

            if (operation.progress >= 0.9f) {
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
                Player.UICanvas.ChangeUIBottomState(UIBottomState.NotVisible);
                _canvasGroup.DOFade(0, 0.5f * Settings.AnimationSpeed).SetUpdate(true).OnComplete(() => {
                    _imageGO.SetActive(false);
                    StopCoroutine(animateImages());
                });
            }

            yield return null;
        }

        IsLoading = false;
    }


    private IEnumerator animateImages() {
        int index = 0;
        while(true) {
            _image.sprite = _sprites[index];
            index = (index + 1) % _sprites.Count;
            yield return new WaitForSeconds(0.1f);
        }
    }

}
