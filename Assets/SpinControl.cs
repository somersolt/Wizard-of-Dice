using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class RotatePos
{
    public static List<Vector3> dicesPosList = new List<Vector3>();

    static RotatePos()
    {
        dicesPosList.Add(new Vector3(150, 45, 120));  // One
        dicesPosList.Add(new Vector3(225, -5, 225));    // Two
        dicesPosList.Add(new Vector3(30, 125, -30));   // Three
        dicesPosList.Add(new Vector3(30, 120, 145));     // Four
        dicesPosList.Add(new Vector3(45, -10, -135));      // Five
        dicesPosList.Add(new Vector3(-30, 50, 55));     // Six
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
    public void DiceSpin(int count, Vector3 Pos, DiceMgr diceMgr, Action callback = null)
    {
        spinCount = count;
        spinCallback = callback;
        coroutine = StartCoroutine(Spin(Pos));
        diceMgr.coroutineList.Add(this);
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
