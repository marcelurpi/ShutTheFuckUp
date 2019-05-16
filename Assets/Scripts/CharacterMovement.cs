using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Charater Move Speed")]
    [SerializeField] private float CharacterMoveSpeedMin;
    [SerializeField] private float CharacterMoveSpeedMax;

    [Header("Time To Change Direction")]
    [SerializeField] private float TimeToChangeDirectionMin;
    [SerializeField] private float TimeToChangeDirectionMax;

    private bool Entering = false;
    private bool Escaping = false;
    private float CharacterMoveSpeed;
    private float TimeToChangeDirection;
    private Vector2 CurrentDirection;

    private void Start()
    {
        CharacterMoveSpeed = UnityEngine.Random.Range(CharacterMoveSpeedMin, CharacterMoveSpeedMax);
        TimeToChangeDirection = UnityEngine.Random.Range(TimeToChangeDirectionMin, TimeToChangeDirectionMax);
    }

    private void Update()
    {
        if (!Escaping && !Entering)
        {
            MoveCharacter();
        }
    }

    private void MoveCharacter()
    {
        transform.position = transform.position + (Vector3)CurrentDirection * CharacterMoveSpeed * Time.deltaTime;
        if (Mathf.Abs(transform.position.x) > 6.5f || Mathf.Abs(transform.position.y + 0.5f) > 2f)
        {
            CurrentDirection = -transform.position.normalized;
            StopCoroutine(ChangeDirection());
            StartCoroutine(ChangeDirection(1.5f));
        }
    }

    private IEnumerator ChangeDirection(float multiplier = 1f)
    {
        if (!Escaping && !Entering)
        {
            yield return new WaitForSeconds(TimeToChangeDirection * multiplier);
            CurrentDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            CurrentDirection = CurrentDirection.magnitude < 0.6f ? Vector2.zero : CurrentDirection;
            GetComponent<Animator>().SetBool("Walking", CurrentDirection != Vector2.zero);
            CharacterMoveSpeed = UnityEngine.Random.Range(CharacterMoveSpeedMin, CharacterMoveSpeedMax);
            TimeToChangeDirection = UnityEngine.Random.Range(TimeToChangeDirectionMin, TimeToChangeDirectionMax);
        }
        else
        {
            yield return null;
        }
        StartCoroutine(ChangeDirection());
    }

    public void EnterCenterArea(Action action)
    {
        GetComponent<Animator>().SetBool("Walking", true);
        Entering = true;
        Vector2 direction = -transform.position.normalized;
        StartCoroutine(EnterCoroutine(direction, () =>
        {
            GetComponent<Animator>().SetBool("Walking", false);
            action();
        }));
    }

    public void Escape(Action action)
    {
        GetComponent<Animator>().SetBool("Walking", true);
        Escaping = true;
        Vector2 direction = transform.position.normalized;
        StartCoroutine(EscapeCoroutine(direction, () =>
        {
            GetComponent<Animator>().SetBool("Walking", false);
            action();
        }));
    }

    private IEnumerator EnterCoroutine(Vector2 direction, Action action = null)
    {
        bool characterIsInViewArea = false;
        while (!characterIsInViewArea)
        {
            transform.position += (Vector3)direction * CharacterMoveSpeed * 2 * Time.deltaTime;
            characterIsInViewArea = transform.position.x > -6f && transform.position.x < 6f;
            characterIsInViewArea = characterIsInViewArea && (transform.position.y > -3f && transform.position.y < 3f);
            yield return null;
        }
        Entering = false;
        StartCoroutine(ChangeDirection(0.1f));
        if (action != null)
        {
            action();
        }
    }

    private IEnumerator EscapeCoroutine(Vector2 direction, Action action = null)
    {
        bool characterIsInViewArea = true;
        while (characterIsInViewArea)
        {
            transform.position += (Vector3)direction * CharacterMoveSpeed * 10 * Time.deltaTime;
            characterIsInViewArea = transform.position.x > -10f && transform.position.x < 10f;
            characterIsInViewArea = characterIsInViewArea && (transform.position.y > -5f && transform.position.y < 5f);
            yield return null;
        }
        StopCoroutine(ChangeDirection());
        Escaping = false;
        if (action != null)
        {
            action();
        }
    }

    public bool IsCharacterEscaping()
    {
        return Escaping;
    }
}
