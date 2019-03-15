using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTest : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        yield return  new WaitForSeconds(1);
        Debug.Log("co end");
    }
	
}
