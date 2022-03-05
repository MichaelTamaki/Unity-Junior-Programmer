using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 20.0f;
    public float force = 1500.0f;

    public void FireTowardDirection(Transform enemyTransform)
    {
        transform.LookAt(enemyTransform.position);
        transform.Rotate(new Vector3(90, 0, 0));
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (true)
        {
            transform.Translate(Vector3.up * Time.deltaTime * speed);
            if (Mathf.Abs(transform.position.x) > 30.0f || Mathf.Abs(transform.position.z) > 30.0f)
            {
                Destroy(gameObject);
            }
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * force, ForceMode.Impulse);
            Destroy(gameObject);
        }
    }
}
