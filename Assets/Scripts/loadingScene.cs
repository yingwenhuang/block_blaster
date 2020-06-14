using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class loadingScene : MonoBehaviour
{
    int levelNum;
    
    // Start is called before the first frame update
    void Start()
    {
        levelNum = PlayerPrefs.GetInt("levelNum");
    }
    
    // Update is called once per frame
    void Update()
    {
        Invoke("showMessage1", 1f);
        GameObject obj1 = GameObject.Find("Message1");
        GameObject.Destroy(obj1, 2f);
        
        if(!obj1)
        {
            Invoke("showMessage2", 1f);
            GameObject obj2 = GameObject.Find("Message2");
            GameObject.Destroy(obj2, 2f);
            
            if(!obj2)
            {
                string levelName = "level" + levelNum.ToString();
                SceneManager.LoadScene(levelName);
            }
        }
        
    }
    
    public void backToMenu()
    {
        SceneManager.LoadScene("menu");
    }
    
    void showMessage1()
    {
        GameObject.Find("Message1").GetComponent<Text>().text = "Level " + levelNum.ToString() + "!";
    }
    
    void showMessage2()
    {
        GameObject.Find("Message2").GetComponent<Text>().text = "Ready!";
    }
}
