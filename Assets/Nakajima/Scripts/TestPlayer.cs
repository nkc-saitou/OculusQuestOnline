using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakajima.Movement;

/// <summary>
/// テスト用のプレイヤークラス(消しても問題ない)
/// </summary>
public class TestPlayer : MonoBehaviour
{
    // 自身のMovement;
    MovementComponetBase myMovement;

    Rigidbody myRig;

    Vector3 inputVec;

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
        inputVec.x = Input.GetAxis("Horizontal");
        inputVec.z = Input.GetAxis("Vertical");
        
        myMovement.AddInputVector(inputVec);
        
        myRig.velocity = myMovement.Velocity;
    }
}
