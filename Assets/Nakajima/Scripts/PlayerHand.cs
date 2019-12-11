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
        // 同期したい
        public event Action<int, string> syncWeapon;
        public event Action<int, GameObject, string> setOppesite;

        // WeaponCreateの参照
        private WeaponCreate weaponCreate;
        private WeaponManager weaponMgr;

        private PhotonView photonView;

        // 自身のProvider
        public DisplayPlayerProvider myProvider;

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

            var manager = FindObjectOfType<NetworkEventManager>();
            //manager.EventBind(this);
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        void Update()
        {
            // 武器所持時のみ実行
            if (photonView.IsMine && HasWeapon) {
                WeaponAction();
                return;
            }

            // イベント実行
            updateHandStatus?.Invoke(this);

            // 武器を掴む
            if (OVRInput.GetDown(myTouch) && HasWeapon == false) {
                GrabWeapon();
            }
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
        public void GrabWeapon()
        {
            // 何も触れていないならリターン
            if (hasObj == null) return;

            // 生成中の武器だったら装備する
            // 掴んだなら他の武器は削除
            weaponCreate.DeleteWeapon();

            // 武器のデータを持ってくる
            weaponMgr.LoadWeapon();
            var handList = weaponMgr.CreateWeapon(GetWeaponName(hasObj.name));
            hasObj = handList[0].GetBody();
            hasObj.transform.parent = transform;
            hasObj.transform.localPosition = Vector3.zero;
            hasObj.transform.localRotation = Quaternion.identity;

            HasWeapon = true;
            if (photonView.IsMine) weaponCreate.CanCreate = false;

            // 同期したい
            Debug.Log("ID ; " + TestOnlineData.PlayerID + " 通過");
            syncWeapon?.Invoke(myProvider.MyID, GetWeaponName(hasObj.name));

            if (handList.Length > 1)
            {
                var obj = handList[1].GetBody();
                oppositeWeapon?.Invoke(this, obj);
                setOppesite?.Invoke(myProvider.MyID, obj, GetWeaponName(hasObj.name));
            }
        }

        /// <summary>
        /// 武器のセット
        /// </summary>
        /// <param name="_weaponName"></param>
        public void GrabWeapon(string _weaponName)
        {
            // 武器を持っているならリターン
            if (HasWeapon || photonView.IsMine) return;

            weaponMgr.LoadWeapon();
            var handList = weaponMgr.CreateWeapon(_weaponName);
            hasObj = handList[0].GetBody();
            hasObj.transform.parent = transform;
            hasObj.transform.localPosition = Vector3.zero;
            hasObj.transform.localRotation = Quaternion.identity;

            HasWeapon = true;
            if(photonView.IsMine) weaponCreate.CanCreate = false;
            syncWeapon?.Invoke(myProvider.MyID, GetWeaponName(hasObj.name));

            // 反対の手にも装備
            if (handList.Length > 1)
            {
                var obj = handList[1].GetBody();
                oppositeWeapon?.Invoke(this, obj);
                setOppesite?.Invoke(myProvider.MyID, obj, GetWeaponName(hasObj.name));
            }
        }

        /// <summary>
        /// 武器をセットする(両手武器用)
        /// </summary>
        /// <param name="_weapon">セットする武器</param>
        public void SetWeapon(GameObject _weapon)
        {
            hasObj = _weapon;
            hasObj.transform.parent = transform;
            hasObj.transform.localPosition = Vector3.zero;
            hasObj.transform.localRotation = Quaternion.identity;
            HasWeapon = true;
        }

        /// <summary>
        /// 武器の名前を抜き出す
        /// </summary>
        /// <param name="_objName">武器のオブジェクト</param>
        /// <returns>武器の名前</returns>
        public string GetWeaponName(string _objName)
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
            if (HasWeapon == false || hasObj == null) { Debug.Log("リターン"); return; }

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