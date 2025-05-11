using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Music")]
    [field: SerializeField] public EventReference _MusicPlayer { get; private set; }
    
    [field: Header("UI SFX")]
    [field: SerializeField] public EventReference _UIClickSound { get; private set; }
    [field: SerializeField] public EventReference _showOtherPane { get; private set; }
    [field: SerializeField] public EventReference _TitleScreenAnim { get; private set; }
    [field: SerializeField] public EventReference _GiveCard { get; private set; }
    [field: SerializeField] public EventReference _CardSelect { get; private set; }
    [field: SerializeField] public EventReference _CardHover { get; private set; }
    [field: SerializeField] public EventReference _CardHoverExit { get; private set; }
    [field: SerializeField] public EventReference _MoneySpend { get; private set; }
    [field: SerializeField] public EventReference _MoneyEarned { get; private set; }

    [field: Header("Forest Ambience")]
    [field: SerializeField] public EventReference _forestAmbience { get; private set; }
    [field: SerializeField] public EventReference _bossAmbience { get; private set; }
    [field: SerializeField] public EventReference _fightAmbience { get; private set; }
    [field: SerializeField] public EventReference _hubAmbience { get; private set; }


    [field: Header("Respawn SFX")]
    [field: SerializeField] public EventReference _RespawnSound { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference _PlayerWalkSound { get; private set; }
    [field: SerializeField] public EventReference _PlayerDashSound { get; private set; }

    [field: Header("Item Equip SFX")]
    [field: SerializeField] public EventReference _EquipArmor { get; private set; }
    [field: SerializeField] public EventReference _EquipWeapon { get; private set; }
    [field: SerializeField] public EventReference _EquipItem { get; private set; }
    [field: SerializeField] public EventReference _EquipAmulet { get; private set; }
    [field: Header("Player Attack SFX")]
    [field: SerializeField] public EventReference _shortSword { get; private set; }
    [field: SerializeField] public EventReference _longSword { get; private set; }
    [field: SerializeField] public EventReference _axe { get; private set; }
    [field: SerializeField] public EventReference _fireball { get; private set; }
    [field: SerializeField] public EventReference _iceShard { get; private set; }
    
    [field: Header("Hit SFX")]
    [field: SerializeField] public EventReference _fireballHit { get; private set; }

    [field: Header("Hog SFX")]
    [field: SerializeField] public EventReference _HogAttack { get; private set; }
    [field: SerializeField] public EventReference _HogDeath { get; private set; }
    [field: SerializeField] public EventReference _HogHit { get; private set; }

    [field: Header("Slime SFX")]
    [field: SerializeField] public EventReference _SlimeAttack { get; private set; }
    [field: SerializeField] public EventReference _SlimeDeath { get; private set; }
    [field: SerializeField] public EventReference _SlimeHit { get; private set; }

    [field: Header("Mantis SFX")]
    [field: SerializeField] public EventReference _MantisAttack { get; private set; }
    [field: SerializeField] public EventReference _MantisDeath { get; private set; }
    [field: SerializeField] public EventReference _MantisHit { get; private set; }

    [field: Header("Tree SFX")]
    [field: SerializeField] public EventReference _TreeAttack { get; private set; }
    [field: SerializeField] public EventReference _TreeDeath { get; private set; }
    [field: SerializeField] public EventReference _TreeHit { get; private set; }

    [field: Header("Slime Boss SFX")]
    [field: SerializeField] public EventReference _SlimeBossNormalAttack { get; private set; }
    [field: SerializeField] public EventReference _SlimeBossDeath { get; private set; }
    [field: SerializeField] public EventReference _SlimeBossHit { get; private set; }

    [field: Header("Ent Boss SFX")]
    [field: SerializeField] public EventReference _EntBossAttack { get; private set; }
    [field: SerializeField] public EventReference _EntBossDeath { get; private set; }
    [field: SerializeField] public EventReference _EntBossHit { get; private set; }

    [field: Header("VekThar Boss SFX")]
    [field: SerializeField] public EventReference _VekTharAttack { get; private set; }
    [field: SerializeField] public EventReference _VekTharDeath { get; private set; }
    [field: SerializeField] public EventReference _VekTharHit { get; private set; }

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
