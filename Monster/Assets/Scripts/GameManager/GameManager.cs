using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void FaseChange();
    FaseChange faseChange;

    private enum GameState
    {
        title,
        mainStart,
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
                break;
            case GameState.mainStart:
                break;
            case GameState.mainFase1Start:
                FaseStart(1);
                break;
            case GameState.mainFase1:
                break;
            case GameState.mainFase2Start:
                FaseStart(2);
                break;
            case GameState.mainFase2:
                break;
            case GameState.mainFase3Start:
                FaseStart(3);
                break;
            case GameState.mainFase3:
                break;
            case GameState.result:
                break;
            case GameState.Pause:
                break;
            default:
                Quit();
                break;
        }
    }


    private void FaseStart(int faseNumber)
    {
        if(faseNumber == 1)
        {
            faseChange = FaseStartFade.Fase1Start;
        }else if(faseNumber == 2)
        {
            faseChange = FaseStartFade.Fase2Start;
        }
        else if(faseNumber == 3)
        {
            faseChange = FaseStartFade.Fase3Start;
        }
        else
        {
            Quit();
        }

        faseChange();

    }


}
