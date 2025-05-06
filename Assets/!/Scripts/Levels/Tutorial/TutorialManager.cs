using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private LanguageChoose _languageChoose;
    [SerializeField] private List<ItemData> _starterItems;

    void Start() {
        Player.Animator.SetTrigger("die");
        Random.InitState(System.DateTime.Now.Millisecond);
        ItemEntity.Spawn(_starterItems[Random.Range(0, _starterItems.Count -1)], 1, Player.Instance.transform.position + Vector3.up * 2 + Player.Instance.transform.forward * 2, 1, Quaternion.identity);
    }

    
}
