using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour {
    public Sprite icon;
    public BlockShape shape;
    public readonly IVector3[] dirs = new IVector3[] { IVector3.up, IVector3.right, IVector3.left, IVector3.forward, IVector3.back, IVector3.down };
    public List<Vector3> no_weld = new List<Vector3>();
    private IVector3 movedir = IVector3.zero;
    private IVector3 opos;
    public IVector3 original_pos;
    public IVector3 pos;
    private State engine;
    public void Start()
    {
        engine = GameObject.FindGameObjectWithTag("Engine").GetComponent<State>();
        pos = new IVector3(transform.position);
        shape = new BlockShape(this);
    }
    public void OnStart(State s)
    {
        foreach (var d in dirs)
        {
            if (!no_weld.Contains(d))
            {
                var tpos = d + pos;
                if (s.in_world(tpos))
                {
                    var tblock = s[tpos];
                    if (tblock != null && tblock.shape != null && !tblock.no_weld.Contains(-d) && tblock.shape != shape)
                    {
                        tblock.shape.Join(shape);
                    }
                }
                else if (pos.y==0)
                {
                    shape.stat = true;
                }
            }
        }
        original_pos = pos;
    }
    public void OnEnd(State s)
    {
        movedir = IVector3.zero;
        shape = new BlockShape(this);
    }
    public void Update()
    {
        if (movedir != IVector3.zero)
        {
            transform.position = engine.cycle * (Vector3)movedir + opos;
        }
    }
    public void OnCycleEnd(State s)
    {
        transform.position = pos;
        movedir = IVector3.zero;
    }
    public void Move(State s, IVector3 direction)
    {
        if (s[pos] == this)
        {
            s[pos] = null;
        }
        s[pos+direction] = this;
        opos = pos;
        pos += direction;
        movedir = direction;
        shape.moved = true;
    }
    public virtual void Add_Forces(State s) { }
}
public class BlockShape: object
{
    public bool stat;
    public bool moved=false;
    public List<BlockScript> components;
    public BlockShape(BlockScript un)
    {
        components = new List<BlockScript>() { un };
    }
    public BlockShape(List<BlockScript> comps)
    {
        components = comps;
    }
    public BlockShape Add(BlockScript to_add)
    {
        components.Add(to_add);
        to_add.shape = this;
        return this;
    }
    public BlockShape Join(BlockShape other)
    {
        foreach (var c in other.components)
        {
            Add(c);
        }
        if (other.stat)
        {
            stat = true;
        }
        return this;
    }
}