using UnityEngine;
using DG.Tweening;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject _text, _btn;

    public void ShowDeathScreen() {
        gameObject.SetActive(true);
        GetComponent<CanvasGroup>().DOFade(1, 1).OnComplete(() => 
            _text.GetComponent<CanvasGroup>().DOFade(1, 1).OnComplete(() => 
                _btn.GetComponent<CanvasGroup>().DOFade(1, 1)
            )
        );
    }

    public void RestartLevel() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
