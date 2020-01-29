using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx.Async;
using Photon.Pun;
using Nakajima.Weapon;
using Matsumoto.Weapon;

/// <summary>
/// プレイヤーハンドクラス
/// </summary>
namespace Nakajima.Player
{
    public class PlayerHand : HandMaster
    {
        // 判定の手にも武器をもたせる
        public event Action<HandMaster, GameObject> oppositeWeapon;
        // 武器の削除
        public event Action<HandMaster> deleteWeapon;
        public event Action<int, string, bool> netDeleteWeapon;
        // 同期したい
        public event Action<int, string, string> syncWeapon;
        public event Action<int, string, GameObject, string> setOppesite;

        // PhotonView
        private PhotonView photonView;

        [SerializeField]
        private GameObject GunObj;

        // 右手か左手か
        private string handName;

        int temp = 0;

        /// <summary>
        /// 初回処理
        /// </summary>
        public override void Start()
        {
            HasWeapon = false;
            weaponMgr = FindObjectOfType<WeaponManager>();
            weaponCreate = GetComponent<WeaponCreate>();
            photonView = GetComponent<PhotonView>();

            if (myTouch == OVRInput.RawButton.RHandTrigger) handName = "_right";
            else if(myTouch == OVRInput.RawButton.LHandTrigger) handName = "_left";
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public override void Update()
        {

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
        /// 武器を掴む(ローカル)
        /// </summary>
        public override void GrabWeapon()
        {
            // 何も触れていないならリターン
            if (HasWeapon) return;

            // 武器のデータを持ってくる
            weaponMgr.LoadWeapon();
            var handList = weaponMgr.CreateWeapon(GetWeaponName(hasObj.name), 0.5f);
            hasObj = handList[0].GetBody();
            hasObj.transform.parent = transform;
            hasObj.transform.localPosition = Vector3.zero;
            hasObj.transform.localRotation = Quaternion.identity;

            // 生成中の武器だったら装備する
            // 掴んだなら他の武器は削除
            weaponCreate.DeleteWeapon();

            handList[0].SetOwner(this);

            HasWeapon = true;
            if (photonView.IsMine) weaponCreate.CanCreate = false;

            // 同期したい
            Debug.Log("ID : " + TestOnlineData.PlayerID + " 通過");
            syncWeapon?.Invoke(GetMyProvider.MyID, handName, GetWeaponName(hasObj.name));

            if (weaponMgr.HasOtherWeapon(GetWeaponName(hasObj.name)))
            {
                var obj = handList[1].GetBody();
                oppositeWeapon?.Invoke(this, obj);
            }
        }

        /// <summary>
        /// 武器を掴む(ネットワーク)
        /// </summary>
        /// <param name="_weaponName">武器の名前</param>
        public override void GrabWeapon(string _weaponName)
        {
            // 武器を持っているならリターン
            if (HasWeapon || photonView.IsMine) return;

            HasWeapon = true;

            weaponMgr.LoadWeapon();
            var handList = weaponMgr.CreateWeapon(_weaponName, 0.5f);
            hasObj = handList[0].GetBody();
            hasObj.transform.parent = transform;
            hasObj.transform.localPosition = Vector3.zero;
            hasObj.transform.localRotation = Quaternion.identity;

            handList[0].SetOwner(this);

            if (photonView.IsMine) weaponCreate.CanCreate = false;

            // 反対の手にも装備
            if (weaponMgr.HasOtherWeapon(GetWeaponName(hasObj.name)))
            {
                var obj = handList[1].GetBody();
                setOppesite?.Invoke(GetMyProvider.MyID, handName, obj, GetWeaponName(hasObj.name));
            }
        }

        /// <summary>
        /// 武器をセットする(両手武器用)
        /// </summary>
        /// <param name="_weapon">セットする武器オブジェクト</param>
        public override void SetWeapon(GameObject _weapon)
        {
            // 武器を持っているなら削除
            if(HasWeapon) DeleteWeapon(CheckDelete());
            
            hasObj = _weapon;
            hasObj.transform.parent = transform;
            hasObj.transform.localPosition = Vector3.zero;
            hasObj.transform.localRotation = Quaternion.identity;
            HasWeapon = true;
            isBoth = true;

            _weapon.GetComponent<IWeapon>().SetOwner(this);
        }

        public override void WeaponAction(bool _getButton, bool _UpOrDown)
        {
            if (photonView.IsMine == false || _getButton == false) return;

            var weapon = hasObj.GetComponent<IWeapon>();

            // 武器使用
            if (_UpOrDown) {
                weapon.OnButtonUp(OVRInput.Button.One);
            }
            else {
                weapon.OnButtonDown(OVRInput.Button.One);
            }
        }

        /// <summary>
        /// 武器生成(まだ所持ではない)
        /// </summary>
        public async override void Create()
        {
            if (isBoth) return;

            // 武器所持中なら武器を削除する
            // 削除するまで待機
            if (HasWeapon) DeleteWeapon(CheckDelete());
            
            await UniTask.WaitUntil(() => hasObj == null);

            weaponCreate.ActiveHand = this;
            weaponCreate.Create();
        }

        /// <summary>
        /// 所持中の武器を破棄する
        /// </summary>
        public override bool CheckDelete()
        {
            // 武器を所持していないならfalse
            if (HasWeapon == false || hasObj == null) return false;

            // 両手武器の場合逆の手も削除する
            if (weaponMgr.HasOtherWeapon(GetWeaponName(hasObj.name))) return true;

            return false;
        }

        /// <summary>
        /// 武器の削除
        /// </summary>
        /// <param name="_flag">両手武器かどうか</param>
        public override void DeleteWeapon(bool _flag)
        {
            // なにもないならリターン
            if (HasWeapon == false) return;

            // 両手武器の場合
            if (_flag) deleteWeapon?.Invoke(this);

            // 削除
            var weapon = hasObj.GetComponent<IWeapon>();
            if (weapon == null) return;

            weapon.Destroy(0.5f);
            if(photonView.IsMine) netDeleteWeapon?.Invoke(GetMyProvider.MyID, handName, _flag);
            HasWeapon = false;
            isBoth = false;
            weaponCreate.Reset();
        }

        /// <summary>
        /// 武器の削除
        /// </summary>
        public override void DeleteWeapon()
        {
            // なにもないならリターン
            if (HasWeapon == false || photonView.IsMine) return;

            // 削除
            var weapon = hasObj.GetComponent<IWeapon>();
            if (weapon == null) return;

            weapon.Destroy(0.5f);
            HasWeapon = false;
            isBoth = false;
            weaponCreate.Reset();
        }

        /// <summary>
        /// 触れている状態
        /// </summary>
        /// <param name="_col">コリジョン</param>
        private void OnTriggerStay(Collider _col)
        {
            // 武器を持っているならリターン
            if (HasWeapon) return;

            var obj = _col.gameObject.GetComponent<ProvisionalWeapon>();
            if (obj == null) return;
            hasObj = _col.gameObject;
        }
    }
}