using System;
using UnityEngine;

[Serializable]
public class CascadeRow
{
    public float[] values;

    public CascadeRow(int axisCount)
    {
        values = new float[axisCount];
    }
}