using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private Sprite[] shapeSprites;
    [SerializeField] private float revealTime = 0.5f;
    [SerializeField] private float previewTime = 2f;
    [SerializeField] private float matchDelay = 1f;

    private enum GameState
    {
        Preview,
        WaitingForFirst,
        WaitingForSecond,
        Processing,
        GameOver
    }

    private GameState currentState;
    private Card firstCard;
    private Card secondCard;
    private int matchesFound = 0;
    private int totalPairs;
    private bool isProcessing = false;

    private void Start()
    {
        currentState = GameState.Preview;
        StartCoroutine(StartGameAfterPreview());
    }

    private IEnumerator StartGameAfterPreview()
    {
        yield return new WaitForSeconds(previewTime);
        currentState = GameState.WaitingForFirst;
    }

    public void OnCardClicked(Card clickedCard)
    {
        // Prevent any clicks during processing
        if (isProcessing || currentState == GameState.Preview || currentState == GameState.GameOver)
        {
            Debug.Log($"Card click ignored - State: {currentState}, Processing: {isProcessing}");
            return;
        }

        // Prevent clicking already revealed cards
        if (clickedCard.IsRevealed)
        {
            Debug.Log("Card already revealed - click ignored");
            return;
        }

        // Prevent clicking the same card
        if (clickedCard == firstCard)
        {
            Debug.Log("Same card clicked - ignored");
            return;
        }

        Debug.Log($"Card clicked - ShapeID: {clickedCard.ShapeId}");
        clickedCard.RevealCard();

        if (firstCard == null)
        {
            // First card selection
            firstCard = clickedCard;
            currentState = GameState.WaitingForSecond;
            Debug.Log($"First card selected - ShapeID: {firstCard.ShapeId}");
        }
        else
        {
            // Second card selection
            secondCard = clickedCard;
            Debug.Log($"Second card selected - ShapeID: {secondCard.ShapeId}");
            StartCoroutine(ProcessMatch());
        }
    }

    private IEnumerator ProcessMatch()
    {
        isProcessing = true;
        currentState = GameState.Processing;
        Debug.Log("Processing match...");

        // Wait for cards to be fully revealed
        yield return new WaitForSeconds(matchDelay);

        bool isMatch = false;

        // Verify both cards still exist and have the same ShapeId
        if (firstCard != null && secondCard != null)
        {
            isMatch = firstCard.ShapeId == secondCard.ShapeId;
            Debug.Log($"Match check - First card ID: {firstCard.ShapeId}, Second card ID: {secondCard.ShapeId}, Is Match: {isMatch}");
        }

        if (isMatch)
        {
            Debug.Log("Match found! Destroying cards...");
            matchesFound++;

            // Store references before destroying
            Card card1 = firstCard;
            Card card2 = secondCard;

            // Clear references first
            firstCard = null;
            secondCard = null;

            // Destroy the matched cards
            Destroy(card1.gameObject);
            Destroy(card2.gameObject);

            if (matchesFound == totalPairs)
            {
                Debug.Log("Game Over - All matches found!");
                currentState = GameState.GameOver;
            }
            else
            {
                currentState = GameState.WaitingForFirst;
            }
        }
        else
        {
            Debug.Log("No match - hiding cards");
            // Hide both cards
            if (firstCard != null) firstCard.HideCard();
            if (secondCard != null) secondCard.HideCard();

            // Clear references
            firstCard = null;
            secondCard = null;
            currentState = GameState.WaitingForFirst;
        }

        isProcessing = false;
    }

    public void SetTotalPairs(int pairs)
    {
        totalPairs = pairs;
        matchesFound = 0;
        currentState = GameState.Preview;
        Debug.Log($"Game initialized with {pairs} pairs");
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
        // Create a list to hold our pairs with their IDs
        List<ShapeData> shapePairs = new List<ShapeData>();
        
        // Create pairs ensuring same sprites get same IDs
        for (int i = 0; i < pairCount; i++)
        {
            int spriteIndex = i % shapeSprites.Length;
            // Both cards of the pair get the spriteIndex as their ID
            shapePairs.Add(new ShapeData(shapeSprites[spriteIndex], spriteIndex));
            shapePairs.Add(new ShapeData(shapeSprites[spriteIndex], spriteIndex));
            Debug.Log($"Created pair with sprite {shapeSprites[spriteIndex].name} and ID {spriteIndex}");
        }

        // Shuffle the pairs
        for (int i = shapePairs.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            var temp = shapePairs[i];
            shapePairs[i] = shapePairs[randomIndex];
            shapePairs[randomIndex] = temp;
        }

        // Convert to sprite array while maintaining the shape IDs
        Sprite[] randomizedSprites = new Sprite[shapePairs.Count];
        for (int i = 0; i < shapePairs.Count; i++)
        {
            randomizedSprites[i] = shapePairs[i].sprite;
            Debug.Log($"Position {i}: Sprite {shapePairs[i].sprite.name} with ID {shapePairs[i].shapeId}");
        }

        // Store the shape IDs for the cards to use
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
        Debug.LogError($"Invalid card index: {cardIndex}");
        return -1;
    }
} 