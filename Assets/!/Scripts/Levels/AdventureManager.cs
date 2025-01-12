using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AdventureManager : MonoBehaviour, ILevelManager
{
    // This is just a place holder to see if portals work
    // In main menu player will not be spawned
    public Vector3 playerStartingPos = new(3,3,3);
    [SerializeField] private GameObject _player;
    [SerializeField] private CameraManager _cameraManager;

    [Range(10, 300)]
    public int width = 10, height = 10;
    [Range(1, 20)]
    public int iterations = 0;
    private MapGenerator _generator = new MapGenerator();
    private void Start()
    {
        MapGenerator.Tile[,] grid = _generator.GetMap(width, height, iterations);
        StringBuilder sb = new StringBuilder();
        for(int i=0; i< grid.GetLength(0); i++)
        {
            for(int j=0; j< grid.GetLength(1); j++)
            {
                sb.Append(grid[i,j]);
                sb.Append(' ');				   
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());

        Instantiate(_player, playerStartingPos, Quaternion.identity);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _cameraManager.Target = player.transform;
    }

    private void Update()
    {
        
    }
}