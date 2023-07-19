using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class RecordsScript : MonoBehaviour
{
    private string scoresFilePath;

    private Transform entryContainer;
    private Transform entryTemplate;

    public List<Transform> highscoreEntryTransformList;

    public SaveRecordsScript SaveRecordsScript;

    private void Awake()
    {
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        scoresFilePath = Application.dataPath + "/scores.txt";
        // Проверяем, существует ли файл
        if (!File.Exists(scoresFilePath))
        {
            // Если файл не существует, создаем его
            File.Create(scoresFilePath).Dispose();
        }

        List<ScoreData> scores = SaveRecordsScript.ReadScoresFromFile();

        highscoreEntryTransformList = new List<Transform>();
        foreach (ScoreData scoreData in scores)
        {
            CreateHighscoreEntryTransform(scoreData, entryContainer, highscoreEntryTransformList);
        }
    }

    public void CreateHighscoreEntryTransform(ScoreData ScoreData, Transform container, List<Transform> trasformList)
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

        int score = ScoreData.score;
        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        if (rank == 1)
        {
            entryTransform.Find("posText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
        }

        if (rank % 2 != 1)
        {
            entryTransform.Find("background").gameObject.SetActive(false);
        }

        switch (rank)
        {
            default:
                entryTransform.Find("Best").gameObject.SetActive(false);
                break;
            case 1:
                entryTransform.Find("Best").GetComponent<Image>().color = new Color(1f, 0.85f, 0.00f); //FFAB00
                break;
            case 2:
                entryTransform.Find("Best").GetComponent<Image>().color = new Color(0.78f, 0.78f, 0.78f); //C6C6C6
                break;
            case 3:
                entryTransform.Find("Best").GetComponent<Image>().color = new Color(0.72f, 0.44f, 0.20f); //B76F56
                break;
        }

        trasformList.Add(entryTransform);
    }

}


