using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockScript : MonoBehaviour {
    public Sprite icon;
    public Block block;
    public Func<BlockScript, Block> factory = b => new Block(b);
    public List<Vector3> no_weld = new List<Vector3>();
    public bool indest = false;
    private State engine;
    public void Awake()
    {
        block = factory(this);
    }
    public void Start()
    {
        engine = GameObject.FindGameObjectWithTag("Engine").GetComponent<State>();
    }
    public void Update()
    {
        if (block.movedir != IVector3.zero)
        {
            transform.position = engine.cycle * (Vector3)block.movedir + block.opos;
        }
    }
}
public class Block
{
    public IVector3 pos;
    public IVector3 original_pos;
    public IVector3 opos;
    public IVector3 movedir= IVector3.zero;
    public BlockShape shape;
    public bool indest;
    public List<IVector3> no_weld;
    public readonly IVector3[] dirs = new IVector3[] { IVector3.up, IVector3.right, IVector3.left, IVector3.forward, IVector3.back, IVector3.down };
    public BlockScript bs;
    public Block(BlockScript bs)
    {
        pos = new IVector3(bs.transform.position);
        indest = bs.indest;
        no_weld = (from d in bs.no_weld select new IVector3(bs.transform.rotation * d)).ToList();
        shape = new BlockShape(this);
        this.bs = bs;
    }
    public Block(IVector3 pos)
    {
        this.pos = pos;
        indest = true;
        no_weld = new List<IVector3>();
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
            }
        }
        original_pos = pos;
    }
    public void OnEnd(State s)
    {
        movedir = IVector3.zero;
        shape = new BlockShape(this);
    }
    public void OnCycleEnd(State s)
    {
        if (bs != null)
        {
            bs.transform.position = pos;
        }
        movedir = IVector3.zero;
    }
    public void Move(State s, IVector3 direction)
    {
        if (s[pos] == this)
        {
            s[pos] = null;
        }
        s[pos + direction] = this;
        opos = pos;
        pos += direction;
        movedir = direction;
        shape.moved = true;
    }
    public virtual void Add_Forces(State s) {}
    public virtual IVector3[] weld_pos(State s) { return new IVector3[0]; }
}
public class BlockShape: object
{
    public bool stat;
    public bool moved=false;
    public List<Block> components;
    public BlockShape(Block un)
    {
        components = new List<Block>() { un };
        stat = un.indest;
    }
    public BlockShape(List<Block> comps)
    {
        components = comps;
    }
    public BlockShape Add(Block to_add)
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