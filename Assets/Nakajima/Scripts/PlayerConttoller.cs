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

            // イベントをバインド
            myHand[0].grabWeapon += SetDominant;
            myHand[1].grabWeapon += SetDominant;
            myHand[1].oppositeWeapon += SetOpposite;
            myHand[1].oppositeWeapon += SetOpposite;
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
            if (OVRInput.GetDown(OVRInput.RawButton.Y))
            {
                foreach(PlayerHand _hand in myHand)
                {
                    if (_hand.DeleteWeapon()){
                        weaponCreate.CanCreate = true;
                    }
                }
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

        /// <summary>
        /// 利き手の設定
        /// </summary>
        /// <param name="_hand">利き手</param>
        /// <param name="_weapon">武器</param>
        private void SetDominant(PlayerHand _hand, GameObject _weapon)
        {
            
        }

        /// <summary>
        /// 反対の手の設定
        /// </summary>
        /// <param name="_hand">利き手</param>
        /// <param name="_weapon">武器</param>
        private void SetOpposite(PlayerHand _hand, GameObject _weapon)
        {
            if (_weapon == null) return;

            if (_hand.myTouch == OVRInput.RawButton.LHandTrigger)
            {
                myHand[0].SetWeapon(_weapon);
            }
            else if (_hand.myTouch == OVRInput.RawButton.RHandTrigger)
            {
                myHand[1].SetWeapon(_weapon);
            }

            weaponCreate.CanCreate = false;
        }
    }
}
