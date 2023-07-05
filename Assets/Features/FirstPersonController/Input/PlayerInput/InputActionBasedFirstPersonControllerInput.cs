using System;
using System.Runtime.CompilerServices;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class InputActionBasedFirstPersonControllerInput : FirstPersonControllerInput
{
    private FirstPersonInputAction _controls;
    private IObservable<Vector2> _move;
    private IObservable<Vector2> _look;
    private ReadOnlyReactiveProperty<bool> _run;
    private Subject<Unit> _jump = new Subject<Unit>();
    Vector2 _smoothLookValue;
    public float lookSmoothingFactor;

    private void Awake()
    {
        _controls = new FirstPersonInputAction();
        _move = this.UpdateAsObservable()
            .Select(_=>
            {
                return _controls.Character.Move.ReadValue<Vector2>();
            });

        _look = this.UpdateAsObservable()
            .Select(_=>
            {
                Vector2 rawLookValue = _controls.Character.Look.ReadValue<Vector2>();
                
                _smoothLookValue.x = Mathf.Lerp(_smoothLookValue.x, rawLookValue.x, lookSmoothingFactor * Time.deltaTime);
                _smoothLookValue.y = Mathf.Lerp(_smoothLookValue.y, rawLookValue.y, lookSmoothingFactor * Time.deltaTime);
                return _smoothLookValue;
            });
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _run = this.UpdateAsObservable()
            .Select(_ => _controls.Character.Run.ReadValueAsObject() != null)
            .ToReadOnlyReactiveProperty();

        _controls.Character.Jump.performed += context => { _jump.OnNext(Unit.Default); };

    }

    private void OnEnable()
    {
        _controls?.Enable();
    }

    private void OnDisable()
    {
        _controls?.Disable();
    }
    
    public override IObservable<Vector2> Move => _move;

    public override IObservable<Unit> Jump => _jump;

    public override ReadOnlyReactiveProperty<bool> Run => _run;

    public override IObservable<Vector2> Look => _look;
}