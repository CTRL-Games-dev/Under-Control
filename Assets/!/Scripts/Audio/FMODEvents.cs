using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Music")]
    [field: SerializeField] public EventReference MusicPlayer { get; private set; }
    
    [field: Header("UI SFX")]
    [field: SerializeField] public EventReference UIClickSound { get; private set; }
    [field: SerializeField] public EventReference showOtherPane { get; private set; }
    [field: SerializeField] public EventReference TitleScreenAnim { get; private set; }
    [field: SerializeField] public EventReference GiveCard { get; private set; }
    [field: SerializeField] public EventReference CardSelect { get; private set; }
    [field: SerializeField] public EventReference CardHover { get; private set; }
    [field: SerializeField] public EventReference CardHoverExit { get; private set; }
    [field: SerializeField] public EventReference MoneySpend { get; private set; }
    [field: SerializeField] public EventReference MoneyEarned { get; private set; }

    [field: Header("Forest Ambience")]
    [field: SerializeField] public EventReference ForestAmbience { get; private set; }
    [field: SerializeField] public EventReference BossAmbience { get; private set; }
    [field: SerializeField] public EventReference FightAmbience { get; private set; }
    [field: SerializeField] public EventReference HubAmbience { get; private set; }


    [field: Header("Respawn SFX")]
    [field: SerializeField] public EventReference RespawnSound { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference PlayerWalkSound { get; private set; }
    [field: SerializeField] public EventReference PlayerDashSound { get; private set; }
    [field: SerializeField] public EventReference PlayerHitIndicator { get; private set; }
    [field: SerializeField] public EventReference PlayerAttackIndicator { get; private set; }
    [field: SerializeField] public EventReference playerStunnedIndicator { get; private set; }
    [field: SerializeField] public EventReference PlayerDeathSound { get; private set; }

    [field: Header("Item Equip SFX")]
    [field: SerializeField] public EventReference EquipArmor { get; private set; }
    [field: SerializeField] public EventReference EquipWeapon { get; private set; }
    [field: SerializeField] public EventReference EquipItem { get; private set; }
    [field: SerializeField] public EventReference EquipAmulet { get; private set; }
    [field: Header("Player Attack SFX")]
    [field: SerializeField] public EventReference PlayerAttack { get; private set; }
    [field: SerializeField] public EventReference AttackContact { get; private set; }
    [field: SerializeField] public EventReference Fireball { get; private set; }
    [field: SerializeField] public EventReference IceShard { get; private set; }
    
    [field: Header("Hit SFX")]
    [field: SerializeField] public EventReference FireballHit { get; private set; }

    [field: Header("Hog SFX")]
    [field: SerializeField] public EventReference HogAttack { get; private set; }
    [field: SerializeField] public EventReference HogDeath { get; private set; }
    [field: SerializeField] public EventReference HogHit { get; private set; }

    [field: Header("Slime SFX")]
    [field: SerializeField] public EventReference SlimeAttack { get; private set; }
    [field: SerializeField] public EventReference SlimeDeath { get; private set; }
    [field: SerializeField] public EventReference SlimeJump { get; private set; }

    [field: Header("Mantis SFX")]
    [field: SerializeField] public EventReference MantisAttack { get; private set; }
    [field: SerializeField] public EventReference MantisDeath { get; private set; }
    [field: SerializeField] public EventReference MantisHit { get; private set; }

    [field: Header("Tree SFX")]
    [field: SerializeField] public EventReference TreeAttack { get; private set; }
    [field: SerializeField] public EventReference TreeDeath { get; private set; }
    [field: SerializeField] public EventReference TreeHit { get; private set; }
    [field: SerializeField] public EventReference TreeAppear { get; private set; }
    [field: SerializeField] public EventReference TreeAttackStart { get; private set; }


    [field: Header("Slime Boss SFX")]
    [field: SerializeField] public EventReference SlimeBossNormalAttack { get; private set; }
    [field: SerializeField] public EventReference SlimeBossDeath { get; private set; }
    [field: SerializeField] public EventReference SlimeBossHit { get; private set; }

    [field: Header("Ent Boss SFX")]
    [field: SerializeField] public EventReference EntBossAttack { get; private set; }
    [field: SerializeField] public EventReference EntBossDeath { get; private set; }
    [field: SerializeField] public EventReference EntBossHit { get; private set; }

    [field: Header("VekThar Boss SFX")]
    [field: SerializeField] public EventReference VekTharAttack { get; private set; }
    [field: SerializeField] public EventReference VekTharDeath { get; private set; }
    [field: SerializeField] public EventReference VekTharHit { get; private set; }

    [field: Header("World Interaction SFX")]
    [field: SerializeField] public EventReference ChestOpen { get; private set; }
    [field: SerializeField] public EventReference TalkingSounds { get; private set; }
    

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
