using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UIElements;
using Math = UnityEngine.ProBuilder.Math;

public class CompassBarElement : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform target;
    [SerializeField] private bool useFixDirection = false;
    [SerializeField] private Vector3 fixDirection;

    private CompassBar bar;
    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        bar = GetComponentInParent<CompassBar>();
    }

    private void Update()
    {
        var positionTarget = target.transform.position;
        var positionPlayer = player.transform.position;
        var direction = useFixDirection? fixDirection : target.position - player.position;

        var angle = Vector2.SignedAngle(new Vector2(direction.x, direction.z), new Vector2(player.forward.x, player.forward.z));
        float mappedAngle = -1 + (angle - -180) * ( 1 - -1 ) / ( 180 - -180 );;
        float xPosition = mappedAngle * (360 / bar.BarRange) * (bar.BarRectTransform.rect.width / 2);

        _rectTransform.anchoredPosition = new Vector2(xPosition, 0);

    }
}