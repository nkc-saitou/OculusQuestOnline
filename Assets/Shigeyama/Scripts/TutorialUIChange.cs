using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIChange : MonoBehaviour
{
    [SerializeField]
    Sprite[] tutorialImagePrefabs;

    [SerializeField]
    Image tutorialImage;

    int imageNum = 0;

    Animator anim;

    float timer = 0;
    float timeInterval = 4;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > timeInterval)
        {
            timer = 0;
            anim.SetTrigger("IsChange");
        }
    }

    public void ImageChange()
    {
        imageNum++;
        if (imageNum >= tutorialImagePrefabs.Length)
        {
            imageNum = 0;
        }

        tutorialImage.sprite = tutorialImagePrefabs[imageNum];
    }
}
