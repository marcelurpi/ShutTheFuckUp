using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundHandler : MonoBehaviour
{
    [SerializeField] private float RoundTimerStart;
    [SerializeField] private float TimeBetweenRounds;
    [SerializeField] private float RoundTimerIncrease;

    public event Action OnRoundStart;
    public event Action OnRoundEnd;

    private bool RoundRunning = true;
    private float CurrentRoundTimer = 0;
    private int CurrentRoundNum = 1;

    private void Start()
    {
        StartCoroutine(StartRound(0, 0.5f));
    }

    private void Update()
    {
        if (RoundRunning)
        {
            UpdateRoundNumber();
        }
    }

    private void UpdateRoundNumber()
    {
        CurrentRoundTimer += Time.deltaTime;
        int roundTimerNumber = Mathf.RoundToInt(RoundTimerStart - CurrentRoundTimer);
        if (roundTimerNumber <= 0)
        {
            EndRound();
        }
    }

    public void EndRound()
    {
        RoundRunning = false;
        CurrentRoundTimer = 0;
        RoundTimerStart += RoundTimerIncrease;
        if (OnRoundEnd != null)
        {
            OnRoundEnd();
        }
    }

    public IEnumerator StartRound(int roundNum = -1, float timeBtwRoundsMultiplier = 1f)
    {
        yield return new WaitForSeconds(TimeBetweenRounds * timeBtwRoundsMultiplier);
        CurrentRoundNum = roundNum != -1 ? roundNum != 0 ? roundNum : 1 : CurrentRoundNum + 1;
        if (OnRoundStart != null)
        {
            OnRoundStart();
        }
        RoundRunning = true;
    }

    public bool IsRoundRunning()
    {
        return RoundRunning;
    }

    public int GetRoundNum()
    {
        return CurrentRoundNum;
    }

    public int GetRoundTimer()
    {
        if (CurrentRoundTimer == 0)
        {
            return 0;
        }
        return Mathf.RoundToInt(RoundTimerStart - CurrentRoundTimer);
    }
}
