using UnityEngine;
using DG.Tweening;
public class ChangePortal : MonoBehaviour, IInteractable
{
    [SerializeField] private Portal _portal;
    [SerializeField] private GameObject _panel;
    [SerializeField] private Vector2 _panelStartingPosition;
    private bool _opened;
    public void Start()
    {
        _panelStartingPosition = _panel.transform.position;
    }

    private void Update()
    {
        
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