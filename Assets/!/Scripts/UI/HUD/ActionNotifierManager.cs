using System.Collections.Generic;
using UnityEngine;

public class ActionNotifierManager : MonoBehaviour
{
    [SerializeField] private GameObject _actionNotifierPrefab;
    private ActionNotifier _lastActionNotifier;



    public void SpawnActionNotifier(Sprite icon, string text, Color color, int amount) {
        GameObject actionNotifier = Instantiate(_actionNotifierPrefab, transform);
        _lastActionNotifier = actionNotifier.GetComponent<ActionNotifier>();
        _lastActionNotifier.Setup(icon, text, color, amount, this);
    }

    public void ClearChildren() {
        ActionNotifier[] children = GetComponentsInChildren<ActionNotifier>(true);

        for (int i = 0; i < children.Length; i++) {
            children[i].Destroy();
        }
    }

    public void TryClearChildren(ActionNotifier actionNotifier) {
        if (_lastActionNotifier != actionNotifier) return;

        ClearChildren();
    }
}
