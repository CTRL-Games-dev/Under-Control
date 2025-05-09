using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MusicPlayer))]
public class GameManager : MonoBehaviour {

    [Serializable]
    public struct DimensionMusic {
        public Dimension Dimension;
        public AudioClip[] Clips;
    }

    // New struct for ambient sounds per dimension
    [Serializable]
    public struct DimensionAmbient {
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
        {Dimension.CARD_CHOOSE, "CardChoose"}
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

    [Header("Ambient")]
    // Changed to use DimensionAmbient
    public DimensionAmbient[] AmbientPalette;
    private List<AudioSource> ambientAudioSources = new List<AudioSource>();

    [Header("State")]
    [HideInInspector] private List<Card> _alreadyAddedCards = new();
    [HideInInspector] private List<Card> _availableCards = new();
    [SerializeField] private List<Card> _cards = new();
    [Space]
    public float SaveCooldown = 15f;

    // Events
    public UnityEvent SceneReadyEvent;
    public bool IsStarterDialogueOver = false;

    private void Awake()  {
        _musicPlayer = GetComponent<MusicPlayer>();
        // SceneManager.sceneLoaded += OnLevelChange;

        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
        SetDefault();

        // Remove initial ambient initialization from Awake
        // Ambient will be started in Start()

        foreach(var c in _cards) {
            _alreadyAddedCards.Add(c);
        }

        ResetCards();
    }

    private void Start() {
        playMusicForDimension(CurrentDimension);
        playAmbientForDimension(CurrentDimension);
        // For some reason "scene change" is being called, even if it is the first scene?
        // ConnectPortals();
    }

    void Update() {
        DebugCommands();
    }

    public void ChangeDimension(Dimension dimension) {
        ChangeDimension(dimension, TotalInfluence);
    }

    public void ChangeDimension(Dimension dimension, float newInfluence)  {
        Debug.Log($"New influence: {newInfluence}");
        if(newInfluence < TotalInfluence) {
            Debug.LogError($"New influence ({newInfluence}) is smaller that previous influence ({TotalInfluence})!");
        }

        CurrentDimension = dimension;
        InfluenceDelta = newInfluence - TotalInfluence;
        TotalInfluence = newInfluence;

        Debug.Log($"Influence delta: {InfluenceDelta}");
        if(MaxInfluenceDelta < InfluenceDelta)
            Debug.LogWarning($"Influence delta ({InfluenceDelta}) is bigger that maximum allowed influence delta ({MaxInfluenceDelta})!");

        Debug.Log("Loading new scene: " + CurrentDimension.ToString());
        LoadingScreen.LoadScene(SceneDictionary[CurrentDimension]);

        playMusicForDimension(dimension);
        playAmbientForDimension(dimension);
    }

    private void playMusicForDimension(Dimension dimension) {
        _musicPlayer.Stop();

        int dimensionMusicIndex = Array.FindIndex(MusicPalette, x => x.Dimension == dimension);
        if(dimensionMusicIndex != -1) {
            _musicPlayer.MusicClips = MusicPalette[dimensionMusicIndex].Clips;
            _musicPlayer.Play();
        }
    }

    // New method to play ambient sounds for a specific dimension.
    // All the clips defined for that dimension are played concurrently.
    private void playAmbientForDimension(Dimension dimension) {
        // Stop previous ambient sounds
        foreach(var source in ambientAudioSources) {
            source.Stop();
            Destroy(source.gameObject);
        }
        ambientAudioSources.Clear();

        int index = Array.FindIndex(AmbientPalette, a => a.Dimension == dimension);
        if(index != -1) {
            foreach(var clip in AmbientPalette[index].Clips) {
                GameObject ambientPlayer = new GameObject("Ambient_" + clip.name);
                ambientPlayer.transform.parent = transform;
                AudioSource source = ambientPlayer.AddComponent<AudioSource>();
                source.clip = clip;
                source.loop = true;
                source.playOnAwake = false;
                source.Play();
                ambientAudioSources.Add(source);
            }
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

        var weaponCards = copiedCards.Where(x => x.GetType() == typeof(WeaponCard)).ToList();
        if (weaponCards.Count == 0)
            throw new Exception("No WeaponCards available");

        Card weaponCard = weaponCards[UnityEngine.Random.Range(0, weaponCards.Count)];
        cards[0] = weaponCard;
        copiedCards.Remove(weaponCard);

        var nonWeaponCards = copiedCards.Where(x => x.GetType() != typeof(WeaponCard)).ToList();
        for (int i = 1; i < numberOfcards; i++) {
            if (nonWeaponCards.Count == 0)
                throw new Exception("Not enough non-WeaponCards available");

            int index = UnityEngine.Random.Range(0, nonWeaponCards.Count);
            Card card = nonWeaponCards[index];
            nonWeaponCards.RemoveAt(index);
            cards[i] = card;
        }
        return cards;
    }

    public bool ChooseCard(Card chosenCard) {
        foreach(var card in _cards) {
            if(card != chosenCard)
                continue;
            _availableCards.Remove(chosenCard);
            foreach(var c in chosenCard.NextCards) {
                if(_alreadyAddedCards.Contains(c))
                    continue;
                _availableCards.Add(c);
                _alreadyAddedCards.Add(c);
            }
            return true;
        }
        return false;
    }

    public void OnLevelLoaded() {
        SceneReadyEvent?.Invoke();
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