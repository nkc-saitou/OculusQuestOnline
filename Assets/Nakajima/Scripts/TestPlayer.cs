using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Nakajima.Movement;
using Nakajima.Weapon;

/// <summary>
/// テスト用のプレイヤークラス(消しても問題ない)
/// </summary>
public class TestPlayer : MonoBehaviour
{
    // 自身のMovement;
    private MovementComponetBase myMovement;

    // 武器生成
    private WeaponCreate weaponCreate;

    private Rigidbody myRig;

    private Vector3 inputVec;

    // Start is called before the first frame update
    void Start()
    {
        myRig = GetComponent<Rigidbody>();
        myMovement = GetComponent<MovementComponetBase>();
        weaponCreate = GetComponent<WeaponCreate>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        Actoin();
    }

    /// <summary>
    /// アクション
    /// </summary>
    private void Actoin()
    {
        // Xボタンで生成
        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            Debug.Log("押した");

            weaponCreate.Create();
        }
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
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
