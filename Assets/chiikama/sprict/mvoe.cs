using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mvoe : MonoBehaviour
{
    public float speed;
    public float destroyTime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;

    }
}
