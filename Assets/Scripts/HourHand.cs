using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HourHand : HandDragable
{
    [SerializeField] private TMP_InputField _hourInputField;

    private int _hourRotations = 0; 

    private void Start()
    {
        _clockCenter = gameObject.transform.position;
        _lastHandAngle = GetAngle(gameObject.transform.position);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        float angle = GetAngle(eventData.position);
        float deltaAngle = angle - _lastHandAngle;

        UpdateHourHand(angle, deltaAngle);

        _lastHandAngle = angle; 
    }

    private void UpdateHourHand(float angle, float deltaAngle)
    {
        gameObject.transform.localRotation = Quaternion.Euler(0, 0, -angle);

        if (deltaAngle < 0 && _lastHandAngle > 300 && angle < 60) 
        {
            _hourRotations++;
        }
        else if (deltaAngle > 0 && _lastHandAngle < 60 && angle > 300) 
        {
            _hourRotations--;
        }

        int hourDegrees = Mathf.FloorToInt(angle / 30f);
        int hours24 = hourDegrees + (_hourRotations * 12);

        hours24 = hours24 % 24;
        if (hours24 < 0) hours24 += 24;

        _hourInputField.text = hours24.ToString("D2");
    }
}
