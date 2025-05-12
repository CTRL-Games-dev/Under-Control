using UnityEngine;

public class TreeBossBattleManager : MonoBehaviour
{
    [SerializeField] private LivingEntity _treeBoss;
    [SerializeField] private Vector3 _spawnPosition;
    

    void Start() {
        AudioManager.instance.setMusicArea(MusicArea.BOSSFIGHT);
        Player.UICanvas.ChangeUIBottomState(UIBottomState.HUD);
        Player.UICanvas.HUDCanvas.ShowBossBar(_treeBoss);
        Player.Instance.MaxCameraDistance = 50f;
        Player.Instance.transform.position = _spawnPosition;
        Player.Instance.SetPlayerPosition(_spawnPosition);

        EventBus.SceneReadyEvent?.Invoke();
    }

    
}
