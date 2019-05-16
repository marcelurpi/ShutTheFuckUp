using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private int StartingPopularityPercentage;
    [SerializeField] private int PopularityPercentageChange;
    [SerializeField] private int RoundStartEnemyCount;
    [SerializeField] private float CharacterSpawnDelayStart;
    [SerializeField] private float CharacterSpawnDelayDecrease;
    [SerializeField] private GameObject CharacterPrefab1;
    [SerializeField] private GameObject CharacterPrefab2;

    private float CharacterSpawnDelayMin = 2f;
    private int MaxCharacterCapInScreen = 50;
    private int CharacterCount = 0;
    private int CharacterLayer = 0;
    private int PopularityPercentage;
    private float CharacterSpawnDelay;
    private RoundHandler roundHandler;
    private List<GameObject> CharacterList = new List<GameObject>();

    private void Start()
    {
        PopularityPercentage = StartingPopularityPercentage;
        roundHandler = FindObjectOfType<RoundHandler>();
        roundHandler.OnRoundStart += OnRoundStart;
        roundHandler.OnRoundEnd += OnRoundEnd;
    }

    private void OnRoundStart()
    {
        CharacterSpawnDelay = CharacterSpawnDelayStart;
        SpawnCharacters(RoundStartEnemyCount);
        StartCoroutine(SpawnRegularly());
    }

    private void OnRoundEnd()
    {
        StartCoroutine(RemoveAllCharacters(() =>
        {
            StartCoroutine(roundHandler.StartRound());
            //RoundStartEnemyCount++;
        }));
    }

    private IEnumerator SpawnRegularly()
    {
        if (roundHandler.IsRoundRunning())
        {
            yield return new WaitForSeconds(CharacterSpawnDelay);
            if (roundHandler.IsRoundRunning())
            {
                SpawnCharacters(1);
            }
        }
        else
        {
            yield return null;
        }
        StartCoroutine(SpawnRegularly());
    }

    public void SpawnCharacters(int characterSpawnCount)
    {
        if (CharacterCount >= MaxCharacterCapInScreen) return;
        for (int i = 0; i < characterSpawnCount; i++)
        {
            GameObject character;
            if (CharacterList.Count > 0)
            {
                character = CharacterList[0];
                CharacterList.RemoveAt(0);
                character.SetActive(true);
            }
            else
            {
                GameObject chosenCharacter = UnityEngine.Random.Range(0, 2) == 0 ? CharacterPrefab1 : CharacterPrefab2;
                character = Instantiate(chosenCharacter, gameObject.transform);
                CharacterLayer++;
                character.GetComponent<SpriteRenderer>().sortingOrder = CharacterLayer;
            }
            CharacterCount++;
            PlaceCharacter(character, () =>
            {
                if (CharacterSpawnDelay > CharacterSpawnDelayMin)
                {
                    CharacterSpawnDelay -= CharacterSpawnDelayDecrease;
                }
                StartCoroutine(character.GetComponent<CharacterBubble>().ShowBubbleWithCooldown());
            });
        }
    }

    private void PlaceCharacter(GameObject character, Action action)
    {
        bool randomAxisIsX = UnityEngine.Random.Range(0, 2) == 1 ? true : false;
        if (randomAxisIsX)
        {
            float randomPositionX = UnityEngine.Random.Range(-10f, 10f);
            float randomPositionY = UnityEngine.Random.Range(0, 2) == 1 ? 5f : -5f;
            character.transform.position = new Vector3(randomPositionX, randomPositionY, 0f);
        }
        else
        {
            float randomPositionY = UnityEngine.Random.Range(-5f, 5f);
            float randomPositionX = UnityEngine.Random.Range(0, 2) == 1 ? 10f : -10f;
            character.transform.position = new Vector3(randomPositionX, randomPositionY, 0f);
        }
        character.GetComponent<CharacterMovement>().EnterCenterArea(action);
    }

    public void RemoveCharacter(GameObject character, Action action = null)
    {
        CharacterBubble characterBubble = character.GetComponent<CharacterBubble>();
        CharacterMovement characterMovement = character.GetComponent<CharacterMovement>();
        characterBubble.StopAllCoroutines();
        characterBubble.HideAllChatBubbles();
        characterMovement.StopAllCoroutines();
        if (!character.activeSelf)
        {
            action();
            return;
        }
        characterMovement.Escape(() =>
        {
            CharacterCount--;
            character.SetActive(false);
            CharacterList.Add(character);
            if (action != null)
            {
                action();
            }
        });
    }

    public IEnumerator RemoveAllCharacters(Action action)
    {
        int childCount = gameObject.transform.childCount;
        int removedCharacters = 0;
        for (int i = 0; i < childCount; i++)
        {
            RemoveCharacter(gameObject.transform.GetChild(i).gameObject, () =>
            {
                removedCharacters++;
            });
        }
        while (removedCharacters < childCount)
        {
            yield return null;
        }
        action();
    }

    public int GetPopularityPercentage()
    {
        return PopularityPercentage;
    }
}
