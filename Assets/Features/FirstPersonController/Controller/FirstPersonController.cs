using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;


/// <summary>
///     Controller that handles the character controls and camera controls of the first person player.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FirstPersonControllerInput firstPersonControllerInput;
    private CharacterController _characterController;
    private Camera _camera;
    
    private void Awake() {
        _characterController = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
        firstPersonControllerInput = gameObject.AddComponent<InputActionBasedFirstPersonControllerInput>();
    }

    private void Start()
    {
        
    }

}
