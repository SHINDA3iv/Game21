using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RecordsScript : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    public List<Transform> highscoreEntryTransformList;
    public List<HighscoreEntry> highscoreEntryList;

    public SaveRecordsScript SaveRecordsScript;

    private void Awake()
    {
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        highscoreEntryList = new List<HighscoreEntry>()
        {
            new HighscoreEntry{score = 6600},
            new HighscoreEntry{score = 4500},
            new HighscoreEntry{score = 3400},
            new HighscoreEntry{score = 4600},
            new HighscoreEntry{score = 7600},
            new HighscoreEntry{score = 5800},
            new HighscoreEntry{score = 8200},
            new HighscoreEntry{score = 6200},
            new HighscoreEntry{score = 5400},
        };

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        //AddHighscoreEntry(5100);
        for(int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for(int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }
        //PlayerPrefs.DeleteAll();
        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
    }

    public void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> trasformList)
    {
        float templateHeight = 50f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * trasformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = trasformList.Count + 1;
        string rankString;

        switch (rank)
        {
            default:
                rankString = rank.ToString(); break;
        }

        entryTransform.Find("posText").GetComponent<Text>().text = rankString;

        int score = highscoreEntry.score;
        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        if (rank == 1)
        {
            entryTransform.Find("posText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
        }

        if(rank % 2 != 1)
        {
            entryTransform.Find("background").gameObject.SetActive(false);
        }

        switch (rank)
        {
            default:
                entryTransform.Find("Best").gameObject.SetActive(false);
                break;
            case 1:
                //entryTransform.Find("Best").gameObject.SetActive(true);
                entryTransform.Find("Best").GetComponent<Image>().color = new Color(255, 170, 0); //FFAB00
                break;
            case 2:
                //entryTransform.Find("Best").gameObject.SetActive(true);
                entryTransform.Find("Best").GetComponent<Image>().color = new Color(198, 198, 198); //C6C6C6
                break;
            case 3:
                //entryTransform.Find("Best").gameObject.SetActive(true);
                entryTransform.Find("Best").GetComponent<Image>().color = new Color(183, 111, 86); //B76F56
                break;
        }

        trasformList.Add(entryTransform);
    }

    public void AddHighscoreEntry(int score)
    {
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score };

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        highscores.highscoreEntryList.Add(highscoreEntry);

        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);

        /*if (highscores.Count > 10)  
        {
            highscores = highscores.GetRange(0, 10);
        }*/
        // Сохранение нового списка рекордов
        PlayerPrefs.Save();
    }
    public class Highscores //: List<int>
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    [System.Serializable]
    public class HighscoreEntry
    {
        public int score;
    }
}
