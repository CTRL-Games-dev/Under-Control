using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

public class BoarBoss : MonoBehaviour 
{
    private BetterGenerator _generator;
    private GameObject _boarPrefab;
    private int _boarNumber = 0;
    private int _maxBoarNumber = 10;
    public void Start()
    {
        _boarPrefab = Resources.Load<GameObject>("Prefabs/Forest/Enemies/Boar");
        _generator = GameObject.Find("AdventureManager").GetComponent<BetterGenerator>();
        InvokeRepeating("SpawnBoars", 20, 7);
    }

    public void SpawnBoars()
    {
        Debug.Log("Spawning boars");
        Vector3 pos = transform.position;

        WorldData wd = _generator.wd;
        ForestBossArena location = _generator.Getlocation<ForestBossArena>();
        Vector2 corner = location.GetAbsoluteCorner(wd.Offset, wd.Scale);
    
        if
        (!(
            pos.x > corner.x ||
            pos.x < corner.x + (location.TileWidth * wd.Scale) ||
            pos.y > corner.y ||
            pos.y < corner.y + (location.TileHeight * wd.Scale)
        ))
        {
            Debug.Log($"Cannot spawn boars, to little space {corner}");
            return;
        }

        List<BoarController> boars = new();

        for(int b = 0; b < 1; b++)
        {
            if(_boarNumber == _maxBoarNumber) { break; }
            LivingEntity boar = Instantiate(_boarPrefab, new(pos.x + b*3 + 5, 0.1f, pos.z), this.transform.rotation).GetComponent<LivingEntity>();
            boar.OnDeath.AddListener(changeCounter);
            _boarNumber++;
        }
        for(int b = 0; b < 1; b++)
        {
            if(_boarNumber == _maxBoarNumber) { break; }
            LivingEntity boar = Instantiate(_boarPrefab, new(pos.x - b*3 - 5, 0.1f, pos.z), this.transform.rotation).GetComponent<LivingEntity>();
            boar.OnDeath.AddListener(changeCounter);
            _boarNumber++;
        }
    }

    private void changeCounter()
    {
        _boarNumber--;
    }
}