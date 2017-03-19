using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboidBlocks : MonoBehaviour {
    public Vector3 delta_root;
    public Vector3 size;
	// Use this for initialization
	void Start () {
        var engine = GameObject.FindGameObjectWithTag("Engine").GetComponent<State>();
        IVector3 start = new IVector3(transform.position + delta_root);
        for (var x = start.x; x < start.x+size.x; x++)
        {
            for (var y = start.y; y < start.y + size.y; y++)
            {
                for (var z = start.z; z < start.z + size.z; z++)
                {
                    engine.Spawn(new Block(new IVector3(x, y, z)));
                }
            }
        }
    }
	
}
