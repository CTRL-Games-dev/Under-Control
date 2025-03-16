using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;
    [SerializeField] private GameObject _image;
    [SerializeField] private Image _fillImage;
    private CanvasGroup _canvasGroup;

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

    private void Start()
    {
        _canvasGroup = _image.GetComponent<CanvasGroup>();
        _image.SetActive(false);
    }

    public static void LoadScene(string sceneName)
    {
        Instance._canvasGroup.alpha = 0;
        Instance._image.SetActive(true);
        Instance._canvasGroup.DOFade(1, 0.5f).SetUpdate(true).OnComplete(() => Instance.StartCoroutine(Instance.LoadSceneAsync(sceneName))); 
    }

    private System.Collections.IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            _fillImage.fillAmount = Mathf.Clamp01(operation.progress / 0.9f);

            // Check if the loading is complete
            if (operation.progress >= 0.9f)
            {
                // Wait for a frame before activating the scene
                yield return new WaitForSeconds(0.5f);
                _image.SetActive(false);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

    }


}
