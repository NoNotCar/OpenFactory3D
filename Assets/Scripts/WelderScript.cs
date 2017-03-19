using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WelderScript : BlockScript
{
    public WelderScript()
    {
        factory = bs => new Welder(bs);
    }
}
public class Welder : Block {
    public Welder(BlockScript bs) : base(bs) { }
    public override IVector3[] weld_pos(State s)
    {
        return new IVector3[]{ pos + new IVector3(bs.transform.rotation * Vector3.right) };
    }
}
