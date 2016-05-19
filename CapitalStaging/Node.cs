namespace CapitalStaging
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public struct Node : IEquatable<Node>, IComparable<Node>
    {
        public override bool Equals(object obj)
        {
            return obj is Node && Equals((Node)obj);
        }

        public static Node Empty = new Node(-5000, -5000);

        public readonly short X;
        public readonly short Y;
        public readonly int Key;

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public Node(short x, short y)
        {
            X = x;
            Y = y;

            uint hash = 0;

            uint xVal = (uint)Math.Abs(x) & 0xFFF;
            uint yVal = (uint)Math.Abs(y) & 0xFFF;

            yVal = yVal << 12;

            hash |= xVal;
            hash |= yVal;

            if (x < 0) hash |= 1 << 25;
            if (y < 0) hash |= 1 << 26;
            Key = (int)hash;
        }

        public bool Equals(Node other)
        {
            return X == other.X && Y == other.Y;
        }

        public static bool operator ==(Node a, Node b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Node a, Node b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public int CompareTo(Node other)
        {
            if (Key < other.Key)
                return -1;

            if (Key > other.Key)
                return 1;

            return 0;
        }

        public override int GetHashCode()
        {
            return Key;
        }

        public override string ToString()
        {
            return string.Format("({0:0000}:{1:0000})", X, Y);
        }
    }
}