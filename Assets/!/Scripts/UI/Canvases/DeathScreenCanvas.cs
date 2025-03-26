using UnityEngine;
using DG.Tweening;

public class DeathScreenCanvas : MonoBehaviour
{
    [SerializeField] private GameObject _text, _btn;

    public void ShowUI() {
        gameObject.SetActive(true);
        GetComponent<CanvasGroup>().DOFade(1, 1).OnComplete(() => 
            _text.GetComponent<CanvasGroup>().DOFade(1, 1).OnComplete(() => 
                _btn.GetComponent<CanvasGroup>().DOFade(1, 1)
            )
        );
    }

    public void HideUI() {
        gameObject.SetActive(false);
    }

    public void RestartLevel() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
