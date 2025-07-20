using UnityEngine;
using DG.Tweening;
using System;

public class CardAnimationController : MonoBehaviour
{
    [Header("Card References")]
    [SerializeField] private SpriteRenderer frontSprite;
    [SerializeField] private SpriteRenderer backSprite;

    [Header("Animation Settings")]
    [SerializeField] private float flipDuration = 0.3f;
    [SerializeField] private float clickAnimDuration = 0.1f;
    [SerializeField] private float matchAnimDuration = 0.2f;
    [SerializeField] private float destroyDuration = 0.8f;
    [SerializeField] private float previewDuration = 0.4f;
    [SerializeField] private float previewHoldTime = 0.5f; // Time to show the card during preview

    [Header("Energy Pulse Settings")]
    [SerializeField] private Color pulseColor = new Color(0f, 1f, 0.8f, 1f); // Cyan-ish glow
    [SerializeField] private float pulseIntensity = 1.5f;
    [SerializeField] private int pulseCount = 2;

    private bool isAnimating = false;
    private Sequence currentAnimation;
    private Material frontMaterial;
    private Material backMaterial;

    public bool IsAnimating => isAnimating;

    private void Awake()
    {
        // Create material instances to avoid affecting other cards
        if (frontSprite != null)
        {
            frontMaterial = new Material(frontSprite.material);
            frontSprite.material = frontMaterial;
        }
        if (backSprite != null)
        {
            backMaterial = new Material(backSprite.material);
            backSprite.material = backMaterial;
        }
    }

    private void OnDisable()
    {
        currentAnimation?.Kill();
        DOTween.Kill(transform);
    }

    public void PlayPreviewAnimation(Action onComplete = null)
    {
        if (isAnimating) return;
        isAnimating = true;

        // Ensure proper initial state
        frontSprite.gameObject.SetActive(true);
        backSprite.gameObject.SetActive(false);
        frontSprite.color = new Color(1f, 1f, 1f, 0f);

        currentAnimation = DOTween.Sequence()
            // Quick fade in
            .Append(frontSprite.DOFade(1f, previewDuration * 0.3f).SetEase(Ease.OutQuad))
            // Hold briefly
            .AppendInterval(previewHoldTime)
            // Quick fade out
            .Append(frontSprite.DOFade(0f, previewDuration * 0.3f).SetEase(Ease.InQuad))
            .OnComplete(() => {
                frontSprite.gameObject.SetActive(false);
                //backSprite.gameObject.SetActive(true);
                isAnimating = false;
                onComplete?.Invoke();
            });
    }

    private void CompleteAnimation(Action onComplete)
    {
        isAnimating = false;
        onComplete?.Invoke();
    }

    public void PlayClickAnimation(Action onComplete = null)
    {
        if (isAnimating) return;

        currentAnimation = DOTween.Sequence()
            .Append(transform.DOScale(1.1f, clickAnimDuration).SetEase(Ease.OutBack))
            .Append(transform.DOScale(1f, clickAnimDuration))
            .OnComplete(() => onComplete?.Invoke());
    }

    public void FlipToFront(Action onComplete = null)
    {
        if (isAnimating) return;
        isAnimating = true;

        // Ensure proper initial state
        frontSprite.gameObject.SetActive(true);
        frontSprite.color = new Color(1f, 1f, 1f, 0f);

        currentAnimation = DOTween.Sequence()
            .Append(frontSprite.DOFade(1f, flipDuration).SetEase(Ease.OutQuad))
            .Join(backSprite.DOFade(0f, flipDuration).SetEase(Ease.InQuad))
            .OnComplete(() => {
                backSprite.gameObject.SetActive(false);
                isAnimating = false;
                onComplete?.Invoke();
            });
    }

    public void FlipToBack(Action onComplete = null)
    {
        if (isAnimating) return;
        isAnimating = true;

        // Ensure proper initial state
        backSprite.gameObject.SetActive(true);
        backSprite.color = new Color(1f, 1f, 1f, 0f);

        currentAnimation = DOTween.Sequence()
            .Append(backSprite.DOFade(1f, flipDuration).SetEase(Ease.OutQuad))
            .Join(frontSprite.DOFade(0f, flipDuration).SetEase(Ease.InQuad))
            .OnComplete(() => {
                frontSprite.gameObject.SetActive(false);
                isAnimating = false;
                onComplete?.Invoke();
            });
    }

    public void PlayMatchAnimation(Action onComplete = null)
    {
        if (isAnimating) return;
        isAnimating = true;

        currentAnimation = DOTween.Sequence()
            .Append(transform.DOScale(1.2f, matchAnimDuration).SetEase(Ease.OutBack))
            .Join(transform.DORotate(new Vector3(0, 0, 360), matchAnimDuration * 2, RotateMode.FastBeyond360))
            .Append(transform.DOScale(1f, matchAnimDuration).SetEase(Ease.InBack))
            .OnComplete(() => CompleteAnimation(onComplete));
    }

    public void PlayDestroyAnimation(Action onComplete = null)
    {
        if (isAnimating) return;
        isAnimating = true;

        // Store original colors
        Color originalFrontColor = frontSprite.color;
        Color originalBackColor = backSprite.color;

        // Calculate the bright pulse color
        Color brightPulseColor = new Color(
            pulseColor.r * pulseIntensity,
            pulseColor.g * pulseIntensity,
            pulseColor.b * pulseIntensity,
            1f
        );

        float pulseDuration = destroyDuration / (pulseCount * 2);
        
        Sequence pulseSequence = DOTween.Sequence();

        // Create pulse effect
        for (int i = 0; i < pulseCount; i++)
        {
            // Pulse out
            pulseSequence.Append(DOTween.To(() => frontSprite.color, x => frontSprite.color = x,
                brightPulseColor, pulseDuration).SetEase(Ease.InOutSine));
            
            // Pulse in
            pulseSequence.Append(DOTween.To(() => frontSprite.color, x => frontSprite.color = x,
                originalFrontColor, pulseDuration).SetEase(Ease.InOutSine));
        }

        currentAnimation = DOTween.Sequence()
            // Initial scale up
            .Append(transform.DOScale(1.2f, destroyDuration * 0.2f).SetEase(Ease.OutBack))
            
            // Play pulse effect while spinning
            .Join(pulseSequence)
            .Join(transform.DORotate(new Vector3(0, 360, 0), destroyDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutSine))
            
            // Final shrink and fade
            .Append(transform.DOScale(0f, destroyDuration * 0.3f).SetEase(Ease.InBack))
            .Join(DOTween.To(() => frontSprite.color, x => frontSprite.color = x,
                new Color(brightPulseColor.r, brightPulseColor.g, brightPulseColor.b, 0f),
                destroyDuration * 0.3f))
            
            .OnComplete(() => {
                // Reset colors
                frontSprite.color = originalFrontColor;
                backSprite.color = originalBackColor;
                CompleteAnimation(onComplete);
            });
    }

    private void OnDestroy()
    {
        // Clean up material instances
        if (frontMaterial != null)
        {
            Destroy(frontMaterial);
        }
        if (backMaterial != null)
        {
            Destroy(backMaterial);
        }
    }
} 