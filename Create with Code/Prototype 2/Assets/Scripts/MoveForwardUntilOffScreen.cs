using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwardUntilOffScreen : MonoBehaviour
{
    public float speed = 40.0f;
    public float zLimit = 20.0f;

    // Update is called once per frame
    void Update()
    {
        GameManager gameManager = (GameManager)GameObject.FindGameObjectWithTag("GameManager").GetComponent("GameManager");
        if (gameManager.getIsGameOver())
        {
            return;
        }

        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        if (transform.position.z > zLimit)
        {
            Destroy(gameObject);
        }
        else if (transform.position.z < -zLimit)
        {
            Destroy(gameObject);
            gameManager.loseLife();
        }
    }
}
