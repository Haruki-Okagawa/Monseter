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


    private float CurrentTime = 0f;




    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        UnityEngine.Application.Quit();
#endif
    }


    private void Initialize()
    {
        CurrentState = GameState.title;
    }


    private void Start()
    {
        //CurrentState = GameState.title;
        Initialize();
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
        FaseChanger(1, 0);
    }


    private void MainFase1Start()
    {
        FaseChanger(2, 0);
    }


    private void MainFase1()
    {
        FaseChanger(3, SectionTime[0]);
    }


    private void MainFase2Start()
    {
        FaseChanger(4, 0);
    }


    private void MainFase2()
    {
        FaseChanger(5, SectionTime[1]);
    }


    private void MainFase3Start()
    {
        FaseChanger(6, 0);

    }


    private void MainFase3()
    {
        FaseChanger(7, SectionTime[2]);

    }


    private void ResultFase()
    {
        FaseChanger(8, 0);

    }

    private void PauseFase()
    {

    }




    private void FaseChanger(int faseNumber , float timeLimit)
    {
        if(CurrentTime >= timeLimit)
        {
            CurrentState = (GameState)faseNumber;
        }

        //if(faseNumber == 1)
        //{
        //    faseChange = { CurrentState = GameState.mainFase1Start; }
        //    //faseChange = FaseStartFade.Fase1Start;
        //}else if(faseNumber == 2)
        //{
        //    faseChange = { CurrentState = GameState.mainFase1; }
        //    //faseChange = FaseStartFade.Fase2Start;
        //}
        //else if(faseNumber == 3)
        //{
        //    faseChange = { if (sectionTime >= _sectionTime) CurrentState = GameState.mainFase2; }
        //    //faseChange = FaseStartFade.Fase3Start;
        //}
        //else
        //{
        //    Quit();
        //}

        //faseChange();

    }


    private void SectionTimeCount()
    {
        CurrentTime += Time.deltaTime;

    }

    public void hoge()
    {
        Debug.Log("うおおおおお");
    }

}
