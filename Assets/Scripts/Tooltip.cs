using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI rarityField;
    public TextMeshProUGUI powerField;
    public TextMeshProUGUI dpsField;
    public TextMeshProUGUI descField;
    public TextMeshProUGUI levelField;
    public LayoutElement layoutElement;
    public int characterWrapLimit;
    public RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    public void SetText(string header, string rarity, int power, int dps, int lvl, string desc)
    {
        headerField.text = header;
        rarityField.text = rarity;
        powerField.text = power.ToString() + " Item Power";
        dpsField.text = dps.ToString() + " Damage per Second ";
        descField.text = desc;
        levelField.text = "Level " + lvl.ToString();

        int headerLength = headerField.text.Length;
        int rarityLength = rarityField.text.Length;
        int powerLength = powerField.text.Length;
        int descLength = descField.text.Length;
        int dpsLength = dpsField.text.Length;
        int levelLength = levelField.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || rarityLength > characterWrapLimit || powerLength > characterWrapLimit || dpsLength > characterWrapLimit || descLength > characterWrapLimit || levelLength > characterWrapLimit) ? true : false;
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            int headerLength = headerField.text.Length;
            int rarityLength = rarityField.text.Length;
            int powerLength = powerField.text.Length;
            int descLength = descField.text.Length;
            int dpsLength = dpsField.text.Length;
            int levelLength = levelField.text.Length;

            layoutElement.enabled = (headerLength > characterWrapLimit || rarityLength > characterWrapLimit || powerLength > characterWrapLimit || dpsLength > characterWrapLimit || descLength > characterWrapLimit || levelLength > characterWrapLimit) ? true : false;
        }

        var position = Input.mousePosition;
        var normalizedPosition = new Vector2(position.x / Screen.width, position.y / Screen.height);
        var pivot = CalculatePivot(normalizedPosition);
        _rectTransform.pivot = pivot;
        transform.position = position;
    }

    private Vector2 CalculatePivot(Vector2 normalizedPosition)
    {
        var pivotTopLeft = new Vector2(-0.05f, 1.05f);
        var pivotTopRight = new Vector2(1.05f, 1.05f);
        var pivotBottomLeft = new Vector2(-0.05f, -0.05f);
        var pivotBottomRight = new Vector2(1.05f, -0.05f);

        if (normalizedPosition.x < 0.5f && normalizedPosition.y >= 0.5f)
        {
            return pivotTopLeft;
        }
        else if (normalizedPosition.x > 0.5f && normalizedPosition.y >= 0.5f)
        {
            return pivotTopRight;
        }
        else if (normalizedPosition.x <= 0.5f && normalizedPosition.y < 0.5f)
        {
            return pivotBottomLeft;
        }
        else
        {
            return pivotBottomRight;
        }
    }

}
