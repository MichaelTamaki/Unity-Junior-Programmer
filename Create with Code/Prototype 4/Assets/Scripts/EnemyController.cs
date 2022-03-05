using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float force = 60.0f;
    public bool isToughEnemy = false;
    public float toughEnemyForce = 850.0f;
    private GameObject playerObj;
    private Rigidbody rigidbodyComponent;
    // Start is called before the first frame update
    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        rigidbodyComponent = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (transform.position.y < -5.0f)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        Vector3 directionToPlayer = (playerObj.transform.position - transform.position).normalized;
        rigidbodyComponent.AddForce(directionToPlayer * force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isToughEnemy && collision.gameObject.CompareTag("Player"))
        {
            Vector3 enemyDir = (collision.gameObject.transform.position - transform.position).normalized;
            collision.gameObject.GetComponent<Rigidbody>().AddForce(enemyDir * toughEnemyForce, ForceMode.Impulse);
        }
    }
}
