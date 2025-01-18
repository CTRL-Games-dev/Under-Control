using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    public const string MainMenuSceneName = "MainMenu";
    public const string HubSceneName = "Hub";
    public const string AdventureSceneName = "Adventure";
    public static Vector3 StartingPos = new(10, 10, 10);
    [SerializeField] private GameContext _context;
    public static GameManager Gm;
    private void Awake() 
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnLevelChange;

        // We need to check if there is already existing manager
        // Manager don't destoy itself on load, but since it needs to be defined in every scene
        // singelton pattern must be used.
        if(Gm == null)
        {
            Gm = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        // For some reason "scene change" is being called, even if it is the first scene?
        ConnectPortals();
    }
    private void ChangeDimension(Dimension dimension)
    {
        _context.CurrentDimension = dimension;

        Debug.Log("Loading new scene: " + _context.CurrentDimension);
        if(_context.CurrentDimension == Dimension.MAIN_MENU)
        {
            SceneManager.LoadScene(MainMenuSceneName);
        }
        else if(_context.CurrentDimension == Dimension.HUB)
        {
            SceneManager.LoadScene(HubSceneName);
        }
        else 
        {
            SceneManager.LoadScene(AdventureSceneName);
        }
    }
    private void ConnectPortals()
    {
        GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");
        foreach(GameObject p in portals) 
        {
            var portal = p.GetComponent<Portal>();
            portal.PlayerEnteredPortal.AddListener(ChangeDimension);
        }
        Debug.Log("Connected portals. This message will appear twice");
    }
    private void OnLevelChange(Scene scene, LoadSceneMode mode)
    {
        // Connect portals
        ConnectPortals();
    }

    public float GetInfluence()
    {
        return context.Influence;
    }
}