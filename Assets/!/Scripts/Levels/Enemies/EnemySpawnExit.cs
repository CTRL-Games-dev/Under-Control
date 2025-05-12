using System.Collections;
using DG.Tweening;
using UnityEngine;
public class EnemySpawnExit : MonoBehaviour
{
    [SerializeField] public MeshRenderer _sphereRenderer;
    public void SetDestroyTimer(float secs)
    {
        Material dissolveMaterial = _sphereRenderer.materials[1];

        float appearTime;
        float dissapearTime;
        float doNothingTime;

        if (secs > 1) {
            appearTime = 0.5f;
            dissapearTime = 0.5f;
            doNothingTime = secs - 1f;
        } else {
            appearTime = secs / 2;
            dissapearTime = secs / 2;
            doNothingTime = 0;
        }

        float dissolve = 0f;
        DOTween.To(() => dissolve, x => dissolve = x, 1f, appearTime).OnUpdate(() => {
            dissolveMaterial.SetFloat("_DissolveStrength", dissolve);
        });
        DOTween.To(() => dissolve, x => dissolve = x, 0f, dissapearTime).SetDelay(doNothingTime + appearTime).OnUpdate(() => {
            dissolveMaterial.SetFloat("_DissolveStrength", dissolve);
        }).OnComplete(() => {Destroy(this.gameObject);});
    }
}