using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class tutorial3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void backToMenu()
    {
        SceneManager.LoadScene("menu");
    }
    
    public void backScene()
    {
        SceneManager.LoadScene("tutorial2");
    }
    
    public void nextScene()
    {
        SceneManager.LoadScene("tutorial4");
    }
}
