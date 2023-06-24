using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    public int rollVal;
    public bool canRoll;
    public bool rolled;
    public bool turn;
    public GameObject rollAmtText;
    public GameObject[] homePegs;
    public bool[] homePegsTaken;
    public int pegsHome;
    public int pegOffset;
    public GameObject nextPlayer;
    public player teamMate;
    public Peg[] pegs;
    public bool finished;
    int timeSinceLastMoveAI;

    public bool AI;
    public bool takingTurnAI;
    public bool makingMoveAI;

    public float turnSpeed;
    public float moveSpeed;
    public int turnTimer;

    public bool canEndTurn;

    public bool extraTeamRollDone;

    // Start is called before the first frame update
    void Start()
    {
        extraTeamRollDone = false;
        timeSinceLastMoveAI = 0;
        pegsHome = 0;
        rollVal = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (turn && AI)
        {
            GameObject.Find("GameMngr").GetComponent<Manager>().rollButton.interactable = false;
            GameObject.Find("GameMngr").GetComponent<Manager>().endTurnButton.interactable = false;
            timeSinceLastMoveAI++;
            if(timeSinceLastMoveAI > turnTimer)
            {
                GameObject.Find("GameMngr").GetComponent<Manager>().EndTurn();
                timeSinceLastMoveAI = 0;
            }
        }

        //AI Turn
        if (AI && !takingTurnAI)
        {
            if (turn)
            {
                Invoke("AutomateTurn", turnSpeed);
                takingTurnAI = true;
            }
        }

        if (finished && turn)
        {
            EndRoll();
            GameObject.Find("GameMngr").GetComponent<Manager>().EndTurn();
        }

        if(pegsHome > 3 && !finished)
        {     
            finished = true;
            if (GameObject.Find("GameMngr").GetComponent<Manager>().teams)
            {
                if (GameObject.Find("GameMngr").GetComponent<Manager>().noWinner)
                {
                    GameObject.Find("GameMngr").GetComponent<AudioSource>().Play();
                    if ("Player " + (GameObject.Find("GameMngr").GetComponent<Manager>().currentPlayer + 1) == "Player 1")
                    {
                        GameObject.Find("GameMngr").GetComponent<Manager>().winnerText.text = "Blurple Wins!";
                    }
                    else if ("Player " + (GameObject.Find("GameMngr").GetComponent<Manager>().currentPlayer + 1) == "Player 2")
                    {
                        GameObject.Find("GameMngr").GetComponent<Manager>().winnerText.text = "Greenge Wins!";
                    }
                    else if ("Player " + (GameObject.Find("GameMngr").GetComponent<Manager>().currentPlayer + 1) == "Player 3")
                    {
                        GameObject.Find("GameMngr").GetComponent<Manager>().winnerText.text = "Blurple Wins!";
                    }
                    else if ("Player " + (GameObject.Find("GameMngr").GetComponent<Manager>().currentPlayer + 1) == "Player 4")
                    {
                        GameObject.Find("GameMngr").GetComponent<Manager>().winnerText.text = "Greenge Wins!";
                    }
                    GameObject.Find("GameMngr").GetComponent<Manager>().noWinner = false;
                    GameObject.Find("GameMngr").GetComponent<Manager>().endScreen.SetActive(true);
                }
            }
            else
            {
                if (GameObject.Find("GameMngr").GetComponent<Manager>().noWinner)
                {
                    GameObject.Find("GameMngr").GetComponent<AudioSource>().Play();
                    if ("Player " + (GameObject.Find("GameMngr").GetComponent<Manager>().currentPlayer + 1) == "Player 1")
                    {
                        GameObject.Find("GameMngr").GetComponent<Manager>().winnerText.text = "Blue Wins!";
                    }
                    else if ("Player " + (GameObject.Find("GameMngr").GetComponent<Manager>().currentPlayer + 1) == "Player 2")
                    {
                        GameObject.Find("GameMngr").GetComponent<Manager>().winnerText.text = "Green Wins!";
                    }
                    else if ("Player " + (GameObject.Find("GameMngr").GetComponent<Manager>().currentPlayer + 1) == "Player 3")
                    {
                        GameObject.Find("GameMngr").GetComponent<Manager>().winnerText.text = "Purple Wins!";
                    }
                    else if ("Player " + (GameObject.Find("GameMngr").GetComponent<Manager>().currentPlayer + 1) == "Player 4")
                    {
                        GameObject.Find("GameMngr").GetComponent<Manager>().winnerText.text = "Orange Wins!";
                    }
                    GameObject.Find("GameMngr").GetComponent<Manager>().noWinner = false;
                    GameObject.Find("GameMngr").GetComponent<Manager>().endScreen.SetActive(true);
                }
            }
            GameObject.Find("GameMngr").GetComponent<Manager>().numFinished++;
            EndRoll();
            GameObject.Find("GameMngr").GetComponent<Manager>().EndTurn();

        }

        if (rolled)
        {
            //if all in start
            if (pegs[0].inStart && pegs[1].inStart && pegs[2].inStart && pegs[3].inStart)
            {
                //and can't leave end turn
                if (!(rollVal == 1 || rollVal == 6 || rollVal == 12))
                {
                    if (GameObject.Find("GameMngr").GetComponent<Manager>().teams && teamMate.finished && !extraTeamRollDone)
                    {
                        ReRoll();
                        extraTeamRollDone = true;
                    }
                    else
                    {
                        canEndTurn = true;
                    }
                }
            }
            else
            {
                checkMoves();
                //check if any pegs can move
                if (!(pegs[0].canMove || pegs[1].canMove || pegs[2].canMove || pegs[3].canMove))
                {
                    if (rollVal == 6 || rollVal == 12)
                    {
                        ReRoll();
                    }
                    else
                    {
                        if (GameObject.Find("GameMngr").GetComponent<Manager>().teams && teamMate.finished && !extraTeamRollDone)
                        {
                            ReRoll();
                            extraTeamRollDone = true;
                        }
                        else
                        {
                            canEndTurn = true;
                        }
                    }
                }
            }

        }

        if (canEndTurn)
        {
            EndRoll();
            if (AI)
            {
                extraTeamRollDone = false;
                GameObject.Find("GameMngr").GetComponent<Manager>().EndTurn();
            }      
        }
        else
        {
            if (AI && !makingMoveAI && turn)
            {
                Invoke("AutomateMove", moveSpeed);
                makingMoveAI = true;
            }
        }
    }

    public void Roll()
    {
        if (turn && canRoll)
        {
            if(GameObject.Find("GameMngr").GetComponent<Manager>().fastGame)
            {
                rollVal = Random.Range(1, 13);
            }
            else{
                rollVal = Random.Range(1, 7);
            }
            canRoll = false;
            rolled = true;
            rollAmtText.GetComponent<TextMeshProUGUI>().text = rollVal.ToString();
        }
    }

    void checkMoves()
    {
        pegs[0].checkMove();
        pegs[1].checkMove();
        pegs[2].checkMove();
        pegs[3].checkMove();
    }

    public void EndTurn()
    {
        canEndTurn = false;
        extraTeamRollDone = false;
        turn = false;
        takingTurnAI = false;
        makingMoveAI = false;
        GameObject.Find("GameMngr").GetComponent<Manager>().currentPlayer++;
        nextPlayer.GetComponent<player>().turn = true;
        nextPlayer.GetComponent<player>().canRoll = true;
        nextPlayer.GetComponent<player>().rolled = false;
        nextPlayer.GetComponent<player>().canEndTurn = false;
        nextPlayer.GetComponent<player>().extraTeamRollDone = false;

    }

    public void EndRoll()
    {
        rollVal = 0;
        canRoll = false;
        rolled = false;
    }

    public void ReRoll()
    {
        rollVal = 0;
        canRoll = true;
        rolled = false;
        makingMoveAI = false;
        takingTurnAI = false;
        canEndTurn = false;
    }

    public void AutomateTurn()
    {
        GameObject.Find("GameMngr").GetComponent<Manager>().Roll();
        checkMoves();
    }

    public void AutomateMove()
    {
        int index = 0;
        Peg[] moveablePegs = new Peg[4];

        //priority
        //into home, killing, away from other pegs, in home, randomchoice(from start, around board)
        //can move into home
        foreach (Peg peg in pegs)
        {
            if (peg.canMove)
            {
                if (peg.canMoveIntoHome)
                {
                    peg.OnMouseDown();
                    timeSinceLastMoveAI = 0;
                    return;
                }
            }
        }
        //can kill
        foreach (Peg peg in pegs)
        {
            if (peg.canMove)
            {
                if (peg.canKill)
                {
                    peg.OnMouseDown();
                    timeSinceLastMoveAI = 0;
                    return;
                }
            }
        }
        //near enemy pegs
        foreach (Peg peg in pegs)
        {
            if (peg.canMove)
            {
                if (peg.nearEnemy)
                {
                    peg.OnMouseDown();
                    timeSinceLastMoveAI = 0;
                    return;
                }
            }
        }
        //in home
        foreach (Peg peg in pegs)
        {
            if (peg.canMove)
            {
                if (peg.inHome)
                {
                    peg.OnMouseDown();
                    timeSinceLastMoveAI = 0;
                    return;
                }
            }
        }
        //enemy ahead
        foreach (Peg peg in pegs)
        {
            if (peg.canMove)
            {
                if (peg.nearEnemyAhead)
                {
                    peg.OnMouseDown();
                    timeSinceLastMoveAI = 0;
                    return;
                }
            }
        }
        //random choice
        foreach (Peg peg in pegs)
        {
            if (peg.canMove)
            {
                moveablePegs[index] = peg;
                index++;
            }
        }

        if (index != 0)
        {
            moveablePegs[Random.Range(0, index)].OnMouseDown();
            timeSinceLastMoveAI = 0;
        }

    }
}
