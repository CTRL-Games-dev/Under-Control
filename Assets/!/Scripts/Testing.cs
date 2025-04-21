using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public List<Effect> Effects = new List<Effect>();



    void Update() {
        if (Input.GetKeyUp(KeyCode.F1)) {
            Player.LivingEntity.ApplyEffect(Effects[0]);
        } 
        else if (Input.GetKeyUp(KeyCode.F2)) {
            Player.LivingEntity.ApplyEffect(Effects[1]);
        }
        else if (Input.GetKeyUp(KeyCode.F3)) {
            Player.LivingEntity.ApplyEffect(Effects[2]);
        }
    }
}
