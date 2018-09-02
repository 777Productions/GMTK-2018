using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icicle : MonoBehaviour {

    public AudioClip audioClip;
    public AudioClip[] impacts;

    private AudioSource audioSource;


	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

        if (playerController)
        {
            PlayImpact();
            playerController.Die();
        }
    }

    public void Swoosh()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private void PlayImpact()
    {
        audioSource.clip = impacts[Random.Range(0, impacts.Length)];
        audioSource.Play();
    }
}
