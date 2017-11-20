using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IntVector3{

	public static IntVector3 Zero = new IntVector3(0, 0, 0);

	public int x;
	public int y;
	public int z;

	public IntVector3(int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public int SqrMagnitude()
	{
		return x * x + y * y + z * z;
	}

	public static bool operator == (IntVector3 lhs, IntVector3 rhs)
	{
		return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
	}

	public static bool operator != (IntVector3 lhs, IntVector3 rhs)
	{
		return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
	}

	public override bool Equals (object obj)
	{
		if (!(obj is IntVector3))
			return 	false;
		IntVector3 other = (IntVector3)obj;
		return this == other;
	}

	public static IntVector3 operator + (IntVector3 lhs, IntVector3 rhs)
	{
		IntVector3 ret = lhs;
		ret.x += rhs.x;
		ret.y += rhs.y;
		ret.z += rhs.z;
		return ret;
	}

	public static IntVector3 operator / (IntVector3 lhs, IntVector3 rhs)
	{
		IntVector3 ret = lhs;
		ret.x = lhs.x / rhs.x;
		ret.y = lhs.y / rhs.y;
		ret.z = lhs.z / rhs.z;
		return ret;
	}

	public static IntVector3 operator * (IntVector3 lhs, IntVector3 rhs)
	{
		IntVector3 ret = lhs;
		ret.x *= rhs.x;
		ret.y *= rhs.y;
		ret.z *= rhs.z;
		return ret;
	}

	public static IntVector3 operator - (IntVector3 lhs, IntVector3 rhs)
	{
		IntVector3 ret = lhs;
		ret.x -= rhs.x;
		ret.y -= rhs.y;
		ret.z -= rhs.z;
		return ret;
	}

	public override int GetHashCode ()
	{
		return (x << 20) | (y << 10) | z;
	}

	public override string ToString ()
	{
		return string.Format ("[{0:D}, {1:D}], {2:D}]", x, y, z);
	}
}
