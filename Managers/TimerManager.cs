using UnityEngine;
using System.Collections;
using System;

public class TimerManager : Manager<TimerManager>
{

    public static TimerManager Instance
    {
        get
        {
            return Manager<TimerManager>.Get();
        }
    }

    public struct Timer
    {
        public string id;
        public DateTime expireDate;
        public TimeSpan elapsedTime;
        public TimeSpan remainingTime;

        public void Print(string prefix)
        {
            prefix += "\n\tid: " + id;
            prefix += "\n\texpireDate: " + expireDate;
            prefix += "\n\telapsedTime: " + elapsedTime;
            prefix += "\n\tremainingTime: " + remainingTime;

            Debug.Log(prefix);
        }
    }

    private const string kLastSessionEndDateKey = "lsed";

    public UnityEventWithParameters<Timer> onTimerExpired = new UnityEventWithParameters<Timer>();

    private DateTime currentSessionBegin;
    private DateTime lastSessionEnd;

    private long secondsPassed;

    private long timersTimestamp;
    private Timer[] timers = new Timer[0];

    public void SetTimer(string id, TimeSpan duration, bool resetTime)
    {
        DateTime expDate;
        bool isValid;
        DateTimeManager.Instance.GetDate(out expDate, out isValid);

        expDate += duration;
        int index = Array.FindIndex<Timer>(timers, t => t.id == id);

        if (-1 == index)
        {
            index = timers.Length;
            Array.Resize<Timer>(ref timers, index + 1);

            timers[index] = new Timer()
            {
                id = id,
                expireDate = expDate,
                elapsedTime = TimeSpan.FromSeconds(.0),
                remainingTime = duration
            };
        }
        else
        {
            var timer = timers[index];

            timer.expireDate = expDate;
            if (resetTime)
            {
                timer.elapsedTime = TimeSpan.FromSeconds(.0);
                timer.remainingTime = duration;
            }
            else
                timer.remainingTime = duration - timer.elapsedTime;

            timers[index] = timer;
        }
    }

    public bool TryGetTimer(string id, out Timer timer)
    {
        var index = Array.FindIndex<Timer>(timers, t => t.id == id);
        if (-1 == index)
        {
            timer = new Timer();
            return false;
        }
        timer = timers[index];
        return true;
    }

    public bool HasTimer(string id)
    {
        int index = Array.FindIndex<Timer>(timers, t => t.id == id);
        return index > -1;
    }

    public bool RemoveTimer(string id)
    {
        int index = Array.FindIndex<Timer>(timers, t => t.id == id);
        if (-1 == index)
            return false;

        int newSize = timers.Length - 1;
        timers[index] = timers[newSize];
        Array.Resize<Timer>(ref timers, newSize);

        return true;
    }

    private const string kTimersCountKey = "tc";
    private const string kTimerIdPrefixKey = "tid";
    private const string kTimerExpDatePrefixKey = "ted";
    private const string kTimerElapsedTimePrefixKey = "tet";
    private const string kTimerRemainingTimePrefixKey = "trt";

    private void LoadTimers()
    {
        int timersCount = PlayerPrefs.GetInt(kTimersCountKey, 0);
        timers = new Timer[timersCount];
        timersTimestamp = Environment.TickCount;

        for (int i = 0; i < timersCount; ++i)
        {
            timers[i] = new Timer()
            {
                id = PlayerPrefs.GetString(kTimerIdPrefixKey + i, string.Empty),
                expireDate = DateTime.Parse(PlayerPrefs.GetString(kTimerExpDatePrefixKey + i, DateTime.UtcNow.ToString())),
                elapsedTime = TimeSpan.Parse(PlayerPrefs.GetString(kTimerElapsedTimePrefixKey + i, TimeSpan.FromSeconds(0.0).ToString())),
                remainingTime = TimeSpan.Parse(PlayerPrefs.GetString(kTimerRemainingTimePrefixKey + i, TimeSpan.FromSeconds(0.0).ToString())),
            };
        }
    }

    private void SaveTimers()
    {
        PlayerPrefs.SetInt(kTimersCountKey, timers.Length);
        for (int i = 0; i < timers.Length; i++)
        {
            var timer = timers[i];

            PlayerPrefs.SetString(kTimerIdPrefixKey + i, timer.id);
            PlayerPrefs.SetString(kTimerExpDatePrefixKey + i, timer.expireDate.ToString());
            PlayerPrefs.SetString(kTimerElapsedTimePrefixKey + i, timer.elapsedTime.ToString());
            PlayerPrefs.SetString(kTimerRemainingTimePrefixKey + i, timer.remainingTime.ToString());
        }
    }

    private void UpdateTimerOnResume()
    {
        timersTimestamp = Environment.TickCount;

        var timeSpan = TimeSpan.FromSeconds(secondsPassed < 0 ? 0 : secondsPassed);

        for (int i = timers.Length - 1; i >= 0; --i)
        {
            var timer = timers[i];

            timer.elapsedTime += timeSpan;
            timer.remainingTime -= timeSpan;

            if (currentSessionBegin >= timer.expireDate)
            {
                timer.remainingTime = TimeSpan.FromSeconds(.0);

                int lastIndex = timers.Length - 1;
                timers[i] = timers[lastIndex];
                Array.Resize<Timer>(ref timers, lastIndex);

                onTimerExpired.Invoke(timer);
            }
            else
                timers[i] = timer;
        }
    }

    private void UpdateTimersOnFrame()
    {
        var dt = TimeSpan.FromSeconds((Environment.TickCount - timersTimestamp) * 0.001);
        timersTimestamp = Environment.TickCount;

        for (int i = timers.Length - 1; i >= 0; --i)
        {
            var timer = timers[i];

            timer.elapsedTime += dt;
            timer.remainingTime -= dt;

            if (timer.remainingTime.TotalSeconds <= 0)
            {
                timer.remainingTime = TimeSpan.FromSeconds(.0);

                int lastIndex = timers.Length - 1;
                timers[i] = timers[lastIndex];
                Array.Resize<Timer>(ref timers, lastIndex);

                onTimerExpired.Invoke(timer);
            }
            else
                timers[i] = timer;
        }
    }

    new void Awake()
    {
        base.Awake();

        currentSessionBegin = DateTime.UtcNow;
        lastSessionEnd = DateTime.Parse(PlayerPrefs.GetString(kLastSessionEndDateKey, DateTime.UtcNow.ToString()));

        secondsPassed = (long)(currentSessionBegin - lastSessionEnd).TotalSeconds;

        LoadTimers();
        UpdateTimerOnResume();
    }

    new void Start()
    {
        base.Start();
    }

    void Update()
    {
        UpdateTimersOnFrame();
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetString(kLastSessionEndDateKey, DateTime.UtcNow.ToString());
        SaveTimers();
    }
}
