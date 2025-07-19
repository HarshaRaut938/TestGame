using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI attemptsText;
    [SerializeField] private TextMeshProUGUI matchesText;

    private void Start()
    {
        if (attemptsText == null || matchesText == null)
        {
            Debug.LogError("ScoreDisplay: Text components not assigned!");
            enabled = false;
            return;
        }

        ScoreHandler.Instance.OnScoreUpdated += UpdateScoreDisplay;
        Debug.Log("ScoreDisplay: Subscribed to score updates");
    }

    private void OnDestroy()
    {
        if (ScoreHandler.Instance != null)
        {
            ScoreHandler.Instance.OnScoreUpdated -= UpdateScoreDisplay;
        }
    }

    private void UpdateScoreDisplay(int attempts, int matches)
    {
        Debug.Log($"ScoreDisplay: Updating UI - Attempts: {attempts}, Matches: {matches}");
        
        if (attemptsText != null)
        {
            attemptsText.text = $"{attempts}";
        }

        if (matchesText != null)
        {
            matchesText.text = $"{matches}";
        }
    }
} 