using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Nakajima.Movement;
using Nakajima.Weapon;
using Saitou.Network;


/// <summary>
/// プレイヤークラス
/// </summary>
namespace Nakajima.Player
{
    public class PlayerController : PlayerMaster
    {
        // 自身の手(0 右手、1 左手)
        [SerializeField, Header("<自分の手(0 右手 1 左手)>")]
        protected PlayerHand[] myHand;

        // 自身のMovement;
        private MovementComponetBase myMovement;

        // オンライン用のプレイヤーの生成
        private TestPlayerCreate testPlayerCreate;

        /// <summary>
        /// 初回処理
        /// </summary>
        public override void Start()
        {
            myRig = GetComponent<Rigidbody>();
            myMovement = GetComponent<MovementComponetBase>();
            weaponCreate = GetComponent<WeaponCreate>();
            testPlayerCreate = FindObjectOfType<TestPlayerCreate>();

            //// イベントをバインド
            //myHand[0].oppositeWeapon += SetOpposite;
            //myHand[1].oppositeWeapon += SetOpposite;

            testPlayerCreate.OnPlayerCreate += (myHandArray) =>
            {
                myHand = myHandArray;

                // イベントをバインド
                myHand[0].oppositeWeapon += SetOpposite;
                myHand[1].oppositeWeapon += SetOpposite;

                Debug.Log("aaaa");
            };
        }
        
        /// <summary>
        /// 更新処理
        /// </summary>
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

            // 移動力の計算
            myMovement.Move(inputVec);
            // 移動
            myRig.velocity = myMovement.Velocity;
        }

        /// <summary>
        /// 武器アクション
        /// </summary>
        public override void Actoin()
        {
            // Xボタンで武器生成
            if (OVRInput.GetDown(OVRInput.RawButton.X))
            {
                weaponCreate.ActiveHand = myHand[1];
                weaponCreate.Create();
            }
            if (OVRInput.GetUp(OVRInput.RawButton.X))
            {
                weaponCreate.DeleteWeapon();
            }

            // Yボタンで所持中の武器の削除
            if (OVRInput.GetDown(OVRInput.RawButton.Y))
            {
                foreach (PlayerHand _hand in myHand)
                {
                    if (_hand.DeleteWeapon())
                    {
                        weaponCreate.CanCreate = true;
                    }
                }
            }
        }

        /// <summary>
        /// 逆の手の設定
        /// </summary>
        /// <param name="_hand">現在所持中の手</param>
        /// <param name="_weapon">武器オブジェクト</param>
        public override void SetOpposite(HandMaster _hand, GameObject _weapon)
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
        }
    }
}
