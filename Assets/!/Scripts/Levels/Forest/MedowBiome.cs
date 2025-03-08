// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// public class MedowBiome : IBiome
// {
//     private GameObject[] _treeVariants;
//     private GameObject[] _stoneVariants;

//     public MedowBiome()
//     {
//         _treeVariants = Resources.LoadAll<GameObject>("Prefabs/Forest/Trees");
//         _stoneVariants = Resources.LoadAll<GameObject>("Prefabs/Forest/Rocks");
//     }
//     public void GenerateBiome(Tile[] biomeTiles)
//     {
//         GenerateBiome(biomeTiles.ToList());
//     }
//     public void GenerateBiome(List<Tile> biomeTiles)
//     {
//         foreach (Tile tile in biomeTiles)
//         {
//             int treeNumber = Random.Range(1,5) - 4;
//             int rockNumber = Random.Range(2,9);
//             float influence = GameManager.instance.GetInfluence();

//             for (int t = 0; t < treeNumber; t++)
//             {
//                 GameObject treeVariant = _treeVariants[Random.Range(0, _treeVariants.Length)];
//                 float x = (float)(tile.X + 0.1 + Random.Range(0f, 0.8f));
//                 float y = (float)(tile.Y + 0.1 + Random.Range(0f, 0.8f));

//                 GameObject newTree = GameObject.Instantiate(treeVariant, new(x,0,y), Quaternion.identity);
//                 if(influence > Random.Range(0.001f, 1f)) 
//                     newTree.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", new Color(76f / 255f, 36f / 255f, 84f / 255f, 1f));

//                 newTree.transform.eulerAngles = new Vector3(-90, UnityEngine.Random.Range(0, 360), 0);
//             }

//             for (int r = 0; r < rockNumber; r++)
//             {
//                 GameObject treeVariant = _treeVariants[Random.Range(0, _treeVariants.Length)];
//                 float x = (float)(tile.X + 0.1 + Random.Range(0f, 0.8f));
//                 float y = (float)(tile.Y + 0.1 + Random.Range(0f, 0.8f));

//                 GameObject newRock = GameObject.Instantiate(treeVariant, new(x,0,y), Quaternion.identity);
//                 newRock.transform.eulerAngles = new Vector3(-90, UnityEngine.Random.Range(0, 360), 0);
//             }
//         }
//     }
// }