using System.Collections.Generic;
using UnityEngine;

public class ActionNotifierManager : MonoBehaviour
{
    [SerializeField] private GameObject _actionNotifierPrefab;
    private List<ActionNotifier> _actionNotifiers = new List<ActionNotifier>();


    public void SpawnActionNotifier(Sprite icon, string text, Color color, int amount) {
        ActionNotifier actionNotifier = Instantiate(_actionNotifierPrefab, transform).GetComponent<ActionNotifier>();
        ActionNotifier[] actionNotifiers = _actionNotifiers.ToArray();
        foreach (ActionNotifier notifier in actionNotifiers) {
            notifier.MoveUp();
        }
        actionNotifier.Setup(icon, text, color, amount, this);
        _actionNotifiers.Add(actionNotifier);
    }


    public void TryClearChildren(ActionNotifier actionNotifier) {
        if (_actionNotifiers.Contains(actionNotifier)) {
            _actionNotifiers.Remove(actionNotifier);
            actionNotifier.Destroy();
        }
    }
}
