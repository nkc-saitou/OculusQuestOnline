using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotator : MonoBehaviour
{

	public float RotateSpeed;
	public float SinSpeed;
	public Vector3 RotateVec;

    // Update is called once per frame
    void Update()
    {
		transform.rotation *= Quaternion.AngleAxis(RotateSpeed + Mathf.Sin(Time.time * SinSpeed), RotateVec);
	}
}
