using UnityEngine;
using UnityEngine.UI;

public class CardsPanel : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Transform _cardsParent;

    void Start() {
        EventBus.RunCardClickedEvent.AddListener(OnCardClicked);
    }
    
    private void OnCardClicked(RunModifier runModifier) {
        GameObject card = Instantiate(_cardPrefab, _cardsParent);
        RunCardUI runCardUI = card.GetComponent<RunCardUI>();
        runCardUI.SetRunModifier(runModifier);
        runCardUI.Setup();
    }

}
