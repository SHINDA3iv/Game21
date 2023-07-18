using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button dealButton;
    public Button repeatButton;
    public Button hitButton;
    public Button standButton;
    public Button bet50Button;
    public Button bet100Button;
    public Button popupMenuButton;
    public Button backButton; 
    public Button menuButton;


    private int standClicks = 0;

    public PlayerScript playerScript;
    public PlayerScript dealerScript;
    public SaveRecordsScript SaveRecordsScript;

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
        bet50Button.onClick.AddListener(() => BetClicked(bet50Button));
        bet100Button.onClick.AddListener(() => BetClicked(bet100Button));
        popupMenuButton.onClick.AddListener(() => popupClicked());
        backButton.onClick.AddListener(() => BackClicked());
        menuButton.onClick.AddListener(() => MenuClicked());

        hitButton.gameObject.SetActive(false);
        standButton.gameObject.SetActive(false);
        dealButton.gameObject.SetActive(false);
        repeatButton.gameObject.SetActive(false);
        dealerScoreText.gameObject.SetActive(false);
        hideCard.gameObject.SetActive(false);
        popupMenu.gameObject.SetActive(false);
        HidePanel.gameObject.SetActive(false);

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

        bet50Button.gameObject.SetActive(false);
        bet100Button.gameObject.SetActive(false);

        if (playerScript.handValue == 21 || dealerScript.handValue == 21) RoundOver();
    }

    private void RepeatClicked()
    {
        bet50Button.gameObject.SetActive(true);
        bet100Button.gameObject.SetActive(true);
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
        if (playerScript.cardIndex <= 10)
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
            mainText.text = "У всех перебор: ставки возвращены!";
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
            mainText.text = "Ничья: ставки возвращены!";
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
            dealButton.gameObject.SetActive(false);
            mainText.gameObject.SetActive(true);
            repeatButton.gameObject.SetActive(true);
            dealerScoreText.gameObject.SetActive(true);
            hideCard.GetComponent<Renderer>().enabled = false;
            chipsText.text = "Ваш банк: " + playerScript.GetMoney().ToString();
            bankText.text = "Банк дилера: " + dealerScript.GetMoney().ToString();
            PlayerPrefs.SetInt("highscoreTable", playerScript.GetMoney());
            pot = 0;
            betText.text = "Ставка: " + pot.ToString();
            standClicks = 0;
        }
    }

    // Add money to pot if bet clicked
    void BetClicked(Button bet)
    {
        Text newBet = bet.GetComponentInChildren(typeof(Text)) as Text;
        int intBet = int.Parse(newBet.text.ToString());
        if (playerScript.GetMoney() >= intBet && dealerScript.GetMoney() >= intBet)
        {
            dealButton.gameObject.SetActive(true);
            playerScript.AdjustMoney(-intBet);
            dealerScript.AdjustMoney(-intBet);
            chipsText.text = "Ваш банк: " + playerScript.GetMoney().ToString();
            bankText.text = "Банк дилера: " + dealerScript.GetMoney().ToString();
            pot += (intBet * 2);
            betText.text = "Ставка: " + pot.ToString();
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
        bet50Button.interactable = false;
        bet100Button.interactable = false;

        for (int i = 0; i < playerScript.hand.Length; i++)
        {
            var rendererPlayer = playerScript.hand[i].GetComponent<Renderer>();
            var rendererDealer = dealerScript.hand[i].GetComponent<Renderer>();
            rendererPlayer.material.color *= 0.5f;
            rendererDealer.material.color *= 0.5f;
        }
        dealerScript.hand[0].GetComponent<Renderer>().gameObject.SetActive(false);
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
        bet50Button.interactable = true;
        bet100Button.interactable = true;

        for (int i = 0; i < playerScript.hand.Length; i++)
        {
            var rendererplayer = playerScript.hand[i].GetComponent<Renderer>();
            var rendererdealer = dealerScript.hand[i].GetComponent<Renderer>();
            rendererplayer.material.color *= 2f;
            rendererdealer.material.color *= 2f;
        }
        dealerScript.hand[0].GetComponent<Renderer>().gameObject.SetActive(true);
        var rendererHide = hideCard.GetComponent<Renderer>();
        var rendererDeck = Deck.GetComponent<Renderer>();
        rendererHide.material.color *= 2f;
        rendererDeck.material.color *= 2f;
    }

    public void MenuClicked()
    {
        SaveRecordsScript.SaveScore(playerScript.GetMoney());
    }
}
