using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    bool[] ifLocked = new bool[10];
    
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 10; i++)
        {
            ifLocked[i] = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ifLocked[0] = false;
        for(int i = 1; i < 10; i++)
        {
            if(PlayerPrefs.GetInt("score" + i.ToString()) != 0)
            {
                ifLocked[i] = false;
            }
        }
        
        Sprite start = Resources.Load<Sprite>("start");
        for(int i = 0; i < 10; i++)
        {
            if(ifLocked[i] == false)
            {
                string name = "level" + (i+1).ToString();
                GameObject.Find(name).GetComponent<Image>().sprite = start;
            }
        }
    }
    
    public void selectlevel(int levelNumber)
    {
        if(!ifLocked[levelNumber -1])
        {
            PlayerPrefs.SetInt("levelNum", levelNumber);
            SceneManager.LoadScene("loading");
        }
    }
    
    public void randomLowLevel()
    {
        while(true)
        {
            int levelNum = UnityEngine.Random.Range(1,6);
            if(!ifLocked[levelNum-1])
            {
                PlayerPrefs.SetInt("levelNum", levelNum);
                SceneManager.LoadScene("loading");
                break;
            }
        }
    }
    
    public void randomHighLevel()
    {
        bool exist = false;
        for(int i = 5; i < 10; i++)
        {
            if(!ifLocked[i])
            {
                exist = true;
            }
        }
        
        while(exist)
        {
            int levelNum = UnityEngine.Random.Range(6,11);
            if(!ifLocked[levelNum-1])
            {
                PlayerPrefs.SetInt("levelNum", levelNum);
                SceneManager.LoadScene("loading");
                break;
            }
        }
    }
    
    public void loadScore()
    {
        SceneManager.LoadScene("score");
    }
    
    public void loadTutorial()
    {
        SceneManager.LoadScene("tutorial1");
    }
}
