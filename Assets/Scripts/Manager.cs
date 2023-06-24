using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public GameObject[] spots;
    public bool[] spotTaken;
    public player[] players;
    public int currentPlayer;
    public TextMeshProUGUI currentPText;

    public Button rollButton;
    public Button endTurnButton;
    public bool noWinner;
    public int numFinished;

    public TextMeshProUGUI winnerText;
    public GameObject endScreen;
    public GameObject continueButton;

    public bool fastGame;
    public bool teams;

    // Start is called before the first frame update
    void Start()
    {
        fastGame = false;
        numFinished = 0;
        noWinner = true;
        currentPlayer = -1;
        spotTaken = new bool[112];
        currentPText.text = "Player " + (currentPlayer + 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            endScreen.SetActive(!endScreen.activeInHierarchy);
        }

        if (currentPlayer != -1)
        {
            if (currentPlayer > players.Length-1)
            {
                currentPlayer = 0;
                if ("Player " + (currentPlayer + 1) == "Player 1")
                {
                    currentPText.text = "Blue's Turn";
                }
                else if ("Player " + (currentPlayer + 1) == "Player 2")
                {
                    currentPText.text = "Green's Turn";
                }
                else if ("Player " + (currentPlayer + 1) == "Player 3")
                {
                    currentPText.text = "Purple's Turn";
                }
                else if ("Player " + (currentPlayer + 1) == "Player 4")
                {
                    currentPText.text = "Orange's Turn";
                }
            }

            if (players[currentPlayer].canRoll)
            {
                rollButton.interactable = true;
            }
            else
            {
                rollButton.interactable = false;
            }

            if (!players[currentPlayer].canRoll && !players[currentPlayer].rolled)
            {
                endTurnButton.interactable = true;
            }
            else
            {
                endTurnButton.interactable = false;
            }
        }

        if (numFinished == (players.Length - 1))
        {
            EndGame();
        }
    }

    public void Roll()
    {
        if (currentPlayer > players.Length - 1)
        {
            currentPlayer = 0;
            if ("Player " + (currentPlayer + 1) == "Player 1")
            {
                currentPText.text = "Blue's Turn";
            }
            else if ("Player " + (currentPlayer + 1) == "Player 2")
            {
                currentPText.text = "Green's Turn";
            }
            else if ("Player " + (currentPlayer + 1) == "Player 3")
            {
                currentPText.text = "Purple's Turn";
            }
            else if ("Player " + (currentPlayer + 1) == "Player 4")
            {
                currentPText.text = "Orange's Turn";
            }
        }
        players[currentPlayer].Roll();
    }

    public void SetAISpeed(bool normal)
    {
        if (normal)
        {
            foreach (player playr in players)
            {
                playr.turnSpeed = 0.5f;
                playr.moveSpeed = 1.5f;
                playr.turnTimer = 150;
            }
        }
        else //fast
        {
            foreach (player playr in players)
            {
                playr.turnSpeed = 0.05f;
                playr.moveSpeed = 0.1f;
                playr.turnTimer = 20;
            }
        }
        
    }

    public void SetGameSpeed(bool fast)
    {
        fastGame = fast;
    }

    public void SetTeams(bool teamss)
    {
        teams = teamss;
    }

    void EndGame()
    {
        //bring up end game screen
        endScreen.SetActive(true);
        continueButton.SetActive(false);
    }

    public void EndTurn()
    {
        if (currentPlayer > players.Length - 1)
        {
            currentPlayer = 0;
            if ("Player " + (currentPlayer + 1) == "Player 1")
            {
                currentPText.text = "Blue's Turn";
            }
            else if ("Player " + (currentPlayer + 1) == "Player 2")
            {
                currentPText.text = "Green's Turn";
            }
            else if ("Player " + (currentPlayer + 1) == "Player 3")
            {
                currentPText.text = "Purple's Turn";
            }
            else if ("Player " + (currentPlayer + 1) == "Player 4")
            {
                currentPText.text = "Orange's Turn";
            }
        }
        if (!players[currentPlayer].canRoll && !players[currentPlayer].rolled)
        {
            players[currentPlayer].EndTurn();
            if ("Player " + (currentPlayer + 1) == "Player 1")
            {
                currentPText.text = "Blue's Turn";
            }
            else if ("Player " + (currentPlayer + 1) == "Player 2")
            {
                currentPText.text = "Green's Turn";
            }
            else if ("Player " + (currentPlayer + 1) == "Player 3")
            {
                currentPText.text = "Purple's Turn";
            }
            else if ("Player " + (currentPlayer + 1) == "Player 4")
            {
                currentPText.text = "Orange's Turn";
            }
            //currentPText.text = "Player " + (currentPlayer + 1);
        }
    }

    public void startingRolls()
    {
        int[] rolls = new int[players.Length];
        for(int i = 0; i < players.Length; i++)
        {
            rolls[i] = UnityEngine.Random.Range(0, 7);
        }

        int highest = 0;

        for(int i = 0; i < rolls.Length; i++)
        {
            if(rolls[i] > highest)
            {
                highest = rolls[i];
                currentPlayer = i;
            }
        }

        players[currentPlayer].canRoll = true;
        players[currentPlayer].turn = true;
        if ("Player " + (currentPlayer + 1) == "Player 1")
        {
            currentPText.text = "Blue Goes First!";
        }
        else if ("Player " + (currentPlayer + 1) == "Player 2")
        {
            currentPText.text = "Green Goes First!";
        }
        else if ("Player " + (currentPlayer + 1) == "Player 3")
        {
            currentPText.text = "Purple Goes First!";
        }
        else if ("Player " + (currentPlayer + 1) == "Player 4")
        {
            currentPText.text = "Orange Goes First!";
        }
        GameObject.Find("StartingRollHolder").SetActive(false);
    }
}
