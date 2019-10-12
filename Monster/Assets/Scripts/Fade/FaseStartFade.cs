using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaseStartFade : MonoBehaviour
{
    private Material _material;
    [SerializeField]
    private Text[] startMassage = new Text[3];

    private bool[] isFaseStart = new bool[3];

    private Text moveText;

    // Start is called before the first frame update
    void Start()
    {
        _material = this.GetComponent<Material>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameManager.Instance.hoge();
        }


    }

    public static void Fase1StartFade()
    {
        
    }

    public static void Fase2StartFade()
    {

    }

    public static void Fase3StartFade()
    {

    }

}
