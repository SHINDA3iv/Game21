using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class DeckScript : MonoBehaviour
{
    public Sprite[] cardSprites;
    int[] cardValues = new int[37];
    int currentIndex = 0;

    void Start()
    {
        GetCardValues();
    }

    //Дает картам колоды значения
    void GetCardValues()
    {
        cardValues[0] = 0;
        int num = 0;
        // Loop to assign values to the cards
        for (int i = 1; i < cardSprites.Length; i += 9)
        {
            num = i;
            num %= 9;
            for (int j = i; j < i + 9; j += 1)
            {
                if (j < i + 3)
                {
                    cardValues[j] = num + 1;
                }
                else if (j < i + 9)
                {
                    cardValues[j] = num + 2;
                }
                num++;
            }
        }
    }

    //Перетасовка колоды
    public void Shuffle()
    {
        // Standard array data swapping technique
        for (int i = cardSprites.Length - 1; i > 0; --i)
        {
            int j = Mathf.FloorToInt(Random.Range(0.03f, 1.0f) * cardSprites.Length - 1) + 1;
            Sprite face = cardSprites[i];
            cardSprites[i] = cardSprites[j];
            cardSprites[j] = face;

            int value = cardValues[i];
            cardValues[i] = cardValues[j];
            cardValues[j] = value;
        }
        currentIndex = 1;
    }

    //Назначает карту из колоды
    public int DealCard(CardScript cardScript)
    {
        cardScript.SetSprite(cardSprites[currentIndex]);
        cardScript.SetValue(cardValues[currentIndex]);
        currentIndex++;
        return cardScript.GetValueOfCard();
    }

    //Возвращает спрайт обратной стороны карт
    public Sprite GetCardBack()
    {
        return cardSprites[0];
    }
}