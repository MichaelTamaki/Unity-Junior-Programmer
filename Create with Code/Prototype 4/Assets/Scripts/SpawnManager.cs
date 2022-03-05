using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject toughEnemyPrefab;
    public GameObject[] powerupPrefabs;
    public float spawnBound = 9.0f;
    private int waveCount = 0;

    Vector3 GetRandomSpawn(float yPos)
    {
        float xPos = Random.Range(-spawnBound, spawnBound);
        float zPos = Random.Range(-spawnBound, spawnBound);
        return new Vector3(xPos, yPos, zPos);
    }

    void SpawnWave(int numEnemies)
    {
        int numEnemiesToSpawn = numEnemies;
        if (waveCount % 3 == 1)
        {
            int powerupIndex = Random.Range(0, powerupPrefabs.Length);
            GameObject powerupPrefab = powerupPrefabs[powerupIndex];
            Instantiate(powerupPrefab, GetRandomSpawn(0.1f), powerupPrefab.transform.rotation);
        }
        else if (waveCount % 3 == 0)
        {
            for (int i = 0; i < waveCount / 3; i += 1)
            {
                Instantiate(toughEnemyPrefab, GetRandomSpawn(3.0f), toughEnemyPrefab.transform.rotation);
                numEnemiesToSpawn -= 1;
            }
        }

        for (int i = 0; i < numEnemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GetRandomSpawn(0.1f), enemyPrefab.transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            waveCount += 1;
            SpawnWave(waveCount);
        }
    }

    public void RestartGame()
    {
        waveCount = 0;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] powerUps = GameObject.FindGameObjectsWithTag("Powerup");
        for (int i = 0; i < enemies.Length; i += 1)
        {
            Destroy(enemies[i]);
        }

        for (int i = 0; i < powerUps.Length; i += 1)
        {
            Destroy(powerUps[i]);
        }
    }
}
