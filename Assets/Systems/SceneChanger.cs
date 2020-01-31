﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum SceneChangeType
{
    BlackFade,
    WhiteFade,
}
public class SceneChanger : SingletonMonoBehaviour<SceneChanger>
{
    private bool _isMoving;
    private Camera _camera;
    private List<Material> materials = new List<Material>();
    private Material _fadeType;
    private float _ratio;
    protected override void Init()
    {
        base.Init();
        _camera = gameObject.AddComponent<Camera>();
        _camera.clearFlags = CameraClearFlags.Nothing;
        _camera.cullingMask = 0;
        LoadShader();
    }
    public bool MoveScene(string sceneName, float fadeInTime, float fadeOutTime, SceneChangeType type, bool isPhoton = false)
    {
        if (_isMoving) return false;
        _fadeType = materials[(int)type];
        if (isPhoton)
        {
            StartCoroutine(MoveScenePhotonAnim(sceneName, fadeInTime, fadeOutTime));
        }
        else
        {
            StartCoroutine(MoveSceneAnim(sceneName, fadeInTime, fadeOutTime));
        }
        return true;
    }
    private void LoadShader()
    {
        materials.Clear();
        var basePath = "Materials/";
        materials.Add(Resources.Load<Material>(basePath + "BlackFade"));
        materials.Add(Resources.Load<Material>(basePath + "WhiteFade"));
        //materials.Add(Resources.Load<Material>(basePath + "StarBlackFade"));
    }
    private IEnumerator MoveSceneAnim(string sceneName, float fadeInTime, float fadeOutTime)
    {
        _isMoving = true;
        var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        operation.allowSceneActivation = false;
        _ratio = 0;
        while (_ratio < 1.0f)
        {
            _ratio = Mathf.Min(_ratio + Time.unscaledDeltaTime / fadeInTime, 1.0f);
            yield return null;
        }
        operation.allowSceneActivation = true;
        yield return operation;
        while (_ratio > 0.0f)
        {
            _ratio = Mathf.Max(_ratio - Time.unscaledDeltaTime / fadeOutTime, 0.0f);
            yield return null;
        }
        _isMoving = false;
    }
    private IEnumerator MoveScenePhotonAnim(string sceneName, float fadeInTime, float fadeOutTime)
    {
        _isMoving = true;
        _ratio = 0;
        while (_ratio < 1.0f)
        {
            _ratio = Mathf.Min(_ratio + Time.unscaledDeltaTime / fadeInTime, 1.0f);
            yield return null;
        }
        Photon.Pun.PhotonNetwork.LoadLevel(sceneName);
        while (_ratio > 0.0f)
        {
            _ratio = Mathf.Max(_ratio - Time.unscaledDeltaTime / fadeOutTime, 0.0f);
            yield return null;
        }
        _isMoving = false;
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_fadeType)
        {
            _fadeType.SetFloat("_Ratio", _ratio);
            Graphics.Blit(source, destination, _fadeType);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}