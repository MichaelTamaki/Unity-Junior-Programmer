using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public int scoreAmount;
    public ParticleSystem explosionParticle;
    Rigidbody targetRb;
    private float minForce = 13f;
    private float maxForce = 16f;
    private float spawnTorque = 4f;
    private float spawnXRange = 5f;
    private float spawnY = -5f;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        targetRb = GetComponent<Rigidbody>();
        targetRb.AddForce(Vector3.up * Random.Range(minForce, maxForce), ForceMode.Impulse);
        targetRb.AddTorque(new Vector3(GenerateRandomRange(spawnTorque), GenerateRandomRange(spawnTorque), GenerateRandomRange(spawnTorque)), ForceMode.Impulse);
        transform.position = new Vector3(GenerateRandomRange(spawnXRange), spawnY);
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    private void OnMouseOver()
    {
        if (!Input.GetKey(KeyCode.Mouse0) || !gameManager.isGameActive || gameManager.isGamePaused)
        {
            return;
        }

        Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
        gameManager.AddScore(scoreAmount);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gameObject.CompareTag("Bad") && gameManager.isGameActive)
        {
            gameManager.SubtractLives(1);
        }
        Destroy(gameObject);
    }

    float GenerateRandomRange(float range)
    {
        return Random.Range(-range, range);
    }
}
