using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChange : MonoBehaviour
{

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Any)||Input.GetKeyDown(KeyCode.Z)) SceneFade();//questのボタンかZキーが入力されたら
    }

    /// <summary>
    /// シーンの移動処理
    /// </summary>
    void SceneFade()
    {
        SceneChanger.Instance.MoveScene("Entry", 1.0f, 1.0f, SceneChangeType.BlackFade, false);//Entryシーンへ移動
    }
}
