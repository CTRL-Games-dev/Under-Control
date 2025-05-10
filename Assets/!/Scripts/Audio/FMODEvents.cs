using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Music")]
    [field: SerializeField] public EventReference _MusicPlayer { get; private set; }
    [field: Header("UI SFX")]
    [field: SerializeField] public EventReference _UIClickSound { get; private set; }

    [field: Header("Give Card SFX")]
    [field: SerializeField] public EventReference _GiveCard { get; private set; }


    [field: Header("Option Animation SFX")]
    [field: SerializeField] public EventReference _TitleScreenAnim { get; private set; }

    [field: Header("UI Pane SFX")]
    [field: SerializeField] public EventReference _showOtherPane { get; private set; }

    [field: Header("Forest Ambience")]
    [field: SerializeField] public EventReference _ambience { get; private set; }

    [field: Header("Respawn SFX")]
    [field: SerializeField] public EventReference _RespawnSound { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference _PlayerWalkSound { get; private set; }


    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one FMODEvents in the scene");
        }
        instance = this;
    }
}
