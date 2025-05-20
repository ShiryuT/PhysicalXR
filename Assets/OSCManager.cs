using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public int touch;
    public int slope;

    public void getTouch(int val){
        touch=val;
    }

    public void getSlope(int val){
        slope=val;
    }
}

