using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] private Transform _hourHand;
    [SerializeField] private Transform _minuteHand;
    [SerializeField] private Transform _secondHand;
    [SerializeField] private TMP_Text _digitalClockText;

    [SerializeField] private TimeSyncService _timeSyncService;
    [SerializeField] private Alarm _analogAlarm;

    private bool _isAlarmModeOn;

    private void Awake()
    {
        _timeSyncService.OnTimeChanged += UpdateClock;
        _analogAlarm.OnAlarmMode += AlarmMode;
    }

    void UpdateClock(DateTime currentTime)
    {
        if (!_isAlarmModeOn)
        {
            float hours = currentTime.Hour;
            float minutes = currentTime.Minute;

            _hourHand.localRotation = Quaternion.Euler(0, 0, -hours * 30f - minutes * 0.5f);
            _minuteHand.localRotation = Quaternion.Euler(0, 0, -minutes * 6f);
        }

        float seconds = currentTime.Second;
        _secondHand.localRotation = Quaternion.Euler(0, 0, -seconds * 6f);
        _digitalClockText.text = currentTime.ToString("HH:mm:ss");
    }

    private void AlarmMode(bool alarmMode)
    {
        _isAlarmModeOn = alarmMode;
    }

    private void OnDestroy()
    {
        _timeSyncService.OnTimeChanged -= UpdateClock;
        _analogAlarm.OnAlarmMode -= AlarmMode;
    }
}
