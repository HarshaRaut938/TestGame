using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    public int TotalAttempts { get; private set; }
    public int MatchesFound { get; private set; }

    // Event for score updates
    public delegate void ScoreUpdateHandler(int attempts, int matches);
    public event ScoreUpdateHandler OnScoreUpdated;

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
                    Debug.Log("ScoreHandler: Created new instance");
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("ScoreHandler: Destroying duplicate instance");
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        Debug.Log("ScoreHandler: Instance initialized");
        ResetScore();
    }

    public void ProcessMatchAttempt(bool isMatch)
    {
        TotalAttempts++;
        if (isMatch)
        {
            MatchesFound++;
        }
        Debug.Log($"ScoreHandler: Processing match attempt - IsMatch: {isMatch}, TotalAttempts: {TotalAttempts}, MatchesFound: {MatchesFound}");
        
        if (OnScoreUpdated != null)
        {
            OnScoreUpdated.Invoke(TotalAttempts, MatchesFound);
        }
        else
        {
            Debug.LogWarning("ScoreHandler: No listeners for score updates!");
        }
    }

    public void ResetScore()
    {
        TotalAttempts = 0;
        MatchesFound = 0;
        Debug.Log("ScoreHandler: Score reset");
        
        if (OnScoreUpdated != null)
        {
            OnScoreUpdated.Invoke(TotalAttempts, MatchesFound);
        }
        else
        {
            Debug.LogWarning("ScoreHandler: No listeners for score updates!");
        }
    }
} 