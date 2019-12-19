using System;
using UnityEngine;

public struct TransformStamp {
	public DateTime TimeStamp;
	public Vector3 Position;
	public Quaternion Rotation;

	public TransformStamp(DateTime timeStamp, Vector3 position, Quaternion rotation) {
		TimeStamp = timeStamp;
		Position = position;
		Rotation = rotation;
	}
}
