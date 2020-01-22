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

public struct TransformVectorStamp
{
    public DateTime TimeStamp;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Vector;

    public TransformVectorStamp(DateTime timeStamp, Vector3 position, Quaternion rotation, Vector3 vector)
    {
        TimeStamp = timeStamp;
        Position = position;
        Rotation = rotation;
        Vector = vector;
    }
}