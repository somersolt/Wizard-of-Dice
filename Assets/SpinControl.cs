using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class RotatePos
{
    public static List<Vector3> posList = new List<Vector3>();

    static RotatePos()
    {
        //posList.Add(new Vector3(180, 270, 0));  // One
        //posList.Add(new Vector3(0, 180, 0));    // Two
        //posList.Add(new Vector3(0, 90, 270));   // Three
        //posList.Add(new Vector3(90, 0, 0));     // Four
        //posList.Add(new Vector3(0, 0, 0));      // Five
        //posList.Add(new Vector3(0, 90, 0));     // Six

        posList.Add(new Vector3(150, 45, 120));  // One
        posList.Add(new Vector3(225, -5, 225));    // Two
        posList.Add(new Vector3(30, 125, -30));   // Three
        posList.Add(new Vector3(30, 120, 145));     // Four
        posList.Add(new Vector3(45, -10, -135));      // Five
        posList.Add(new Vector3(-30, 50, 55));     // Six
    }
}

public class SpinControl : MonoBehaviour
{
    private int spinCount;
    private float spinDuration = 0.03f;
    private float stopsecond;
    private Vector3 spinAmount = new Vector3(50, 0, 50);
    private Action spinCallback;
    public Coroutine coroutine;
    public void DiceSpin(int count, Vector3 Pos, Action callback = null)
    {
        spinCount = count;
        spinCallback = callback;
        coroutine = StartCoroutine(Spin(Pos));
        DiceMgr.Instance.coroutineList.Add(this);
    }

    IEnumerator Spin(Vector3 Pos)
    {
        for (int i = 0; i < spinCount; i++)
        {
            transform.Rotate(spinAmount);
            yield return new WaitForSeconds(spinDuration);
        }
        transform.rotation = Quaternion.Euler(Pos);

        spinCallback?.Invoke();
    }
        
}
