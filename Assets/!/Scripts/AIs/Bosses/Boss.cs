using UnityEngine;
using UnityEngine.Events;

public interface IBoss {
    public float TotalHP { get; }
    public float CurrentHP { get; }
    event UnityAction OnDeath;
}