using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChange : MonoBehaviour
{

    void SceneFade()
    {
        SceneChanger.Instance.MoveScene("", 1.0f, 1.0f, SceneChangeType.BlackFade, false);
    }
}
