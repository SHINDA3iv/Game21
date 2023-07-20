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
        //Идентифицирование обьектов на сцене для их дальнейшего клонирования
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        //Путь к файлу с рекордами
        scoresFilePath = Application.dataPath + "/scores.txt";
        // Проверяем, существует ли файл
        if (!File.Exists(scoresFilePath))
        {
            // Если файл не существует, создаем его
            File.Create(scoresFilePath).Dispose();
        }
        //Считывание рекордов с файла
        List<ScoreData> scores = SaveRecordsScript.ReadScoresFromFile();
        
        //Цикл добавления рекордов на сцену
        highscoreEntryTransformList = new List<Transform>();
        foreach (ScoreData scoreData in scores)
        {
            CreateHighscoreEntryTransform(scoreData, entryContainer, highscoreEntryTransformList);
        }
    }

    public void CreateHighscoreEntryTransform(ScoreData ScoreData, Transform container, List<Transform> trasformList)
    {
        //Создание клонов обьектов и их размещение
        float templateHeight = 50f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * trasformList.Count);
        entryTransform.gameObject.SetActive(true);

        //Отображение текста рекордов, их места и того, кто набрал данный рекорд
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

        string name = ScoreData.playerName;
        entryTransform.Find("nameText").GetComponent<Text>().text = name.ToString();

        //Отображение текста зеленым цветом у первого места
        if (rank == 1)
        {
            entryTransform.Find("posText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("nameText").GetComponent<Text>().color = Color.green;
        }

        //Удаление фона у каждого второго места для улучшения видимости
        if (rank % 2 != 1)
        {
            entryTransform.Find("background").gameObject.SetActive(false);
        }

        //Отображение изображения кубка и его цвета в зависимости от места
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

        //Добавление обьекто в список
        trasformList.Add(entryTransform);
    }
}