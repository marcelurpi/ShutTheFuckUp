using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private GameObject PointsText;
    [SerializeField] private GameObject MainText;
    [SerializeField] private GameObject RoundText;
    [SerializeField] private GameObject RoundTimerText;
    [SerializeField] private GameObject AnnouncerText;
    [SerializeField] private GameObject RankEmblem;
    [SerializeField] private GameObject PopulaityIcon;
    [SerializeField] private Sprite[] rankEmblemSprites;
    [SerializeField] private Sprite[] PopularityIconSprites;
    [SerializeField] private int StartingPointsCount;

    private int PointsCount = 0;
    private bool GameStopped = false;

    private void Start()
    {
        MainText.SetActive(false);
        AnnouncerText.SetActive(false);
        PointsCount = StartingPointsCount;
        UpdatePointsText();
        FindObjectOfType<RoundHandler>().OnRoundStart += UpdateRoundText;
        FindObjectOfType<RoundHandler>().OnRoundStart += UpdateRankEmblem;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GameStopped)
        {
            GameStopped = false;
            MainText.SetActive(false);
            PointsCount = StartingPointsCount;
            UpdatePointsText();
            FindObjectOfType<RoundHandler>().StartRound(0, 0.25f);
        }
        UpdateRoundTimerText();
        if(AnnouncerText.activeSelf)
        {
            StartCoroutine(WaitAndHideAnnouncerText());
        }
    }

    public void UpdatePointsCount(int amount)
    {
        PointsCount += amount;
        CheckPlayerLost();
        UpdatePointsText();
        GameObject.Find("MoneySign").GetComponent<Animator>().SetInteger("Money", PointsCount);
    }

    private void UpdatePointsText()
    {
        PointsText.GetComponent<Text>().text = PointsCount.ToString();
    }

    public void UpdateRoundText()
    {
        int roundNum = FindObjectOfType<RoundHandler>().GetRoundNum();
        RoundText.GetComponent<Text>().text = roundNum.ToString();
        if (roundNum > 1)
        {
            MainText.GetComponent<Text>().text = "You got promoted!";
            MainText.GetComponent<Text>().color = Color.white;
            MainText.GetComponent<Text>().fontSize = 150;
            MainText.SetActive(true);
            StartCoroutine(WaitAndHideMainText());
        }
    }

    private IEnumerator WaitAndHideMainText()
    {
        yield return new WaitForSeconds(2f);
        MainText.SetActive(false);
    }

    public void UpdateRoundTimerText()
    {
        int roundTimer = FindObjectOfType<RoundHandler>().GetRoundTimer();
        RoundTimerText.GetComponent<Text>().text = roundTimer.ToString();
    }

    private void UpdateRankEmblem()
    {
        int emblemIndex = FindObjectOfType<RoundHandler>().GetRoundNum()-1;
        RankEmblem.GetComponent<Image>().sprite = rankEmblemSprites[emblemIndex];
    }

    private void UpdatePopularityIcon()
    {
        int emblemIndex = FindObjectOfType<RoundHandler>().GetRoundNum() - 1;
        RankEmblem.GetComponent<Image>().sprite = rankEmblemSprites[emblemIndex];
    }

    private void CheckPlayerLost()
    {
        if (PointsCount <= 0)
        {
            PointsCount = 0;
            FindObjectOfType<RoundHandler>().EndRound();
            StartCoroutine(FindObjectOfType<CharacterManager>().RemoveAllCharacters(() =>
            {
                MainText.GetComponent<Text>().text = "You Lost";
                MainText.GetComponent<Text>().color = Color.red;
                MainText.GetComponent<Text>().fontSize = 200;
                MainText.SetActive(true);
                GameStopped = true;
            }));
        }
    }

    public void ShowAnnouncerText(string text, Color color)
    {
        AnnouncerText.GetComponent<Text>().text = text;
        AnnouncerText.GetComponent<Text>().color = color;
        AnnouncerText.SetActive(true);
    }

    private IEnumerator WaitAndHideAnnouncerText()
    {
        yield return new WaitForSeconds(3f);
        AnnouncerText.SetActive(false);
    }

    [ContextMenu("Save Sample Texts")]
    private void SaveSampleTexts()
    {
        TextLoader.SaveSampleTexts();
    }

    [ContextMenu("Load Saved Texts")]
    private void LoadSavedTexts()
    {
        TextLoader.LoadSavedTexts();
    }
}
