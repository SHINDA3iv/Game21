using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{   
    //������ �� �����
    public Button dealButton;
    public Button hitButton;
    public Button standButton;
    public Button repeatButton;
    public Button bet100Button;
    public Button bet250Button;
    public Button bet500Button;
    public Button popupMenuButton;
    public Button backButton;
    public Button menuButton;
    public Button gameoverButton;

    //����������� ��������
    public PlayerScript playerScript;
    public PlayerScript dealerScript;
    public SaveRecordsScript saveRecordsScript;

    //������ �� �����
    public Text scoreText;
    public Text dealerScoreText;
    public Text betText;
    public Text chipsText;
    public Text bankText;
    public Text mainText;
    public Text standButtonText;

    public GameObject hideCard; //����� ���������� ����� ������
    public GameObject popupMenu; //����������� ����
    public GameObject Deck; //������ ����

    int pot = 0; //�������� ������
    private int standClicks = 0; //���������� ������ �� standButton

    //��������� UI ��� ������ ����
    void Start()
    {
        dealButton.onClick.AddListener(() => DealClicked());
        hitButton.onClick.AddListener(() => HitClicked());
        standButton.onClick.AddListener(() => StandClicked());
        repeatButton.onClick.AddListener(() => RepeatClicked());
        bet100Button.onClick.AddListener(() => BetClicked(bet100Button));
        bet250Button.onClick.AddListener(() => BetClicked(bet250Button));
        bet500Button.onClick.AddListener(() => BetClicked(bet500Button));
        popupMenuButton.onClick.AddListener(() => popupClicked());
        backButton.onClick.AddListener(() => BackClicked());
        menuButton.onClick.AddListener(() => MenuClicked());
        gameoverButton.onClick.AddListener(() => MenuClicked());

        dealButton.gameObject.SetActive(false);
        hitButton.gameObject.SetActive(false);
        standButton.gameObject.SetActive(false);
        repeatButton.gameObject.SetActive(false);
        gameoverButton.gameObject.SetActive(false);

        dealerScoreText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);

        hideCard.gameObject.SetActive(false);
        popupMenu.gameObject.SetActive(false);

        mainText.text = "�������� ������!";

        playerScript.ResetHand();
        dealerScript.ResetHand();
    }

    //��������� UI ��� ������ ������
    private void DealClicked()
    {
        dealButton.gameObject.SetActive(false);
        hitButton.gameObject.SetActive(true);
        standButton.gameObject.SetActive(true);
        bet100Button.gameObject.SetActive(false);
        bet250Button.gameObject.SetActive(false);
        bet500Button.gameObject.SetActive(false);

        scoreText.gameObject.SetActive(true);
        mainText.gameObject.SetActive(false);

        //�������� ����� ������
        hideCard.gameObject.SetActive(true);

        GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();

        playerScript.StartHand();
        dealerScript.StartHand();

        scoreText.text = "��� ����: " + playerScript.handValue.ToString();
        dealerScoreText.text = "���� ������: " + dealerScript.handValue.ToString();
        standButtonText.text = "������";

        //�������� �� 21 � ������ � ������ ��� ������ ����
        if (playerScript.handValue == 21 || dealerScript.handValue == 21) RoundOver();
    }

    //��������� UI ��� ������ ������
    private void RepeatClicked()
    {
        dealerScoreText.gameObject.SetActive(false);
        mainText.gameObject.SetActive(false);

        repeatButton.gameObject.SetActive(false);
        bet100Button.gameObject.SetActive(true);
        bet250Button.gameObject.SetActive(true);
        bet500Button.gameObject.SetActive(true);

        playerScript.ResetHand();
        dealerScript.ResetHand();

        scoreText.text = "��� ����: " + playerScript.handValue.ToString();
        dealerScoreText.text = "���� ������: " + dealerScript.handValue.ToString();
    }

    //������� �� ������ hitButton
    private void HitClicked()
    {
        //�������� ����������� ����� ��� ���� � ������
        if (playerScript.cardIndex <= 11)
        {
            playerScript.GetCard();
            scoreText.text = "��� ����: " + playerScript.handValue.ToString();
            if (playerScript.CardCount() == playerScript.AceCount() && playerScript.CardCount() == 2) RoundOver();
            else if (playerScript.handValue > 20) RoundOver();
        }
    }

    //������� �� ������ standButton
    private void StandClicked()
    {
        int hitDealer = dealerScript.cardIndex;
        standClicks++;
        HitDealer();
        standButtonText.text = "�������";
        //���� ������ ��� ���� ������ ���� ��� � ����� �� ������� �����, �� ����� ������
        if (standClicks > 1 && hitDealer == dealerScript.cardIndex) RoundOver();
    }

    //����� ���� ������
    private void HitDealer()
    {
        //�������� ����������� ����� ��� ���� � ������, � ����� �� �������� ���� ������ 17 �  ������ ��� � ������
        while (dealerScript.handValue < 17 && dealerScript.cardIndex < 10 && dealerScript.handValue <= playerScript.handValue)
        {
            dealerScript.GetCard();
            dealerScoreText.text = "���� ������: " + dealerScript.handValue.ToString();
            if (dealerScript.CardCount() == dealerScript.AceCount() && dealerScript.CardCount() == 2)
            { 
                RoundOver();
                break;
            }
            else if (dealerScript.handValue > 20) RoundOver();
        }
    }

    //�������� �� ���������� � ������������, ��������� �������
    void RoundOver()
    {
        //���������� ��� �������� �� ������� ��� 21
        bool playerBust = playerScript.handValue > 21;
        bool dealerBust = dealerScript.handValue > 21;
        bool player21 = playerScript.handValue == 21;
        bool dealer21 = dealerScript.handValue == 21;
        bool player2ace = (dealerScript.CardCount() == dealerScript.AceCount() && dealerScript.CardCount() == 2);
        bool dealer2ace = (dealerScript.CardCount() == dealerScript.AceCount() && dealerScript.CardCount() == 2);

        //���� ������ Stand ���� ������ ����� ���� ���, ��� ��������� ��� 21, ��������� ������ �������
        if (standClicks < 2 && !playerBust && !dealerBust && !player21 && !dealer21 && !player2ace && !dealer2ace) return;
        bool roundOver = true;

        //� ���� �������, ������� ������
        if (playerBust && dealerBust)
        {
            mainText.text = "� ���� �������!";
            playerScript.AdjustMoney(pot / 2);
            dealerScript.AdjustMoney(pot / 2);
        }
        //� ������ ������� ����, ��� ������
        else if (player2ace)
        {
            mainText.text = "������� ����!";
            playerScript.AdjustMoney(pot);
        }
        //� ������ ������� ����, ��� ������
        else if (dealer2ace)
        {
            mainText.text = "������� ����!";
            dealerScript.AdjustMoney(pot);
        }
        //���� ������ � ������ ������� ��� � ������ ������ �����, ����� ����������
        else if (playerBust || (!dealerBust && dealerScript.handValue > playerScript.handValue))
        {
            mainText.text = "�� ���������!";
            dealerScript.AdjustMoney(pot);
        }
        //���� ������ � ������ ������� ��� � ������ ������ �����, ����� ����������
        else if (dealerBust || playerScript.handValue > dealerScript.handValue)
        {
            mainText.text = "�� ��������!";
            playerScript.AdjustMoney(pot);
        }
        //�������� �� �����, ������� ������
        else if (playerScript.handValue == dealerScript.handValue)
        {
            mainText.text = "�����!";
            playerScript.AdjustMoney(pot / 2);
            dealerScript.AdjustMoney(pot / 2);
        }
        else
        {
            roundOver = false;
        }
        // ��������� UI ��� ���������� ����/�������/�������/��������� ����
        if (roundOver)
        {
            hitButton.gameObject.SetActive(false);
            standButton.gameObject.SetActive(false);

            dealerScoreText.gameObject.SetActive(true);
            mainText.gameObject.SetActive(true);

            hideCard.gameObject.SetActive(false);

            chipsText.text = "��� ����: " + playerScript.GetMoney().ToString();
            bankText.text = "���� ������: " + dealerScript.GetMoney().ToString();

            pot = 0;
            betText.text = "������: " + pot.ToString();
            standClicks = 0;

            //�������� �� ��������� ���� 
            if (playerScript.GetMoney() == 0 || dealerScript.GetMoney() == 0) GameOver();
            else repeatButton.gameObject.SetActive(true);
        }
    }

    //��������� ���� 
    void GameOver()
    {
        gameoverButton.gameObject.SetActive(true);
        if (playerScript.GetMoney() == 0 && dealerScript.GetMoney() == 0) mainText.text = "������ ���������!";
    }

    //���������� ����� � ���� ��� ������� ������ bet
    void BetClicked(Button bet)
    {
        mainText.gameObject.SetActive(false);

        Text newBet = bet.GetComponentInChildren(typeof(Text)) as Text;
        int intBet = int.Parse(newBet.text.ToString());

        //�������� �� ������� ������������ ���������� ����� � ������ � ������
        if (playerScript.GetMoney() >= intBet && dealerScript.GetMoney() >= intBet)
        {
            dealButton.gameObject.SetActive(true);
            mainText.gameObject.SetActive(false);
            playerScript.AdjustMoney(-intBet);
            dealerScript.AdjustMoney(-intBet);
            chipsText.text = "��� ����: " + playerScript.GetMoney().ToString();
            bankText.text = "���� ������: " + dealerScript.GetMoney().ToString();
            pot += (intBet * 2);
            betText.text = "������: " + pot.ToString();
        }
        else
        {
            mainText.gameObject.SetActive(true);
            mainText.text = "������ ���������!";
        }
    }

    //��������� UI ��� ������� ������ ��� �������� ������������ ����
    void popupClicked()
    {
        popupMenuButton.gameObject.SetActive(false);

        popupMenu.gameObject.SetActive(true);

        dealButton.interactable = false;
        repeatButton.interactable = false;
        hitButton.interactable = false;
        standButton.interactable = false;
        bet100Button.interactable = false;
        bet250Button.interactable = false;
        bet500Button.interactable = false;
    }

    //��������� UI ��� ������� ������ ��� �������� ������������ ����
    public void BackClicked()
    {
        popupMenuButton.gameObject.SetActive(true);

        popupMenu.gameObject.SetActive(false);

        dealButton.interactable = true;
        repeatButton.interactable = true;
        hitButton.interactable = true;
        standButton.interactable = true;
        bet100Button.interactable = true;
        bet250Button.interactable = true;
        bet500Button.interactable = true;
    }

    //���������� ����������� ��� �������� ������ �� ��� � ������� �������� ��� ������� ������ ������ � ����
    public void MenuClicked()
    {
        if (dealerScript.GetMoney() < playerScript.GetMoney()) saveRecordsScript.WriteNewScore("��", playerScript.GetMoney());
        else if (dealerScript.GetMoney() > playerScript.GetMoney()) saveRecordsScript.WriteNewScore("�����", dealerScript.GetMoney());
    }
}