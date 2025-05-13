using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class Wall : MonoBehaviour
{
    public Vector3 Start, End;
    public GameObject WallObject;

    public void PlaceWall(Location location, Vector3 start, Vector3 end)
    {
        AudioManager.Instance.setMusicArea(MusicArea.FIGHTING);
        transform.position = Vector3.Lerp(start, end, 0.5f);

        Vector3 positionDelta = Vector3.Normalize(transform.position - location.transform.position);

        float offset = 5;
        float wallLength = Vector3.Distance(start, end);
        wallLength += offset * 2;

        // Debug.Log("=====");
        // Debug.Log($"Start {start}, end {end}");
        // Debug.Log($"Wall length {wallLength}");

        transform.position += new Vector3(offset * positionDelta.x, 0, offset * positionDelta.z);
        transform.localScale = new(transform.localScale.x, transform.localScale.y, wallLength);

        Vector3 direction = end - start;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        WallObject.transform.DOMoveY(4, 3f);

        Start = start;
        End = end;
    }

    public void RemoveWall()
    {
        AudioManager.Instance.setMusicArea(MusicArea.EXPLORING);
        WallObject.transform.DOMoveY(-6, 1f);
        StartCoroutine(destroyWall(1f));
    }

    private IEnumerator destroyWall(float secs)
    {
        yield return new WaitForSeconds(secs);
        Destroy(this.gameObject);
    }
}