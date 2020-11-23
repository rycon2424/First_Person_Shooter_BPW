using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    public bool taken;
    [SerializeField] private float remainingDistance;

    public void UpdateDistance(float d)
    {
        remainingDistance = d;
    }
}
