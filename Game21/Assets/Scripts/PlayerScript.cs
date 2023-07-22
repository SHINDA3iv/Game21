using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    //Скрипт игры для игрока и дилера

    // Get other scripts
    public CardScript cardScript;
    public DeckScript deckScript;

    // Total value of player/dealer's hand
    public int handValue = 0;

    // Betting money
    private int money = 5000;

    // Array of card objects on table
    public GameObject[] hand;
    // Index of next card to be turned over
    public int cardIndex = 0;
    // Tracking aces for 1 to 11 conversions
    List<CardScript> aceList = new List<CardScript>();

    public void StartHand()
    {
        GetCard();
    }

    //Добавляет карту к комбинации карт игрока/дилера
    public int GetCard()
    {
        //назначение карты на раздачу
        int cardValue = deckScript.DealCard(hand[cardIndex].GetComponent<CardScript>());
        //Показывает карту на сцене
        hand[cardIndex].GetComponent<Renderer>().enabled = true;
        //Добавьте значение карты к общему значению раздачи
        handValue += cardValue;
        //Если значение равно 11, то это туз
        if (cardValue == 11)
        {
            aceList.Add(hand[cardIndex].GetComponent<CardScript>());
        }
        //Проверка того, следует ли использовать 1 вместо 11
        AceCheck();
        cardIndex++;
        return handValue;
    }
    
    //Проверяет необходимое значение туза(1 или 11), и назначает его
    public void AceCheck()
    {
        //Проверка каждого туза в списке
        foreach (CardScript ace in aceList)
        {
            if (handValue + 10 < 22 && ace.GetValueOfCard() == 1)
            {
                //Изменение значения туза и комбинации карт игрока/дилера
                ace.SetValue(11);
                handValue += 10;
            }
            else if (handValue > 21 && ace.GetValueOfCard() == 11)
            {
                //Изменение значения туза и комбинации карт игрока/дилера
                ace.SetValue(1);
                handValue -= 10;
            }
        }
    }

    //Количество эйсов на руке игрока
    public int AceCount()
    {
        return aceList.Count;
    }

    //Количество карт на руке игрока
    public int CardCount()
    {
        return cardIndex;
    }

    //Измение суммы банка
    public void AdjustMoney(int amount)
    {
        money += amount;
    }

    //Вывод текущей суммы денег игроков
    public int GetMoney()
    {
        return money;
    }

    //Скрывает все карты, сбрасывает необходимые переменные
    public void ResetHand()
    {
        for (int i = 0; i < hand.Length; i++)
        {
            hand[i].GetComponent<CardScript>().ResetCard();
            hand[i].GetComponent<Renderer>().enabled = false;
        }
        cardIndex = 0;
        handValue = 0;
        aceList = new List<CardScript>();
    }
}
