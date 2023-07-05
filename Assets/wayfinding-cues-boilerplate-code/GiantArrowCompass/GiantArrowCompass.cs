using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantArrowCompass : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void Update()
    {
        gameObject.transform.LookAt(target.transform);
    }
}