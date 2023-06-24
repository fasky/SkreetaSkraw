using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peg : MonoBehaviour
{
    public Sprite img;

    public bool inStart;
    public bool home; //in final spot
    public bool inHome; //in last 4

    public bool goneAround;

    public bool canMove;
    public bool canKill;
    public bool canMoveIntoHome;
    public bool nearEnemy;
    public bool nearEnemyAhead;

    //first peg when out
    public int startPeg;
    //current peg
    public int currentPeg;
    public int endPeg; //last peg before home

    //initial position in start
    public GameObject initialPeg;
    public Peg[] otherPegs;

    public Sprite futureMove;
    public Sprite killMove;

    Manager manager;
    player player;

    public AudioSource pegMove;

    // Start is called before the first frame update
    void Start()
    {
        
        if (startPeg != 0)
        {
            goneAround = false;
        }
        else
        {
            goneAround = true;
        }
        
        player = gameObject.GetComponentInParent<player>();
        gameObject.GetComponent<SpriteRenderer>().sprite = img;
        manager = GameObject.Find("GameMngr").GetComponent<Manager>();

    }

    public void CheckAheadForEnemy()
    {
        if (player.turn && !inHome && !inStart)
        {
            for (int i = 1; i < 10; i++)
            {
                int pegToCheck = currentPeg + i;
                if (pegToCheck > 55)
                {
                    pegToCheck -= 56;
                }
                if (manager.spotTaken[pegToCheck])
                {
                    bool friendly = false;
                    foreach (Peg peg in otherPegs)
                    {
                        if (peg.currentPeg == pegToCheck && !peg.inHome)
                        {
                            friendly = true;
                            break;
                        }
                    }
                    if (!friendly)
                    {
                        nearEnemyAhead = true;
                        return;
                    }
                    else
                    {
                        nearEnemyAhead = false;
                    }
                }
                else
                {
                    nearEnemyAhead = false;
                }
            }
        }
    }

    public void CheckBehindForEnemy()
    {
        if (player.turn && !inHome && !inStart)
        {
            for(int i = 1; i < 7; i++)
            {
                int pegToCheck = currentPeg - i;
                if(pegToCheck < 0)
                {
                    pegToCheck += 56; 
                }
                if (manager.spotTaken[pegToCheck])
                {
                    bool friendly = false;
                    foreach (Peg peg in otherPegs)
                    {
                        if(peg.currentPeg == pegToCheck && !peg.inHome)
                        {
                            friendly = true;
                            break;
                        }
                    }
                    if (!friendly)
                    {
                        nearEnemy = true;
                        return;
                    }
                    else
                    {
                        nearEnemy = false;
                    }
                }
                else
                {
                    nearEnemy = false;
                }
            }
        }
    }

    public void checkMove()
    {
        if (player.turn)
        {
            //check for move from start
            if (inStart && (player.rollVal == 1 || player.rollVal == 6 || player.rollVal == 12))
            {

                if (manager.spotTaken[startPeg])
                {
                    bool friendly = false;
                    foreach (Peg peg in otherPegs)
                    {
                        if (peg.currentPeg == startPeg && !peg.inHome)
                        {
                            friendly = true;
                            canMove = false;
                            break;
                        }
                        else
                        {
                            canMove = true;
                        }
                    }
                    if (!friendly)
                    {
                        canMove = true;
                        canKill = true;
                    }
                }
                else
                {
                    canMove = true;
                }
            }
            //check for move on board
            else if (!inStart && !home && !inHome)
            {
                //into home
                if (goneAround && ((currentPeg + player.rollVal) > endPeg))
                {
                    if ((((currentPeg + player.rollVal) % (endPeg + 1)) <= 3) && player.homePegs[(currentPeg + player.rollVal) % (endPeg + 1)] && !player.homePegsTaken[(currentPeg + player.rollVal) % (endPeg + 1)])
                    {
                        //check pegs before home if necessary
                        if(currentPeg < endPeg)
                        {
                            canMove = true;
                            canMoveIntoHome = true;
                            for (int i = currentPeg + 1; i <= endPeg; i++)
                            {
                                if (manager.spotTaken[i] == true)
                                {
                                    bool friendly = false;
                                    foreach (Peg peg in otherPegs)
                                    {
                                        if (peg.currentPeg == i && !peg.inHome)
                                        {
                                            friendly = true;
                                            canMove = false;
                                            canMoveIntoHome = false;
                                            canKill = false;
                                            break;
                                        }
                                    }

                                    if (!friendly && i == currentPeg + player.rollVal)
                                    {
                                        canMove = true;
                                        canMoveIntoHome = true;
                                        canKill = true;
                                        break;
                                    }
                                }

                                if (!canMove)
                                {
                                    return;
                                }
                            }
                        }
                        //check if pegs in
                        for (int i = 0; i <= ((currentPeg + player.rollVal) % (endPeg + 1)); i++)
                        {
                            if (player.homePegsTaken[i])
                            {
                                canMove = false;
                                canMoveIntoHome = false;
                                break;
                            }
                            else
                            {
                                canMove = true;
                                canMoveIntoHome = true;
                            }
                        }
                    }
                    else
                    {
                        canMove = false;
                        canMoveIntoHome = false;
                    }
                }
                else //normally
                {
                    canMove = true;
                    //check if other player pegs in
                    for (int i = currentPeg + 1; i <= currentPeg + player.rollVal; i++)
                    {
                        if (manager.spotTaken[i] == true)
                        {
                            bool friendly = false;
                            foreach (Peg peg in otherPegs)
                            {
                                if (peg.currentPeg == i && !peg.inHome)
                                {
                                    friendly = true;
                                    canMove = false;
                                    canKill = false;
                                    break;
                                }
                            }

                            if (!friendly && i == currentPeg + player.rollVal)
                            {
                                canMove = true;
                                canKill = true;
                                break;
                            }
                        }

                        if (!canMove)
                        {
                            return;
                        }
                    }
                }
            }
            //check for move in home
            else if (!home && inHome && (currentPeg + player.rollVal <= 3) && !player.homePegsTaken[currentPeg + player.rollVal])
            {
                //check if pegs in
                for (int i = currentPeg + 1; i <= currentPeg + player.rollVal; i++)
                {
                    if (player.homePegsTaken[i])
                    {
                        canMove = false;
                        return;
                    }
                    else
                    {
                        canMove = true;
                    }
                }
            }
            else
            {
                canMove = false;
                canKill = false;
            }
        }
        else
        {
            canMove = false;
            canMoveIntoHome = false;
            canKill = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        checkMove();
        CheckBehindForEnemy();
        CheckAheadForEnemy();

        if (!player.rolled && player.turn)
        {
            foreach(GameObject spot in manager.spots)
            {
                spot.GetComponent<SpriteRenderer>().sprite = null;
            }
            foreach (GameObject spot in player.homePegs)
            {
                spot.GetComponent<SpriteRenderer>().sprite = null;
            }
        }
    }
    public void OnMouseDown()
    {
        if (player.rolled && canMove)
        {
            pegMove.Play();

            //move from start
            if ((player.rollVal == 1 || player.rollVal == 6 || player.rollVal == 12) && inStart)
            {
                gameObject.transform.position = manager.spots[startPeg].transform.position;
                currentPeg = startPeg;
                inStart = false;
                manager.spotTaken[startPeg] = true;
                if (player.rollVal != 6 && player.rollVal != 12)
                {
                    if (GameObject.Find("GameMngr").GetComponent<Manager>().teams && player.teamMate.finished && !player.extraTeamRollDone)
                    {
                        player.ReRoll();
                        player.extraTeamRollDone = true;
                    }
                    else
                    {
                        player.EndRoll();
                    }
                    
                }
                else
                {
                    player.ReRoll();
                }
            }
            else if (!inStart && !home && !inHome)
            {
                //move into home
                if (goneAround && ((currentPeg + player.rollVal) > endPeg))
                {
                    if ((((currentPeg + player.rollVal) % (endPeg + 1)) <= 3) && player.homePegs[(currentPeg + player.rollVal) % (endPeg + 1)] && !player.homePegsTaken[(currentPeg + player.rollVal) % (endPeg + 1)])
                    {
                        manager.spotTaken[currentPeg] = false;
                        gameObject.transform.position = player.homePegs[(currentPeg + player.rollVal) % (endPeg + 1)].transform.position;
                        player.homePegsTaken[(currentPeg + player.rollVal) % (endPeg + 1)] = true;
                        currentPeg = (currentPeg + player.rollVal) % (endPeg + 1);

                        inHome = true;
                        home = true;
                        for (int i = currentPeg; i < 4; i++)
                        {
                            if (!player.homePegsTaken[i])
                            {
                                home = false;
                            }
                        }
                        if (home)
                        {
                            player.pegsHome++;
                        }

                        if (player.rollVal != 6 && player.rollVal != 12)
                        {
                            if (GameObject.Find("GameMngr").GetComponent<Manager>().teams && player.teamMate.finished && !player.extraTeamRollDone)
                            {
                                player.ReRoll();
                                player.extraTeamRollDone = true;
                            }
                            else
                            {
                                player.EndRoll();
                            }
                        }
                        else
                        {
                            player.ReRoll();
                        }

                    }
                }
                else //move normally
                {
                    manager.spotTaken[currentPeg] = false;
                    gameObject.transform.position = manager.spots[currentPeg + player.rollVal].transform.position;
                    currentPeg = currentPeg + player.rollVal;
                    if (currentPeg > 55)
                    {
                        currentPeg -= 56;
                        goneAround = true;
                    }
                    manager.spotTaken[currentPeg] = true;

                    if (player.rollVal != 6 && player.rollVal != 12)
                    {
                        if (GameObject.Find("GameMngr").GetComponent<Manager>().teams && player.teamMate.finished && !player.extraTeamRollDone)
                        {
                            player.ReRoll();
                            player.extraTeamRollDone = true;
                        }
                        else
                        {
                            player.EndRoll();
                        }
                    }
                    else
                    {
                        player.ReRoll();
                    }

                }
            }
            //move in home
            else if (!home && inHome && (currentPeg + player.rollVal <= 3) && !player.homePegsTaken[currentPeg + player.rollVal])
            {
                player.homePegsTaken[currentPeg] = false;
                gameObject.transform.position = player.homePegs[currentPeg + player.rollVal].transform.position;
                player.homePegsTaken[currentPeg + player.rollVal] = true;
                currentPeg = currentPeg + player.rollVal;

                home = true;
                for (int i = currentPeg; i < 4; i++)
                {
                    if (!player.homePegsTaken[i])
                    {
                        home = false;
                    }
                }
                if (home)
                {
                    player.pegsHome++;
                }

                if (player.rollVal != 6 && player.rollVal != 12)
                {
                    if (GameObject.Find("GameMngr").GetComponent<Manager>().teams && player.teamMate.finished && !player.extraTeamRollDone)
                    {
                        player.ReRoll();
                        player.extraTeamRollDone = true;
                    }
                    else
                    {
                        player.EndRoll();
                    }
                }
                else
                {
                    player.ReRoll();
                }

            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!player.turn)
        {
            currentPeg = -1;
            inStart = true;
            if (startPeg != 0)
            {
                goneAround = false;
            }
            gameObject.transform.position = initialPeg.transform.position;
        }
    }

    private void OnMouseOver()
    {
        if (player.turn && player.rolled && !(manager.endTurnButton.IsInteractable()))
        {
            //check for move from start
            if (inStart && (player.rollVal == 1 || player.rollVal == 6 || player.rollVal == 12))
            {
                if (canMove && !canKill)
                {
                    manager.spots[startPeg].GetComponent<SpriteRenderer>().sprite = futureMove;
                }
                else if (canMove && canKill)
                {
                    manager.spots[startPeg].GetComponent<SpriteRenderer>().sprite = killMove;
                }
            }
            //check for move on board
            else if (!inStart && !home && !inHome)
            {
                //into home
                if (goneAround && ((currentPeg + player.rollVal) > endPeg))
                {
                    if ((((currentPeg + player.rollVal) % (endPeg + 1)) <= 3) && player.homePegs[(currentPeg + player.rollVal) % (endPeg + 1)] && !player.homePegsTaken[(currentPeg + player.rollVal) % (endPeg + 1)])
                    {
                        if (canMove)
                        {
                            player.homePegs[(currentPeg + player.rollVal) % (endPeg + 1)].GetComponent<SpriteRenderer>().sprite = futureMove;
                        }
                    }
                }
                else //normally
                {
                    if (canMove && !canKill)
                    {
                        manager.spots[currentPeg + player.rollVal].GetComponent<SpriteRenderer>().sprite = futureMove;
                    }
                    else if (canMove && canKill)
                    {
                        manager.spots[currentPeg + player.rollVal].GetComponent<SpriteRenderer>().sprite = killMove;
                    }
                }
            }
            //check for move in home
            else if (!home && inHome && (currentPeg + player.rollVal <= 3) && !player.homePegsTaken[currentPeg + player.rollVal])
            {
                if (canMove)
                {
                    player.homePegs[currentPeg + player.rollVal].GetComponent<SpriteRenderer>().sprite = futureMove;
                }
            }
        }
    }

    private void OnMouseExit()
    {
        foreach (GameObject spot in manager.spots)
        {
            spot.GetComponent<SpriteRenderer>().sprite = null;
        }
        foreach (GameObject spot in player.homePegs)
        {
            spot.GetComponent<SpriteRenderer>().sprite = null;
        }
    }
}
