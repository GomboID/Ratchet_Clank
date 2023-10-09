using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADSManager : Singleton<ADSManager>
{
    public event Action<bool> Action_IsRewarded;

    private void Start()
    {
        
    }

    public void ShowInterstitial()
    {
        
    }

    public void ShowRewarded()
    {
        
    }

    private void RewardClosed(bool _isReward)
    {
        Action_IsRewarded?.Invoke(_isReward);
    }
}
