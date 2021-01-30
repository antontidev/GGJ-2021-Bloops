using MyBox;
using SukharevShared;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Zenject;


public class PlayerController : MonoBehaviour{
    [Inject]
    private IPlayerMover playerMover;

    [Header("Camera movement")]
    [SerializeField]
    private bool cameraRelativeMovement;

    [SerializeField]
    [ConditionalField("cameraRelativeMovement")]
    private Camera cam;

    [Header("Movement")]
    public float movementSpeed = 3f;
    
    private float currentIdle = 0f;

    [Header("Player components")]
    public Animator anim;

    void Update() {
        ControllPlayer();
    }

    void ControllPlayer() {
        var move = playerMover.Move;

        currentIdle += Time.deltaTime;

        float moveHorizontal = move.x;
        float moveVertical = move.y;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        Move(movement);
    }

    public void Move(Vector3 movement) {
        if (movement != Vector3.zero) {
            currentIdle = 0f;

            if (cameraRelativeMovement) {
                movement = CameraRelativeMovement(movement);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.30f);

            transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);
        }

        anim.SetFloat("Speed", movement.magnitude);
        anim.SetFloat("IdleTime", currentIdle);
    }

    private Vector3 CameraRelativeMovement(Vector3 movement) {
        var camTransform = cam.transform;

        var forward = camTransform.forward;
        var right = camTransform.right;
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return forward * movement.z + right * movement.x;
    }
}