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
    [SerializeField] private float destroyDuration = 0.3f;
    [SerializeField] private float previewDuration = 0.4f;

    private bool isAnimating = false;
    private Sequence currentAnimation;

    public bool IsAnimating => isAnimating;

    private void Awake()
    {

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
        frontSprite.gameObject.SetActive(true);
        backSprite.gameObject.SetActive(false);

        currentAnimation = DOTween.Sequence();
        currentAnimation.Append(frontSprite.DOFade(1f, previewDuration * 0.3f).SetEase(Ease.InOutSine))
            .Join(frontSprite.transform.DOScale(1.2f, previewDuration * 0.3f).SetEase(Ease.OutBack))
            .AppendInterval(0.2f)
           .Append(frontSprite.DOFade(0f, previewDuration * 0.4f).SetEase(Ease.InOutSine))
            .Join(frontSprite.transform.DOScale(1f, previewDuration * 0.4f))
           .OnComplete(() => {
                frontSprite.gameObject.SetActive(false);
                CompleteAnimation(onComplete);
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
        currentAnimation = DOTween.Sequence()
            .OnStart(() => {
                frontSprite.gameObject.SetActive(true);
                frontSprite.color = new Color(1, 1, 1, 0);
            })
            .Append(frontSprite.DOFade(1, flipDuration).SetEase(Ease.InOutSine))
            .Join(backSprite.DOFade(0, flipDuration).SetEase(Ease.InOutSine))
            .OnComplete(() => {
                backSprite.gameObject.SetActive(false);
                CompleteAnimation(onComplete);
            });
    }

    public void FlipToBack(Action onComplete = null)
    {
        if (isAnimating) return;
        isAnimating = true;
        currentAnimation = DOTween.Sequence()
            .OnStart(() => {
                backSprite.gameObject.SetActive(true);
                backSprite.color = new Color(1, 1, 1, 0);
            })
            .Append(backSprite.DOFade(1, flipDuration).SetEase(Ease.InOutSine))
            .Join(frontSprite.DOFade(0, flipDuration).SetEase(Ease.InOutSine))
            .OnComplete(() => {
                frontSprite.gameObject.SetActive(false);
                CompleteAnimation(onComplete);
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

        currentAnimation = DOTween.Sequence()
            .Join(transform.DOScale(0, destroyDuration).SetEase(Ease.InBack))
            .Join(transform.DORotate(new Vector3(0, 360, 0), destroyDuration, RotateMode.FastBeyond360))
            .Join(frontSprite.DOFade(0, destroyDuration))
            .Join(backSprite.DOFade(0, destroyDuration))
            .OnComplete(() => CompleteAnimation(onComplete));
    }
} 