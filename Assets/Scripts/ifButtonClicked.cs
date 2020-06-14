using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class ifButtonClicked : MonoBehaviour
{
    bool ifClicked = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Button>().onClick.AddListener(isClicked);
    }
    
    void isClicked()
    {
        if(!ifClicked)
        {
            print("I am in isClicked!");
            ifClicked = true;
            
        }
    }
    
    public bool getIfClicked()
    {
        return ifClicked;
    }
    
    public void resetBack()
    {
        if(ifClicked)
        {
            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
            ifClicked = false;
        }
        print("I am in resetBack()");
    }
}
