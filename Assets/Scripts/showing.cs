using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class showing : MonoBehaviour
{
    int ifNew;
    int levelNum;
    // Start is called before the first frame update
    void Start()
    {
        ifNew = PlayerPrefs.GetInt("ifNew");
        levelNum = PlayerPrefs.GetInt("levelNum");
        
        if(ifNew == 0)
        {
            GameObject.Find("coin1").GetComponent<Image>().enabled = false;
            GameObject.Find("coin2").GetComponent<Image>().enabled = false;
            GameObject.Find("newScore").GetComponent<Text>().enabled = false;
        }
    
        int score = PlayerPrefs.GetInt("tempscore");
        string message = "Level " + levelNum.ToString() + "\n" + score.ToString() + ",000";
        GameObject.Find("Score").GetComponent<Text>().text = message;
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
