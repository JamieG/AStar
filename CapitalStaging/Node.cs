namespace CapitalStaging
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    //public struct Node : IEquatable<Node>, IComparable<Node>
    //{
    //    public override bool Equals(object obj)
    //    {
    //        return obj is Node && Equals((Node)obj);
    //    }

    //    public static Node Empty = new Node(-5000, -5000);

    //    public readonly short X;
    //    public readonly short Y;
    //    public readonly int Key;

    //    public double F;

    //    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    //    public Node(short x, short y)
    //    {
    //        X = x;
    //        Y = y;
    //        F = 0;

    //        //int hash = 17;
    //        //hash = hash * 23 + X.GetHashCode();
    //        //hash = hash * 23 + Y.GetHashCode();
    //        Key = new {x, y}.GetHashCode();
    //    }

    //    public bool Equals(Node other)
    //    {
    //        return X == other.X && Y == other.Y;
    //    }

    //    public static bool operator ==(Node a, Node b)
    //    {
    //        return a.X == b.X && a.Y == b.Y;
    //    }

    //    public static bool operator !=(Node a, Node b)
    //    {
    //        return a.X != b.X || a.Y != b.Y;
    //    }

    //    public int CompareTo(Node other)
    //    {
    //        if (Key < other.Key)
    //            return -1;

    //        if (Key > other.Key)
    //            return 1;

    //        return 0;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return Key;
    //    }

    //    public override string ToString()
    //    {
    //        return string.Format("({0:0000}:{1:0000})", X, Y);
    //    }
    //}
}