using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpValueTest : MonoBehaviour
{
    private Action[] actions;

	// Use this for initialization

	void Start () {
		actions = new Action[10];
        for (int i = 0; i < 10; i++)
        {
            actions[i] = () => { Debug.Log(i); };
        }

        foreach (var action in actions)
        {
            action.Invoke();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
