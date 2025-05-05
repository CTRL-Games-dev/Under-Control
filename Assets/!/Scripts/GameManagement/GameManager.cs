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
    public bool DebugMode = false;

    [Header("References")]
    public Weapon UnknownWeaponPrefab;
    public GameObject UnknownModelPrefab;
    public ItemEntity ItemEntityPrefab;
    public InventoryPanel InventoryPanel;
    public InvTileEquipment ArmorTile;
    public InvTileEquipment WeaponTile;
    public InvTileEquipment AmuletTile;
    public InvTileEquipment Consumable1Tile;
    public InvTileEquipment Consumable2Tile;

    public static readonly Dictionary<Dimension, string> SceneDictionary = new() {
        {Dimension.HUB, "NewHub"},
        {Dimension.FOREST, "Adventure"},
        {Dimension.FOREST_VECTOR, "VectorBossBattle"},
    };

    [HideInInspector] public GameDifficulty Difficulty { get; private set; }
    [HideInInspector] public Dimension CurrentDimension { get; private set; }
    [Range(0, 1)]
    public float TotalInfluence {get; private set; }
    public float InfluenceDelta {get; private set; }
    [HideInInspector] public static readonly float MinInfluenceDelta = 5.0f; 
    [HideInInspector] public static readonly float MaxInfluenceDelta = 10.0f;
    public bool ShowMainMenu = true;
    public bool ShowNewGame = true;

    [Header("Music")]
    public DimensionMusic[] MusicPalette;
    private MusicPlayer _musicPlayer;

    [Header("State")]
    // public Card[] AllPossibleCards;
    [HideInInspector] private List<Card> _alreadyAddedCards = new();
    [HideInInspector] private List<Card> _availableCards = new();
    [SerializeField] private List<Card> _cards = new();
    [Space]
    public float SaveCooldown = 15f;

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

        ResetCards();
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

    public void ResetInfluence() {
        TotalInfluence = 0;
        InfluenceDelta = 0;
    }

    public void ResetCards() {
        _alreadyAddedCards.Clear();
        _cards.ForEach(x => _availableCards.Add(x));
    }

    public float GetInfluenceModifier() {
        return (InfluenceDelta / MaxInfluenceDelta) + 0.5f;
    }

    public Card[] GetRandomCards(int numberOfcards = 3) {

        numberOfcards = Math.Min(numberOfcards, _availableCards.Count);
        Card[] cards = new Card[numberOfcards];

        List<Card> copiedCards = FluffyUtils.CloneList(_cards);
        Debug.Log($"Normal {_availableCards.Count}");
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
            _availableCards.Remove(chosenCard);

            foreach(var c in chosenCard.NextCards) {
                if(_alreadyAddedCards.Contains(c)) continue;
                _availableCards.Add(c);
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
        Player.Instance.CameraDistance = 10f;
    }


    public void DebugCommands() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            Player.Instance.DamageDisabled = true;
        }
        if(Input.GetKeyDown(KeyCode.F9)) {
            Debug.Log("<color=red>Debug Tools - Saved game via hotkey");
            SaveSystem.Save();
        }
        if(Input.GetKeyDown(KeyCode.F10)) {
            Debug.Log("<color=red>Debug Tools - Loaded game via hotkey");
            SaveSystem.Load();
        }
    }
}