using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Alarm : MonoBehaviour
{
    [SerializeField] private GameObject _alarmBlock;

    [SerializeField] private TMP_InputField _hourInputField;
    [SerializeField] private TMP_InputField _minuteInputField;
    [SerializeField] private TMP_Text _alarmStatusText;

    [SerializeField] private RectTransform _hourHand;
    [SerializeField] private RectTransform _minuteHand;

    [SerializeField] private Button _switchClockMode;
    [SerializeField] private Button _enableAlarmButton;
    [SerializeField] private Button _disableAlarmButton;

    [SerializeField] private TimeSyncService _timeSyncService;

    private int _alarmHour;
    private int _alarmMinute;
    private bool _isAlarmSet = false;

    public event Action<bool> OnAlarmMode;

    private void Awake()
    {
        _timeSyncService.OnTimeChanged += UpdateAlarm;

        _switchClockMode.onClick.AddListener(SwitchAlarmMode);

        _enableAlarmButton.onClick.AddListener(SetAlarm);
        _disableAlarmButton.onClick.AddListener(StopAlarm);

        _hourInputField.onEndEdit.AddListener(delegate { OnInputFieldTimeChanged(); });
        _minuteInputField.onEndEdit.AddListener(delegate { OnInputFieldTimeChanged(); });
    }

    private void Start()
    {
        StopAlarm();
    }

    private void OnInputFieldTimeChanged()
    {
        if (int.TryParse(_hourInputField.text, out int hour))
        {
            hour = Mathf.Clamp(hour, 1, 24);
            float hourAngle = (hour % 24) * 30f;
            _hourHand.localRotation = Quaternion.Euler(0, 0, -hourAngle);
        }

        if (int.TryParse(_minuteInputField.text, out int minute))
        {
            minute = Mathf.Clamp(minute, 0, 59);
            float minuteAngle = minute * 6f;
            _minuteHand.localRotation = Quaternion.Euler(0, 0, -minuteAngle);
        }
    }

    private void SwitchAlarmMode()
    {
        if (!_alarmBlock.activeSelf)
        {
            _alarmBlock.SetActive(true);
            OnAlarmMode?.Invoke(true);
            _hourHand.localRotation = Quaternion.identity;
            _minuteHand.localRotation = Quaternion.identity;

            if (_isAlarmSet)
            {
                float hourAngle = (_alarmHour % 24) * 30f;
                float minuteAngle = _alarmMinute * 6f;
                _hourHand.localRotation = Quaternion.Euler(0, 0, -hourAngle);
                _minuteHand.localRotation = Quaternion.Euler(0, 0, -minuteAngle);
            }
        }
        else 
        {
            _alarmBlock.SetActive(false);
            OnAlarmMode?.Invoke(false);
        }
    }
 
    private void UpdateAlarm(DateTime currentTime) 
    {
        if (_isAlarmSet) 
        {
            if (currentTime.Hour == _alarmHour && currentTime.Minute == _alarmMinute)
            {
                TriggerAlarm();
            }
        }
    }

    private void SetAlarm()
    {
        if (int.TryParse(_hourInputField.text, out _alarmHour) && int.TryParse(_minuteInputField.text, out _alarmMinute))
        {
            _isAlarmSet = true;
            _alarmStatusText.text = $"Будильник установлен на {_alarmHour:D2}:{_alarmMinute:D2}";
        }
        else
        {
            Debug.LogWarning("Неверный формат времени!");
        }
    }

    private void StopAlarm()
    {
        _isAlarmSet = false;
        _alarmStatusText.text = "Будильник не установлен.";
    }

    private void TriggerAlarm()
    {
        _alarmStatusText.text = "Будильник сработал!";
    }

    private void OnDestroy()
    {
        _timeSyncService.OnTimeChanged -= UpdateAlarm;

        _switchClockMode.onClick.RemoveListener(SwitchAlarmMode);

        _enableAlarmButton.onClick.RemoveListener(SetAlarm);
        _disableAlarmButton.onClick.RemoveListener(StopAlarm);

        _hourInputField.onEndEdit.RemoveListener(delegate { OnInputFieldTimeChanged(); });
        _minuteInputField.onEndEdit.RemoveListener(delegate { OnInputFieldTimeChanged(); });
    }
}
