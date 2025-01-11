using UnityEngine;
using UnityEngine.UI;
public class SellerNPCUI : MonoBehaviour
{
    [SerializeField] private Seller npc;

    public void Start()
    {
        npc.StartedInteraction.AddListener(Show);
        npc.StoppedInteraction.AddListener(Hide);
    }

    public void Update()
    {
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}