using UnityEngine;

public class GenrateGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int p_rows = 2;
    [SerializeField] private int p_columns = 3;
    [SerializeField] private float p_spacing = 0.5f;
    [SerializeField] private float p_cardWidth = 1f;
    [SerializeField] private float p_cardHeight = 1f;

    [Header("Game Settings")]
    [SerializeField] private GameManager gameManager;

    private float p_totalWidth;
    private float p_totalHeight;
    private Vector3 p_startPosition;

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        CalculateGridDimensions();
        CalculateStartPosition();
        CreateGrid();
        AdjustCameraToFitGrid();
    }

    private void CalculateGridDimensions()
    {
        p_totalWidth = (p_columns * p_cardWidth) + ((p_columns - 1) * p_spacing);
        p_totalHeight = (p_rows * p_cardHeight) + ((p_rows - 1) * p_spacing);
    }

    private void CalculateStartPosition()
    {
        float startX = -(p_totalWidth / 2) + (p_cardWidth / 2);
        float startY = (p_totalHeight / 2) - (p_cardHeight / 2);
        p_startPosition = transform.position + new Vector3(startX, startY, 0f);
    }

    private void CreateGrid()
    {
        int totalCards = p_rows * p_columns;
        if (totalCards % 2 != 0)
        {
            Debug.LogError("Grid dimensions must create an even number of cards for matching!");
            return;
        }

        Sprite[] randomizedShapes = gameManager.GetRandomizedShapePairs(totalCards / 2);
        gameManager.SetTotalPairs(totalCards / 2);
        int cardIndex = 0;

        for (int row = 0; row < p_rows; row++)
        {
            for (int col = 0; col < p_columns; col++)
            {
                CreateCard(row, col, randomizedShapes[cardIndex], cardIndex);
                cardIndex++;
            }
        }
    }

    private void CreateCard(int row, int col, Sprite shape, int cardIndex)
    {
        Vector3 position = p_startPosition + new Vector3(
            col * (p_cardWidth + p_spacing),
            -row * (p_cardHeight + p_spacing),
            0f
        );

        GameObject cardObject = Instantiate(cardPrefab, position, Quaternion.identity, transform);
        cardObject.name = $"Card_{row}_{col}";

        Card card = cardObject.GetComponent<Card>();
        if (card != null)
        {
            int shapeId = gameManager.GetShapeIdForCard(cardIndex);
            card.SetCard(shape, shapeId);
            Debug.Log($"Created Card at {row},{col} with ShapeID: {shapeId} and Sprite: {shape.name}");
        }
    }

    private void AdjustCameraToFitGrid()
    {
        if (Camera.main == null) return;

        float gridAspect = p_totalWidth / p_totalHeight;
        float screenAspect = (float)Screen.width / Screen.height;
        float orthoSize;

        if (screenAspect > gridAspect)
        {
            orthoSize = p_totalHeight / 2f + 1f; // Adding 1 unit padding
        }
        else
        {
            orthoSize = (p_totalWidth / screenAspect) / 2f + 1f;
        }

        Camera.main.orthographicSize = orthoSize;
    }
}
