using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ButtonTutorialisation : MonoBehaviour
{
    [SerializeField] private CanvasGroup myUIGroup;
    [SerializeField] private CanvasGroup leftButton;

    [SerializeField] private bool fadeIn = false;
    [SerializeField] private bool fadeOut = false;

    [SerializeField] private Button left;
    public bool accessedStream = false;

    public void Start()
    {
        myUIGroup = leftButton.GetComponent<CanvasGroup>();
        Repeat();
    }

    public void streamAccessed()
    {
        accessedStream = true;
    }

    private void Repeat()
    {
        while (accessedStream == false)
        {
            if (myUIGroup.alpha >= 1)
            {
                fadeOut = true;
            }
            if (myUIGroup.alpha <= 1)
            {
                fadeIn = true;
            }
        }
    }

    public void ShowUI()
    {
        fadeIn = true;
    }

    public void HideUI()
    {
        fadeOut = true;
    }

    private void Update()
    {


        if (fadeIn)
        {
            if (myUIGroup.alpha < 1)
            {
                myUIGroup.alpha += Time.deltaTime;
                if (myUIGroup.alpha >= 1)
                {
                    fadeIn = false;
                }
            }
        }
        if (fadeOut)
        {
            if (myUIGroup.alpha >= 0)
            {
                myUIGroup.alpha -= Time.deltaTime;
                if (myUIGroup.alpha == 0)
                {
                    fadeOut = false;
                }
            }
        }

    }
}
