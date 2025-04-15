using UnityEngine;

public class WellLocation : Location
{
    public GameObject Well;
    void Awake()
    {
        Well.transform.eulerAngles = new(0, UnityEngine.Random.Range(0f, 359f));
    }
}
