using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Nakajima.Movement;
using Nakajima.Weapon;
using Saitou.Network;
using Matsumoto.Weapon;


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
                myHand[0].deleteWeapon += CheckDelete;
                myHand[1].oppositeWeapon += SetOpposite;
                myHand[1].deleteWeapon += CheckDelete;
                
                myHead = myHand[0].GetMyProvider.GetMyObj("Head");
                myBody = myHand[0].GetMyProvider.GetMyObj("Body");
                offset = Mathf.Abs(myBody.transform.position.y - myHead.transform.position.y);
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
            TrackingMove();

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
        /// トラッキングするObjectの移動
        /// </summary>
        private void TrackingMove()
        {
            if (myBody == null) return;

            // トラッキングした場合の座標
            Vector3 trackingPos = myHead.transform.position;
            
            // 振動数を計算
            frequency = 1.0f / moveTime;
            float sin = Mathf.Cos(2 * Mathf.PI * frequency * Time.time) * moveValue;
            Vector3 trackingPos_B = new Vector3(trackingPos.x, myMovement.GetMyCamera.transform.localPosition.y + 0.5f * sin, trackingPos.z);

            myBody.transform.position = trackingPos_B;
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

            // X/Aボタンを離したら武器の削除
            if (OVRInput.GetUp(OVRInput.RawButton.X))
                myHand[1].weaponCreate.DeleteWeapon();
            if (OVRInput.GetUp(OVRInput.RawButton.A))
                myHand[0].weaponCreate.DeleteWeapon();
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
                myHand[0].DeleteWeapon(false);
            else if (_hand.myTouch == OVRInput.RawButton.RHandTrigger)
                myHand[1].DeleteWeapon(false);
        }

        /// <summary>
        /// 当たり判定
        /// </summary>
        /// <param name="col"></param>
        public void OnTriggerEnter(Collider col)
        {
            var module = col.gameObject.GetComponent<ModuleObject>();

            if (module == null) return;
            
            GetScore += module.GetPower();

            Debug.Log("相手のスコア : " + GetScore);
        }
    }
}
