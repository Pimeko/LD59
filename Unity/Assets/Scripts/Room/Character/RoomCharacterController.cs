using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class RoomCharacterController : MonoBehaviour
{
    [SerializeField]
    Animator animator;
    [SerializeField]
    Vector2 durationRangeBeforeMove;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    List<Sprite> spriteDances;
    [SerializeField]
    GameObject bubbleGO;
    [SerializeField]
    TMP_Text bubbleText;
    [SerializeField]
    List<string> textsNoise, textsFast, textsSlow, textsVolume, textsWin, textsFail;
    [SerializeField]
    float durationBubble;

    public enum BubbleType
    {
        NOISE,
        FAST,
        SLOW,
        VOLUME,
        WIN,
        FAIL,
    }

    float elapsedTimeSinceLastMove, timeToMove;
    Vector3 targetPosition;
    bool isMoving, isDancing;

    float elapsedTimeBubble;
    bool isShowingBubble;
    BubbleType bubbleType;

    Sprite originalSprite;
    int danceBeatIndex;

    Vector3 originalPosition;
    bool isLeaving;

    void Start()
    {
        originalSprite = spriteRenderer.sprite;
        originalPosition = transform.position;
        isLeaving = false;

        elapsedTimeSinceLastMove = 0;
        timeToMove = UnityEngine.Random.Range(durationRangeBeforeMove.x, durationRangeBeforeMove.y);
        MusicController.OnBeat += OnBeat;
        GameManager.Instance.OnAllKnobsOK += StartDancing;
        GameManager.Instance.OnAllKnobsNotOK += StopDancing;
        GameManager.Instance.OnNextMusic += StopDancing;

        StartMoving();
    }

    void StartMoving()
    {
        animator.SetBool("isMoving", true);
        targetPosition = GetRandomPointOnQuad(GameManager.Instance.characterZone);
        isMoving = true;
    }

    void Move()
    {
        Vector3 toTarget = targetPosition - transform.position;
        float distance = toTarget.magnitude;

        if (distance <= 0.001f)
        {
            StopMoving();
            return;
        }

        Vector3 direction = toTarget.normalized;
        float moveAmount = moveSpeed * Time.deltaTime;

        if (moveAmount >= distance)
            transform.position = targetPosition;
        else
            transform.position += direction * moveAmount;
    }

    void StopMoving()
    {
        if (isLeaving)
            Destroy(gameObject);
        animator.SetBool("isMoving", false);
        elapsedTimeSinceLastMove = 0;
        isMoving = false;
    }

    Vector3 GetRandomPointOnQuad(Transform quad)
    {
        float x = UnityEngine.Random.Range(-0.5f, 0.5f);
        float y = UnityEngine.Random.Range(-0.5f, 0.5f);

        Vector3 localPoint = new Vector3(x, y, 0f);

        return quad.TransformPoint(localPoint);
    }

    void StartDancing()
    {
        if (isLeaving)
            return;

        spriteRenderer.sprite = spriteDances[UnityEngine.Random.Range(0, spriteDances.Count)];
        isDancing = true;
        StopMoving();
        danceBeatIndex = UnityEngine.Random.Range(0, 2);
    }

    void StopDancing()
    {
        if (isShowingBubble && bubbleType == BubbleType.WIN)
            HideBubble();
        spriteRenderer.sprite = originalSprite;
        isDancing = false;
    }

    void OnBeat()
    {
        if (isDancing)
        {
            spriteRenderer.sprite = spriteDances[UnityEngine.Random.Range(0, spriteDances.Count)];
            animator.SetTrigger("danceBeat" + (danceBeatIndex + 1));
            danceBeatIndex++;
            danceBeatIndex %= 2;
        }
    }

    public void ShowBubble(BubbleType type)
    {
        bubbleGO.SetActive(true);
        List<string> texts = null;
        bubbleType = type;
        switch (type)
        {
            case BubbleType.NOISE:
                texts = textsNoise;
                break;
            case BubbleType.FAST:
                texts = textsFast;
                break;
            case BubbleType.SLOW:
                texts = textsSlow;
                break;
            case BubbleType.VOLUME:
                texts = textsVolume;
                break;
            case BubbleType.WIN:
                texts = textsWin;
                break;
            case BubbleType.FAIL:
                texts = textsFail;
                break;
        }
        bubbleText.text = texts[UnityEngine.Random.Range(0, texts.Count)];
        isShowingBubble = true;
        elapsedTimeBubble = 0;
    }

    void HideBubble()
    {
        bubbleGO.SetActive(false);
        isShowingBubble = false;
    }

    public void Leave()
    {
        ShowBubble(BubbleType.FAIL);
        animator.SetBool("isMoving", true);
        targetPosition = originalPosition;
        isMoving = true;
        isLeaving = true;
    }

    void Update()
    {
        if (isMoving)
            Move();
        else if (!isDancing)
        {
            elapsedTimeSinceLastMove += Time.deltaTime;
            if (elapsedTimeSinceLastMove > timeToMove)
                StartMoving();
        }

        if (isShowingBubble)
        {
            elapsedTimeBubble += Time.deltaTime;
            if (elapsedTimeBubble > durationBubble)
            {
                HideBubble();
            }
        }
    }

    void OnDestroy()
    {
        MusicController.OnBeat -= OnBeat;
        GameManager.Instance.OnAllKnobsOK -= StartDancing;
        GameManager.Instance.OnAllKnobsNotOK -= StopDancing;
        GameManager.Instance.OnNextMusic -= StopDancing;
    }
}
