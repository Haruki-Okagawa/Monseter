using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thow_ball : MonoBehaviour
{

    Rigidbody hoge;
    // Start is called before the first frame update
    void Start()
    {
        hoge = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            hoge.velocity += new Vector3(0, 0, 100.0f);
        }
    }
}
