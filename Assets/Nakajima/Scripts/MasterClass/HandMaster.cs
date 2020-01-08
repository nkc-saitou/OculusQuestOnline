using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Nakajima.Weapon;
using Matsumoto.Weapon;

/// <summary>
/// Handマスタークラス
/// </summary>
namespace Nakajima.Player
{
    public class HandMaster : MonoBehaviour
    {
        // WeaponCreateの参照
        public WeaponCreate weaponCreate;
        protected WeaponManager weaponMgr;

        // どっちの手か
        public OVRInput.RawButton myTouch;

        // 触れたオブジェクト
        [HideInInspector]
        public GameObject hasObj;

        // 武器を所持しているか
        private bool hasWeapon;
        public bool HasWeapon
        {
            set { hasWeapon = value; }
            get { return hasWeapon; }
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
        /// 武器を掴む(ローカル)
        /// </summary>
        public virtual void GrabWeapon() { }

        /// <summary>
        /// 武器を掴む(ネットワーク)
        /// </summary>
        /// <param name="_weaponName">武器の名前</param>
        public virtual void GrabWeapon(string _weaponName) { }

        /// <summary>
        /// 武器をセットする(両手武器用)
        /// </summary>
        /// <param name="_weapon">セットする武器オブジェクト</param>
        public virtual void SetWeapon(GameObject _weapon) { }

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
        /// 武器ごとのアクション実行
        /// </summary>
        protected void WeaponAction()
        {
            // 武器がないならリターン
            if (HasWeapon == false || hasObj == null) { Debug.Log("リターン"); return; }

            var weapon = hasObj.GetComponent<IWeapon>();

            // 武器使用
            switch (myTouch)
            {
                case OVRInput.RawButton.RHandTrigger:
                    if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
                    {
                        weapon.OnButtonDown(OVRInput.Button.One);
                    }
                    if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
                    {
                        weapon.OnButtonUp(OVRInput.Button.One);
                    }
                    break;
                case OVRInput.RawButton.LHandTrigger:
                    if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
                    {
                        weapon.OnButtonDown(OVRInput.Button.One);
                    }
                    if (OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger))
                    {
                        weapon.OnButtonUp(OVRInput.Button.One);
                    }
                    break;
            }
        }

        /// <summary>
        /// 武器生成(まだ所持ではない)
        /// </summary>
        public virtual void Create() { }

        /// <summary>
        /// 所持中の武器を破棄する
        /// </summary>
        public virtual bool DeleteWeapon(bool _flag) { return false; }
    }
}

  
