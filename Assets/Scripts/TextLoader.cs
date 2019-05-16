using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextLoader
{
    private static bool TextsLoaded = false;
    private static BubbleText[] BubbleTextCollection;
    private static List<BubbleText> BubbleTextFollowerCollection;
    private static List<BubbleText> BubbleTextDissidentCollection;

    [Serializable]
    public class BubbleText
    {
        public string text;
        public bool isFollower;

        public BubbleText(string text, bool isFollower)
        {
            this.text = text;
            this.isFollower = isFollower;
        }
    }

    [Serializable]
    public class BubbleTextContainer
    {
        public BubbleText[] BubbleTexts;
        public List<BubbleText> FollowerbubbleTexts;
        public List<BubbleText> DissidentbubbleTexts;

        public BubbleTextContainer(BubbleText[] bubbleTexts)
        {
            this.BubbleTexts = bubbleTexts;
            FollowerbubbleTexts = new List<BubbleText>();
            DissidentbubbleTexts = new List<BubbleText>();
            foreach (BubbleText bubble in bubbleTexts)
            {
                if(bubble.isFollower)
                {
                    FollowerbubbleTexts.Add(bubble);
                }
                else
                {
                    DissidentbubbleTexts.Add(bubble);
                }
            }
        }
    }

    public static void SaveSampleTexts()
    {
        BubbleText leaderSucks = new BubbleText("The leader sucks", false);
        BubbleText leaderGreat = new BubbleText("The leader is great", true);
        BubbleText likeLeader = new BubbleText("I really like the Leader", true);
        BubbleText hateLeader = new BubbleText("I really hate the Leader", false);

        BubbleText[] bubbleTexts = new BubbleText[] { leaderSucks, leaderGreat, likeLeader, hateLeader };

        BubbleTextContainer textContainer = new BubbleTextContainer(bubbleTexts);

        string json = JsonUtility.ToJson(textContainer, true);
        File.WriteAllText(Application.dataPath + "/BubbleTexts/bubbleTexts.txt", json);
    }

    public static void LoadSavedTexts()
    {
        string json = File.ReadAllText(Application.dataPath + "/BubbleTexts/bubbleTexts.txt");
        BubbleTextContainer bubbleTextContainer = JsonUtility.FromJson<BubbleTextContainer>(json);
        BubbleTextCollection = bubbleTextContainer.BubbleTexts;
        BubbleTextFollowerCollection = new List<BubbleText>();
        BubbleTextDissidentCollection = new List<BubbleText>();
        BubbleTextFollowerCollection = bubbleTextContainer.FollowerbubbleTexts;
        BubbleTextDissidentCollection = bubbleTextContainer.DissidentbubbleTexts;
    }

    public static BubbleText GetRandomBubbleText()
    {
        if(!TextsLoaded)
        {
            LoadSavedTexts();
            TextsLoaded = true;
        }
        int randomText = UnityEngine.Random.Range(0, BubbleTextCollection.Length);
        return BubbleTextCollection[randomText];
    }

    public static BubbleText GetRandomBubbleTextWithPercentage(int percentage)
    {
        if (!TextsLoaded)
        {
            LoadSavedTexts();
            TextsLoaded = true;
        }
        int randomSide = UnityEngine.Random.Range(0,101);
        int randomText;
        if (randomSide > percentage)
        {
            randomText = UnityEngine.Random.Range(0, BubbleTextDissidentCollection.Count);
            return BubbleTextDissidentCollection[randomText];
        }
        else
        {
            randomText = UnityEngine.Random.Range(0, BubbleTextFollowerCollection.Count);
            return BubbleTextFollowerCollection[randomText];
        }
    }
}
