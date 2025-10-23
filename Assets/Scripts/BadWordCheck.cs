using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

public class BadWordCheck : MonoBehaviour
{
    public TextAsset[] languages;

    public List<string> words;

    public List<string> excludedWords;

    int wordCounter;

    private void Start()
    {
        foreach (TextAsset languagePack in languages)
        {
            string languageText = languagePack.text;
            string[] languageWords = Regex.Split(languageText, "\n|\r|\r\n");
            foreach (string word in languageWords) 
            {
                if (!string.IsNullOrEmpty(word)) { words.Add(word); }
            }
        }
    }

    public bool ModerationCheck(string username)
    {
        foreach (string word in words)
        {
            if (username.Contains(word, System.StringComparison.OrdinalIgnoreCase))
            {
                /*
                foreach (string excludedWord in excludedWords) 
                {
                    if (excludedWord.Contains(username, System.StringComparison.OrdinalIgnoreCase) || username.Contains(excludedWord, System.StringComparison.OrdinalIgnoreCase)) { return true; }
                }
                */
                Debug.Log($"Bad Word {word} found in username {username}");
                return false; 
            }
            wordCounter++;
            Debug.Log(wordCounter);
        }
        return true;
    }
}