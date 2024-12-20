﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Defines the logic behind a single achievement on the UI
/// </summary>
public class UIAchievement : MonoBehaviour
{
    public TextMeshProUGUI Title, Description, Percent;
    public Image ProgressBar;
    public GameObject SpoilerOverlay;
    public TextMeshProUGUI SpoilerText;
    [HideInInspector]public AchievenmentStack AS;

    /// <summary>
    /// Destroy object after a certain amount of time
    /// </summary>
    public void StartDeathTimer ()
    {
        StartCoroutine(Wait());
    }

    /// <summary>
    /// Add information  about an Achievement to the UI elements
    /// </summary>
    public void Set (AchievementInfromation Information, AchievementState State)
    {
        if(Information.Spoiler && !State.Achieved)
        {
            SpoilerOverlay.SetActive(true);
            SpoilerText.text = AchievementManager.instance.SpoilerAchievementMessage;
        }
        else
        {
            Title.text = Information.DisplayName;
            Description.text = Information.Description;

            if (Information.Progression)
            {
                float CurrentProgress = AchievementManager.instance.ShowExactProgress ? State.Progress : (State.LastProgressUpdate * Information.NotificationFrequency);
                float DisplayProgress = State.Achieved ? Information.ProgressGoal : CurrentProgress;

                if (State.Achieved)
                {
                    Percent.text = Information.ProgressGoal + Information.ProgressSuffix + " / " + Information.ProgressGoal + Information.ProgressSuffix + " (Achieved)";
                }
                else
                {
                    Percent.text = DisplayProgress + Information.ProgressSuffix +  " / " + Information.ProgressGoal + Information.ProgressSuffix;
                }

                ProgressBar.fillAmount = DisplayProgress / Information.ProgressGoal;
            }
            else //Single Time
            {
                ProgressBar.fillAmount = State.Achieved ? 1 : 0;
                Percent.text = State.Achieved ? "(Achieved)" : "(Locked)";
            }
        }
    }

    private IEnumerator Wait ()
    {
        yield return new WaitForSeconds(AchievementManager.instance.DisplayTime);
        GetComponent<Animator>().SetTrigger("ScaleDown");
        yield return new WaitForSeconds(0.1f);
        AS.CheckBackLog();
        Destroy(gameObject);
    }
}
