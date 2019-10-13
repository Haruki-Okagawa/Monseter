using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create_cube : MonoBehaviour
{

    public GameObject Cube;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            GameObject temp = Instantiate(Cube, new Vector3(Random.Range(-15.0f, 15.0f), 15, Random.Range(7.0f, 18.0f)), Quaternion.identity);
            temp.transform.localScale = new Vector3(Random.Range(0.5f, 5.0f), Random.Range(0.5f, 5.0f), Random.Range(0.5f, 5.0f));
        }
    }
}
