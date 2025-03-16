using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MusicPlayer))]
public class GameManager : MonoBehaviour {
    [Serializable]
    public struct DimensionMusic {
        public Dimension Dimension;
        public AudioClip[] Clips;
    }

    public static GameManager Instance;

    [Header("References")]
    public Weapon UnknownWeaponPrefab;
    public GameObject UnknownModelPrefab;
    public ItemEntity ItemEntityPrefab;
    public static readonly Dictionary<Dimension, string> SceneDictionary = new() {
        {Dimension.HUB, "Hub"},
        {Dimension.FOREST, "Adventure"},
        {Dimension.FOREST_BOSS, "Adventure"},
    };

    [Header("Music")]
    public DimensionMusic[] MusicPalette;

    [Header("State")]
    [SerializeField] private GameContext _context;
    private MusicPlayer _musicPlayer;
    
    private void Awake() 
    {
        DontDestroyOnLoad(this);
        // SceneManager.sceneLoaded += OnLevelChange;

        // We need to check if there is already existing manager
        // Manager don't destoy itself on load, but since it needs to be defined in every scene
        // singleton pattern must be used.
        if(Instance == null) {
            Instance = this;
        } else{
            Destroy(gameObject);
        }

        _musicPlayer = GetComponent<MusicPlayer>();
    }
    private void Start()
    {
        // For some reason "scene change" is being called, even if it is the first scene?
        // ConnectPortals();
    }

    public void ChangeDimension(Dimension dimension)
    {
        _context.CurrentDimension = dimension;

        Debug.Log("Loading new scene: " + _context.CurrentDimension.ToString());

        _musicPlayer.Stop();

        int dimensionMusicIndex = Array.FindIndex(MusicPalette, x => x.Dimension == dimension);
        if(dimensionMusicIndex != -1) {
            _musicPlayer.MusicClips = MusicPalette[dimensionMusicIndex].Clips;
            _musicPlayer.Play();
        }

        LoadingScreen.LoadScene(SceneDictionary[_context.CurrentDimension]);
    }

    public Dimension GetCurrentDimension()
    {
        return _context.CurrentDimension;
    }

    public float GetInfluence()
    {
        return _context.Influence;
    }
}