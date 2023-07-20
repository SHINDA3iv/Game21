using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveRecordsScript : MonoBehaviour
{
    private string scoresFilePath;

    void Start()
    {
        //Путь к файлу с рекордами
        scoresFilePath = Application.dataPath + "/scores.txt";
        // Проверяем, существует ли файл
        if (!File.Exists(scoresFilePath))
        {
            // Если файл не существует, создаем его
            File.Create(scoresFilePath).Dispose();
        }
    }

    //Создание нового рекорда
    public void WriteNewScore(string playerName, int score)
    {
        // Получаем список всех рекордов из файла
        List<ScoreData> scores = ReadScoresFromFile();

        // Добавляем новый рекорд
        scores.Add(new ScoreData(playerName, score));

        // Сортируем рекорды по убыванию
        scores = scores.OrderByDescending(s => s.score).ToList();

        // Если количество рекордов превышает 10, удаляем лишние
        if (scores.Count > 10)
        {
            scores = scores.Take(10).ToList();
        }

        // Записываем отсортированные рекорды обратно в файл
        WriteScoresToFile(scores);
    }

    //Считывание рекорда с файла
    public List<ScoreData> ReadScoresFromFile()
    {
        List<ScoreData> scores = new List<ScoreData>();
        //Путь к файлу с рекордами
        scoresFilePath = Application.dataPath + "/scores.txt";

        // Читаем все строки из файла и сохраняем их в массив строк
        string[] lines = File.ReadAllLines(scoresFilePath);

        //Цикл разделения строк на данные и добавление каждого рекорда с его данными в список
        foreach (string line in lines)
        {
            string[] data = line.Split(',');
            string playerName = data[0];
            int score = int.Parse(data[1]);

            scores.Add(new ScoreData(playerName, score));
        }

        return scores;
    }

    //Добавление рекорда в файл
    void WriteScoresToFile(List<ScoreData> scores)
    {
        // Перезаписываем файл с рекордами
        File.WriteAllText(scoresFilePath, string.Empty);
        
        // Создаем StreamWriter для записи в файл
        using (StreamWriter writer = new StreamWriter(scoresFilePath, true))
        {
            // Проходимся по каждому элементу в списке scores
            for (int i = 0; i < scores.Count; i++)
            {
                // Записываем рекорд в файл
                writer.WriteLine(scores[i].playerName + "," + scores[i].score);
            }
        }
    }
}
