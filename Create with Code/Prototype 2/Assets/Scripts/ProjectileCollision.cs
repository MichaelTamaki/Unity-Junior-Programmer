using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        GameManager gameManager = (GameManager)GameObject.FindGameObjectWithTag("GameManager").GetComponent("GameManager");
        Destroy(gameObject);
        Destroy(other.gameObject);
        if (!gameManager.getIsGameOver())
        {
            gameManager.addScore();
        }
    }
}
