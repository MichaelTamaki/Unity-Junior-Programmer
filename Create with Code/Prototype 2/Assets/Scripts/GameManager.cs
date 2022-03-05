using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int lives = 3;
    private int score = 0;
    private bool isGameOver = false;

    void Start()
    {
        Debug.Log("Starting game with " + lives + " lives!");
    }

    public bool getIsGameOver()
    {
        return isGameOver;
    }

    public void loseLife()
    {
        lives -= 1;
        Debug.Log("Lives: " + lives);
        if (lives <= 0)
        {
            isGameOver = true;
            Debug.Log("Game Over! Final score: " + score);
        }
    }

    public void addScore()
    {
        score += 1;
        Debug.Log("Score: " + score);
    }
}
