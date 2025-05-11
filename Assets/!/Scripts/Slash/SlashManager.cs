using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlashManager : MonoBehaviour
{
    [Serializable]
    public struct SlashColor {
        public WeaponTrait Trait;
        public Color Color;
    }

    public List<SlashColor> SlashColorsList = new List<SlashColor>();
    public List<GameObject> SlashObjects = new List<GameObject>();
    public Material SlashMaterial;
    public List<Collider> HitEnemies = new List<Collider>();

    [HideInInspector]
    public UnityEvent<LivingEntity> OnHit = new();

    public void OnTriggerEnter(Collider other) {
        LivingEntity victim = other.GetComponentInParent<LivingEntity>(includeInactive: true);

        if(victim == null) return;

        OnHit?.Invoke(victim);
    }

    public void SetSlashColor(WeaponTrait weaponTrait) {
        if (SlashMaterial == null) return;
        foreach (var slashColor in SlashColorsList) {
            if (slashColor.Trait == weaponTrait) {
                SlashMaterial.color = slashColor.Color;
                return;
            }
        }
    }

    public void EnableSlash() {
        foreach (var slashObject in SlashObjects) {
            HitEnemies.Clear();
            slashObject.SetActive(true);
        }
    }

    public void DisableSlash() {
        foreach (var slashObject in SlashObjects) {
            slashObject.SetActive(false);
        }
    }
}
