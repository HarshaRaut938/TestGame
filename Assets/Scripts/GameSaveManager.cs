using UnityEngine;
using System;

public class GameSaveManager : MonoBehaviour
{
    private const string SCORE_KEY = "CurrentScore";
    private const string MATCHES_KEY = "MatchesFound";
    private const string ATTEMPTS_KEY = "TotalAttempts";
    private const string BEST_SCORE_KEY = "BestScore";
    private const string LAST_PLAYED_KEY = "LastPlayedTime";
    private const string HAS_SAVE_KEY = "HasSaveGame";

    public static GameSaveManager Instance { get; private set; }

    public bool HasSaveGame => PlayerPrefs.GetInt(HAS_SAVE_KEY, 0) == 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGameProgress(int currentScore, int matchesFound, int totalAttempts)
    {
        PlayerPrefs.SetInt(SCORE_KEY, currentScore);
        PlayerPrefs.SetInt(MATCHES_KEY, matchesFound);
        PlayerPrefs.SetInt(ATTEMPTS_KEY, totalAttempts);
        PlayerPrefs.SetString(LAST_PLAYED_KEY, DateTime.Now.ToString());
        PlayerPrefs.SetInt(HAS_SAVE_KEY, 1);
        int bestScore = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
        if (currentScore > bestScore)
        {
            PlayerPrefs.SetInt(BEST_SCORE_KEY, currentScore);
        }

        PlayerPrefs.Save();
    }

    public (int score, int matches, int attempts, int bestScore) LoadGameProgress()
    {
        if (!HasSaveGame)
        {
            Debug.Log("No saved game found");
            return (0, 0, 0, PlayerPrefs.GetInt(BEST_SCORE_KEY, 0));
        }

        int score = PlayerPrefs.GetInt(SCORE_KEY, 0);
        int matches = PlayerPrefs.GetInt(MATCHES_KEY, 0);
        int attempts = PlayerPrefs.GetInt(ATTEMPTS_KEY, 0);
        int bestScore = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
        return (score, matches, attempts, bestScore);
    }

    public void ClearSaveGame()
    {
        PlayerPrefs.DeleteKey(SCORE_KEY);
        PlayerPrefs.DeleteKey(MATCHES_KEY);
        PlayerPrefs.DeleteKey(ATTEMPTS_KEY);
        PlayerPrefs.DeleteKey(LAST_PLAYED_KEY);
        PlayerPrefs.DeleteKey(HAS_SAVE_KEY);
        PlayerPrefs.Save();
    }

    public string GetLastPlayedTime()
    {
        return PlayerPrefs.GetString(LAST_PLAYED_KEY, "Never");
    }

    public int GetBestScore()
    {
        return PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
    }
} 