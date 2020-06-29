using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFlight : MonoBehaviour
{
    public float mainSpeed = 100;
    public float shiftAdd = 250;
    public float maxShift = 1000;
    public float sensitivity = 0.25f;
    Vector3 lastPos;
    private float totalRun;
    bool canFly = true;

    private void Update()
    {
        if (canFly)
        {
            lastPos = Input.mousePosition - lastPos;
            lastPos = new Vector3(-lastPos.y * sensitivity, lastPos.x * sensitivity, 0);
            lastPos = new Vector3(transform.eulerAngles.x + lastPos.x, transform.eulerAngles.y + lastPos.y, 0);
            transform.eulerAngles = lastPos;
            lastPos = Input.mousePosition;
            //Mouse  camera angle done.  
            var p = GetBaseInput();
            if (Input.GetKey(KeyCode.LeftShift))
            {
                totalRun += Time.deltaTime;
                p = p * totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            }
            else
            {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1, 1000);
                p = p * mainSpeed;
            }

            p = p * Time.deltaTime;


            transform.Translate(p);

        }

        if (Input.GetKey(KeyCode.Space))
        {
            canFly = !canFly;
        }
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity = new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity = new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity = new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity = new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }
}