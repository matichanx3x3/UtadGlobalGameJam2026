using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActualPlayerState
{
    NoActive,
    Active,
    WaitingForMask,
}
public enum Masktype
{
    NoMask = 0, 
    Rojo = 1,
    Verde = 2,
    Azul = 3,
}
public class PlayerController : MonoBehaviour
{
    public PlayerData data;
    public bool isActive = true;
    public PlayerInputRead pInput;
    public ActualPlayerState actualPlayerState = ActualPlayerState.Active;
    
    public Masktype masktype = Masktype.NoMask;
    //Referencia del PlayerMask;
    public PlayerMask playerMask;
    public int maskActual = 0;
    public bool noMask = true;

    private int actualHP;
    [SerializeField]private int score = 0;

    public static Action<int> OnPointsEarned;

    public bool IsInputBlocked => actualPlayerState == ActualPlayerState.WaitingForMask;

    private void Awake()
    {
        if (pInput == null)
        {
            pInput = GetComponent<PlayerInputRead>();
            
            if (pInput == null)
            {
                Debug.LogError("PlayerInputRead no encontrado.");
            }
        }

        if (playerMask == null)
        {
            playerMask = GetComponent<PlayerMask>();
            if (playerMask == null)
            {
                Debug.LogError("PlayerMask no encontrado.");
            }
        }
    }

    void Start()
    {
        actualHP = data.hp + data.hpBonus; 
        score = data.scoreSaved;
        GameCanva.instance.SetInitialValue(score);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            AddScore(1000);
        }
    }

    void OnEnable()
    {
        OnPointsEarned += AddScore;
    }

    void OnDisable()
    {
        OnPointsEarned -= AddScore;
    }

    public void ActivatePlayer()
    {
        isActive = true;
        
    }

    public void ChangePlayerState(ActualPlayerState state)
    {
        actualPlayerState = state;
    }


    public void ChangeMaskType(Masktype mask)
    {
        masktype = mask;
        
        switch (mask)
        {
            case Masktype.NoMask:
                maskActual = 0;
                noMask = true;
                break;
                
            case Masktype.Rojo:
                maskActual = 1;
                noMask = false;
                break;
                
            case Masktype.Verde: 
                maskActual = 2;
                noMask = false;
                break;
                
            case Masktype.Azul:
                maskActual = 3;   
                noMask = false;
                break;
        }
    }

    public void AddScore(int scoreToAdd)
    {
       int finalScore = scoreToAdd * data.scoreMultiplier; 
        score += finalScore;
        GameCanva.instance.UpdateScore(score);
        SaveScore();
    }

    public void SaveScore()
    {
        data.scoreSaved = score;
    }

    public void UseScore(int scoreToLose)
    {
        if(score - scoreToLose < 0) return;
        score -= scoreToLose;
        GameCanva.instance.UpdateScore(score);
        SaveScore();
    }
}
