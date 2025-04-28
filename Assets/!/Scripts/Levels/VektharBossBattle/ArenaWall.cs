using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class ArenaWall : MonoBehaviour
{
    public enum WallState {
        Up,
        Down,
    }
    public WallState State { get; private set; }
    public float Height = 4f;
    public float MoveTime = 3f;
    public void Switch()
    {
        
        if(State == WallState.Up) {
            transform.DOMoveY(Height, MoveTime);
        } else {
            transform.DOMoveY(-Height, MoveTime);
        }
    }
}