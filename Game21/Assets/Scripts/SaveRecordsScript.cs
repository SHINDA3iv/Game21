using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveRecordsScript : MonoBehaviour
{
    public static void SaveScore(int score ) {
        PlayerPrefs.SetInt("highscoreTable", score );
        PlayerPrefs.Save();
    }
 
    public static int LoadScore() {
        if( !PlayerPrefs.HasKey("highscoreTable") )
            return 0;
        return PlayerPrefs.GetInt("highscoreTable", 0 );
    }
};

public class Example : MonoBehaviour
{
    void Start()
    {
        var score = SaveRecordsScript.LoadScore();
        Debug.Log(string.Format("���������� ����� ���������: {0}!", score));

        score += 1000; // �������� ���������� ���������� �����

        SaveRecordsScript.SaveScore(score); // ��������� ����� ������

    }

}