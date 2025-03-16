using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDimension : MonoBehaviour
{
    private Image _img;
    private RectTransform _rect;

    public string Name = "Sample Dimension";
    public string Description = "Sample Description";
    public DimensionType Type;
    public Dimension WhatDimension;
    public DimensionDifficulty Difficulty;
    public int VekhtarControl = 0;
    public List<ItemData> AvaliableItems = new List<ItemData>();
    private float _offset;


    private void Awake() {
        _img = GetComponent<Image>();
        _rect = GetComponent<RectTransform>();
        _offset = Random.Range(0, 10);
    }

    private void FixedUpdate() {
        _rect.localScale = new Vector3(1, 1, 1) * Mathf.Sin(Time.time + _offset) * 0.1f + new Vector3(1, 1, 1);
        transform.Rotate(Vector3.forward, Time.deltaTime * 10);
    }

    public void OnPointerEnter() {
        ChangePortal.Instance.SetDimensionInfo(this);
    }

    public void OnPointerExit() {
        ChangePortal.Instance.SetDimensionInfo(null);
    }

    public void OnPoinerClick() {
        ChangePortal.Instance.LockDimension(this);
    }
    
}
