using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float startAnimationLength = 2.5f;
    public float jumpPower = 100.0f;
    public float gravityMultiplier = 2.0f;
    private bool isOnGround = true;
    public bool isGameOver = false;
    public ParticleSystem explosionParticle;
    public ParticleSystem dirtParticle;
    public AudioClip jumpSound;
    public AudioClip crashSound;
    private Animator animatorComponent;
    private AudioSource audioSourceComponent;
    private float animationTime = 0.0f;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity *= gravityMultiplier;
        animatorComponent = GetComponent<Animator>();
        audioSourceComponent = GetComponent<AudioSource>();
        dirtParticle.Play();
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            return;
        }
        else if (animationTime < startAnimationLength)
        {
            animationTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos - new Vector3(4, 0, 0), startPos, animationTime / startAnimationLength);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            Rigidbody rbComponent = GetComponent<Rigidbody>();
            rbComponent.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isOnGround = false;
            animatorComponent.SetTrigger("Jump_trig");
            dirtParticle.Stop();
            audioSourceComponent.PlayOneShot(jumpSound, 0.5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            animatorComponent.SetBool("Jump_b", false);
            dirtParticle.Play();
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            isGameOver = true;
            animatorComponent.SetInteger("DeathType_int", 1);
            animatorComponent.SetBool("Death_b", true);
            dirtParticle.Stop();
            explosionParticle.Play();
            audioSourceComponent.PlayOneShot(crashSound, 0.5f);
        }
    }
}
