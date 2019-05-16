using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBubble : MonoBehaviour
{
    [SerializeField] private GameObject ChatBubblePrefab;
    [SerializeField] private GameObject BubbleTextPrefab;

    [Header("Show Cooldown")]
    [SerializeField] private float MinRandomBubbleShowCooldown;
    [SerializeField] private float MaxRandomBubbleShowCooldown;

    [Header("Hide Cooldown")]
    [SerializeField] private float MinRandomBubbleHideCooldown;
    [SerializeField] private float MaxRandomBubbleHideCooldown;

    private Color[] ChatBubbleColors = new Color[] { Color.red, new Color(0, 0.75f, 0) };

    private bool ShowingBubble = true;
    private GameObject ActiveBubble = null;
    private bool ActivebubbleIsFollower = false;
    private Vector3 BigCharacterScale;
    private Vector3 SmallCharacterScale;
    private Vector3 BigBubbleScale = Vector3.one;
    private Vector3 SmallbubbleScale = Vector3.one;
    private GameObject BubbleParent = null;

    private void Start()
    {
        BigCharacterScale = transform.localScale * 1.2f;
        SmallCharacterScale = transform.localScale;
    }

    private void Update()
    {
        HandleMouseClick();
        if (ActiveBubble != null)
        {
            UpdateBubblePosition();
        }
    }

    public IEnumerator ShowBubbleWithCooldown()
    {
        float randomBubbleCooldown = UnityEngine.Random.Range(MinRandomBubbleShowCooldown, MinRandomBubbleShowCooldown);
        if (FindObjectOfType<RoundHandler>().GetRoundTimer() - randomBubbleCooldown > 0.5f) {
            yield return new WaitForSeconds(randomBubbleCooldown);
            ShowingBubble = true;
            int percentage = FindObjectOfType<CharacterManager>().GetPopularityPercentage();
            TextLoader.BubbleText randomText = TextLoader.GetRandomBubbleTextWithPercentage(percentage);
            if(!ShowChatBubble(randomText.isFollower, randomText.text))
            {
                CreateChatBubbles();
                ShowChatBubble(randomText.isFollower, randomText.text);
            }
            if (gameObject.activeSelf)
            {
                StartCoroutine(HideBubbleWithCooldown());
            }
        }
    }

    private IEnumerator HideBubbleWithCooldown()
    {
        float randomBubbleCooldown = UnityEngine.Random.Range(MinRandomBubbleHideCooldown, MaxRandomBubbleHideCooldown);
        yield return new WaitForSeconds(randomBubbleCooldown);
        ShowingBubble = false;
        if (ActiveBubble != null && !ActivebubbleIsFollower)
        {
            FindObjectOfType<GameHandler>().UpdatePointsCount(-100);
            FindObjectOfType<GameHandler>().ShowAnnouncerText("A dissident escaped, so you have to pay 100$", Color.red);
        }
        HideAllChatBubbles();
        if (gameObject.activeSelf)
        {
            StartCoroutine(ShowBubbleWithCooldown());
        }
    }

    private void HandleMouseClick()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        bool hittingGameObject = hit.collider != null && (hit.collider.gameObject == gameObject || hit.collider.gameObject == ActiveBubble);
        if (hittingGameObject && ShowingBubble)
        {
            transform.localScale = BigCharacterScale;
            if (ActiveBubble != null) {
                ActiveBubble.GetComponent<RectTransform>().sizeDelta = BigBubbleScale;
                ActiveBubble.transform.GetChild(0).GetComponent<Text>().fontSize = 30;
                if (Input.GetMouseButtonDown(0))
                {
                    int points = !ActivebubbleIsFollower ? 10 : -50;
                    transform.localScale = SmallCharacterScale;
                    ActiveBubble.GetComponent<RectTransform>().sizeDelta = SmallbubbleScale;
                    ActiveBubble.transform.GetChild(0).GetComponent<Text>().fontSize = 20;
                    if(ActivebubbleIsFollower)
                    {
                        FindObjectOfType<GameHandler>().ShowAnnouncerText("You hit a follower, so you have to pay 50$", Color.red);
                    }
                    FindObjectOfType<CharacterManager>().RemoveCharacter(gameObject);
                    FindObjectOfType<GameHandler>().UpdatePointsCount(points);
                }
            }
        }
        else
        {
            if(ActiveBubble != null)
            {
                ActiveBubble.GetComponent<RectTransform>().sizeDelta = SmallbubbleScale;
                ActiveBubble.transform.GetChild(0).GetComponent<Text>().fontSize = 20;
            }
            transform.localScale = SmallCharacterScale;
        }
    }

    private void UpdateBubblePosition()
    {
        string bubbleText = ActiveBubble.transform.GetChild(0).GetComponent<Text>().text;
        Vector3 newPosition = new Vector3(25 + transform.position.x * 108, 25 + transform.position.y * 108);
        ActiveBubble.GetComponent<RectTransform>().localPosition = newPosition;
    }

    private bool ShowChatBubble(bool isFollower, string bubbleText)
    {
        if (BubbleParent == null) return false;
        HideAllChatBubbles();
        GetComponent<Animator>().SetBool("Talking", true);
        for (int i = 0; i < BubbleParent.transform.childCount; i++)
        {
            GameObject bubble = BubbleParent.transform.GetChild(i).gameObject;
            if((i == 0 && isFollower) || (i == 1 && !isFollower))
            {
                bubble.transform.GetChild(0).GetComponent<Text>().text = bubbleText;
                bubble.transform.GetChild(0).GetComponent<Text>().fontSize = 20;
                bubble.GetComponent<RectTransform>().sizeDelta = new Vector3(20 + 10 * bubbleText.Length, 50, 1);
                bubble.GetComponent<BoxCollider2D>().size = new Vector3(20 + 10 * bubbleText.Length, 50, 1);
                SmallbubbleScale = bubble.GetComponent<RectTransform>().sizeDelta;
                BigBubbleScale = 1.5f * bubble.GetComponent<RectTransform>().sizeDelta;
                ActiveBubble = bubble;
                ActivebubbleIsFollower = isFollower;
                UpdateBubblePosition();
                bubble.SetActive(true);
                ShowingBubble = true;
                return true;
            }
        }
        return false;
    }

    [ContextMenu("Show Red Bubble")]
    private void ShowRedBubble()
    {
        if (!ShowChatBubble(false, "The Leader Sucks!"))
        {
            CreateChatBubbles();
            ShowChatBubble(false, "The Leader Sucks!");
        }
    }

    [ContextMenu("Show Green Bubble")]
    private void ShowGreenBubble()
    {
        if (!ShowChatBubble(true, "The Leader is Great"))
        {
            CreateChatBubbles();

            ShowChatBubble(true, "The Leader is Great");
        }
    }

    [ContextMenu("Create Bubbles")]
    private void CreateChatBubbles()
    {
        DestroyChatBubbles();
        GameObject bubbleCanvas = FindObjectOfType<Canvas>().transform.GetChild(0).gameObject;
        GameObject chatBubbleParent = new GameObject("Chat Bubble Parent", typeof(RectTransform));
        BubbleParent = chatBubbleParent;
        chatBubbleParent.transform.SetParent(bubbleCanvas.transform);
        chatBubbleParent.GetComponent<RectTransform>().localPosition = Vector3.zero;
        chatBubbleParent.GetComponent<RectTransform>().localScale = Vector3.one;
        GameObject chatBubble1 = Instantiate(ChatBubblePrefab, chatBubbleParent.transform);
        GameObject bubbleText1 = Instantiate(BubbleTextPrefab, chatBubble1.transform);
        GameObject chatBubble2 = Instantiate(ChatBubblePrefab, chatBubbleParent.transform);
        GameObject bubbleText2 = Instantiate(BubbleTextPrefab, chatBubble2.transform);
        chatBubble1.SetActive(false);
        chatBubble2.SetActive(false);
    }

    [ContextMenu("Destroy Bubbles")]
    private void DestroyChatBubbles()
    {
        ActiveBubble = null;
        DestroyImmediate(BubbleParent);
        BubbleParent = null;
        ShowingBubble = false;
    }

    [ContextMenu("Hide All Bubbles")]
    public void HideAllChatBubbles()
    {
        GetComponent<Animator>().SetBool("Talking", false);
        ActiveBubble = null;
        if (BubbleParent == null) return;
        for (int i = 0; i < BubbleParent.transform.childCount; i++)
        {
            GameObject child = BubbleParent.transform.GetChild(i).gameObject;
            if (child.activeSelf)
            {
                child.SetActive(false);
            }
        }
        ShowingBubble = false;
    }
}
