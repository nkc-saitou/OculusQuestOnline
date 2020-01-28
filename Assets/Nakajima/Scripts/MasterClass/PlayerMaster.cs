using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakajima.Main;
using Nakajima.Weapon;
using Nakajima.Movement;
using Matsumoto.Weapon;

namespace Nakajima.Player
{
    public class PlayerMaster : MonoBehaviour
    {
        // MainManager
        protected MainManager mainMgr;
        // 自身のMovement;
        protected MovementComponetBase myMovement;
        // 自身のダメージ表示用クラス
        protected DamageEffect myDamageEffect;
        // ルートObj
        [SerializeField]
        protected GameObject rootObj;
        // 自身の頭
        [SerializeField]
        protected GameObject myHead;
        // 自身の体
        [SerializeField]
        protected GameObject myBody;
        // 頭と体の距離
        protected float offset;

        // 武器生成
        protected WeaponCreate weaponCreate;
        protected WeaponManager weaponMgr;
        // 自身のRigidbody
        protected Rigidbody myRig;
        // 入力ベクター
        protected Vector3 inputVec;

        // 往復時間
        [SerializeField, Header("往復にかかる時間")]
        protected float moveTime;
        [SerializeField, Header("振幅(移動量)")]
        protected float moveValue;
        // 周波数(振動数)
        protected float frequency;

        // 武器を使用できるかどうか
        public bool CanUseWeapon {
            protected set; get;
        }

        // 武器を生成できるかどうか
        public bool CanCreateWeapon {
            protected set; get;
        }

        // スコア
        public int Score {
            protected set; get;
        }
        
        /// <summary>
        /// 初回処理
        /// </summary>
        public virtual void Start()
        {

        }
        
        /// <summary>
        /// 更新処理
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// アクション
        /// </summary>
        public virtual void Actoin() { }

        /// <summary>
        /// 移動
        /// </summary>
        public virtual void Move() { }

        /// <summary>
        /// 武器の名前を抜き出す
        /// </summary>
        /// <param name="_objName">武器オブジェクト名</param>
        /// <returns>武器の名前</returns>
        public string GetWeaponName(string _objName)
        {
            // オブジェクト名から(Clone)を抜く
            string[] weaponName = _objName.Split('(');
            return weaponName[0];
        }

        /// <summary>
        /// 手の状態を更新
        /// </summary>
        /// <param name="_hand"></param>
        protected void UpdateHand(PlayerHand _hand) { weaponCreate.UnfoldUpdate(_hand); }

        /// <summary>
        /// 反対の手の設定
        /// </summary>
        /// <param name="_hand">利き手</param>
        /// <param name="_weapon">武器</param>
        public virtual void SetOpposite(HandMaster _hand, GameObject _weapon) { }

        /// <summary>
        /// 削除チェック
        /// </summary>
        /// <param name="_hand">利き手</param>
        public virtual void CheckDelete(HandMaster _hand) { }
    }
}