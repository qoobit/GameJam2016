using UnityEngine;
using System.Collections;

public class ScrewPlatform : MonoBehaviour {
    Animator animator;
    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        float offset = Random.Range(0.0f, 2.0f);
        StartCoroutine(StartIdling(offset));
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator StartIdling(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if(animator) animator.CrossFade("Idle",0f);
    }
    
}
