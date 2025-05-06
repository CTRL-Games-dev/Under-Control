using UnityEngine;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private LanguageChoose _languageChoose;

    void Start() {
        Player.Animator.SetTrigger("die");
    }

    
}
