using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    public int TotalAttempts { get; private set; }
    public int MatchesFound { get; private set; }
    public int CurrentScore { get; private set; }

    // Event for score updates
    public delegate void ScoreUpdateHandler(int attempts, int matches);
    public event ScoreUpdateHandler OnScoreUpdated;

    [Header("Score Settings")]
    [SerializeField] private int pointsPerMatch = 10;
    [SerializeField] private int pointsPerMismatch = -2;

    // Singleton pattern
    private static ScoreHandler instance;
    public static ScoreHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ScoreHandler>();
                if (instance == null)
                {
                    GameObject go = new GameObject("ScoreHandler");
                    instance = go.AddComponent<ScoreHandler>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        ResetScore();
    }

    public void ProcessMatchAttempt(bool isMatch)
    {
        TotalAttempts++;
        if (isMatch)
        {
            MatchesFound++;
            CurrentScore += pointsPerMatch;
        }
        else
        {
            CurrentScore += pointsPerMismatch;
        }
        OnScoreUpdated?.Invoke(TotalAttempts, MatchesFound);
    }

    public void ResetScore()
    {
        TotalAttempts = 0;
        MatchesFound = 0;
        CurrentScore = 0;
        OnScoreUpdated?.Invoke(TotalAttempts, MatchesFound);
    }

    public void RestoreState(int score, int attempts)
    {
        CurrentScore = score;
        TotalAttempts = attempts;
        OnScoreUpdated?.Invoke(TotalAttempts, MatchesFound);
        Debug.Log($"Score state restored - Score: {score}, Attempts: {attempts}");
    }
} 