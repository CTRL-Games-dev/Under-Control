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

    [HideInInspector] public GameDifficulty Difficulty { get; private set; }
    [HideInInspector]  public Dimension CurrentDimension { get; private set; }
    [Range(0, 1)]
    public float TotalInfluence {get; private set; }
    public float InfluenceDelta {get; private set; }

    [Header("Music")]
    public DimensionMusic[] MusicPalette;

    [Header("State")]
    private MusicPlayer _musicPlayer;
    
    private void Awake() 
    {
        // SceneManager.sceneLoaded += OnLevelChange;

        // We need to check if there is already existing manager
        // Manager don't destoy itself on load, but since it needs to be defined in every scene
        // singleton pattern must be used.
        if(Instance == null) {
            Instance = this;
        } else{
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
        SetDefault();

        _musicPlayer = GetComponent<MusicPlayer>();

        playMusicForDimension(CurrentDimension);
    }
    private void Start()
    {
        // For some reason "scene change" is being called, even if it is the first scene?
        // ConnectPortals();
    }

    public void ChangeDimension(Dimension dimension, float newInfluence)
    {
        Debug.Log($"New influence: {newInfluence}");
        if(newInfluence <= TotalInfluence)
        {
            Debug.LogError("New influence is smaller that previous influence!");
        }

        CurrentDimension = dimension;
        InfluenceDelta = newInfluence - TotalInfluence;
        TotalInfluence = newInfluence;

        Debug.Log($"Influence delta: {InfluenceDelta}");

        Debug.Log("Loading new scene: " + CurrentDimension.ToString());

        LoadingScreen.LoadScene(SceneDictionary[CurrentDimension]);

        playMusicForDimension(dimension);
    }

    private void playMusicForDimension(Dimension dimension) {
        _musicPlayer.Stop();

        int dimensionMusicIndex = Array.FindIndex(MusicPalette, x => x.Dimension == dimension);
        if(dimensionMusicIndex != -1) {
            _musicPlayer.MusicClips = MusicPalette[dimensionMusicIndex].Clips;
            _musicPlayer.Play();
        }
    }

    public void SetDefault() {
        Difficulty = GameDifficulty.NORMAL;
        CurrentDimension = Dimension.HUB;
        TotalInfluence = 0;
        InfluenceDelta = 0;
    }
}