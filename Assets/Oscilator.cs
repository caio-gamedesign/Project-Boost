using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscilator : MonoBehaviour
{
    [SerializeField] Vector3 movement;
    [SerializeField] Vector3 rotation;

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
        Vector3 offset = movementFactor * movement;
        transform.position = startingPosition + offset;

        Vector3 currentRotation = movementFactor * rotation;
        transform.rotation = Quaternion.Euler(startingRotation + currentRotation);
    }
}
