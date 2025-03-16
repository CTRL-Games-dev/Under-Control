using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;
    [SerializeField] private GameObject _imageGO;
    [SerializeField] private Image _image;
    [SerializeField] private Image _fillImage;
    [SerializeField] private List<Sprite> _sprites;
    private CanvasGroup _canvasGroup;
    private bool _isAnimating = false;

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
        Instance._canvasGroup.alpha = 0;
        Instance._imageGO.SetActive(true);
        Instance._isAnimating = true;
        Instance._canvasGroup.DOFade(1, 0.5f).SetUpdate(true).OnComplete(() => Instance.StartCoroutine(Instance.loadSceneAsync(sceneName))); 
        UICanvas.Instance.HideUI();
        Instance.StartCoroutine(Instance.animateImages());
    }


    private IEnumerator loadSceneAsync(string sceneName) {        
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone) {
            _fillImage.fillAmount = Mathf.Clamp01(operation.progress / 0.9f);

            if (operation.progress >= 0.9f) {
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
                UICanvas.Instance.HideUI();
                _canvasGroup.DOFade(0, 0.5f).SetUpdate(true).OnComplete(() => {
                    _imageGO.SetActive(false);
                    StopCoroutine(animateImages());
                    UICanvas.Instance.ShowUI();
                    UICanvas.Instance.OpenUIState(UIState.NotVisible);
                });
            }

            yield return null;
        }
    }


    private IEnumerator animateImages() {
        int index = 0;
        while(true) {
            _image.sprite = _sprites[index];
            if (index + 1 == _sprites.Count) _isAnimating = false;
            index = (index + 1) % _sprites.Count;
            yield return new WaitForSeconds(0.1f);
        }
    }

}
