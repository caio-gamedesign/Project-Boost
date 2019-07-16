using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscilator : MonoBehaviour
{
    [SerializeField] Vector3 movement = new Vector3(10f, 10f, 10f);
    [SerializeField] Vector3 rotation = new Vector3(10f, 10f, 10f);

    [SerializeField] float period = 2f;

    [Range(0, 1)] [SerializeField] float movementFactor;

    Vector3 startingPosition;
    Vector3 startingRotation;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {

        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2;
        float rawSineWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSineWave / 2f + 0.5f;

        Vector3 offset = movementFactor * movement;
        transform.position = startingPosition + offset;

        Vector3 currentRotation = movementFactor * rotation;
        transform.rotation = Quaternion.Euler(startingRotation + currentRotation);
    }
}
