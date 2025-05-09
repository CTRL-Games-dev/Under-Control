using UnityEngine;
using DG.Tweening;

public class Well : MonoBehaviour, IInteractable
{
    private bool _wasUsed = false;
    private Animator _animator;
    [SerializeField] private Transform _waterBucket, _waterWell;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    public void Interact() {
        if(_wasUsed) return;
        _wasUsed = true;
        

        _animator.SetTrigger("goDown");
    }

    public void OnAnimationEnd(int animationIndex) {
        if(animationIndex == 0) {
            _waterWell.DOScaleY(0, 0.25f).SetEase(Ease.Linear);
        } else {
            _waterBucket.DOScaleY(0, 0.3f).SetEase(Ease.Linear);
            Player.LivingEntity.Health = Player.LivingEntity.MaxHealth;
            Player.LivingEntity.Mana = Player.LivingEntity.MaxMana;
        }


    }
}
