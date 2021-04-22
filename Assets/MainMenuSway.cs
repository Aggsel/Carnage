using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSway : MonoBehaviour
{
    [Header("Rotation sway: ")]
    [SerializeField] private float rotationAmount = 0.5f;
    [SerializeField] private float rotationSpeed = 2f;

    private Vector3 desiredRot = Vector3.zero;
    private Vector3 startRot = Vector3.zero;

    private float mouseX = 0f;
    private float mouseY = 0f;

    private void Start()
    {
        startRot = transform.localEulerAngles;
        desiredRot = startRot;
    }

    private void LateUpdate()
    {
        CameraSway();
    }

    //weaponsway from camera rotation (changes rotation)
    private void CameraSway()
    {
        mouseY = Input.GetAxis("Mouse X")  * (rotationAmount * 0.1f);
        mouseX = Input.GetAxis("Mouse Y") * (rotationAmount * 0.1f);

        desiredRot = new Vector3(-mouseX, mouseY, 0);
        Quaternion dest = Quaternion.Euler(startRot + desiredRot);

        float step = rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, dest, step);
    }
}