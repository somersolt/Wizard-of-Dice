using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public static class RotatePos
{
    public static List<Vector3> posList = new List<Vector3>();

    static RotatePos()
    {
        posList.Add(new Vector3(180, 270, 0));  // One
        posList.Add(new Vector3(0, 180, 0));    // Two
        posList.Add(new Vector3(0, 90, 270));   // Three
        posList.Add(new Vector3(90, 0, 0));     // Four
        posList.Add(new Vector3(0, 0, 0));      // Five
        posList.Add(new Vector3(0, 90, 0));     // Six
    }
}

public class SpinControl : MonoBehaviour
{
    private int spinCount;
    private float spinDuration = 0.03f;
    private float stopsecond;
    private Vector3 spinAmount = new Vector3(50, 0, 50);

    public void DiceSpin(int count, Vector3 Pos)
    {
        spinCount = count;
        StartCoroutine(Spin(Pos));
    }

    IEnumerator Spin(Vector3 Pos)
    {
        for (int i = 0; i < spinCount; i++)
        {
            transform.Rotate(spinAmount);
            yield return new WaitForSeconds(spinDuration);
        }
        transform.rotation = Quaternion.Euler(Pos);
    }
        
}
