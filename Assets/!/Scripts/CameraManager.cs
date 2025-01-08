
using UnityEngine;
public class CameraManager : MonoBehaviour {
    public Transform Target;
    public Camera Camera;

    // States
    public TopdownState Topdown = new();
    public CameraState CurrentState;

    private void Start()
    {
        // By Default use topdown
        CurrentState = Topdown;

        Camera = GetComponent<Camera>();
    }
    private void LateUpdate()
    {
        if(Target != null && CurrentState != null)
        {
            CurrentState.RunCameraLogic(this);
        }
    }

}

public interface CameraState 
{
    public void RunCameraLogic(CameraManager camera);
}

public class TopdownState : CameraState
{
    public Vector3 Offset = new(0, 8, 2);
    public Vector3 Rotation = new(70, 180, 0);
    public void RunCameraLogic(CameraManager camera)
    {
        camera.transform.localPosition = (camera.Target.position + Offset);
        camera.transform.eulerAngles = Rotation;
    }
}