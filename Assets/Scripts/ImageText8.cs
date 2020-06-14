using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ImageText8 : MonoBehaviour
{
    // coordinate for the first selected image
    int selected1x = 0;
    int selected1y = 0;
    // coordinate for the second selected image
    int selected2x = 0;
    int selected2y = 0;
    // total score
    int score = 0;
    // number of consecutive perfect moves
    int perfect = 1;
    // to calculate the time for one successful deletion
    DateTime startTime;
    DateTime endTime;
    // to indicate if corresponding image is still existed/active
    bool[,] exist = new bool[7,6];
    // coodninate for the turn(s)
    int mid1x = -1;
    int mid1y = -1;
    int mid2x = -1;
    int mid2y = -1;
    
    // Start is called before the first frame update
    void Start()
    {
        // construct exist[][]
        for(int x = 0; x < 7; x++)
        {
            for(int y = 0; y < 6; y++)
            {
                if(x == 0 || x == 6 || y == 0 || y == 5)
                {
                    exist[x, y] = false;
                }
                else
                {
                    exist[x, y] = true;
                }
            }
        }
        
        // to generate 10 equations
        int[] adder1 = new int[10];
        int[] adder2 = new int[10];
        string[] result = new string[10];
        string[] leftEquation = new string[10];
        string s1, s2;
        
        for(int i = 0; i < 10; i++)
        {
            // randomly generating the adders
            // complete the generation of results and leftEquations
            adder1[i] = UnityEngine.Random.Range(1,13);
            adder2[i] = UnityEngine.Random.Range(1,13);
            result[i] = (adder1[i] * adder2[i]).ToString();
            s1 = adder1[i].ToString();
            s2 = adder2[i].ToString();
            leftEquation[i] = "\t" + s1 + " \n*  " + s2;
        }
        
        // randomly generate the map
        for(int i = 1; i <= 5; i++)
        {
            for(int j = 1; j <= 4; j++)
            {
                int temp;
                string textName;
                bool done = false;
                while(!done)
                {
                    temp = UnityEngine.Random.Range(-9, 11);
                    // in the case of non-positive number, text will get from leftEquation
                    if(temp <= 0 && leftEquation[0-temp] != null)
                    {
                        textName = "Text" + i.ToString() + j.ToString();
                        GameObject.Find(textName).GetComponent<Text>().text = leftEquation[0-temp];
                        leftEquation[0-temp] = null;
                        done = true;
                    }
                    // in the case of positive number, text will get from result
                    if(temp > 0 && result[temp-1] !=null)
                    {
                        textName = "Text" + i.ToString() + j.ToString();
                        GameObject.Find(textName).GetComponent<Text>().text = result[temp-1];
                        result[temp-1] = null;
                        done = true;
                    }
                }
            }
        }
        
        // check if there is any available move
        if(!ifAvailable())
        {
            resetMap();
        }
        
        // start message
        GameObject.Find("Message").GetComponent<Text>().text = "Go!";
        // set start time for the first deletion
        startTime = DateTime.Now;
    }
    
    // Update is called once per frame
    void Update()
    {
        // to check if the game is over
        if(numImagesLeft() == 0)
        {
            GameObject.Find("Message").GetComponent<Text>().text = "Good job!";
            int temp = PlayerPrefs.GetInt("score8");
            PlayerPrefs.SetInt("levelNum", 8);
            PlayerPrefs.SetInt("tempscore", score);
            if(temp < score)
            {
                PlayerPrefs.SetInt("ifNew", 1);
                PlayerPrefs.SetInt("score8", score);
            }
            else
            {
                PlayerPrefs.SetInt("ifNew", 0);
            }
            SceneManager.LoadScene("show");
        }
        
        // check if there is a available move
        if(!ifAvailable() && numImagesLeft() != 0)
        {
            GameObject.Find("Message").GetComponent<Text>().text = "No more available!";
            Invoke("resetMap", 0.5f);
            GameObject.Find("Message").GetComponent<Text>().text = "Keep going!";
        }
        
        // check how many images are currently selected
        int numSelected = checkNumSelected();
        // if there are two images being selected
        if(numSelected == 2)
        {
            // calculate the results on the images
            int result1 = calculateResult(selected1x, selected1y);
            int result2 = calculateResult(selected2x, selected2y);
            // generate the names to find corresponding images
            string name1 = "Image" + selected1x.ToString() + selected1y.ToString();
            string name2 = "Image" + selected2x.ToString() + selected2y.ToString();
            // if they are a match
            if(result1 == result2 && ifThereIsAPath(selected1x, selected1y, selected2x, selected2y))
            {
                // set the ending time for this match
                endTime = DateTime.Now;
                // calculate the time difference
                TimeSpan timeSpan = endTime - startTime;
                // less than 5 second: a perfect match
                // more points will be added if there are consecutive perfect matches
                if(timeSpan.Seconds <= 10)
                {
                    GameObject.Find("Message").GetComponent<Text>().text = "Perfect*" + perfect.ToString() + "!";;
                    score = score + 5 * perfect;
                    perfect++;
                }
                // less than 10: a great match
                else if(timeSpan.Seconds <= 15)
                {
                    GameObject.Find("Message").GetComponent<Text>().text = "Great!";
                    score = score + 3;
                    perfect = 1;
                }
                // less than 15: a cool match
                else if(timeSpan.Seconds <= 20)
                {
                    GameObject.Find("Message").GetComponent<Text>().text = "Cool!";
                    score = score + 2;
                    perfect = 1;
                }
                // other cases: a good match
                else
                {
                    GameObject.Find("Message").GetComponent<Text>().text = "Good!";
                    score = score + 1;
                    perfect = 1;
                }
                // change the score
                GameObject.Find("Score").GetComponent<Text>().text = "Score\n" + score.ToString() + ",000";
                
                // generate the name for text components
                string text1 = "Text" + selected1x.ToString() + selected1y.ToString();
                string text2 = "Text" + selected2x.ToString() + selected2y.ToString();
                // disable the images and texts
                GameObject.Find(name1).GetComponent<Image>().enabled = false;
                GameObject.Find(name2).GetComponent<Image>().enabled = false;
                GameObject.Find(text1).GetComponent<Text>().enabled = false;
                GameObject.Find(text2).GetComponent<Text>().enabled = false;
                // draw lines on the map
                drawLines();
                // disable in exist[][]
                exist[selected1x,selected1y] = false;
                exist[selected2x,selected2y] = false;
                // reset the start time for the next move
                startTime = DateTime.Now;
            }
            // a miss match
            else
            {
                // change the image backe to unselected background
                StartCoroutine(imageFlash(selected1x, selected1y, selected2x, selected2y));
                Sprite notSelectedSprite = Resources.Load<Sprite>("beforeSelected");
                GameObject.Find(name1).GetComponent<Image>().sprite = notSelectedSprite;
                GameObject.Find(name2).GetComponent<Image>().sprite = notSelectedSprite;
                // change the message
                GameObject.Find("Message").GetComponent<Text>().text = "Oh no!";
                perfect = 1;
            }
            
            // reset
            selected1x = 0;
            selected1y = 0;
            selected2x = 0;
            selected2y = 0;
            mid1x = -1;
            mid1y = -1;
            mid2x = -1;
            mid2y = -1;
        }
    }
    
    IEnumerator imageFlash(int x1, int y1, int x2, int y2)
    {
        string name1 = "Image" + x1.ToString() + y1.ToString();
        string name2 = "Image" + x2.ToString() + y2.ToString();
        GameObject.Find(name1).GetComponent<Image>().color = new Color32(212,205,205,255);
        GameObject.Find(name2).GetComponent<Image>().color = new Color32(212,205,205,255);
        yield return new WaitForSeconds(0.2f);
        GameObject.Find(name1).GetComponent<Image>().color = Color.white;
        GameObject.Find(name2).GetComponent<Image>().color = Color.white;
        yield return new WaitForSeconds(0.2f);
        GameObject.Find(name1).GetComponent<Image>().color = new Color32(212,205,205,255);
        GameObject.Find(name2).GetComponent<Image>().color = new Color32(212,205,205,255);
        yield return new WaitForSeconds(0.2f);
        GameObject.Find(name1).GetComponent<Image>().color = Color.white;
        GameObject.Find(name2).GetComponent<Image>().color = Color.white;
    }
    
    // draw line
    void drawLines()
    {
        GameObject line1 = new GameObject("line");
        if(ifStraightLine(selected1x, selected1y, selected2x, selected2y))
        {
            Vector3 pos0, pos1;
            pos0.x = (-2.75f + selected1y * 1.1f);
            pos0.y = (2.75f - selected1x * 1.1f);
            pos0.z = 0f;
            pos1.x = (-2.75f + selected2y * 1.1f);
            pos1.y = (2.75f - selected2x * 1.1f);
            pos1.z = 0f;
            line1.transform.position = pos0;
            line1.AddComponent<LineRenderer>();
            LineRenderer l1 = line1.GetComponent<LineRenderer>();
            l1.material.color = Color.black;
            l1.startWidth = 0.1f;
            l1.endWidth = 0.1f;
            l1.SetPosition(0, pos0);
            l1.SetPosition(1, pos1);
            GameObject.Destroy(line1, 0.3f);
        }
        else if(oneTurn(selected1x, selected1y, selected2x, selected2y))
        {
            Vector3 pos0, pos1, pos2;
            pos0.x = (-2.75f + selected1y * 1.1f);
            pos0.y = (2.75f - selected1x * 1.1f);
            pos0.z = 0f;
            pos1.x = (-2.75f + mid1y * 1.1f);
            pos1.y = (2.75f - mid1x * 1.1f);
            pos1.z = 0f;
            pos2.x = (-2.75f + selected2y * 1.1f);
            pos2.y = (2.75f - selected2x * 1.1f);
            pos2.z = 0f;
            line1.transform.position = pos0;
            line1.AddComponent<LineRenderer>();
            LineRenderer l1 = line1.GetComponent<LineRenderer>();
            l1.material.color = Color.black;
            l1.startWidth = 0.1f;
            l1.endWidth = 0.1f;
            l1.positionCount = 3;
            l1.SetPosition(0, pos0);
            l1.SetPosition(1, pos1);
            l1.SetPosition(2, pos2);
            GameObject.Destroy(line1, 0.3f);
        }
        else if(twoTurns(selected1x, selected1y, selected2x, selected2y))
        {
            Vector3 pos0, pos1, pos2, pos3;
            pos0.x = (-2.75f + selected1y * 1.1f);
            pos0.y = (2.75f - selected1x * 1.1f);
            pos0.z = 0f;
            pos3.x = (-2.75f + selected2y * 1.1f);
            pos3.y = (2.75f - selected2x * 1.1f);
            pos3.z = 0f;
            if(ifStraightLine(selected1x, selected1y, mid1x, mid1y))
            {
                pos1.x = (-2.75f + mid1y * 1.1f);
                pos1.y = (2.75f - mid1x * 1.1f);
                pos1.z = 0f;
                pos2.x = (-2.75f + mid2y * 1.1f);
                pos2.y = (2.75f - mid2x * 1.1f);
                pos2.z = 0f;
            }
            else
            {
                pos1.x = (-2.75f + mid2y * 1.1f);
                pos1.y = (2.75f - mid2x * 1.1f);
                pos1.z = 0f;
                pos2.x = (-2.75f + mid1y * 1.1f);
                pos2.y = (2.75f - mid1x * 1.1f);
                pos2.z = 0f;
            }
            line1.transform.position = pos0;
            line1.AddComponent<LineRenderer>();
            LineRenderer l1 = line1.GetComponent<LineRenderer>();
            l1.material.color = Color.black;
            l1.startWidth = 0.1f;
            l1.endWidth = 0.1f;
            l1.positionCount = 4;
            l1.SetPosition(0, pos0);
            l1.SetPosition(1, pos1);
            l1.SetPosition(2, pos2);
            l1.SetPosition(3, pos3);
            GameObject.Destroy(line1, 0.3f);
        }
    }
    
    // reset the map
    public void resetMap()
    {
        int l = numImagesLeft();
        string[] tempStr = new string[l];
        for(int x = 1; x < 6; x++)
        {
            for(int y = 1; y < 5; y++)
            {
                if(exist[x, y])
                {
                    string textName = "Text" + x.ToString() + y.ToString();
                    bool ifDone = false;
                    int z = 0;
                    while(!ifDone && z < l)
                    {
                        if(tempStr[z] == null)
                        {
                            tempStr[z] = GameObject.Find(textName).GetComponent<Text>().text;
                            ifDone = true;
                        }
                        z++;
                    }
                }
            }
        }
        
        for(int i = 1; i <= 5; i++)
        {
            for(int j = 1; j <= 4; j++)
            {
                if(exist[i,j])
                {
                    int temp;
                    string textName;
                    bool done = false;
                    while(!done)
                    {
                        temp = UnityEngine.Random.Range(0, l);
                        // in the case of non-positive number, text will get from leftEquation
                        if(tempStr[temp] != null)
                        {
                            textName = "Text" + i.ToString() + j.ToString();
                            GameObject.Find(textName).GetComponent<Text>().text = tempStr[temp];
                            tempStr[temp] = null;
                            done = true;
                        }
                    }
                }
            }
        }
    }
    
    bool ifAvailable()
    {
        for(int x = 1; x < 6; x++)
        {
            for(int y = 1; y < 5; y++)
            {
                if(exist[x,y])
                {
                    for(int m = y + 1; m < 5; m++)
                    {
                        if(exist[x, m])
                        {
                            int result1 = calculateResult(x, y);
                            int result2 = calculateResult(x, m);
                            if(ifThereIsAPath(x, y, x, m) && result1 == result2)
                            {
                                return true;
                            }
                        }
                    }
                    
                    for(int m = x + 1; m < 6; m++)
                    {
                        for(int n = 1; n < 5; n++)
                        {
                            if(exist[m, n])
                            {
                                int result1 = calculateResult(x, y);
                                int result2 = calculateResult(m, n);
                                if(ifThereIsAPath(x, y, m, n) && result1 == result2)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }
        
        return false;
    }
    
    bool ifThereIsAPath(int x1, int y1, int x2, int y2)
    {
        if(ifStraightLine(x1, y1, x2, y2))
        {
            return true;
        }
        
        if(oneTurn(x1, y1, x2, y2))
        {
            return true;
        }
        
        if(twoTurns(x1, y1, x2, y2))
        {
            return true;
        }
        
        return false;
    }
    
    bool twoTurns(int x1, int y1, int x2, int y2)
    {
        for(int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                if(i != x1 && i != x2 && j != y1 && j != y2)
                {
                    continue;
                }
                
                if((i == x1 && j == y1) || (i == x2 && j == y2))
                {
                    continue;
                }
                
                if(exist[i,j])
                {
                    continue;
                }
                
                if(oneTurn(x1, y1, i, j) && (ifHorizontal(i, j, x2, y2) || ifVertical(i, j, x2, y2)))
                {
                    mid2x = i;
                    mid2y = j;
                    return true;
                }
                
                if(oneTurn(i, j, x2, y2) && (ifHorizontal(x1, y1, i, j) || ifVertical(x1, y1, i, j)))
                {
                    mid2x = i;
                    mid2y = j;
                    return true;
                }
            }
        }
        
        return false;
    }
    
    bool oneTurn(int x1, int y1, int x2, int y2)
    {
        if(ifHorizontal(x1, y1, x1, y2) && ifVertical(x1, y2, x2, y2) && !exist[x1, y2])
        {
            mid1x = x1;
            mid1y = y2;
            return true;
        }
        
        if(ifVertical(x1, y1, x2, y1) && ifHorizontal(x2, y1, x2, y2) && !exist[x2, y1])
        {
            mid1x = x2;
            mid1y = y1;
            return true;
        }
        
        return false;
    }
    
    bool ifHorizontal(int x1, int y1, int x2, int y2)
    {
        if(x1 == x2)
        {
            if(y1 == (y2+1) || y2 == (y1+1))
            {
                return true;
            }
            else
            {
                int startY = Math.Min(y1,y2);
                int endY = Math.Max(y1,y2);
                for(int i = (startY+1); i < endY; i++)
                {
                    if(exist[x1,i] ==  true)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        
        return false;
    }
    
    bool ifVertical(int x1, int y1, int x2, int y2)
    {
        if(y1 == y2)
        {
            if(x1 == (x2+1) || x2 == (x1+1))
            {
                return true;
            }
            else
            {
                int startX = Math.Min(x1,x2);
                int endX = Math.Max(x1,x2);
                for(int i = (startX+1); i < endX; i++)
                {
                    if(exist[i,y1] ==  true)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        
        return false;
    }
    
    bool ifStraightLine(int x1, int y1, int x2, int y2)
    {
        if(ifHorizontal(x1, y1, x2, y2))
        {
            return true;
        }
        
        if(ifVertical(x1, y1, x2, y2))
        {
            return true;
        }
        
        return false;
    }
    
    int calculateResult(int x, int y)
    {
        string objName = "Text" + x.ToString() + y.ToString();
        string text = GameObject.Find(objName).GetComponent<Text>().text;
        int length = text.Length;
        int position = 0;
        bool ifNegative = false;
        
        if(text[position] == '-')
        {
            ifNegative = true;
            position++;
        }
        
        int result1 = 0;
        int result2 = 0;
        bool ifAdd = false;
        bool ifSub = false;
        bool ifMul = false;
        bool ifDiv = false;
        
        while(position < length)
        {
            if(text[position] >= '0' && text[position] <= '9')
            {
                result1 = result1 * 10 + (int)(text[position] - '0');
            }
            if(text[position] == '+')
            {
                ifAdd = true;
                break;
            }
            if(text[position] == '-')
            {
                ifSub = true;
                break;
            }
            if(text[position] == '*')
            {
                ifMul = true;
                break;
            }
            if(text[position] == '/')
            {
                ifDiv = true;
                break;
            }
            position++;
        }
        
        while(position < length)
        {
            if(text[position] >= '0' && text[position] <= '9')
            {
                result2 = result2 * 10 + (int)(text[position] - '0');
            }
            position++;
        }
        
        if(ifAdd == true)
        {
            result1 = result1 + result2;
        }
        if(ifSub == true)
        {
            result1 = result1 - result2;
        }
        if(ifMul == true)
        {
            result1 = result1 * result2;
        }
        if(ifDiv == true)
        {
            result1 = result1 / result2;
        }
        
        if(ifNegative == true)
        {
            result1 = 0 - result1;
        }
        
        return result1;
    }
    
    int checkNumSelected()
    {
        int num = 0;
        for(int x = 1; x <= 5; x++)
        {
            for(int y = 1; y <= 4; y++)
            {
                if(exist[x, y])
                {
                    if(ifSelected(x,y))
                    {
                        num++;
                        if(num == 1)
                        {
                            selected1x = x;
                            selected1y = y;
                        }
                        if(num == 2)
                        {
                            selected2x = x;
                            selected2y = y;
                        }
                    }
                }
            }
        }
        
        return num;
    }
    
    bool ifSelected(int x, int y)
    {
        string selectedSprite = "afterSelected";
        string objName = "Image" + x.ToString() + y.ToString();
        
        if(GameObject.Find(objName).GetComponent<Image>().sprite.name == selectedSprite)
        {
            return true;
        }
        
        return false;
    }
    
    int numImagesLeft()
    {
        int num = 0;
        for(int x = 0; x < 7; x++)
        {
            for(int y = 0; y < 6; y++)
            {
                if(exist[x,y] == true){
                    num++;
                }
            }
        }
        
        return num;
    }
    
    public void backToMenu()
    {
        SceneManager.LoadScene("menu");
    }
}
