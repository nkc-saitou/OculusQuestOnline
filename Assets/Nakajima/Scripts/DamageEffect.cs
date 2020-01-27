using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// ダメージエフェクトクラス
/// </summary>
namespace Nakajima.Player
{
    public class DamageEffect : MonoBehaviour
    {
        // 自身のCanvas
        public Canvas[] myEyeCanvas;

        // エフェクト用Image
        [SerializeField]
        private Image[] myEffectImg;

        private Material[] myEffectMaterial = new Material[2];

        private bool isFade;

        // キャンバスに使用するアルファ値(今回は1.0fで見えない)
        private float alpha = 0.0f;

        void Start()
        {
            // マテリアルのセット
            myEffectMaterial[0] = myEffectImg[0].material;
            myEffectMaterial[1] = myEffectImg[1].material;

            for(int i = 0;i < myEffectMaterial.Length; i++) {
                myEffectMaterial[i].SetFloat("_GradationPower", alpha);
            }
        }
        
        void Update()
        {

        }

        /// <summary>
        /// ダメージを受けた場合の処理
        /// </summary>
        [ContextMenu("Damage")]
        public void OnDamage()
        {
            StartCoroutine(ChangeParam(1.0f, 1.0f));
        }

        /// <summary>
        /// パラメータの変更
        /// </summary>
        /// <param name="_param">目標のパラメータ</param>
        /// <param name="_interval">変更にかかる時間</param>
        public IEnumerator ChangeParam(float _param, float _interval)
        {
            if (isFade) yield break;

            isFade = true;

            // 経過時間
            float time = 0.0f;
            while (time < _interval)
            {
                alpha = Mathf.Lerp(alpha, _param, time / _interval);

                for (int i = 0; i < myEffectMaterial.Length; i++)
                {
                    myEffectMaterial[i].SetFloat("_GradationPower", alpha);
                }

                time += Time.deltaTime;

                yield return null;
            }

            isFade = false;

            if (alpha == 1.0f) ResetCanvas();
        }

        /// <summary>
        /// キャンバスの状態をリセットする
        /// </summary>
        private void ResetCanvas()
        {
            StartCoroutine(ChangeParam(0.4f, 2.0f));
        }
    }
}


