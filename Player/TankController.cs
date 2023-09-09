using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    public static TankController Instance {get; private set;}
    public bool canMove;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 180f;

    private CharacterController charController;

    private void Awake() {
        charController = GetComponent<CharacterController>();
        Instance = this;
    }

    private void Start() {
        canMove = true;
    }

    private void OnEnable() {
        EventBus.Instance.onGameplayPaused += () => canMove = false;
        EventBus.Instance.onGameplayResumed += () => canMove = true;
    }

    private void OnDisable() {
        EventBus.Instance.onGameplayPaused -= () => canMove = false;
        EventBus.Instance.onGameplayResumed -= () => canMove = true;
    }
    
    private void Update()
    {
        if (!canMove) return;

        if(Input.GetKey(KeyCode.W)) {
            charController.Move(transform.forward * moveSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.S)) {
            charController.Move(-transform.forward * moveSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.A)) {
            transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
        }
        if(Input.GetKey(KeyCode.D)) {
            transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
        }
    }
}