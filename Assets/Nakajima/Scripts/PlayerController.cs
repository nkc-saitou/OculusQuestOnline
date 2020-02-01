using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Nakajima.Main;
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

        // プレイヤーID
        [HideInInspector]
        public int ID;

        // オンライン用のプレイヤーの生成
        private TestPlayerCreate testPlayerCreate;

        /// <summary>
        /// 初回処理
        /// </summary>
        public override void Start()
        {
            mainMgr = FindObjectOfType<MainManager>();
            myRig = GetComponent<Rigidbody>();
            myMovement = GetComponent<MovementComponetBase>();
            myDamageEffect = FindObjectOfType<DamageEffect>();
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

                // IDの設定
                ID = myHand[0].GetMyProvider.MyID;
                if (ID == 1) transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

                if (rootObj == null) return;

                rootObj.transform.GetChild(0).transform.localPosition = new Vector3(0.0f, -1.7f, 0.0f);
                rootObj.transform.GetChild(1).transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            };

            mainMgr.resultEvent += GetWinOrLose;
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public override void Update()
        {
            Move();

            Actoin();

            CheckCreateWeapon();
        }

        /// <summary>
        /// 生成中の武器をチェックする
        /// </summary>
        private void CheckCreateWeapon()
        {

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
            // ステージ内でないなら移動不可
            // バトル外は移動不可
            if (mainMgr.GetStageEdge(transform.position, myMovement.MoveVec) || mainMgr.Battle == false) {
                if (myRig.velocity != Vector3.zero) myRig.velocity = Vector3.zero;
                return;
            }
            // 移動
            myRig.velocity = myMovement.Velocity;
        }

        /// <summary>
        /// トラッキングするObjectの移動
        /// </summary>
        private void TrackingMove(Vector3 _inputVec)
        {
            // ルートがないならリターン
            if (rootObj == null) return;

            // HMDのローカル位置を取得
            Vector3 trackingPos = InputTracking.GetLocalPosition(XRNode.CenterEye);

            // 移動方向
            Vector3 moveVec = _inputVec * 13.0f;
            // rootを傾ける
            rootObj.transform.localPosition = trackingPos;
            rootObj.transform.rotation = Quaternion.Euler(moveVec.z, myMovement.GetMyCamera.transform.eulerAngles.y, -moveVec.x);
        }

        /// <summary>
        /// 武器アクション
        /// </summary>
        public override void Actoin()
        {
            // X/Aボタンで武器生成
            if (OVRInput.Get(OVRInput.RawButton.X))
                myHand[1].Create();
            if (OVRInput.Get(OVRInput.RawButton.A))
                myHand[0].Create();

            // 中指トリガーで武器を掴む
            //if (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger) && myHand[1].HasWeapon == false)
            //    myHand[1].GrabWeapon();
            //if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger) && myHand[0].HasWeapon == false)
            //    myHand[0].GrabWeapon();

            // バトル中のみ武器は使用できる
            if (mainMgr.Battle || mainMgr.Entry)
            {
                // 人差し指トリガーで武器使用
                if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger) && myHand[1].HasWeapon)
                    myHand[1].WeaponAction(true, false);
                else if (OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger) && myHand[1].HasWeapon)
                    myHand[1].WeaponAction(true, true);
                if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger) && myHand[0].HasWeapon)
                    myHand[0].WeaponAction(true, false);
                else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger) && myHand[0].HasWeapon)
                    myHand[0].WeaponAction(true, true);
            }

            // Y/Bボタンで所持中の武器の削除
            //if (OVRInput.GetDown(OVRInput.RawButton.Y) && myHand[1].isBoth == false)
            //    myHand[1].DeleteWeapon(myHand[1].CheckDelete());
            //if (OVRInput.GetDown(OVRInput.RawButton.B) && myHand[0].isBoth == false)
            //    myHand[0].DeleteWeapon(myHand[0].CheckDelete());

            // X/Aボタンを離したら武器の削除
            //if (OVRInput.GetUp(OVRInput.RawButton.X))
            //    myHand[1].weaponCreate.DeleteWeapon();
            //if (OVRInput.GetUp(OVRInput.RawButton.A))
            //    myHand[0].weaponCreate.DeleteWeapon();
            if (OVRInput.GetUp(OVRInput.RawButton.X))
                myHand[1].GrabWeapon();
            if (OVRInput.GetUp(OVRInput.RawButton.A))
                myHand[0].GrabWeapon();
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
        /// 勝敗判定
        /// </summary>
        public override void GetWinOrLose()
        {
            resultCanvas.gameObject.SetActive(true);

            // 勝敗の結果
            mainMgr.WinOrLose(ID, resultCanvas);
        }

        /// <summary>
        /// 当たり判定
        /// </summary>
        /// <param name="col"></param>
        public void OnTriggerEnter(Collider col)
        {
            var module = col.gameObject.GetComponent<ModuleObject>();

            if (module == null || module.Owner.GetMyProvider.MyID == ID) return;
            
            Score += module.GetPower();

            myDamageEffect.OnDamage();

            mainMgr.updateScore(ID, Score);
        }
    }
}
