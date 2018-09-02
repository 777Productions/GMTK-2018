using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLabel : MonoBehaviour {

    public float lifetime = 2.0f;

    private float startTime;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - startTime > lifetime)
        {
            Destroy(gameObject);
        }
	}
}
