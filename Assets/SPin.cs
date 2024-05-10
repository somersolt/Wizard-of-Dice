using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SPin : MonoBehaviour
{
    public int repeat_i;
    private float spinsecond = 0.03f;
    public float stopsecond;
    public float x;
    public float y;
    public float z;
    Coroutine a;
    // Start is called before the first frame update
    void Start()
    {
        a = StartCoroutine(spindice());
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Space))
    //    {
    //        StopCoroutine(a);
    //        a =StartCoroutine(spindice());
            
    //    }
    //}

        
    IEnumerator spindice()
    {
        int i;
        for (i = 0; i <= repeat_i; i++)
        {
            transform.Rotate(new Vector3(50, 0, 50));
            yield return new WaitForSeconds(spinsecond);
        }
        transform.rotation = Quaternion.Euler(x, y, z);
    }
        
}
