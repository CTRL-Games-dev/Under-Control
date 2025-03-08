using System.Collections.Generic;
using UnityEngine;
// public class GrassTileRandomizer : MonoBehaviour, TileRandomizer
// {
//     [SerializeField] private List<GameObject> _stoneVariants;

//     // This function should be called before any scaling is done
//     // and after prefab of the tile is spawned!
//     public void RandomizeTile()
//     {
//         int numberOfRocks = Random.Range(0, 3);
//         GameObject rockHolder = transform.Find("RockHolder").gameObject;
//         for(int i = 0; i < numberOfRocks; i++)
//         {
//             int stoneVariantIndex = Random.Range(0, _stoneVariants.Count);
//             float stoneLocalX = Random.Range(-0.05f, 0.05f);
//             float stoneLocalY = Random.Range(-0.05f, 0.05f);
//             int stoneRoation = Random.Range(0, 360);

//             GameObject stone = _stoneVariants[stoneVariantIndex];
//             stone = Instantiate(stone, new(0,0,0), Quaternion.identity, rockHolder.transform);

//             stone.transform.localPosition = new Vector3(stoneLocalX, 0, stoneLocalY);
//             stone.transform.localScale = new(1,1,1);
//             stone.transform.eulerAngles = new Vector3(-90, stoneRoation, 0);
//         }
//     }
// }
