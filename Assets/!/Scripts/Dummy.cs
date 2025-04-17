using System;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    private LivingEntity _livingEntity;
    private Animator _animator;
    
    private void Awake() {
        _livingEntity = GetComponent<LivingEntity>();
        _animator = GetComponent<Animator>();
    }


    void Start() {
        _livingEntity.OnDamageTaken.AddListener(OnDamageTaken);
    }

    private void OnDamageTaken(DamageTakenEventData d) {
        _animator.SetTrigger("Hit");
    }


}
