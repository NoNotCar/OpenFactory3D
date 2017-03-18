using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeTexture : MonoBehaviour {
    public int size = 10;
	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().material.mainTextureScale = size * transform.lossyScale;
	}
	
}
