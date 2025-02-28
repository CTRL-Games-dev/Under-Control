using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
public class ChangePortal : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Portal _portal;
    [SerializeField] private GameObject _panel;
    [SerializeField] private Vector2 _panelStartingPosition;
    private bool _opened;
    public void Start()
    {
        _panelStartingPosition = _panel.transform.position;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if(Vector3.Distance(player.transform.position, transform.position) > 3f) 
        {
            return;
        }

        if(_opened) CloseUI();
        else OpenUI();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Player hovered over ball of power");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Player stopped hovering over ball of power");
    }
}