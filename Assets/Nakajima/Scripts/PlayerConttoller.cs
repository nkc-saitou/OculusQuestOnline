using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Nakajima.Movement;
using Nakajima.Weapon;

/// <summary>
/// プレイヤークラス
/// </summary>
namespace Nakajima.Player
{
    public class PlayerConttoller : MonoBehaviour
    {
        // 自身のMovement;
        private MovementComponetBase myMovement;

        // 自身の手(0 右手、1 左手)
        [SerializeField, Header("<自分の手(0 右手 1 左手)>")]
        private PlayerHand[] myHand;

        // 武器生成
        private WeaponCreate weaponCreate;
        // 自身のRigidbody
        private Rigidbody myRig;
        // 入力ベクター
        private Vector3 inputVec;
        
        void Start()
        {
            myRig = GetComponent<Rigidbody>();
            myMovement = GetComponent<MovementComponetBase>();
            weaponCreate = GetComponent<WeaponCreate>();
        }
        
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
                weaponCreate.Create();
            }
        }

        /// <summary>
        /// 移動
        /// </summary>
        private void Move()
        {
            // 移動方法ステートで処理わけ
            switch (myMovement.movementState)
            {
                // スティック移動
                case MovementComponetBase.MovementState.MOVE_STICK:
                    inputVec = new Vector3(OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).x, 0.0f, OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).y);
                    break;
                // 傾き移動
                case MovementComponetBase.MovementState.MOVE_INCLINATION:
                    Quaternion Angles = InputTracking.GetLocalRotation(XRNode.Head);
                    inputVec = myMovement.GetMoveDirObj().transform.position - transform.position;
                    break;
            }

            // 移動力の計算
            myMovement.Move(inputVec);
            // 移動
            myRig.velocity = myMovement.Velocity;
        }
    }
}
