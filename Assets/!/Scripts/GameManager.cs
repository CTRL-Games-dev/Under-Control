using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    public readonly static string hubSceneName = "Hub";
    public readonly static string adventureSceneName = "Adventure";
    [SerializeField] private GameContext context;
    private static GameManager instance = null;

    private void Awake() 
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += onLevelChange;
    }
    private void Start()
    {
        ConnectPortals();
    }
    private void ChangeDimension(Dimension dimension)
    {
        context.currentDimension = dimension;
        if(context.currentDimension == Dimension.HUB) 
            SceneManager.LoadScene(hubSceneName);
        else 
            SceneManager.LoadScene(adventureSceneName);
    }

    private void ConnectPortals()
    {
        GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");
        foreach(GameObject p in portals) 
        {
            var portal = p.GetComponent<Portal>();
            portal.playerEnteredPortal.AddListener(ChangeDimension);
        }
    }
    private void onLevelChange(Scene scene, LoadSceneMode mode)
    {
        ConnectPortals();
    }
}