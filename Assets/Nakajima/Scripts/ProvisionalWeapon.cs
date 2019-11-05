using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 仮武器クラス
/// </summary>
namespace Nakajima.Weapon
{
    public class ProvisionalWeapon : MonoBehaviour
    {
        // 自身の生成元の参照
        private GameObject targetObj;
        private WeaponCreate weaponCreate;
        
        [SerializeField, Header("回転スピード")]
        private float rotateSpeed;
        // 差分
        private Vector3 offset = new Vector3(0.0f, 1.0f, 0.0f);

        // 自身の原点
        private Vector3 originPos;

        void Start()
        {
            originPos = transform.position;
            weaponCreate = FindObjectOfType<WeaponCreate>();
            targetObj = weaponCreate.gameObject;
        }

        void Update()
        {
            Move();
        }

        /// <summary>
        /// 円移動を繰り返す
        /// </summary>
        private void Move()
        {
            // 差分計算
            transform.position = targetObj.transform.position + offset;
            transform.Rotate(0.0f, rotateSpeed, 0.0f);
        }
    }
}

