using System.Numerics;
using System.Text;
namespace ModMenuCS2
{
	public class Entity
	{

		public IntPtr Address { get; set; }
		public Vector3 Position { get; set; } // ok
		public Vector3 ViewOffset { get; set; } // ok
		public Vector2 Position2D { get; set; } // ok
		public Vector2 viewPosition2D { get; set; } // ok
		public Vector3 ViewAngles { get; set; }
		public int Team { get; set; } // ok
		public int Health { get; set; }
		public int ID { get; set; }
		public float Distance { get; set; }
		public override string ToString()
		{
			StringBuilder stringBuilder = new();
			stringBuilder.Append("Id: " + ID);
			stringBuilder.Append(" Endereço: " + Address);
			stringBuilder.Append(" Posicao: " + Position);
			stringBuilder.Append(" Team: " + Team);
			stringBuilder.Append(" Health: " + Health);
			stringBuilder.Append(" Distance: " + Distance);
			return stringBuilder.ToString();

		}
	}
	public class ViewMatrix
	{
		public float m11, m12, m13, m14;
		public float m21, m22, m23, m24;
		public float m31, m32, m33, m34;
		public float m41, m42, m43, m44;

		public override string ToString()
		{
			return $"{m11} {m12} {m13} {m14}\n" +
				   $"{m21} {m22} {m23} {m24}\n" +
				   $"{m31} {m32} {m33} {m34}\n" +
				   $"{m41} {m42} {m43} {m44}";
		}
	}

}