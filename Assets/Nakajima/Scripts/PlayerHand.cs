using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Nakajima.Weapon;
using Matsumoto.Weapon;

/// <summary>
/// プレイヤーハンドクラス
/// </summary>
namespace Nakajima.Player
{
    public class PlayerHand : MonoBehaviour
    {
        // 武器を掴んだイベント
        public event Action<PlayerHand, GameObject> grabWeapon;
        // 判定の手にも武器をもたせる
        public event Action<PlayerHand, GameObject> oppositeWeapon;
        // 自身の状態を送る
        public event Action<PlayerHand> updateHandStatus;



        // WeaponCreateの参照
        private WeaponCreate weaponCreate;
        private WeaponManager weaponMgr;

        private PhotonView photonView;

        // どっちの手か
        public OVRInput.RawButton myTouch;

        [SerializeField]
        private GameObject GunObj;

        // 触れたオブジェクト
        [HideInInspector]
        public GameObject hasObj;

        // 武器を所持しているか
        public bool HasWeapon {
            get; private set;
        }

        /// <summary>
        /// 初回処理
        /// </summary>
        void Start()
        {
            HasWeapon = false;
            weaponCreate = FindObjectOfType<WeaponCreate>();
            weaponMgr = FindObjectOfType<WeaponManager>();
            photonView = GetComponent<PhotonView>();
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        void Update()
        {
            // 武器所持時のみ実行
            if (HasWeapon) return;

            // イベント実行
            updateHandStatus?.Invoke(this);

            // 武器を掴む
            if (OVRInput.GetDown(myTouch)) {
                //GrabWeapon();
                photonView.RPC(nameof(GrabWeapon), RpcTarget.All,TestOnlineData.PlayerID);
            }

            WeaponAction();
        }

        /// <summary>
        /// 武器を手にセットする
        /// </summary>
        private void SetHandWeapon()
        {
            // 何も触れていないならリターン
            if (hasObj == null) return;


        }

        /// <summary>
        /// 武器を掴む
        /// </summary>
        //[PunRPC]
        public void GrabWeapon(int _playerID)
        {
            
            // 何も触れていないならリターン
            if (hasObj == null) return;

            // 生成中の武器だったら装備する
            foreach (GameObject weapon in weaponCreate.createWeaponList)
            {
                if (weapon.name == hasObj.name)
                {
                    // 掴んだなら他の武器は削除
                    weaponCreate.DeleteWeapon();
                    weaponCreate.WeaponUnfold = false;

                    // 武器のデータを持ってくる
                    weaponMgr.LoadWeapon();
                    var handList = weaponMgr.CreateWeapon(GetWeaponName(weapon.name));
                    hasObj = handList[0].GetBody();
                    hasObj.transform.parent = transform;
                    hasObj.transform.localPosition = Vector3.zero;
                    hasObj.transform.localRotation = Quaternion.identity;

                    HasWeapon = true;
                    weaponCreate.CanCreate = false;
                    if (handList.Length > 1) {
                        var obj = handList[1].GetBody();
                        oppositeWeapon?.Invoke(this, obj);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 武器をセットする(両手武器用)
        /// </summary>
        /// <param name="_weapon">セットする武器</param>
        public void SetWeapon(GameObject _weapon)
        {
            hasObj = _weapon;
            _weapon.transform.parent = transform;
            _weapon.transform.localPosition = Vector3.zero;
            _weapon.transform.localRotation = Quaternion.identity;
            HasWeapon = true;
        }

        /// <summary>
        /// 武器の名前を抜き出す
        /// </summary>
        /// <param name="_objName">武器のオブジェクト</param>
        /// <returns>武器の名前</returns>
        private string GetWeaponName(string _objName)
        {
            // オブジェクト名から(Clone)を抜く
            string[] weaponName = _objName.Split('(');
            return weaponName[0];
        }

        /// <summary>
        /// 武器ごとのアクション実行
        /// </summary>
        private void WeaponAction()
        {
            // 武器がないならリターン
            if (HasWeapon == false || hasObj == null) return;

            var weapon = hasObj.GetComponent<IWeapon>();

            // 武器使用
            switch (myTouch) {
                case OVRInput.RawButton.RHandTrigger:
                    if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger)) {
                        weapon.OnButtonDown(OVRInput.Button.One);
                    }
                    if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger)) {
                        weapon.OnButtonUp(OVRInput.Button.One);
                    }
                    break;
                case OVRInput.RawButton.LHandTrigger:
                    if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger)) {
                        weapon.OnButtonDown(OVRInput.Button.One);
                    }
                    if (OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger)) {
                        weapon.OnButtonUp(OVRInput.Button.One);
                    }
                    break;
            }
        }

        /// <summary>
        /// 所持中の武器を破棄する
        /// </summary>
        public bool DeleteWeapon()
        {
            // 武器を所持していないならfalse
            if (HasWeapon == false) return false;

            Destroy(hasObj);
            HasWeapon = false;
            return true;
        }

        /// <summary>
        /// 触れている状態
        /// </summary>
        /// <param name="_col">コリジョン</param>
        void OnTriggerStay(Collider _col)
        {
            // 武器を持っているならリターン
            if (HasWeapon) return;

            var obj = _col.gameObject.GetComponent<ProvisionalWeapon>();
            if (obj == null) return;
            hasObj = _col.gameObject;
        }

        /// <summary>
        /// 離れた瞬間
        /// </summary>
        /// <param name="_col">コリジョン</param>
        void OnTriggerExit(Collider _col)
        {
            // 武器を持っているならリターン
            if (HasWeapon) return;

            var obj = _col.gameObject.GetComponent<ProvisionalWeapon>();
            if (obj == null) return;
        }
    }
}