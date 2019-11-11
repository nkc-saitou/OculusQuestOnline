using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotator : MonoBehaviour
{

	public float RotateSpeed;
	public Vector3 RotateVec;

    // Update is called once per frame
    void Update()
    {
		transform.rotation *= Quaternion.AngleAxis(RotateSpeed, RotateVec);
	}
}
