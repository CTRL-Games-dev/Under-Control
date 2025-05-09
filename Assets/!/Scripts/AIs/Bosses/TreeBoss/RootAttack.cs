using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RootAttack : MonoBehaviour {
    public float MinDamage = 30;
    public float MaxDamage = 60;

    public List<GameObject> Roots = new List<GameObject>();
    public LivingEntity Self;

    private List<LivingEntity> _hitEntities = new List<LivingEntity>();

    public void OnTriggerEnter(Collider other) {
        LivingEntity entity = other.GetComponentInParent<LivingEntity>();

        if(entity == null) return;
        if(entity == Self) return;
        if(!entity.Guild.IsHostileTowards(Self.Guild)) return;
        if(_hitEntities.Contains(entity)) return;

        _hitEntities.Add(entity);

        entity.TakeDamage(new Damage {
            Type = DamageType.PHYSICAL,
            Value = Random.Range(MinDamage, MaxDamage)
        });
    }

    public void Attack() {
        _hitEntities.Clear();

        for(var i = 0; i < Roots.Count; i++) {
            StartCoroutine(rootCoroutine(i));
        }
    }

    IEnumerator rootCoroutine(int i) {
        var root = Roots[i];

        yield return new WaitForSeconds(i * 0.20f);

        yield return root.transform.DOLocalRotate(new Vector3(0, 90, -90), 0.20f).SetEase(Ease.InOutCubic).WaitForCompletion();
        yield return root.transform.DOLocalRotate(new Vector3(0, 90, -180), 0.20f).SetEase(Ease.InOutCubic).WaitForCompletion();

        yield return new WaitForSeconds(0.5f);
     
        yield return root.transform.DOLocalRotate(new Vector3(0, 90, -270), 0.20f).SetEase(Ease.InOutCubic).WaitForCompletion();
        yield return root.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.20f).SetEase(Ease.InOutCubic).WaitForCompletion();
    }
}