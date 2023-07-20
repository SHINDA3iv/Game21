using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    //Значение карты
    public int value = 0;

    //Возвращает значение карты
    public int GetValueOfCard()
    {
        return value;
    }

    //Назначает значение карты
    public void SetValue(int newValue)
    {
        value = newValue;
    }

    //Назначает спрайт карты
    public void SetSprite(Sprite newSprite)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    //Скрывает карты
    public void ResetCard()
    {
        Sprite back = GameObject.Find("Deck").GetComponent<DeckScript>().GetCardBack();
        gameObject.GetComponent<SpriteRenderer>().sprite = back;
        value = 0;
    }
}
