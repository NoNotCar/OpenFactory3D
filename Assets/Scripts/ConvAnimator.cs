using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvAnimator : MonoBehaviour {
    private Material gmat;
    private State engine;
	// Use this for initialization
	void Start () {
        gmat=GetComponent<Renderer>().material;
        engine = GameObject.FindGameObjectWithTag("Engine").GetComponent<State>();
	}
	
	// Update is called once per frame
	void Update () {
        gmat.mainTextureOffset = new Vector2(0, engine.cycle);
	}
}
