using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class HighestScores : MonoBehaviour
{
    int[] scores = new int[10];
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 1; i < 11; i++)
        {
            int score = PlayerPrefs.GetInt("score" + i.ToString());
            string name = "Level" + i.ToString() + "Score";
            if(score == 0)
            {
                GameObject.Find(name).GetComponent<Text>().text = "0";
            }
            else
            {
                GameObject.Find(name).GetComponent<Text>().text = score.ToString() + ",000";
        
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void backToMenu()
    {
        SceneManager.LoadScene("menu");
    }
}
