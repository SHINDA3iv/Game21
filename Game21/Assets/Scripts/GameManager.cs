using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{   
    //Кнопки на сцене
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

    //Подключение скриптов
    public PlayerScript playerScript;
    public PlayerScript dealerScript;
    public SaveRecordsScript saveRecordsScript;

    //Тексты на сцене
    public Text scoreText;
    public Text dealerScoreText;
    public Text betText;
    public Text chipsText;
    public Text bankText;
    public Text mainText;
    public Text standButtonText;

    public GameObject hideCard; //Карта скрывабщая карту дилера
    public GameObject popupMenu; //Всплывающее меню
    public GameObject Deck; //Колода карт

    int pot = 0; //Величина ставки
    private int standClicks = 0; //Количество кликов на standButton

    //Настройка UI при начале игры
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

        mainText.text = "Сделайте ставку!";

        playerScript.ResetHand();
        dealerScript.ResetHand();
    }

    //Настройка UI при начале раунда
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

        //Сокрытие карты дилера
        hideCard.gameObject.SetActive(true);

        GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();

        playerScript.StartHand();
        dealerScript.StartHand();

        scoreText.text = "Ваш счет: " + playerScript.handValue.ToString();
        dealerScoreText.text = "Счет дилера: " + dealerScript.handValue.ToString();
        standButtonText.text = "Хватит";

        //Проверка на 21 у игрока и дилера при первом ходе
        if (playerScript.handValue == 21 || dealerScript.handValue == 21) RoundOver();
    }

    //Настройка UI для нового раунда
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

        scoreText.text = "Ваш счет: " + playerScript.handValue.ToString();
        dealerScoreText.text = "Счет дилера: " + dealerScript.handValue.ToString();
    }

    //Нажатие на кнопку hitButton
    private void HitClicked()
    {
        //Проверка оставшегося места для карт у игрока
        if (playerScript.cardIndex <= 11)
        {
            playerScript.GetCard();
            scoreText.text = "Ваш счет: " + playerScript.handValue.ToString();
            if (playerScript.CardCount() == playerScript.AceCount() && playerScript.CardCount() == 2) RoundOver();
            else if (playerScript.handValue > 20) RoundOver();
        }
    }

    //Нажатие на кнопку standButton
    private void StandClicked()
    {
        int hitDealer = dealerScript.cardIndex;
        standClicks++;
        HitDealer();
        standButtonText.text = "Принять";
        //Если кнопка уже была нажата пару раз и дилер не добирал карты, то конец раунда
        if (standClicks > 1 && hitDealer == dealerScript.cardIndex) RoundOver();
    }

    //Добор карт дилера
    private void HitDealer()
    {
        //Проверка оставшегося места для карт у игрока, а также на значение карт меньше 17 и  меньше чем у игрока
        while (dealerScript.handValue < 17 && dealerScript.cardIndex < 10 && dealerScript.handValue <= playerScript.handValue)
        {
            dealerScript.GetCard();
            dealerScoreText.text = "Счет дилера: " + dealerScript.handValue.ToString();
            if (dealerScript.CardCount() == dealerScript.AceCount() && dealerScript.CardCount() == 2)
            { 
                RoundOver();
                break;
            }
            else if (dealerScript.handValue > 20) RoundOver();
        }
    }

    //Проверка на победителя и проигравшего, окончание раздачи
    void RoundOver()
    {
        //Переменные для проверки на перебор или 21
        bool playerBust = playerScript.handValue > 21;
        bool dealerBust = dealerScript.handValue > 21;
        bool player21 = playerScript.handValue == 21;
        bool dealer21 = dealerScript.handValue == 21;
        bool player2ace = (dealerScript.CardCount() == dealerScript.AceCount() && dealerScript.CardCount() == 2);
        bool dealer2ace = (dealerScript.CardCount() == dealerScript.AceCount() && dealerScript.CardCount() == 2);

        //Если кнопка Stand была нажата менее двух раз, нет переборов или 21, завершите работу функции
        if (standClicks < 2 && !playerBust && !dealerBust && !player21 && !dealer21 && !player2ace && !dealer2ace) return;
        bool roundOver = true;

        //У всех перебор, возврат ставок
        if (playerBust && dealerBust)
        {
            mainText.text = "У всех перебор!";
            playerScript.AdjustMoney(pot / 2);
            dealerScript.AdjustMoney(pot / 2);
        }
        //У игрока золотое очко, его победа
        else if (player2ace)
        {
            mainText.text = "Золотое очко!";
            playerScript.AdjustMoney(pot);
        }
        //У игрока золотое очко, его победа
        else if (dealer2ace)
        {
            mainText.text = "Золотое очко!";
            dealerScript.AdjustMoney(pot);
        }
        //Если только у игрока перебор или у дилера больше очков, дилер выигрывает
        else if (playerBust || (!dealerBust && dealerScript.handValue > playerScript.handValue))
        {
            mainText.text = "Вы проиграли!";
            dealerScript.AdjustMoney(pot);
        }
        //Если только у дилера перебор или у игрока больше очков, игрок выигрывает
        else if (dealerBust || playerScript.handValue > dealerScript.handValue)
        {
            mainText.text = "Вы победили!";
            playerScript.AdjustMoney(pot);
        }
        //Проверка на ничью, возврат ставок
        else if (playerScript.handValue == dealerScript.handValue)
        {
            mainText.text = "Ничья!";
            playerScript.AdjustMoney(pot / 2);
            dealerScript.AdjustMoney(pot / 2);
        }
        else
        {
            roundOver = false;
        }
        // Настройка UI для следующего хода/раздачи/очереди/окончания игры
        if (roundOver)
        {
            hitButton.gameObject.SetActive(false);
            standButton.gameObject.SetActive(false);

            dealerScoreText.gameObject.SetActive(true);
            mainText.gameObject.SetActive(true);

            hideCard.gameObject.SetActive(false);

            chipsText.text = "Ваш банк: " + playerScript.GetMoney().ToString();
            bankText.text = "Банк дилера: " + dealerScript.GetMoney().ToString();

            pot = 0;
            betText.text = "Ставка: " + pot.ToString();
            standClicks = 0;

            //Проверка на окончание игры 
            if (playerScript.GetMoney() == 0 || dealerScript.GetMoney() == 0) GameOver();
            else repeatButton.gameObject.SetActive(true);
        }
    }

    //Окончание игры 
    void GameOver()
    {
        gameoverButton.gameObject.SetActive(true);
        if (playerScript.GetMoney() == 0 && dealerScript.GetMoney() == 0) mainText.text = "Деньги кончились!";
    }

    //Добавление денег в банк при нажатии кнопки bet
    void BetClicked(Button bet)
    {
        mainText.gameObject.SetActive(false);

        Text newBet = bet.GetComponentInChildren(typeof(Text)) as Text;
        int intBet = int.Parse(newBet.text.ToString());

        //Проверка на наличие достаточного количества денег у дилера и игрока
        if (playerScript.GetMoney() >= intBet && dealerScript.GetMoney() >= intBet)
        {
            dealButton.gameObject.SetActive(true);
            mainText.gameObject.SetActive(false);
            playerScript.AdjustMoney(-intBet);
            dealerScript.AdjustMoney(-intBet);
            chipsText.text = "Ваш банк: " + playerScript.GetMoney().ToString();
            bankText.text = "Банк дилера: " + dealerScript.GetMoney().ToString();
            pot += (intBet * 2);
            betText.text = "Ставка: " + pot.ToString();
        }
        else
        {
            mainText.gameObject.SetActive(true);
            mainText.text = "Деньги кончились!";
        }
    }

    //настройка UI при нажатии кнопки для открытия всплывающего меню
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

    //настройка UI при нажатии кнопки для закрытия всплывающего меню
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

    //Сохранение результатов для передачи лучших из них в таблицу рекордов при нажатии кнопки выхода в меню
    public void MenuClicked()
    {
        if (dealerScript.GetMoney() < playerScript.GetMoney()) saveRecordsScript.WriteNewScore("Вы", playerScript.GetMoney());
        else if (dealerScript.GetMoney() > playerScript.GetMoney()) saveRecordsScript.WriteNewScore("Дилер", dealerScript.GetMoney());
    }
}