using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    //������ ���� ��� ������ � ������

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

    //��������� ����� � ���������� ���� ������/������
    public int GetCard()
    {
        //���������� ����� �� �������
        int cardValue = deckScript.DealCard(hand[cardIndex].GetComponent<CardScript>());
        //���������� ����� �� �����
        hand[cardIndex].GetComponent<Renderer>().enabled = true;
        //�������� �������� ����� � ������ �������� �������
        handValue += cardValue;
        //���� �������� ����� 11, �� ��� ���
        if (cardValue == 11)
        {
            aceList.Add(hand[cardIndex].GetComponent<CardScript>());
        }
        //�������� ����, ������� �� ������������ 1 ������ 11
        AceCheck();
        cardIndex++;
        return handValue;
    }
    
    //��������� ����������� �������� ����(1 ��� 11), � ��������� ���
    public void AceCheck()
    {
        //�������� ������� ���� � ������
        foreach (CardScript ace in aceList)
        {
            if (handValue + 10 < 22 && ace.GetValueOfCard() == 1)
            {
                //��������� �������� ���� � ���������� ���� ������/������
                ace.SetValue(11);
                handValue += 10;
            }
            else if (handValue > 21 && ace.GetValueOfCard() == 11)
            {
                //��������� �������� ���� � ���������� ���� ������/������
                ace.SetValue(1);
                handValue -= 10;
            }
        }
    }

    //���������� ����� �� ���� ������
    public int AceCount()
    {
        return aceList.Count;
    }

    //���������� ���� �� ���� ������
    public int CardCount()
    {
        return cardIndex;
    }

    //������� ����� �����
    public void AdjustMoney(int amount)
    {
        money += amount;
    }

    //����� ������� ����� ����� �������
    public int GetMoney()
    {
        return money;
    }

    //�������� ��� �����, ���������� ����������� ����������
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
