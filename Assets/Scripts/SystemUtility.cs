using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SystemUtility : MonoBehaviour
{
    void Start()
    {
        OVRManager.display.displayFrequency = 72f; //72Hzモードに変更

        XRSettings.eyeTextureResolutionScale = 1.25f;

        //焦点以外の周囲を圧縮する
        OVRManager.fixedFoveatedRenderingLevel = OVRManager.FixedFoveatedRenderingLevel.Medium;

        //パフォーマンスレベルの変更 Default=2 0~3,低~高 とパフォーマンスが変わる
        //3にすると高クロックになり著しくバッテリー消費と発熱を起こす．
        OVRManager.cpuLevel = 2;
        OVRManager.gpuLevel = 2;


    }
}
