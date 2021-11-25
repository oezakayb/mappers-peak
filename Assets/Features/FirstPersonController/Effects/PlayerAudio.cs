using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Operators;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAudio : MonoBehaviour
{
    public GameObject characterSignalsInterfaceTarget;
    private ICharacterSignals _characterSignals;

    private AudioSource _audioSource;

    public AudioClip jump;
    public AudioClip landed;
    public AudioClip[] steps;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _characterSignals = characterSignalsInterfaceTarget.GetComponent<ICharacterSignals>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _characterSignals.Jumped
            .Subscribe(_ =>
            {
                _audioSource.PlayOneShot(jump);
            }).AddTo(this);

        _characterSignals.Landed
            .Subscribe(_ =>
            {
                _audioSource.PlayOneShot(landed);
            }).AddTo(this);

        var selectRandom = Observable.Create<int>(observer =>
            {
                var sub = _characterSignals.Stepped.Subscribe(_=>
                {
                    var i = Random.Range(0, 9);
                    observer.OnNext(i);
                });
                
                return Disposable.Create((() => sub.Dispose()));
            });

        selectRandom.Subscribe(i =>
        {
            _audioSource.PlayOneShot(steps[i]);
        }).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
