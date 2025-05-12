
using UnityEngine;

enum State{
    Idle,
    CatchWindow
}
public class BobberScript : MonoBehaviour
{
    private float _timer;
    private State _currentState;
    private LineRenderer _lineRenderer;
    private MeshRenderer _meshRenderer;
    private Vector3[] linePoints = new Vector3[2];
    //[SerializeField] private Material material;
    [SerializeField] private float _minCatchTime;
    [SerializeField] public float _maxCatchTime;
    [SerializeField] private float _catchWindow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _lineRenderer = GetComponent<LineRenderer>();
        _timer = Time.time + Random.Range(_minCatchTime, _maxCatchTime);
    }
    void Update()
    {
        if(Time.time > _timer){
            switch(_currentState){
                case State.Idle:{
                    _currentState = State.CatchWindow;
                    _timer += _catchWindow;
                    Player.Instance.FishCatchWindow = true;
                    _meshRenderer.material.color = Color.red;
                    break;
                }
                case State.CatchWindow:{
                    _currentState = State.Idle;
                    _timer += Random.Range(_minCatchTime, _maxCatchTime);
                    Player.Instance.FishCatchWindow = false;
                    _meshRenderer.material.color = Color.white;
                    break;
                }    
            }
        }
        linePoints[0] = gameObject.transform.position;
        linePoints[1] = Player.Instance.FishingBone.transform.position;

        _lineRenderer.SetPositions(linePoints);
    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if(collision.gameObject.layer == LayerMask.NameToLayer("Water") && Player.Instance.AvailableFish <= 0) {Debug.Log("destroyed on water");Destroy(gameObject); }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Terrain")){
            Player.Instance.FishCatchWindow = false;
            Debug.Log("destroyed on terrain");
            Destroy(gameObject);
        }
    }
}
