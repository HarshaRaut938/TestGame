using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private Sprite[] shapeSprites;
    [SerializeField] private float revealTime = 0.5f;
    [SerializeField] private float previewTime = 2f;
    [SerializeField] private float matchDelay = 0.5f; 
    [SerializeField] private bool loadSaveGameOnStart = true;

    private enum GameState
    {
        Preview,
        Playing,
        GameOver
    }

    private GameState currentState;
    private List<Card> selectedCards = new List<Card>();
    private HashSet<Card> processingCards = new HashSet<Card>();
    private int matchesFound = 0;
    private int totalPairs;
    private GameAudio gameAudio;

    private void Awake()
    {
        gameAudio = FindObjectOfType<GameAudio>();
        if (gameAudio == null)
        {
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        if (loadSaveGameOnStart && GameSaveManager.Instance.HasSaveGame)
        {
            LoadGame();
        }
        else
        {
            currentState = GameState.Playing;
        }
    }

    private void LoadGame()
    {
        var (score, matches, attempts, bestScore) = GameSaveManager.Instance.LoadGameProgress();
        matchesFound = matches;
        ScoreHandler.Instance.RestoreState(score, attempts);   
        if (matchesFound >= totalPairs)
        {
            currentState = GameState.GameOver;
        }
        else
        {
            currentState = GameState.Playing;
        }
    }

    private void SaveGame()
    {
        if (currentState != GameState.Preview)
        {
            GameSaveManager.Instance.SaveGameProgress(
                ScoreHandler.Instance.CurrentScore,
                matchesFound,
                ScoreHandler.Instance.TotalAttempts
            );
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void OnCardClicked(Card clickedCard)
    {
        if (currentState == GameState.Preview || currentState == GameState.GameOver)
        {
            return;
        }
        if (processingCards.Contains(clickedCard))
        {
            return;
        }
        if (clickedCard.IsRevealed)
        {
            return;
        }
        clickedCard.RevealCard();
        selectedCards.Add(clickedCard);
        if (selectedCards.Count == 2)
        {
            StartCoroutine(ProcessMatchAsync(selectedCards[0], selectedCards[1]));
            selectedCards.Clear();
        }
    }

    private IEnumerator ProcessMatchAsync(Card firstCard, Card secondCard)
    {
        processingCards.Add(firstCard);
        processingCards.Add(secondCard);
        yield return new WaitForSeconds(matchDelay);

        bool isMatch = false;
        if (firstCard != null && secondCard != null)
        {
            isMatch = firstCard.ShapeId == secondCard.ShapeId;
             }

       
        ScoreHandler.Instance.ProcessMatchAttempt(isMatch);

        if (isMatch)
        {
            matchesFound++;

            // Play match animations
            firstCard.PlayMatchAnimation();
            secondCard.PlayMatchAnimation();

            if (matchesFound == totalPairs)
            {
                currentState = GameState.GameOver;
                gameAudio.PlayGameOver(); 
                SaveGame();
            }
        }
        else
        {
            gameAudio.PlayCardMismatch();
            StartCoroutine(HideCardsDelayed(firstCard, secondCard));
        }
        processingCards.Remove(firstCard);
        processingCards.Remove(secondCard);
        SaveGame();
    }

    private IEnumerator HideCardsDelayed(Card firstCard, Card secondCard)
    {
        yield return new WaitForSeconds(matchDelay);
        
        if (firstCard != null) firstCard.HideCard();
        if (secondCard != null) secondCard.HideCard();
    }

    public void SetTotalPairs(int pairs)
    {
        totalPairs = pairs;
        matchesFound = 0;
        selectedCards.Clear();
        processingCards.Clear();
        currentState = GameState.Preview;
        ScoreHandler.Instance.ResetScore();
        StartCoroutine(StartGameAfterPreview());
    }

    public IEnumerator StartGameAfterPreview()
    {
         yield return new WaitForSeconds(previewTime);
        currentState = GameState.Playing;
    }

    public void RestartGame()
    {
        GameSaveManager.Instance.ClearSaveGame();
        SetTotalPairs(totalPairs);
    }

    public class ShapeData
    {
        public Sprite sprite;
        public int shapeId;

        public ShapeData(Sprite sprite, int id)
        {
            this.sprite = sprite;
            this.shapeId = id;
        }
    }

    public Sprite[] GetRandomizedShapePairs(int pairCount)
    {
        List<ShapeData> shapePairs = new List<ShapeData>();
        for (int i = 0; i < pairCount; i++)
        {
            int spriteIndex = i % shapeSprites.Length;
            shapePairs.Add(new ShapeData(shapeSprites[spriteIndex], spriteIndex));
            shapePairs.Add(new ShapeData(shapeSprites[spriteIndex], spriteIndex));
         }

      
        for (int i = shapePairs.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            var temp = shapePairs[i];
            shapePairs[i] = shapePairs[randomIndex];
            shapePairs[randomIndex] = temp;
        }

        Sprite[] randomizedSprites = new Sprite[shapePairs.Count];
        for (int i = 0; i < shapePairs.Count; i++)
        {
            randomizedSprites[i] = shapePairs[i].sprite;
            
         }
        cardShapeIds = new int[shapePairs.Count];
        for (int i = 0; i < shapePairs.Count; i++)
        {
            cardShapeIds[i] = shapePairs[i].shapeId;
        }

        return randomizedSprites;
    }

    private int[] cardShapeIds;

    public int GetShapeIdForCard(int cardIndex)
    {
        if (cardShapeIds != null && cardIndex < cardShapeIds.Length)
        {
            return cardShapeIds[cardIndex];
        }
        return -1;
    }
} 