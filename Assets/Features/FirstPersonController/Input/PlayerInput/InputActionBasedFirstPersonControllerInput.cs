using System;
using System.Runtime.CompilerServices;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;


public class InputActionBasedFirstPersonControllerInput : FirstPersonControllerInput
{
    [SerializeField] private FirstPersonInputAction fpia;

    private void Awake()
    {
        fpia = new FirstPersonInputAction();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _move = this.UpdateAsObservable()
            .Select(_=> fpia.Character.Move.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        fpia.Enable();
    }

    private void OnDisable()
    {
        fpia.Disable();
    }

    private IObservable<Vector2> _move;

    public override IObservable<Vector2> Move
    {
        get { return _move; }
    }
    public override IObservable<Unit> Jump
    {
        get { return null; }
    }
    public override ReadOnlyReactiveProperty<bool> Run
    {
        get { return null; }
    }
    public override IObservable<Vector2> Look
    {
        get { return null; }
    }
}