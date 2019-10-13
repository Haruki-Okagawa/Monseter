using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space_Cut_Cube : MonoBehaviour
{
    // Start is called before the first frame update

    private int cut_count;

    public int Cut_Count
    {
        get { return this.cut_count; }  //取得用
        set { this.cut_count = value; } //値入力用
    }

    Rigidbody temp;
    void Start()
    {
        temp = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Debug.Log("フランドール・スカーレット！");
        //    Shinobu_am_me.Break_Mesh.Cut(this.gameObject);
        //}
        //Debug.Log("temp.velocity = " + temp.velocity.magnitude);
        if(temp.velocity.magnitude > 40)
        {
            if (cut_count < 4)
            {
                Shinobu_am_me.Break_Mesh.Cut(this.gameObject);
            }
            else
            {
                Debug.Log("キュッとしてどかーん！");
                //Destroy(this.gameObject);
            }
        }
    }
}
