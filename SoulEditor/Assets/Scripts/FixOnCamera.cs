using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixOnCamera : MonoBehaviour
{
    public bool fixX;
    public bool fixY;

    // Update is called once per frame
    void Update()
    {
        float changeX = transform.position.x, changeY=transform.position.y;
        if (fixX)
        {
            changeX = Camera.main.transform.position.x;
        }
        if (fixY)
        {
            changeY = Camera.main.transform.position.y;
        }
        transform.position = new Vector3(changeX,changeY,transform.position.z);
    }
}
