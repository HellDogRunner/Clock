using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class TimeSyncService : MonoBehaviour
{
    [SerializeField] private int _timezoneOffsetHours = 3;
    [SerializeField] private int _requestTimeout = 5;

    private DateTime _currentTime;
    private bool timeSynced = false;

    public event Action<DateTime> OnTimeChanged;

    private void Start()
    {
        InvokeRepeating("StartParsing", 0f, 3600f);
    }

    private void FixedUpdate()
    {
        if (timeSynced)
        {
            _currentTime = _currentTime.AddSeconds(Time.deltaTime);
            OnTimeChanged?.Invoke(_currentTime);
        }
    }

    IEnumerator ParseTimeFromSites()
    {
        string[] urls = { "http://google.com", "http://www.ntp-servers.net" };
        foreach (string url in urls)
        {
            UnityWebRequest request = UnityWebRequest.Head(url);
            request.timeout = _requestTimeout;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string dateHeader = request.GetResponseHeader("Date");
                if (!string.IsNullOrEmpty(dateHeader))
                {
                    DateTime parsedTime;
                    if (DateTime.TryParse(dateHeader, out parsedTime))
                    {
                        _currentTime = parsedTime.ToUniversalTime().AddHours(_timezoneOffsetHours);
                        timeSynced = true;
                        Debug.Log("Синхронизированное время с " + url + ": " + _currentTime);
                        yield break; // Если успешно получили время, выходим из цикла
                    }
                    else
                    {
                        Debug.LogWarning("Ошибка парсинга времени с " + url + ": " + dateHeader);
                    }
                }
                else
                {
                    Debug.LogWarning("Заголовок Date не найден на " + url + ".");
                }
            }
            else
            {
                Debug.LogWarning("Ошибка получения времени с " + url + ": " + request.error);
            }
        }

        Debug.LogError("Не удалось получить время ни с одного сайта.");
    }

    private void StartParsing()
    {
        StartCoroutine(ParseTimeFromSites());
    }

    public DateTime GetCurrentTime()
    {
        return _currentTime;
    }
}
