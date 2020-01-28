using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Matsumoto.Audio;
using UniRx.Async;

public class CountUI : MonoBehaviour
{
    [SerializeField]
    Sprite threeImage;

    [SerializeField]
    Sprite tweImage;

    [SerializeField]
    Sprite oneImage;

    [SerializeField]
    Sprite startImage;

    Sprite[] countImages = new Sprite[4];

    int count = 0;

    Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = transform.GetChild(0).GetComponent<Image>();

        countImages[0] = threeImage;
        countImages[1] = tweImage;
        countImages[2] = oneImage;
        countImages[3] = startImage;

        image.sprite = countImages[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ImageChange()
    {
        image.sprite = countImages[count];

        count++;
    }

    public void CountDown()
    {
        AudioManager.PlaySE("cursor2");
    }

    public async UniTask CountStart()
    {
        AudioManager.PlaySE("decision7");

        await UniTask.Delay(1000);

        AudioManager.PlayBGM("GameScene_BGM");
    }
}
