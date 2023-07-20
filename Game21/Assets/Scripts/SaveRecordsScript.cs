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
        //���� � ����� � ���������
        scoresFilePath = Application.dataPath + "/scores.txt";
        // ���������, ���������� �� ����
        if (!File.Exists(scoresFilePath))
        {
            // ���� ���� �� ����������, ������� ���
            File.Create(scoresFilePath).Dispose();
        }
    }

    //�������� ������ �������
    public void WriteNewScore(string playerName, int score)
    {
        // �������� ������ ���� �������� �� �����
        List<ScoreData> scores = ReadScoresFromFile();

        // ��������� ����� ������
        scores.Add(new ScoreData(playerName, score));

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

    //���������� ������� � �����
    public List<ScoreData> ReadScoresFromFile()
    {
        List<ScoreData> scores = new List<ScoreData>();
        //���� � ����� � ���������
        scoresFilePath = Application.dataPath + "/scores.txt";

        // ������ ��� ������ �� ����� � ��������� �� � ������ �����
        string[] lines = File.ReadAllLines(scoresFilePath);

        //���� ���������� ����� �� ������ � ���������� ������� ������� � ��� ������� � ������
        foreach (string line in lines)
        {
            string[] data = line.Split(',');
            string playerName = data[0];
            int score = int.Parse(data[1]);

            scores.Add(new ScoreData(playerName, score));
        }

        return scores;
    }

    //���������� ������� � ����
    void WriteScoresToFile(List<ScoreData> scores)
    {
        // �������������� ���� � ���������
        File.WriteAllText(scoresFilePath, string.Empty);
        
        // ������� StreamWriter ��� ������ � ����
        using (StreamWriter writer = new StreamWriter(scoresFilePath, true))
        {
            // ���������� �� ������� �������� � ������ scores
            for (int i = 0; i < scores.Count; i++)
            {
                // ���������� ������ � ����
                writer.WriteLine(scores[i].playerName + "," + scores[i].score);
            }
        }
    }
}
