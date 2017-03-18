using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvScript : BlockScript {
    public override void Add_Forces(State s)
    {
        s.Add_Force(pos + IVector3.up, new Force(2,new IVector3(transform.rotation*Vector3.right)));
    }
}
