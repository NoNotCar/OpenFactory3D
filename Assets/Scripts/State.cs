using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class State: MonoBehaviour {
    public Vector3 size;
    public BlockScript[,,] blocks;
    public float cycle=0;
    public bool running;
    private Dictionary<BlockShape, List<Force>>[] fdict;
    private Force gforce = new Force(3, IVector3.down);
    public const int MAX_FORCE = 3;
    private void Start()
    {
        blocks = new BlockScript[(int)size.x, (int)size.y, (int)size.z];
        var floor = GameObject.Find("Floor");
        floor.transform.position = new Vector3(size.x / 2-0.5f, -0.5f, size.y / 2 - 0.5f);
    }
    public bool Spawn(GameObject prefab, IVector3 pos, Quaternion rot)
    {
        if (in_world(pos))
        {
            var newb = Instantiate(prefab, pos, rot);
            this[pos] = newb.GetComponent<BlockScript>();
            return true;
        }
        return false;
    }
    public bool Spawn(GameObject prefab, IVector3 pos)
    {
        return Spawn(prefab, pos, Quaternion.identity);
    }
    public void Dest(IVector3 pos)
    {
        var b = this[pos];
        if (b != null)
        {
            Destroy(b.gameObject);
            this[pos] = null;
        }
    }
    public void Dest(BlockScript block)
    {
        this[block.pos] = null;
        Destroy(block.gameObject);
    }
    public void Update()
    {
        if (running)
        {
            cycle += Time.deltaTime;
            if (cycle >= 1)
            {
                foreach (var b in realblocks().ToArray())
                {
                    b.OnCycleEnd(this);
                }
                Cycle();
            }
        }
    }
    public void StopStart()
    {
        running = !running;
        foreach (var b in realblocks().ToArray()) {
            if (running)
            {
                b.OnStart(this);
            }else
            {
                b.OnEnd(this);
            }
        }
        if (running)
        {
            Cycle();
        }
        else
        {
            var nblocks = new BlockScript[(int)size.x, (int)size.y, (int)size.z];
            foreach (var b in realblocks().ToArray())
            {
                var opos = b.original_pos;
                nblocks[opos.x, opos.y, opos.z] = b;
                b.transform.position = opos;
                b.pos = opos;
            }
            blocks = nblocks;
            cycle = 0;
        }
    }
    private void Cycle()
    {
        cycle = 0;
        fdict = new Dictionary<BlockShape, List<Force>>[MAX_FORCE+1];
        var gshapes = new List<BlockShape>();
        foreach (var b in realblocks())
        {
            b.shape.moved = false;
            b.Add_Forces(this);
            if (!b.shape.stat && !gshapes.Contains(b.shape))
            {
                gshapes.Add(b.shape);
                Add_Force(b.shape, gforce);
            }
        }
        var moves = new Dictionary<IVector3, IVector3>();
        foreach(var fd in fdict.Reverse())
        {
            if (fd != null)
            {
                foreach (var s in fd.Keys)
                {
                    var flist = fd[s];
                    if (flist.Count == 1 && can_push(s, flist[0], moves))
                    {
                        var f = flist[0];
                        foreach (var b in s.components)
                        {
                            moves.Add(b.pos, f.direction);
                        }
                        push(s, f.direction);
                    }
                }
            }
        }
        
    }
    public void Add_Force(BlockShape target, Force f)
    {
        if (target.stat) { return; }
        var fd = fdict[f.intensity];
        if(fd == null)
        {
            fdict[f.intensity] = new Dictionary<BlockShape, List<Force>>();
            fd = fdict[f.intensity];
        }
        if (fd.ContainsKey(target) && !fd[target].Contains(f))
        {
            fd[target].Add(f);
        }
        else
        {
            fd[target] = new List<Force>() { f };
        }
    }
    public void Add_Force(BlockScript target, Force f)
    {
        if (target != null)
        {
            Add_Force(target.shape, f);
        }
    }

    public void Add_Force(IVector3 pos, Force f)
    {
        if (in_world(pos))
        {
            Add_Force(this[pos], f);
        }
    }
    public bool in_world(Vector3 pos)
    {
        return 0 <= pos.x && pos.x < size.x && 0 <= pos.y && pos.y < size.y && 0 <= pos.z && pos.z < size.z;
    }
    public BlockScript this[IVector3 pos]
    {
        get
        {
            return blocks[pos.x, pos.y, pos.z];
        }
        set
        {
            blocks[pos.x, pos.y, pos.z] = value;
        }
    }
    public IEnumerable<BlockScript> realblocks()
    {
        foreach (var b in blocks)
        {
            if (b != null)
            {
                yield return b;
            }
        }

    }
    public bool can_push(BlockShape s, Force f, Dictionary<IVector3,IVector3> moves)
    {
        if (s.moved) { return false; }
        var moving = new List<BlockScript>();
        var mshapes = new List<BlockShape>() { s };
        moving.AddRange(s.components);
        for (var i = 0; i < moving.Count(); i++)
        {
            var b = moving[i];
            var tpos = b.pos + f.direction;
            if (!in_world(tpos) || (moves.ContainsKey(tpos) && moves[tpos]!=f.direction)) { return false; }
            var tb = this[tpos];
            if (tb != null && !mshapes.Contains(tb.shape))
            {
                if (tb.shape.stat || tb.shape.moved) {
                    return false;
                }
                if (fdict[f.intensity].ContainsKey(tb.shape)){
                    foreach (var nf in fdict[f.intensity][tb.shape])
                    {
                        if (nf.direction != f.direction)
                        {
                            return false;
                        }
                    }
                }
                moving.AddRange(tb.shape.components);
                mshapes.Add(tb.shape);
            }
            if (i > 1000)
            {
                throw new System.Exception("OH SNAP");
            }
        }
        return true;

    }
    public void push(BlockShape s, IVector3 dir)
    {
        var moving = new List<BlockScript>();
        var mshapes = new List<BlockShape>() { s };
        moving.AddRange(s.components);
        for (var i = 0; i < moving.Count(); i++)
        {
            var b = moving[i];
            var tpos = b.pos + dir;
            var tb = this[tpos];
            if (tb != null && !mshapes.Contains(tb.shape))
            {
                moving.AddRange(tb.shape.components);
                mshapes.Add(tb.shape);
            }
            if (i > 1000)
            {
                throw new System.Exception("OH SNAP!!!");
            }
        }
        foreach(var b in moving)
        {
            b.Move(this, dir);
        }
    }
}
public struct Force
{
    public int intensity;
    public IVector3 direction;
    public Force(int i, IVector3 dir)
    {
        intensity = i;
        direction = dir;
    }
}