using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinuteHand : HandDragable
{  
    [SerializeField] private TMP_InputField _minuteInputField;

    private void Start()
    {
        _clockCenter = gameObject.transform.position;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        float angle = GetAngle(eventData.position);
        UpdateMinuteHand(angle);
    }

    private void UpdateMinuteHand(float angle)
    {
        gameObject.transform.localRotation = Quaternion.Euler(0, 0, -angle);
        int minutes = Mathf.RoundToInt(angle / 6f) % 60;
        _minuteInputField.text = minutes.ToString("D2");
    }
}
