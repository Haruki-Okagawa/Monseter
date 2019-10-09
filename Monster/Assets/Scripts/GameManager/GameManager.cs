using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : SingletonMonoBehaviour<GameManager>
{

    [SerializeField]
    private float[] SectionTime = new float[3];

    public delegate void FaseChange();
    FaseChange faseChange;



    private enum GameState
    {
        title,
        mainFase1Start,
        mainFase1,
        mainFase2Start,
        mainFase2,
        mainFase3Start,
        mainFase3,
        result,
        Pause
    }

    private GameState CurrentState;


    private float sectionTime = 0f;




    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        UnityEngine.Application.Quit();
#endif
    }

    private void Start()
    {
        CurrentState = GameState.title;
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case GameState.title:
                Title();
                break;
            //case GameState.mainStart:
            //    break;
            case GameState.mainFase1Start:
                MainFase1Start();
                break;
            case GameState.mainFase1:
                MainFase1();
                break;
            case GameState.mainFase2Start:
                MainFase2Start();
                break;
            case GameState.mainFase2:
                MainFase2();
                break;
            case GameState.mainFase3Start:
                MainFase3Start();
                break;
            case GameState.mainFase3:
                MainFase3();
                break;
            case GameState.result:
                ResultFase();
                break;
            case GameState.Pause:
                break;
            default:
                Quit();
                break;
        }
    }


    private void Title()
    {

    }


    private void MainFase1Start()
    {

    }


    private void MainFase1()
    {

    }


    private void MainFase2Start()
    {

    }


    private void MainFase2()
    {

    }


    private void MainFase3Start()
    {

    }


    private void MainFase3()
    {

    }


    private void ResultFase()
    {

    }




    private void FaseChanger(int faseNumber)
    {
        if(faseNumber == 1)
        {
            //faseChange = FaseStartFade.Fase1Start;
        }else if(faseNumber == 2)
        {
            //faseChange = FaseStartFade.Fase2Start;
        }
        else if(faseNumber == 3)
        {
            //faseChange = FaseStartFade.Fase3Start;
        }
        else
        {
            Quit();
        }

        faseChange();

    }


    private void SectionTimeCount()
    {
        sectionTime += Time.deltaTime;

    }

    public void hoge()
    {
        Debug.Log("うおおおおお");
    }

}
