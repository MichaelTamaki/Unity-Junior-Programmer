using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    public GameObject[] animalPrefabs;
    public float spawnFrequency = 1.0f;
    public float xRange = 14.0f;
    public Vector3 spawnMiddle = new Vector3(0, 0, 24);
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnAtRandomLocation", spawnFrequency, spawnFrequency);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SpawnAtRandomLocation()
    {
        GameManager gameManager = (GameManager)GameObject.FindGameObjectWithTag("GameManager").GetComponent("GameManager");
        if (gameManager.getIsGameOver())
        {
            return;
        }

        int animalIndex = Random.Range(0, animalPrefabs.Length);
        GameObject animalPrefab = animalPrefabs[animalIndex];
        Instantiate(animalPrefab, spawnMiddle + (Vector3.right * Random.Range(-xRange, xRange)), animalPrefab.transform.rotation);
    }
}
