using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
public class ChangePortal : MonoBehaviour, IInteractable
{
    [SerializeField] private Portal _portal;
    [SerializeField] private GameObject _panel;
    [SerializeField] private Vector2 _panelStartingPosition;
    [SerializeField] private Material _ballMaterial;
    [SerializeField] private Material _spaceMaterial;
    [SerializeField] private float _speed;
    private bool _opened;
    public void Start()
    {
        _panelStartingPosition = _panel.transform.position;
    }

    void FixedUpdate() {
        float xOffset = Mathf.Sin(Time.time) * 0.1f;
        float yOffset = Mathf.Cos(Time.time) * 0.1f;
        _ballMaterial.mainTextureOffset = new Vector2(xOffset, yOffset);
        _spaceMaterial.mainTextureOffset = new Vector2(_spaceMaterial.mainTextureOffset.x - _speed * Time.deltaTime, _spaceMaterial.mainTextureOffset.y + _speed * Time.deltaTime);
        _spaceMaterial.SetFloat("_HeightMap", Mathf.Abs(Mathf.Sin(Time.time) * 0.8f));
    }

    public void Interact(PlayerController player)
    {
        if(_opened) CloseUI();
        else OpenUI();
    }
    
    public void ChangeDimension(int i)
    {
        Debug.Log(i);
        switch(i) 
        {
            case 1: { 
                _portal.ChangeDimension(Dimension.HUB);
                break;
            }
            case 2: {
                _portal.ChangeDimension(Dimension.MAIN_MENU);
                break;
            }
            case 3: {
                _portal.ChangeDimension(Dimension.FOREST);
                break;
            }
        }
        CloseUI();
    }
    public void OpenUI()
    {
        Debug.Log("Kula pomacana");
        _panel.transform.DOLocalMoveY(0, 0.4f);
        _opened = true;
    }

    public void CloseUI()
    {
        Debug.Log("Kula odmacana");
        _panel.transform.DOLocalMoveY(_panelStartingPosition.y, 0.4f);
        _opened = false;
    }
}