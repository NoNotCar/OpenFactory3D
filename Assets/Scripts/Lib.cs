using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IVector3
{
    public int x = 0;
    public int y = 0;
    public int z = 0;
    public float magnitude { get
        {

            return ((Vector3)this).magnitude;
        }
    }
    public IVector3() { }
    public IVector3(Vector3 v)
    {
        v = v.Rounded();
        x = (int)v.x;
        y = (int)v.y;
        z = (int)v.z;
    }
    public IVector3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public bool adjacent(IVector3 other)
    {
        return (this - other).magnitude == 1;
    }
    public static IVector3 operator +(IVector3 i1, IVector3 i2)
    {
        return new IVector3(i1.x + i2.x, i1.y + i2.y, i1.z + i2.z);
    }
    public static IVector3 operator -(IVector3 i)
    {
        return new IVector3(-i.x, -i.y, -i.z);
    }
    public static IVector3 operator -(IVector3 i1, IVector3 i2)
    {
        return new IVector3(i1.x - i2.x, i1.y - i2.y, i1.z - i2.z);
    }
    public static implicit operator Vector3(IVector3 i)
    {
        return new Vector3(i.x, i.y, i.z);
    }
    public static bool operator ==(IVector3 i1, IVector3 i2)
    {
        return i1.x == i2.x && i1.y == i2.y && i1.z == i2.z;
    }
    public static bool operator !=(IVector3 i1, IVector3 i2)
    {
        return !(i1 == i2);
    }
    public override bool Equals(object obj)
    {
        if(obj is IVector3){
            return this == (IVector3)obj;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return x + y*100 + z*10000;
    }
    public static readonly IVector3 down = new IVector3(0, -1, 0);
    public static readonly IVector3 up = new IVector3(0, 1, 0);
    public static readonly IVector3 left = new IVector3(-1, 0, 0);
    public static readonly IVector3 right = new IVector3(1, 0, 0);
    public static readonly IVector3 back = new IVector3(0, 0, -1);
    public static readonly IVector3 forward = new IVector3(0, 0, 1);
    public static readonly IVector3 zero = new IVector3(0, 0, 0);
}
static class Integerizer
{
    public static Vector3 Rounded(this Vector3 v)
    {
        return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
    }
    public static void Round(this Vector3 v)
    {
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        v.z = Mathf.Round(v.z);

    }
}
