using System;
using System.Timers;

public class Score
{
    private int _currentScore;
    private System.Timers.Timer _intervalTimer;

    public Score()
    {
        _currentScore = 300;

        _intervalTimer = new Timer(10000);
        _intervalTimer.Enabled = true;

        // Hook up the Elapsed event for the timer.
        _intervalTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
    }

    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        _currentScore -= 50;

        if (_currentScore < 0)
        {
            _currentScore = 0;
        }
    }

    public int CurrentScore
    {
        get
        {
            return _currentScore;
        }
    }

    public void AddScore(UInt16 addScore)
    {
        _currentScore += addScore;
    }

    public void RemoveScore(int removeScore)
    {
        _currentScore -= removeScore;
        if (_currentScore < 0)
        {
            _currentScore = 0;
        }
    }

    public void ClearScore()
    {
        _currentScore = 0;
        _intervalTimer.Enabled = false;
        _intervalTimer.Stop();
    }
}