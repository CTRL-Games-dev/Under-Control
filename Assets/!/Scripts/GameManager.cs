using System;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    public readonly static string mainMenuSceneName = "MainMenu";
    public readonly static string hubSceneName = "Hub";
    public readonly static string adventureSceneName = "Adventure";
    public static Vector3 startingPos = new(10, 10, 10);
    [SerializeField] private GameContext context;
    static public GameManager gm;

    private void Awake() 
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnLevelChange;

        // We need to check if there is already existing manager
        // Manager don't destoy itself on load, but since it needs to be defined in every scene
        // singelton pattern must be used.
        if(gm == null)
        {
            gm = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        // For some reason "scene change" is being called, even if it is the first scene?
        // ConnectPortals();
    }
    private void ChangeDimension(Dimension dimension)
    {
        context.currentDimension = dimension;

        Debug.Log("Loading new scene: " + context.currentDimension);
        if(context.currentDimension == Dimension.MAIN_MENU)
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else if(context.currentDimension == Dimension.HUB)
        {
            SceneManager.LoadScene(hubSceneName);
        }
        else 
        {
            SceneManager.LoadScene(adventureSceneName);
        }
    }
    private void ConnectPortals()
    {
        GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");
        foreach(GameObject p in portals) 
        {
            Debug.Log("NUGGER");
            var portal = p.GetComponent<Portal>();
            portal.playerEnteredPortal.AddListener(ChangeDimension);
        }
        Debug.Log("Connected portals");
    }
    private void OnLevelChange(Scene scene, LoadSceneMode mode)
    {
        // Connect portals
        ConnectPortals();
    }
}