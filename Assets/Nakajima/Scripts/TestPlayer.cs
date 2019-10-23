using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Nakajima.Movement;

/// <summary>
/// テスト用のプレイヤークラス(消しても問題ない)
/// </summary>
public class TestPlayer : MonoBehaviour
{
    // 自身のMovement;
    private MovementComponetBase myMovement;

    private Rigidbody myRig;

    private Vector3 inputVec;

    // Start is called before the first frame update
    void Start()
    {
        myRig = GetComponent<Rigidbody>();
        myMovement = GetComponent<MovementComponetBase>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        switch (myMovement.movementState)
        {
            case MovementComponetBase.MovementState.MOVE_STICK:
                inputVec = new Vector3(OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).x, 0.0f, OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).y);
                break;
            case MovementComponetBase.MovementState.MOVE_INCLINATION:
                Quaternion Angles = InputTracking.GetLocalRotation(XRNode.Head);
                inputVec =  myMovement.GetMoveDirObj().transform.position - transform.position;
                break;
        }
        
        myMovement.Move(inputVec);

        myRig.velocity = myMovement.Velocity;
    }
}
