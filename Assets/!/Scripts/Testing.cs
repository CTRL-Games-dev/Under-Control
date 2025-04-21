using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    
    public List<Effect> Effects = new List<Effect>();

    void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            Player.LivingEntity.ApplyEffect(Effects[0]);
        }
        if (Input.GetKeyDown(KeyCode.F2)) {
            Player.LivingEntity.ApplyEffect(Effects[1]);
        }
        if (Input.GetKeyDown(KeyCode.F3)) {
            Player.LivingEntity.ApplyEffect(Effects[2]);
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            Player.UICanvas.ChangeUIMiddleState(UIMiddleState.Choose);
        }
    }
}
