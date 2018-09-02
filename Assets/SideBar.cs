using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideBar : MonoBehaviour {

    public RectTransform indicator;

    public float minY, maxY;

    private float range;

    private float rectHeight;

	// Use this for initialization
	void Start () {
        range = maxY - minY;
        rectHeight = GetComponent<RectTransform>().rect.height;
        Debug.Log(rectHeight);

    }
	
	// Update is called once per frame
	void Update () {
        float completion = (Camera.main.transform.position.y - minY) / range;

        indicator.anchoredPosition = new Vector2(indicator.anchoredPosition.x, completion * rectHeight);
    }
}
