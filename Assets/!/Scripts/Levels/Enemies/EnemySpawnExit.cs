using System.Collections;
using UnityEngine;
public class EnemySpawnExit : MonoBehaviour
{
    public void SetDestroyTimer(float secs)
    {
        StartCoroutine(Destroy(secs));
    }
    public IEnumerator Destroy(float secs)
    {
        yield return new WaitForSeconds(secs);
        Destroy(this.gameObject);
    }
}