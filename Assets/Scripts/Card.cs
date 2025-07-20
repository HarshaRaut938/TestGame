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
    private bool isInteractable = false;
    private bool isDestroyed = false;
    private GameAudio gameAudio;

    private void Awake()
    {
        if (cardCollider == null) cardCollider = GetComponent<BoxCollider2D>();
        if (frontSprite == null) frontSprite = transform.GetComponent<SpriteRenderer>();
        if (backSprite == null) backSprite = transform.GetComponent<SpriteRenderer>();
        if (animationController == null) animationController = GetComponent<CardAnimationController>();
        
        gameAudio = FindObjectOfType<GameAudio>();
        
        if (frontSprite == null || backSprite == null || animationController == null || gameAudio == null)
        {
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            enabled = false;
            return;
        }
    }

    private void OnMouseDown()
    {
        if (!enabled) return;

        if (!isInteractable || isDestroyed || animationController.IsAnimating || gameManager == null)
        {
            return;
        }
        gameManager.OnCardClicked(this);
    }

    public int ShapeId => shapeId;
    public bool IsRevealed => isRevealed;

    public void SetCard(Sprite shape, int id)
    {
        if (!enabled) return;

        frontSprite.sprite = shape;
        shapeId = id;
        isDestroyed = false;
        isInteractable = false;
        frontSprite.gameObject.SetActive(false);
        backSprite.gameObject.SetActive(true);
        
        ShowPreview();
    }

    private void ShowPreview()
    {
        if (!enabled) return;
        isRevealed = true;
        frontSprite.gameObject.SetActive(true);
        backSprite.gameObject.SetActive(false);
        
        animationController.PlayPreviewAnimation(() => {
            HideCard();
            isInteractable = true;
        });
    }

    public void RevealCard()
    {
        if (!enabled || isDestroyed || animationController.IsAnimating) return;
        isRevealed = true;
        isInteractable = false;
        
        frontSprite.gameObject.SetActive(true);
        backSprite.gameObject.SetActive(true);
        
        gameAudio.PlayCardFlip();
        animationController.FlipToFront(() => {
            backSprite.gameObject.SetActive(false);
            isInteractable = true;
        });
    }

    public void HideCard()
    {
        if (!enabled || isDestroyed || animationController.IsAnimating) return;
        isRevealed = false;
        isInteractable = false;
        
        frontSprite.gameObject.SetActive(true);
        backSprite.gameObject.SetActive(true);
        
        gameAudio.PlayCardFlip();
        animationController.FlipToBack(() => {
            frontSprite.gameObject.SetActive(false);
            isInteractable = true;
        });
    }

    public void PlayMatchAnimation()
    {
        if (!enabled || isDestroyed) return;
        gameAudio.PlayCardMatch();
        animationController.PlayMatchAnimation(() => {
            DisableCard();
            if (gameManager != null)
            {
                gameManager.OnCardAnimationComplete();
            }
        });
    }

    public void DisableCard()
    {
        if (!enabled) return;

        isDestroyed = true;
        isInteractable = false;
        
        if (cardCollider != null)
        {
            cardCollider.enabled = false;
        }
        
        animationController.PlayDestroyAnimation(() => {
            Destroy(gameObject);
        });
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
} 