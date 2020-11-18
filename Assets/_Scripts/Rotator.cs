using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 rotationDir;

    void Update()
    {
        transform.Rotate(rotationDir);
    }

}
