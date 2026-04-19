using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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

    float elapsedTimeSinceLastMove, timeToMove;
    Vector3 targetPosition;
    bool isMoving, isDancing;

    int danceBeatIndex;

    void Start()
    {
        elapsedTimeSinceLastMove = 0;
        timeToMove = Random.Range(durationRangeBeforeMove.x, durationRangeBeforeMove.y);
        isMoving = false;

        MusicController.OnBeat += OnBeat;

        //StartDancing();
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
        animator.SetBool("isMoving", false);
        elapsedTimeSinceLastMove = 0;
        isMoving = false;
    }

    Vector3 GetRandomPointOnQuad(Transform quad)
    {
        float x = Random.Range(-0.5f, 0.5f);
        float y = Random.Range(-0.5f, 0.5f);

        Vector3 localPoint = new Vector3(x, y, 0f);

        return quad.TransformPoint(localPoint);
    }

    void StartDancing()
    {
        spriteRenderer.sprite = spriteDances[Random.Range(0, spriteDances.Count - 1)];
        isDancing = true;
        StopMoving();
        danceBeatIndex = Random.Range(0, 2);
    }

    void OnBeat()
    {
        if (isDancing)
        {
            spriteRenderer.sprite = spriteDances[Random.Range(0, spriteDances.Count - 1)];
            animator.SetTrigger("danceBeat" + (danceBeatIndex + 1));
            danceBeatIndex++;
            danceBeatIndex %= 2;
        }
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
    }

    void OnDestroy()
    {
        MusicController.OnBeat -= OnBeat;
    }
}
