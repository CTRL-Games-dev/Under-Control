
using UnityEngine;

public class TreeRandomizer : MonoBehaviour
{
    public bool Randomize = false;
    [SerializeField] private Transform _tree;
    [SerializeField] private GameObject _leaves;
    [SerializeField] private Gradient _gradient;
    void Awake() {
        _tree.eulerAngles = new(0,UnityEngine.Random.Range(0f, 359f));
        _tree.position += new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), 0, UnityEngine.Random.Range(-0.1f, 0.1f));
    }

    void Start() {

        if(_leaves == null) return;

        float influence = GameManager.Instance.TotalInfluence;
        if(influence <= 15) return;

        influence += Random.Range(-5, 5);

        if(influence >= 100) {
            Destroy(_leaves);
            return;
        }

        float influencePercent = influence / 100;

        MeshRenderer leavesRenderer = _leaves.GetComponent<MeshRenderer>();
        leavesRenderer.material.SetColor("_LeavesColor", _gradient.Evaluate(influencePercent));
    }
}
