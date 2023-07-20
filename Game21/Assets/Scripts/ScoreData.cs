using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ScoreData
{
    //Класс с данными рекордов
    public string playerName;
    public int score;

    public ScoreData(string name, int score)
    {
        playerName = name;
        this.score = score;
    }
}
