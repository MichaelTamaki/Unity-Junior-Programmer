using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalieController : MonoBehaviour
{
    [SerializeField] private Animator goalieAnimator;
    private Rigidbody goalieRb;
    private const float maxJumpForce = 225f;
    private const float maxSideForce = 115f;
    private Vector3 spawnPos;
    private Quaternion spawnRotation;

    // Start is called before the first frame update
    void Start()
    {
        //goalieAnimator = GetComponent<Animator>();
        goalieRb = GetComponent<Rigidbody>();
        goalieAnimator.SetInteger("Animation_int", 2);
        spawnPos = gameObject.transform.position;
        spawnRotation = gameObject.transform.rotation;
    }

    public void TriggerRespawn()
    {
        // Reset animator state
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        goalieAnimator.SetBool("Death_b", false);
        goalieAnimator.SetInteger("Animation_int", 2);
        gameObject.transform.position = spawnPos;
        gameObject.transform.rotation = spawnRotation;
        goalieRb.velocity = Vector3.zero;
        goalieRb.angularVelocity = Vector3.zero;
    }

    public void TriggerRandomJump()
    {
        goalieAnimator.SetTrigger("Jump_trig");
        float jumpForce = maxJumpForce * Random.Range(0f, 1f);
        float sideForce = maxSideForce * Random.Range(-1f, 1f);
        goalieRb.AddForce(new Vector3(sideForce, jumpForce, 0f), ForceMode.Impulse);
    }

    public void TriggerDeath()
    {
        goalieAnimator.SetBool("Death_b", true);
        goalieAnimator.SetInteger("DeathType_int", 2);
    }
}
