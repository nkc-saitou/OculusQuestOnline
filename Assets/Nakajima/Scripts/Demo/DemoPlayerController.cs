﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Nakajima.Movement;
using Nakajima.Weapon;
using Matsumoto.Weapon;

namespace Nakajima.Player
{
    public class DemoPlayerController : PlayerMaster
    {
        // 自身の手(0 右手、1 左手)
        [SerializeField, Header("<自分の手(0 右手 1 左手)>")]
        protected DemoHand[] myHand;

        public override void Start()
        {
            myRig = GetComponent<Rigidbody>();
            myMovement = GetComponent<MovementComponetBase>();
            myDamageEffect = FindObjectOfType<DamageEffect>();
            weaponMgr = FindObjectOfType<WeaponManager>();

            // イベントをバインド
            myHand[0].oppositeWeapon += SetOpposite;
            myHand[0].deleteWeapon += CheckDelete;
            myHand[1].oppositeWeapon += SetOpposite;
            myHand[1].deleteWeapon += CheckDelete;
        }

        public override void Update()
        {
            Move();

            Actoin();
        }

        /// <summary>
        /// 移動
        /// </summary>
        public override void Move()
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

            TrackingMove(inputVec);
            // 移動力の計算
            myMovement.Move(inputVec);
            // 移動
            myRig.velocity = myMovement.Velocity;
        }

        /// <summary>
        /// トラッキングするObjectの移動
        /// </summary>
        private void TrackingMove(Vector3 _inputVec)
        {
            if (rootObj == null) return;
            // 移動方向
            Vector3 moveVec = _inputVec * 13.0f;

            rootObj.transform.rotation = Quaternion.Euler(moveVec.z, 0.0f, -moveVec.x);
        }

        /// <summary>
        /// 武器アクション
        /// </summary>
        public override void Actoin()
        {
            // X/Aボタンで武器生成
            if (OVRInput.GetDown(OVRInput.RawButton.X) && myHand[1].HasWeapon == false)
                myHand[1].Create();
            if (OVRInput.GetDown(OVRInput.RawButton.A) && myHand[0].HasWeapon == false)
                myHand[0].Create();

            // X/Aボタンを離したら武器の削除
            if (OVRInput.GetUp(OVRInput.RawButton.X))
                myHand[1].weaponCreate.DeleteWeapon();
            if (OVRInput.GetUp(OVRInput.RawButton.A))
                myHand[0].weaponCreate.DeleteWeapon();

            // 中指トリガーで武器を掴む
            if (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger) && myHand[1].HasWeapon == false)
                myHand[1].GrabWeapon();
            if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger) && myHand[0].HasWeapon == false)
                myHand[0].GrabWeapon();

            // 人差し指トリガーで武器使用
            if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger) && myHand[1].HasWeapon)
                myHand[1].WeaponAction(true, false);
            else if (OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger) && myHand[1].HasWeapon)
                myHand[1].WeaponAction(true, true);
            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger) && myHand[0].HasWeapon)
                myHand[0].WeaponAction(true, false);
            else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger) && myHand[0].HasWeapon)
                myHand[0].WeaponAction(true, true);

            // Y/Bボタンで所持中の武器の削除
            if (OVRInput.GetDown(OVRInput.RawButton.Y) && myHand[1].isBoth == false)
                myHand[1].DeleteWeapon(myHand[1].CheckDelete());
            if (OVRInput.GetDown(OVRInput.RawButton.B) && myHand[0].isBoth == false)
                myHand[0].DeleteWeapon(myHand[0].CheckDelete());
        }

        /// <summary>
        /// 逆の手の設定
        /// </summary>
        /// <param name="_hand">現在所持中の手</param>
        /// <param name="_weapon">武器オブジェクト</param>
        public override void SetOpposite(HandMaster _hand, GameObject _weapon)
        {
            // nullチェック
            if (_weapon == null) return;

            // 逆の手に武器を装備する
            if (_hand.myTouch == OVRInput.RawButton.LHandTrigger)
                myHand[0].SetWeapon(_weapon);
            else if (_hand.myTouch == OVRInput.RawButton.RHandTrigger)
                myHand[1].SetWeapon(_weapon);
        }

        /// <summary>
        /// 逆の手を削除するかチェック
        /// </summary>
        /// <param name="_hand">現在所持中の手</param>
        public override void CheckDelete(HandMaster _hand)
        {
            // 逆の手の武器も削除する
            if (_hand.myTouch == OVRInput.RawButton.LHandTrigger)
                myHand[0].DeleteWeapon();
            else if (_hand.myTouch == OVRInput.RawButton.RHandTrigger)
                myHand[1].DeleteWeapon();
        }
    }
}

