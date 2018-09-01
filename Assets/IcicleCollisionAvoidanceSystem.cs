using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleCollisionAvoidanceSystem : MonoBehaviour {

    public GameObject warningLabelPrefab;

    private List<GameObject> warningLabels = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Icicle>())
        {
            float xPos = collision.transform.position.x;
            Vector2 position = new Vector2(xPos, transform.position.y);

            GameObject newWarning = Instantiate(warningLabelPrefab, position, Quaternion.identity, transform);
            warningLabels.Add(newWarning);
        }
    }


}
