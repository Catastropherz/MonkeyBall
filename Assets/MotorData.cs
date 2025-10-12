using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMotorDataAsset", menuName = "Q/Motor Data", order = 1)]
public class MotorData : ScriptableObject
{
    public float m_MoveSpeed = 12.0f;
}
