using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    
    public List<Effect> Effects = new List<Effect>();
    public Seller Seller;

    void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            Player.Instance.FaceAnimator.StartAnimation("TALK", 5f); 
        }
        if (Input.GetKeyDown(KeyCode.F2)) {
            Player.Instance.FaceAnimator.StartAnimation("EXCITED", 1.5f); 
        }
        if (Input.GetKeyDown(KeyCode.F3)) {
            Player.UICanvas.ChangeUIMiddleState(UIMiddleState.Choose);
        }
    }
}
