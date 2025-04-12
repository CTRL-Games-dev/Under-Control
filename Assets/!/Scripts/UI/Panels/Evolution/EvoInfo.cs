using UnityEngine;
using DG.Tweening;
using TMPro;

public class EvoInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    private CanvasGroup _canvasGroup;

    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }


    public void SetInfo(string title, string description, Vector3 pos) {
        _canvasGroup.DOKill();
        if (title == null) {
            _canvasGroup.DOFade(0, 0.2f * Settings.AnimationSpeed);
            return;
        }
        transform.position = pos + new Vector3(0, -50, 0);
        _titleText.text = title;
        _descriptionText.text = description;
        _canvasGroup.DOFade(1, 0.2f * Settings.AnimationSpeed);
    }
}
