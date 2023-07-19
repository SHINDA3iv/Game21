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
        scoresFilePath = Application.dataPath + "/scores.txt";
        // ���������, ���������� �� ����
        if (!File.Exists(scoresFilePath))
        {
            // ���� ���� �� ����������, ������� ���
            File.Create(scoresFilePath).Dispose();
        }
    }

    public void WriteNewScore(int score)
    {
        // �������� ������ ���� �������� �� �����
        List<ScoreData> scores = ReadScoresFromFile();

        // ��������� ����� ������
        scores.Add(new ScoreData(score));

        // ��������� ������� �� ��������
        scores = scores.OrderByDescending(s => s.score).ToList();

        // ���� ���������� �������� ��������� 10, ������� ������
        if (scores.Count > 10)
        {
            scores = scores.Take(10).ToList();
        }

        // ���������� ��������������� ������� ������� � ����
        WriteScoresToFile(scores);
    }

    public List<ScoreData> ReadScoresFromFile()
    {
        List<ScoreData> scores = new List<ScoreData>();
        scoresFilePath = Application.dataPath + "/scores.txt";

        string[] lines = File.ReadAllLines(scoresFilePath);

        foreach (string line in lines)
        {
            int score = int.Parse(line);
            scores.Add(new ScoreData(score));
        }

        return scores;
    }

    void WriteScoresToFile(List<ScoreData> scores)
    {
        // �������������� ���� � ���������
        File.WriteAllText(scoresFilePath, string.Empty);

        using (StreamWriter writer = new StreamWriter(scoresFilePath, true))
        {
            for (int i = 0; i < scores.Count; i++)
            {
                // ���������� ������ � ����
                writer.WriteLine(scores[i].score);
            }
        }
    }
}
