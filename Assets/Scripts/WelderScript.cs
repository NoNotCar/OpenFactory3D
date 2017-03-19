using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelderScript : BlockScript {

    public override IVector3[] weld_pos()
    {
        return new IVector3[]{ pos + new IVector3(transform.rotation * Vector3.right) };
    }
}
