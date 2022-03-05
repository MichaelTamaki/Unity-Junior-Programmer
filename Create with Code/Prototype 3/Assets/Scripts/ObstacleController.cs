using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private float speed = 15.0f;
    public float leftXBoundary = -10.0f;
    private PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        speed = GameObject.Find("Background").GetComponent<RepeatBackground>().speed;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.isGameOver)
        {
            return;
        }

        transform.Translate(Vector3.left * Time.deltaTime * speed);

        if (transform.position.x < leftXBoundary)
        {
            Destroy(gameObject);
        }
    }
}
