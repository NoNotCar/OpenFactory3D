using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPlane : MonoBehaviour {
    private Renderer r;
	// Use this for initialization
	void Start () {
        var floor = GameObject.Find("Floor");
        transform.localScale = floor.transform.localScale;
        transform.position = floor.transform.position;
        r = GetComponent<Renderer>();
        r.enabled = false;
	}
	
	public void Move(int delta)
    {
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(-0.5f,transform.position.y+delta,8.5f), transform.position.z);
        if (transform.position.y == -0.5f)
        {
            r.enabled = false;
        }else
        {
            r.enabled = true;
        }
    }
}
