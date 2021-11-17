using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
///     Controller that handles the character controls and camera controls of the first person player.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    public FirstPersonControllerInput firstPersonControllerInput;
    private CharacterController _characterController;
    private Camera _camera;
    public float walkingSpeed = 5.0f;
    public float maxViewAngle, minViewAngle;
    
    private void Awake() {
        _characterController = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        firstPersonControllerInput.Move
            .Where(v => v != Vector2.zero)
            .Subscribe(input =>
            {
                var horizontalVelocity = input * walkingSpeed;
                var characterVelocity = gameObject.transform.TransformVector(new Vector3(horizontalVelocity.x, 
                                                                                            - 9.81f,
                                                                                            horizontalVelocity.y));
                var distance = characterVelocity * Time.deltaTime;
                _characterController.Move(distance);
            }).AddTo(this);

        firstPersonControllerInput.Look
            .Where(v => v != Vector2.zero)
            .Subscribe(input =>
            {
                var horizontalLook = input.x * Vector3.up * Time.deltaTime;
                transform.localRotation *= Quaternion.Euler(horizontalLook);

                var verticalLook = input.y * Vector3.left * Time.deltaTime;
                var newQuaternion = _camera.transform.localRotation * Quaternion.Euler(verticalLook);
                _camera.transform.localRotation = //RotationTools.ClampRotationAroundXAxis(
                                                  newQuaternion;
                                                  //, -maxViewAngle, -maxViewAngle);
            }).AddTo(this);
    }

}
