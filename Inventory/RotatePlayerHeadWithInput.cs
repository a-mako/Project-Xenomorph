using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotatePlayerHeadWithInput : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Quaternion initRotation;

    private void Awake() {
        initRotation = transform.rotation;
    }

    private void OnDisable() {
        transform.rotation = initRotation;
    }
    private void Update()
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection;

        if (Cursor.lockState.Equals(CursorLockMode.Locked) && gameObject.activeSelf && EventSystem.current.currentSelectedGameObject != null) {
                targetDirection = EventSystem.current.currentSelectedGameObject.transform.position - transform.position;
                speed = .5f;
        }
        else {
            targetDirection = Input.mousePosition - transform.position;
            speed = 5f;
        }

        // The max radians is equal to speed times frame time.
        float maxRadians = speed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, maxRadians, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
