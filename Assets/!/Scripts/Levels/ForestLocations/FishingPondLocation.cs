using UnityEngine;

public class FishingPondLocation : Location
{
    [SerializeField] private int _fishInPond;
    // [SerializeField] private Material _fishAvailableMaterial;
    // [SerializeField] private Material _fishUnavailableMaterial;
    [SerializeField] private MeshRenderer[] _ponds;


    void Start()
    {
        Player.Instance.AvailableFish = _fishInPond;
        Player.Instance.FishCaughtEvent.AddListener(CheckFishAvailability);
    }

    void CheckFishAvailability(){
        if(Player.Instance.AvailableFish <= 0){
            for (int i = 0; i < _ponds.Length; i++)
            {
                _ponds[i].material.color = new Color(0.439f, 0.58f, 0.38f);
            }
        }
    }
}
