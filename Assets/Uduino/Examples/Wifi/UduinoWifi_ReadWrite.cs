using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class UduinoWifi_ReadWrite : MonoBehaviour {

    bool s = false;
    public float count = 0;
    public  int last = 0;

    int prevC = 0;
    int lost = 0;

    private void Update()
    {
        if (s)
        {
            count += Time.deltaTime;
            if(count > 10.0f)
            {
                Debug.Log("received " + last);
                Debug.Log("prevC " + prevC);
                Debug.Log("lost " + lost);
                Destroy(this);
            }
        }
    }


    public void Received(string data, UduinoDevice u)
    {
        if (s == false)
        {
            s = true;
        }
        Debug.Log(data);
        int d = int.Parse(data);

        lost += d - prevC - 1;
                    prevC = d;

        last++;
       // Debug.Log(data + " " + System.DateTime.UtcNow.Ticks);
    }
}
