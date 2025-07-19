using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour
{
    [Header("Card Components")]
    [SerializeField] private SpriteRenderer frontSprite;
    [SerializeField] private SpriteRenderer backSprite;
    [SerializeField] private BoxCollider2D cardCollider;
    [SerializeField] private CardAnimationController animationController;

    private bool isRevealed = false;
    private int shapeId = -1;
    private GameManager gameManager;
    private bool isInteractable = true;
    private bool isDestroyed = false;

    private void Awake()
    {
        if (cardCollider == null) cardCollider = GetComponent<BoxCollider2D>();
        if (frontSprite == null) frontSprite = transform.GetComponent<SpriteRenderer>();
        if (backSprite == null) backSprite = transform.GetComponent<SpriteRenderer>();
        if (animationController == null) animationController = GetComponent<CardAnimationController>();
        if (frontSprite == null || backSprite == null || animationController == null)
        {
            Debug.LogError("Card: Missing required components! Please check the card prefab.");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("Card: GameManager not found in scene!");
            enabled = false;
            return;
        }
    }

    private void OnMouseDown()
    {
        if (isInteractable && !isDestroyed && !animationController.IsAnimating && gameManager != null)
        {
            animationController.PlayClickAnimation();
            gameManager.OnCardClicked(this);
        }
    }

    public int ShapeId => shapeId;
    public bool IsRevealed => isRevealed;

    public void SetCard(Sprite shape, int id)
    {
        frontSprite.sprite = shape;
        shapeId = id;
        isDestroyed = false;
        HideAfterDelay();
        ShowPreview();
    }

    private void ShowPreview()
    {
        isInteractable = false;
        animationController.PlayPreviewAnimation(() => StartCoroutine(HideAfterDelay()));
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(0f);
        if (!isDestroyed)
        {
            HideCard();
        }
    }

    public void RevealCard()
    {
        if (isDestroyed || animationController.IsAnimating) return;
        isRevealed = true;
        animationController.FlipToFront();
    }

    public void HideCard()
    {
        if (isDestroyed || animationController.IsAnimating) return;
        isRevealed = false;
        animationController.FlipToBack(() => isInteractable = true);
    }

    public void PlayMatchAnimation()
    {
        animationController.PlayMatchAnimation(() => DisableCard());
    }

    public void DisableCard()
    {
        isDestroyed = true;
        isInteractable = false;
        if (cardCollider != null)
        {
            cardCollider.enabled = false;
        }
        animationController.PlayDestroyAnimation();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
} 