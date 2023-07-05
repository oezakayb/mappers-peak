using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
///     Controller that handles the character controls and camera controls of the first person player.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour, ICharacterSignals
{
    [Header("References")]
    public FirstPersonControllerInput firstPersonControllerInput;
    private CharacterController _characterController;
    private Camera _camera;
    public float walkingSpeed = 5.0f;
    public float runningSpeed = 10.0f;
    public float jumpSpeed = 10.0f;
    public float maxViewAngle, minViewAngle;
    public float stickToGroundForceMagnitude = 5.0f;
    
    private Subject<Vector3> _moved;
    public IObservable<Vector3> Moved => _moved;

    private ReactiveProperty<bool> _isRunning;
    public ReactiveProperty<bool> IsRunning => _isRunning;
    
    public float _strideLength = 2.5f;
    public float StrideLength => _strideLength;

    private Subject<Unit> _landed;
    public IObservable<Unit> Landed => _landed;

    private Subject<Unit> _jumped;
    public IObservable<Unit> Jumped => _jumped;

    private Subject<Unit> _stepped;
    public IObservable<Unit> Stepped => _stepped;
    
    public struct MoveInputData
    {
        public readonly Vector2 move;
        public readonly bool jump;

        public MoveInputData(Vector2 move, bool jump)
        {
            this.move = move;
            this.jump = jump;
        }
    }
    
    private void Awake() {
        _characterController = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
        
        _isRunning = new ReactiveProperty<bool>(false);
        _moved = new Subject<Vector3>().AddTo(this);
        _landed = new Subject<Unit>();
        _jumped = new Subject<Unit>();
        _stepped = new Subject<Unit>();
        
        var stepDistance = 0f;
        Moved.Subscribe(w => {
            stepDistance += w.magnitude;
            if (stepDistance > _strideLength)
            {
                _stepped.OnNext(Unit.Default);
            }
            stepDistance %= _strideLength;
        }).AddTo(this);
        
    }

    private void Start()
    {
        _characterController.Move(-stickToGroundForceMagnitude * transform.up);
        var moveObs = firstPersonControllerInput.Move;
            //.Where(v => v != Vector2.zero);
        
        firstPersonControllerInput.Look
            .Where(v => v != Vector2.zero)
            .Subscribe(input =>
            {
                var horizontalLook = input.x * Vector3.up * Time.deltaTime;
                _characterController.transform.localRotation *= Quaternion.Euler(horizontalLook);

                var verticalLook = input.y * Vector3.left * Time.deltaTime;
                var newQuaternion = _camera.transform.localRotation * Quaternion.Euler(verticalLook);
                _camera.transform.localRotation = RotationTools.ClampRotationAroundXAxis(
                    newQuaternion, -maxViewAngle, -minViewAngle);
            }).AddTo(this);
        
        var jumpLatch = LatchObservables.Latch(this.UpdateAsObservable(), firstPersonControllerInput.Jump, false);

        var zip = moveObs.Zip( jumpLatch, (move, jump) =>
            {
                return new MoveInputData(move, jump);
            }
        );
        zip.Subscribe(input =>
        {
            var wasGrounded = _characterController.isGrounded;
            var verticalVelocity = 0f;
            if (input.jump && wasGrounded)
            {
                verticalVelocity = jumpSpeed;
                _jumped.OnNext(Unit.Default);
            }
            else if (!wasGrounded)
            {
                verticalVelocity = _characterController.velocity.y +
                                   (Physics.gravity.y * Time.deltaTime * 3.0f);
            }
            else
            {
                verticalVelocity = -Mathf.Abs(stickToGroundForceMagnitude);
            }

            var run = firstPersonControllerInput.Run.Value;
            
            var horizontalVelocity = run? input.move * runningSpeed : input.move * walkingSpeed;
            var characterVelocity = gameObject.transform.TransformVector(new Vector3(horizontalVelocity.x, 
                verticalVelocity,
                horizontalVelocity.y));
            var distance = characterVelocity * Time.deltaTime;
            _characterController.Move(distance);

            if (!wasGrounded && _characterController.isGrounded)
            {
                _landed.OnNext(Unit.Default);

            }

                var tempIsRunning = false;
            
            if (wasGrounded && _characterController.isGrounded) {
                _moved.OnNext(_characterController.velocity * Time.deltaTime);
                if (_characterController.velocity.magnitude > 0){

                    tempIsRunning = run;
                }
            }
            _isRunning.Value = tempIsRunning;
            
        }).AddTo(this);

    }
    
}
