using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    [HideInInspector] public static readonly float MinInfluenceDelta = 5.0f; 
    [HideInInspector] public static readonly float MaxInfluenceDelta = 10.0f; 
    [Header("Music")]
    public DimensionMusic[] MusicPalette;
    private MusicPlayer _musicPlayer;

    [Header("State")]
    // public Card[] AllPossibleCards;
    [HideInInspector] private List<Card> _alreadyAddedCards = new();
    [SerializeField] private List<Card> _cards = new();
    // Events
    public UnityEvent LevelLoaded;
    public bool IsStarterDialogueOver = false;

    private void Awake()  {
        _musicPlayer = GetComponent<MusicPlayer>();
        // SceneManager.sceneLoaded += OnLevelChange;

        // We need to check if there is already existing manager
        // Manager don't destoy itself on load, but since it needs to be defined in every scene
        // singleton pattern must be used.
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
        SetDefault();

        foreach(var c in _cards) {
            _alreadyAddedCards.Add(c);
        }
    }
    private void Start() {
        playMusicForDimension(CurrentDimension);

        // For some reason "scene change" is being called, even if it is the first scene?
        // ConnectPortals();
    }

    void Update() {
        DebugCommands();
    }

    public void ChangeDimension(Dimension dimension, float newInfluence)  {
        Debug.Log($"New influence: {newInfluence}");
        if(newInfluence <= TotalInfluence)
        {
            Debug.LogError($"New influence ({newInfluence}) is smaller that previous influence ({TotalInfluence})!");
        }

        CurrentDimension = dimension;
        InfluenceDelta = newInfluence - TotalInfluence;
        TotalInfluence = newInfluence;

        Debug.Log($"Influence delta: {InfluenceDelta}");

        if(MaxInfluenceDelta < InfluenceDelta) Debug.LogWarning($"Influence delta ({InfluenceDelta}) is bigger that maximum allowed influence delta ({MaxInfluenceDelta})!");

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

    public float GetInfluenceModifier() {
        return (InfluenceDelta / MaxInfluenceDelta) + 0.5f;
    }

    public Card[] GetRandomCards(int numberOfcards = 3) {

        numberOfcards = Math.Min(numberOfcards, _cards.Count);
        Card[] cards = new Card[numberOfcards];

        List<Card> copiedCards = FluffyUtils.CloneList(_cards);
        Debug.Log($"Normal {_cards.Count}");
        Debug.Log($"Copied cards {copiedCards.Count}");

        for(int i = 0; i < numberOfcards; i++) {
            Card card = copiedCards[UnityEngine.Random.Range(0, copiedCards.Count)];
            copiedCards.Remove(card);
            cards[i] = card;
        }
        return cards;
    }

    public bool ChooseCard(Card chosenCard) {
        foreach(var card in _cards) {
            if(card != chosenCard) continue;
            _cards.Remove(chosenCard);

            foreach(var c in chosenCard.NextCards) {
                if(_alreadyAddedCards.Contains(c)) continue;
                _cards.Add(c);
                _alreadyAddedCards.Add(c);
            }
            return true;
        }
        return false;
    }

    // Each adventure manager calls this function once the level has been loaded
    public void OnLevelLoaded() {
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.Choose);
        LevelLoaded.Invoke();
    }


    public void DebugCommands() {
        if(Input.GetKeyDown(KeyCode.F1)) {
            Debug.Log("<color=red>Debug Tools - Moving to the hub");
            ChangeDimension(Dimension.FOREST, TotalInfluence + 10);
        }

        if(Input.GetKeyDown(KeyCode.F2)) {
            Debug.Log("<color=red>Debug Tools - Moving to the new forest");
            ChangeDimension(Dimension.FOREST, TotalInfluence + 10);
        }

        if(Input.GetKeyDown(KeyCode.F3)) {
            Debug.Log("<color=red>Debug Tools - Moving to Vek'thar's arena");
            // ChangeDimension(Dimension.FOREST_VECTOR, TotalInfluence);
        }

        if(Input.GetKeyDown(KeyCode.F7)) {
            TotalInfluence += 5;
            Debug.Log($"<color=red>Debug Tools - Adding 5 influence. New influence: {TotalInfluence}");
        }

        if(Input.GetKeyDown(KeyCode.F8)) {
            TotalInfluence -= 5;
            Debug.Log($"<color=red>Debug Tools - Subtracting 5 influence. New influence: {TotalInfluence}");
        }
    }
}