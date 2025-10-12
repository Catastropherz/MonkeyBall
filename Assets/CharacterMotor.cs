using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotor : MonoBehaviour
{
    public MotorData m_Data;
    public string m_ObjectName;

    public void Update()
    {
        // Move the character based on input
        if (Input.GetKey(KeyCode.A))
        { 
            transform.position -= Vector3.right * m_Data.m_MoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * m_Data.m_MoveSpeed * Time.deltaTime;
        }
    }
}
