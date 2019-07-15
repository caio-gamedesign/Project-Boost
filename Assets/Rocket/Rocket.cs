using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    new Rigidbody rigidbody;
    AudioSource audioSource;
    [SerializeField] private float mainThrust = 100f;
    [SerializeField] private float rcsThrust = 100f;
    private bool isThrusting = false;
    private bool isRotating = false;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip win;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem winParticles;

    enum State { Alive, Dead, Transcending };
    State state = State.Alive;
    private float delay = 1.5f;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1f;
    }

    void FixedUpdate()
    {
        if (state == State.Alive)
        {
            Rotate();
            Thrust();
        }
    }

    private void Thrust()
    {
        isThrusting = (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) || isRotating;
        if (isThrusting)
        {
            float verticalSpeed = mainThrust;

            if (isRotating)
            {
                verticalSpeed *= .65f;
            }

            rigidbody.AddRelativeForce(Vector3.up * verticalSpeed);

            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(mainEngine);
            }

            mainEngineParticles.Play();
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void Rotate()
    {
        rigidbody.freezeRotation = true;

        float rotationSpeed = rcsThrust * Time.deltaTime;

        isRotating = false;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
            isRotating = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationSpeed);
            isRotating = true;
        }

        rigidbody.freezeRotation = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) return;
        switch (collision.gameObject.tag)
        {
            case "Friendly":

                break;
            case "Finish":
                state = State.Transcending;
                audioSource.volume = .3f;
                audioSource.Stop();
                audioSource.PlayOneShot(win);
                winParticles.transform.position = new Vector3(0,15f,0);
                winParticles.Play();
                mainEngineParticles.Stop();
                Invoke("LoadNextLevel", delay);
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                break;
            default:
                state = State.Dead;
                audioSource.volume = .15f;
                audioSource.Stop();
                audioSource.PlayOneShot(death);
                deathParticles.Play();
                mainEngineParticles.Stop();
                Invoke("ReloadLevel", delay);
                break;
        }
    }

    private void ReloadLevel()
    {
        int level = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(level);
    }

    private void LoadNextLevel()
    {
        int level = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(level);
    }
}
