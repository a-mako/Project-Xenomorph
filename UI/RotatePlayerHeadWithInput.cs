using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotatePlayerHeadWithInput : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private void Update()
    {
        if (gameObject.activeSelf) {
            // Determine which direction to rotate towards
            Vector3 targetDirection;
            if (Cursor.lockState.Equals(CursorLockMode.Locked)) {
                targetDirection = EventSystem.current.currentSelectedGameObject.transform.position - transform.position;
                speed = 2f;
            }
            else {
                targetDirection = Input.mousePosition - transform.position;
                speed = 5f;
            }

            // The step size is equal to speed times frame time.
            float singleStep = speed * Time.deltaTime;

            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            // Draw a ray pointing at our target in
            Debug.DrawRay(transform.position, newDirection, Color.red);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }
}
