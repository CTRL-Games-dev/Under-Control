using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    
    public GameObject BoarPrefab;

    void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            Instantiate(BoarPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
       
        
    }
}
