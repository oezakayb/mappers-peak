using System;
using System.Runtime.CompilerServices;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class InputActionBasedFirstPersonControllerInput : FirstPersonControllerInput
{
    private FirstPersonInputAction controls;
    private IObservable<Vector2> _move;
    private IObservable<Vector2> _look;
    Vector2 _smoothLookValue;
    public float lookSmoothingFactor;

    private void Awake()
    {
        controls = new FirstPersonInputAction();
        _move = this.UpdateAsObservable()
            .Select(_=>
            {
                return controls.Character.Move.ReadValue<Vector2>();
            });

        _look = this.UpdateAsObservable()
            .Select(_=>
            {
                Vector2 rawLookValue = controls.Character.Look.ReadValue<Vector2>();
                
                _smoothLookValue.x = Mathf.Lerp(_smoothLookValue.x, rawLookValue.x, lookSmoothingFactor * Time.deltaTime);
                _smoothLookValue.y = Mathf.Lerp(_smoothLookValue.y, rawLookValue.y, lookSmoothingFactor * Time.deltaTime);
                return _smoothLookValue;
            });
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        controls?.Enable();
    }

    private void OnDisable()
    {
        controls?.Disable();
    }
    
    public override IObservable<Vector2> Move => _move;

    public override IObservable<Unit> Jump => null;

    public override ReadOnlyReactiveProperty<bool> Run => null;

    public override IObservable<Vector2> Look => _look;
}