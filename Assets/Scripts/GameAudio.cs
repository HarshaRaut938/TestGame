using UnityEngine;

public class GameAudio : MonoBehaviour
{
    [Header("Sound Names")]
    [SerializeField] private string cardFlipSound = "CardFlip";
    [SerializeField] private string cardMatchSound = "CardMatch";
    [SerializeField] private string cardMismatchSound = "CardMismatch";
    [SerializeField] private string gameOverSound = "GameOver";

    private bool isInitialized = false;

    private void Awake()
    {
        StartCoroutine(InitializeAudio());
    }

    private System.Collections.IEnumerator InitializeAudio()
    {
        yield return null; 

        if (AudioManager.Instance == null)
        {
            enabled = false;
            yield break;
        }

        isInitialized = true;
    }

    private bool CheckInitialization()
    {
        if (!isInitialized || AudioManager.Instance == null)
        {
            return false;
        }
        return true;
    }

    public void PlayCardFlip()
    {
        if (!CheckInitialization()) return;
        AudioManager.Instance.PlaySound(cardFlipSound);
    }

    public void PlayCardMatch()
    {
        if (!CheckInitialization()) return;
        AudioManager.Instance.PlaySound(cardMatchSound);
    }

    public void PlayCardMismatch()
    {
        if (!CheckInitialization()) return;
        AudioManager.Instance.PlaySound(cardMismatchSound);
    }

    public void PlayGameOver()
    {
        if (!CheckInitialization()) return;
        AudioManager.Instance.PlaySound(gameOverSound);
    }
} 