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

    private bool isTransitioning = false;

    private readonly float delay = 1.5f;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1f;
    }

    private void Update()
    {
        if(Debug.isDebugBuild) DebugKeys();
    }

    private void DebugKeys()
    {
        if (Input.GetKey(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKey(KeyCode.C))
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();

            Debug.Log(colliders.Length);

            foreach (Collider collider in colliders)
            {
                collider.enabled = !collider.enabled;
            }
        }
    }

    void FixedUpdate()
    {
        if (!isTransitioning)
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

            rigidbody.AddRelativeForce(Vector3.up * verticalSpeed * Time.deltaTime);

            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(mainEngine);
            }

            mainEngineParticles.Play();
        }
        else 
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                mainEngineParticles.Stop();
            }
        }
    }

    private void Rotate()
    {
        float rotationSpeed = rcsThrust * Time.deltaTime;

        isRotating = false;

        if (Input.GetKey(KeyCode.A))
        {
            rigidbody.freezeRotation = true;
            transform.Rotate(Vector3.forward * rotationSpeed);
            isRotating = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rigidbody.freezeRotation = true;
            transform.Rotate(Vector3.back * rotationSpeed);
            isRotating = true;
        }

        rigidbody.freezeRotation = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning) return;

        switch (collision.gameObject.tag)
        {
            case "Friendly":

                break;
            case "Finish":
                isTransitioning = true;
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
                isTransitioning = true;
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
