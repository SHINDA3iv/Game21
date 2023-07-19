using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using System.IO;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{   
    public Button dealButton;
    public Button repeatButton;
    public Button hitButton;
    public Button standButton;
    public Button bet100Button;
    public Button bet250Button;
    public Button bet500Button;
    public Button popupMenuButton;
    public Button backButton;
    public Button menuButton;
    public Button gameoverButton;

    private int standClicks = 0;

    public PlayerScript playerScript;
    public PlayerScript dealerScript;
    public SaveRecordsScript saveRecordsScript;

    public Text scoreText;
    public Text dealerScoreText;
    public Text betText;
    public Text chipsText;
    public Text bankText;
    public Text mainText;
    public Text standButtonText;

    // Card hiding dealer's 2nd card
    public GameObject hideCard;
    public GameObject popupMenu;
    public GameObject HidePanel;
    public GameObject Deck;
    // How much is bet
    int pot = 0;

    void Start()
    {
        dealButton.onClick.AddListener(() => DealClicked());
        repeatButton.onClick.AddListener(() => RepeatClicked());
        hitButton.onClick.AddListener(() => HitClicked());
        standButton.onClick.AddListener(() => StandClicked());
        bet100Button.onClick.AddListener(() => BetClicked(bet100Button));
        bet250Button.onClick.AddListener(() => BetClicked(bet250Button));
        bet500Button.onClick.AddListener(() => BetClicked(bet500Button));
        popupMenuButton.onClick.AddListener(() => popupClicked());
        backButton.onClick.AddListener(() => BackClicked());
        menuButton.onClick.AddListener(() => MenuClicked());
        gameoverButton.onClick.AddListener(() => MenuClicked());

        hitButton.gameObject.SetActive(false);
        standButton.gameObject.SetActive(false);
        dealButton.gameObject.SetActive(false);
        repeatButton.gameObject.SetActive(false);
        dealerScoreText.gameObject.SetActive(false);
        hideCard.gameObject.SetActive(false);
        popupMenu.gameObject.SetActive(false);
        HidePanel.gameObject.SetActive(false);
        gameoverButton.gameObject.SetActive(false);

        playerScript.ResetHand();
        dealerScript.ResetHand();
    }

    private void DealClicked()
    {
        // Hide deal hand score at start of deal
        dealerScoreText.gameObject.SetActive(false);
        mainText.gameObject.SetActive(false);
        //dealerScoreText.gameObject.SetActive(false);
        hideCard.gameObject.SetActive(true);

        GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();

        playerScript.StartHand();
        dealerScript.StartHand();

        // Update the scores displayed
        scoreText.text = "Ваш счет: " + playerScript.handValue.ToString();
        dealerScoreText.text = "Счет дилера: " + dealerScript.handValue.ToString();

        // Place card back on dealer card, hide card
        hideCard.GetComponent<Renderer>().enabled = true;

        // Adjust buttons visibility
        dealButton.gameObject.SetActive(false);
        hitButton.gameObject.SetActive(true);
        standButton.gameObject.SetActive(true);
        standButtonText.text = "Хватит";

        bet100Button.gameObject.SetActive(false);
        bet250Button.gameObject.SetActive(false);
        bet500Button.gameObject.SetActive(false);

        if (playerScript.handValue == 21 || dealerScript.handValue == 21) RoundOver();
    }

    private void RepeatClicked()
    {
        bet100Button.gameObject.SetActive(true);
        bet250Button.gameObject.SetActive(true);
        bet500Button.gameObject.SetActive(true);
        repeatButton.gameObject.SetActive(false);
        mainText.gameObject.SetActive(false);
        dealerScoreText.gameObject.SetActive(false);

        playerScript.ResetHand();
        dealerScript.ResetHand();

        scoreText.text = "Ваш счет: " + playerScript.handValue.ToString();
        dealerScoreText.text = "Счет дилера: " + dealerScript.handValue.ToString();
    }

    private void HitClicked()
    {
        // Check that there is still room on the table
        if (playerScript.cardIndex <= 11)
        {
            playerScript.GetCard();
            scoreText.text = "Ваш счет: " + playerScript.handValue.ToString();
            if (playerScript.handValue > 20) RoundOver();
        }
    }

    private void StandClicked()
    {
        standClicks++;
        int tr = dealerScript.cardIndex;
        HitDealer();
        standButtonText.text = "Принять";
        if (standClicks > 1 && tr == dealerScript.cardIndex)
        {
            RoundOver();
        }
    }

    private void HitDealer()
    {
        while (dealerScript.handValue < 17 && dealerScript.cardIndex < 10 && dealerScript.handValue <= playerScript.handValue)
        {
            dealerScript.GetCard();
            dealerScoreText.text = "Счет дилера: " + dealerScript.handValue.ToString();
            if (dealerScript.handValue > 20) RoundOver();
        }
    }

    // Проверка на победителя и проигравшего, окончание раздачи
    void RoundOver()
    {
        // Booleans (true/false) for bust and blackjack/21
        bool playerBust = playerScript.handValue > 21;
        bool dealerBust = dealerScript.handValue > 21;
        bool player21 = playerScript.handValue == 21;
        bool dealer21 = dealerScript.handValue == 21;

        // If stand has been clicked less than twice, no 21s or busts, quit function
        if (standClicks < 2 && !playerBust && !dealerBust && !player21 && !dealer21) return;
        bool roundOver = true;

        // All bust, bets returned
        if (playerBust && dealerBust)
        {
            mainText.text = "У всех перебор!";
            playerScript.AdjustMoney(pot / 2);
            dealerScript.AdjustMoney(pot / 2);
        }
        // if player busts, dealer didnt, or if dealer has more points, dealer wins
        else if (playerBust || (!dealerBust && dealerScript.handValue > playerScript.handValue))
        {
            mainText.text = "Вы проиграли!";
            dealerScript.AdjustMoney(pot);
        }
        // if dealer busts, player didnt, or player has more points, player wins
        else if (dealerBust || playerScript.handValue > dealerScript.handValue)
        {
            mainText.text = "ВЫ победили!";
            playerScript.AdjustMoney(pot);
        }
        //Check for tie, return bets
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
        // Set ui up for next move / hand / turn
        if (roundOver)
        {
            hitButton.gameObject.SetActive(false);
            standButton.gameObject.SetActive(false);
            mainText.gameObject.SetActive(true);
            dealerScoreText.gameObject.SetActive(true);
            hideCard.gameObject.SetActive(false);

            chipsText.text = "Ваш банк: " + playerScript.GetMoney().ToString();
            bankText.text = "Банк дилера: " + dealerScript.GetMoney().ToString();
            
            pot = 0;
            betText.text = "Ставка: " + pot.ToString();
            standClicks = 0;

            if (playerScript.GetMoney() == 0 || dealerScript.GetMoney() == 0)
            {
                GameOver();
            }
            else
                repeatButton.gameObject.SetActive(true);
        }
    }

    void GameOver()
    {
        gameoverButton.gameObject.SetActive(true);
        if (playerScript.GetMoney() == 0) mainText.text = "У вас кончились деньги!";
        else if (dealerScript.GetMoney() == 0) mainText.text = "У дилера кончились деньги!";
    }

    // Add money to pot if bet clicked
    void BetClicked(Button bet)
    {
        Text newBet = bet.GetComponentInChildren(typeof(Text)) as Text;
        int intBet = int.Parse(newBet.text.ToString());
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

    void popupClicked()
    {
        popupMenu.gameObject.SetActive(true);
        popupMenuButton.gameObject.SetActive(false);
        HidePanel.gameObject.SetActive(true);
        dealButton.interactable = false;
        repeatButton.interactable = false;
        hitButton.interactable = false;
        standButton.interactable = false;

        bet100Button.interactable = false;
        bet250Button.interactable = false;
        bet500Button.interactable = false;

        for (int i = 0; i < playerScript.hand.Length; i++)
        {
            var rendererPlayer = playerScript.hand[i].GetComponent<Renderer>();
            rendererPlayer.material.color *= 0.5f;
        }
        for (int i = 0; i < dealerScript.hand.Length; i++)
        {
            var rendererDealer = dealerScript.hand[i].GetComponent<Renderer>();
            rendererDealer.material.color *= 0.5f;
        }
        if (hideCard.activeSelf) dealerScript.hand[0].GetComponent<Renderer>().gameObject.SetActive(false);
        var rendererHide = hideCard.GetComponent<Renderer>();
        var rendererDeck = Deck.GetComponent<Renderer>();
        rendererHide.material.color *= 0.5f;
        rendererDeck.material.color *= 0.5f;
    }

    public void BackClicked()
    {
        popupMenu.gameObject.SetActive(false);
        popupMenuButton.gameObject.SetActive(true);
        HidePanel.gameObject.SetActive(false);
        dealButton.interactable = true;
        repeatButton.interactable = true;
        hitButton.interactable = true;
        standButton.interactable = true;

        bet100Button.interactable = true;
        bet250Button.interactable = true;
        bet500Button.interactable = true;

        for (int i = 0; i < playerScript.hand.Length; i++)
        {
            var rendererPlayer = playerScript.hand[i].GetComponent<Renderer>();
            rendererPlayer.material.color *= 2f;
        }
        for (int i = 0; i < dealerScript.hand.Length; i++)
        {
            var rendererDealer = dealerScript.hand[i].GetComponent<Renderer>();
            rendererDealer.material.color *= 2f;
        }
        dealerScript.hand[0].GetComponent<Renderer>().gameObject.SetActive(true);
        var rendererHide = hideCard.GetComponent<Renderer>();
        var rendererDeck = Deck.GetComponent<Renderer>();
        rendererHide.material.color *= 2f;
        rendererDeck.material.color *= 2f;
    }

    public void MenuClicked()
    {
        if (dealerScript.GetMoney() < playerScript.GetMoney()) saveRecordsScript.WriteNewScore("Вы", playerScript.GetMoney());
        else if (dealerScript.GetMoney() > playerScript.GetMoney()) saveRecordsScript.WriteNewScore("Дилер", dealerScript.GetMoney());
    }
}
