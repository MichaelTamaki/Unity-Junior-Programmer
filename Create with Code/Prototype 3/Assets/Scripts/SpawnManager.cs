using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] obstacleObjects;
    public float startingX = 25.0f;
    public float spawnStartTime = 3.0f;
    public float spawnFrequency = 2.0f;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnRandomObstacle", spawnStartTime, spawnFrequency);
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnRandomObstacle()
    {
        if (playerController.isGameOver)
        {
            return;
        }

        int obstacleIndex = Random.Range(0, obstacleObjects.Length);
        GameObject obstacleObject = obstacleObjects[obstacleIndex];
        Instantiate(obstacleObject, new Vector3(startingX, 0.1f, 0), obstacleObject.transform.rotation);
    }
}
