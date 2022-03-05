using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject mainCameraObj;
    public float force = 60.0f;
    public float powerUpForce = 1500.0f;
    public float powerUpDuration = 8.0f;
    public float nukeRadius = 5.0f;
    public float nukeForce = 4000.0f;
    public float nukeUpwardsModifier = 1.0f;
    private float nukeDuration = 0.2f;
    private float nukeAnimationTime = -1.0f; // Negative numbers denotes not started and user can move
    public GameObject powerupIndicatorObj;
    public GameObject projectilePrefab;
    private Vector3 powerupIndicatorOffset;
    private Coroutine powerupCoroutine;
    private Rigidbody rigidbodyComponent = null;
    private bool hasPower = false;

    public enum PowerUpEnum
    {
        Power,
        Projectiles,
        Nuke
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
        powerupIndicatorOffset = transform.position - powerupIndicatorObj.transform.position;
    }

    void Update()
    {
        if (transform.position.y < -5.0f)
        {
            transform.position = Vector3.zero;
            rigidbodyComponent.velocity = Vector3.zero;
            rigidbodyComponent.angularVelocity = Vector3.zero;
            GameObject.Find("SpawnManager").GetComponent<SpawnManager>().RestartGame();
        }
    }

    void FixedUpdate()
    {
        if (nukeAnimationTime < 0)
        {
            rigidbodyComponent.AddForce(mainCameraObj.transform.forward * force * Input.GetAxis("Vertical"));
        }
        powerupIndicatorObj.transform.position = transform.position - powerupIndicatorOffset;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            PowerUpEnum powerUpType = other.gameObject.GetComponent<PowerUpController>().powerUpType;
            if (powerUpType == PowerUpEnum.Power)
            {
                hasPower = true;
                powerupIndicatorObj.SetActive(true);
                if (powerupCoroutine != null)
                {
                    StopCoroutine(powerupCoroutine);
                }
                powerupCoroutine = StartCoroutine(WaitAndRemovePowerUp());
            }
            else if (powerUpType == PowerUpEnum.Projectiles)
            {
                GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
                for (int i = 0; i < enemyObjs.Length; i += 1)
                {
                    GameObject enemyObj = enemyObjs[i];
                    GameObject projectileObj = Instantiate(projectilePrefab, transform.position, projectilePrefab.transform.rotation);
                    projectileObj.GetComponent<ProjectileController>().FireTowardDirection(enemyObj.transform);
                }
            }
            else if (powerUpType == PowerUpEnum.Nuke)
            {
                StartCoroutine(TriggerNuke());
            }

            Destroy(other.gameObject);
        }
    }

    IEnumerator TriggerNuke()
    {
        Vector3 originalPos = transform.position;
        Vector3 highestPos = originalPos + (Vector3.up * 10.0f);
        float halfNukeDuration = nukeDuration / 2.0f;
        nukeAnimationTime = 0.0f;
        rigidbodyComponent.constraints = RigidbodyConstraints.FreezeAll;
        yield return null;
        while (nukeAnimationTime < nukeDuration)
        {
            nukeAnimationTime += Time.deltaTime;
            if (nukeAnimationTime < halfNukeDuration)
            {
                transform.position = Vector3.Lerp(originalPos, highestPos, nukeAnimationTime / halfNukeDuration);
            }
            else
            {
                transform.position = Vector3.Lerp(highestPos, originalPos, (nukeAnimationTime - halfNukeDuration) / halfNukeDuration);
            }
            yield return null;
        }
        transform.position = originalPos;
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, nukeRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.gameObject == gameObject)
            {
                continue;
            }
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(nukeForce, explosionPos, nukeRadius, nukeUpwardsModifier, ForceMode.Impulse);
        }
        nukeAnimationTime = -1.0f;
        rigidbodyComponent.constraints = RigidbodyConstraints.None;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasPower && collision.gameObject.CompareTag("Enemy"))
        {
            Vector3 enemyDir = (collision.gameObject.transform.position - transform.position).normalized;
            collision.gameObject.GetComponent<Rigidbody>().AddForce(enemyDir * powerUpForce, ForceMode.Impulse);
        }
    }

    IEnumerator WaitAndRemovePowerUp()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPower = false;
        powerupIndicatorObj.SetActive(false);
        powerupCoroutine = null;
    }
}
