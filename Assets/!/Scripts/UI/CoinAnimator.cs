using UnityEngine;
using DG.Tweening;
using TMPro;

public class CoinAnimator : MonoBehaviour
{
    private RectTransform _coinsRectTransform;
    private TextMeshProUGUI _coinsText;
    [SerializeField] private GameObject _coinsVisualizer;
    [SerializeField] private GameObject _changeVisualizer; 
    private RectTransform _changeRectTransform;
    private TextMeshProUGUI _changeText;
    private CanvasGroup _changeCanvasGroup;

    [SerializeField] private float _shakeStrength = 200;
    [SerializeField] private float _shakeVibratoMultiplier = 90;

    private void Awake() {
        _coinsRectTransform = _coinsVisualizer.GetComponent<RectTransform>();
        _coinsText = _coinsVisualizer.GetComponent<TextMeshProUGUI>();

        _changeRectTransform = _changeVisualizer.GetComponent<RectTransform>();
        _changeText = _changeVisualizer.GetComponent<TextMeshProUGUI>();
        _changeCanvasGroup = _changeVisualizer.GetComponent<CanvasGroup>();
    }

    void Start() {
        Player.Instance.CoinsChangeEvent.AddListener(OnCoinsChange);

        OnCoinsChange(0);
    }


    private void OnCoinsChange(int change) {
        if (change == 0) {
            _coinsText.text = $"{Player.Instance.Coins}";
            return;
        }
        if(change > 0) {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.MoneyEarned, this.transform.position);
        } else {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.MoneySpend, this.transform.position);
        }
        float coins = Player.Instance.Coins;
        _coinsRectTransform.DOComplete();
        _changeRectTransform.DOComplete();
        _changeCanvasGroup.DOKill();
        _coinsRectTransform.DOKill();
        _coinsRectTransform.localScale = Vector3.one;
    
        _changeText.text = change > 0 ? $"+{change}" : $"{change}";
        _changeCanvasGroup.alpha = 0;
        _changeRectTransform.anchoredPosition = new Vector2(0, -100);
        _changeCanvasGroup.DOFade(1, 0.2f * Settings.AnimationSpeed).OnComplete(() => {
            _changeRectTransform.DOAnchorPosY(-50, 0.5f * Settings.AnimationSpeed).SetEase(Ease.InBack).OnComplete(() => {
                _changeCanvasGroup.DOFade(0, 0.2f * Settings.AnimationSpeed).OnComplete(() => {
                    _changeRectTransform.anchoredPosition = new Vector2(0, -100);
                });
                _coinsRectTransform.DOScale(change > 0 ? 1.2f : 0.8f, 0.1f * Settings.AnimationSpeed).SetEase(Ease.InExpo).OnComplete(() => {
                    _coinsRectTransform.DOScale(1, 0.35f * Settings.AnimationSpeed).SetEase(Ease.InOutSine);
                });
                _coinsRectTransform.DOShakeRotation(0.3f * Settings.AnimationSpeed, _shakeStrength, (int)((change / coins) * _shakeVibratoMultiplier), 90, false, ShakeRandomnessMode.Harmonic);
                DOTween.To(() => coins, x => coins = x, coins + change, 0.3f * Settings.AnimationSpeed).SetEase(Ease.OutQuart).OnUpdate(() => {
                    _coinsText.text = $"{(int)coins}";
                });
            });
        });
    }
}
