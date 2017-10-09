using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

namespace GameToolbox.Algorithms
{
	/// <summary>
	/// Class representing DNA for a genetic algorithm; a string of bytes, with helpers to make it random and to handle
	/// reproduction. The onus is on the user to interpret the bytes in the desired manner.
	/// </summary>
	[Serializable]
	public class DNA
	{
		protected static Random _random = new Random();
		protected byte[] _genes;
		private readonly byte[] _cast;

		/// <summary>
		/// The mutation probability used for DNA created using the + operator.
		/// </summary>
		public static double DefaultMutationProbability { get; set; }

		public DNA(int length)
		{
			_genes = new byte[length];
			_random.NextBytes(_genes);
			_cast = new byte[_genes.Length];
			_genes.CopyTo(_cast, 0);
		}

		public DNA(DNA parent)
			: this(parent, DefaultMutationProbability) { }

		public DNA(DNA parent, double mutationProbability)
		{
			_genes = (byte[])parent._genes.Clone();
			for (int i = 0; i < _genes.Length; i++)
			{
				if (_random.NextDouble() <= mutationProbability)
				{
					_genes[i] = (byte)_random.Next(0, 256);
				}
			}
			_cast = new byte[_genes.Length];
			_genes.CopyTo(_cast, 0);
		}

		/// <summary>
		/// Note that the newly constructed DNA will have the same length as parent1.
		/// </summary>
		/// <param name="parent1"></param>
		/// <param name="parent2"></param>
		public DNA(DNA parent1, DNA parent2)
			: this(parent1, parent2, DefaultMutationProbability) { }

		/// <summary>
		/// Note that the newly constructed DNA will have the same length as parent1.
		/// </summary>
		/// <param name="parent1"></param>
		/// <param name="parent2"></param>
		/// <param name="mutationProbability"></param>
		public DNA(DNA parent1, DNA parent2, double mutationProbability)
		{
			_genes = (byte[])parent1._genes.Clone();
			for (int i = 0; i < _genes.Length; i++)
			{
				if (_random.Next(0, 2) == 1)
					_genes[i] = parent2[i];
				if (_random.NextDouble() <= mutationProbability)
				{
					_genes[i] = (byte)_random.Next(0, 256);
				}
			}
			_cast = new byte[_genes.Length];
			_genes.CopyTo(_cast, 0);
		}

		public byte this[int gene]
		{
			get
			{
				return _genes[gene];
			}
		}

		public int Length
		{
			get
			{
				return _genes.Length;
			}
		}

		/// <summary>
		/// Note that the resulting DNA will have the same length as the left operand.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static DNA operator +(DNA left, DNA right)
		{
			return new DNA(left, right);
		}

		/// <summary>
		/// Implicit conversion to byte array, to help with using BitConverter.
		/// </summary>
		/// <param name="dna"></param>
		/// <returns></returns>
		public static implicit operator byte[](DNA dna)
		{
			return dna._cast;
		}
	}

	/// <summary>
	/// Class which allows a user-specified type to be used in a genetic algorithm. The type must have a default constructor.
	/// Only fields type byte, sbyte, or types which can be converted by BitConverter: bool, char, short, int, long, float, double,
	/// ushort, uint, or ulong will be stored in and affected by the DNA. Other types of fields may exist, however. Float and double
	/// types will be populated with values in the range of 0.0 to 1.0. In addition, fields of eligible types will be ignored by
	/// this class if they have the DefaultValue attribute applied.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public class DNA<T> : DNA
		where T : new()
	{
		private const byte _byteMaxOverTwo = byte.MaxValue / 2;
		private static int _length = 0;
		private static IList<FieldInfo> _fields
			= typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where((f) =>
			{ return SupportedField(f, typeof(T)); }).OrderBy(f => f.Name).ToList();

		private readonly T _cast;

		/// <summary>
		/// Constructor. Will throw an ArgumentException if T contains a field of an unsupported type.
		/// </summary>
		public DNA()
			:base(Len)
		{
			_cast = new T();
			int offset = 0;
			foreach (var field in _fields)
			{
				SetValue(field, _cast, this, offset);
				offset += SizeOf(field.FieldType);
			}
		}

		/// <summary>
		/// Will throw an ArgumentException if T contains a field of an unsupported type.
		/// </summary>
		/// <param name="parent"></param>
		public DNA(DNA<T> parent)
			: this(parent, DNA.DefaultMutationProbability) { }

		/// <summary>
		/// Will throw an ArgumentException if T contains a field of an unsupported type.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="mutationProbability"></param>
		public DNA(DNA<T> parent, double mutationProbability)
			: base(parent, mutationProbability)
		{
			_cast = new T();
			int offset = 0;
			foreach (var field in _fields)
			{
				SetValue(field, _cast, this, offset);
				offset += SizeOf(field.FieldType);
			}
		}

		/// <summary>
		/// Will throw an ArgumentException if T contains a field of an unsupported type.
		/// </summary>
		/// <param name="parent1"></param>
		/// <param name="parent2"></param>
		public DNA(DNA<T> parent1, DNA<T> parent2)
			: this(parent1, parent2, DNA.DefaultMutationProbability) { }

		/// <summary>
		/// Will throw an ArgumentException if T contains a field of an unsupported type.
		/// </summary>
		/// <param name="parent1"></param>
		/// <param name="parent2"></param>
		/// <param name="mutationProbability"></param>
		public DNA(DNA<T> parent1, DNA<T> parent2, double mutationProbability)
			: base(parent1, parent2, mutationProbability)
		{
			_cast = new T();
			int offset = 0;
			foreach (var field in _fields)
			{
				SetValue(field, _cast, this, offset);
				offset += SizeOf(field.FieldType);
			}
		}

		private static bool SupportedType(Type type)
		{
			return (type == typeof(byte) || type == typeof(sbyte) || type == typeof(bool) || type == typeof(char)
				|| type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(float)
				|| type == typeof(double) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong));
		}

		private static bool SupportedField(FieldInfo f, Type t)
		{
			if (!SupportedType(f.FieldType))
				return false;
			if ((f.Attributes & FieldAttributes.Literal) == FieldAttributes.Literal
				|| (f.Attributes & FieldAttributes.InitOnly) == FieldAttributes.InitOnly)
				return false;
			if (f.GetCustomAttributes(typeof(DefaultValueAttribute), true).Length > 0)
				return false;
			if ((f.Name[0] == '<')
				&& (t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any(p =>
					(f.Name.IndexOf("<" + p.Name + ">") == 0 && p.GetCustomAttributes(typeof(DefaultValueAttribute), true).Length > 0) )))
				return false;
			return true;
		}

		private static int SizeOf(Type type)
		{
			switch (type.Name)
			{
				case "Byte": return 1;
				case "SByte": return 1;
				case "Boolean": return BitConverter.GetBytes(true).Length;
				case "Char": return BitConverter.GetBytes('0').Length;
				case "Int16": return BitConverter.GetBytes((short)0).Length;
				case "Int32": return BitConverter.GetBytes((int)0).Length;
				case "Int64": return BitConverter.GetBytes((long)0).Length;
				case "Single": return BitConverter.GetBytes((uint)0).Length;
				case "Double": return BitConverter.GetBytes((ulong)0).Length;
				case "UInt16": return BitConverter.GetBytes((ushort)0).Length;
				case "UInt32": return BitConverter.GetBytes((uint)0).Length;
				case "UInt64": return BitConverter.GetBytes((ulong)0).Length;
				default: throw new ArgumentException("Attempted to get size of unsupported type: " + type.FullName);
			}
		}

		private static void SetValue(FieldInfo field, T item, DNA<T> dna, int offset)
		{
			switch (field.FieldType.Name)
			{
				case "Byte": field.SetValue(item, dna[offset]);
					break;
				case "SByte": field.SetValue(item, (sbyte)dna[offset]);
					break;
				case "Boolean": field.SetValue(item, dna[offset] < _byteMaxOverTwo);
					break;
				case "Char": field.SetValue(item, BitConverter.ToChar(dna, offset));
					break;
				case "Int16": field.SetValue(item, BitConverter.ToInt16(dna, offset));
					break;
				case "Int32": field.SetValue(item, BitConverter.ToInt32(dna, offset));
					break;
				case "Int64": field.SetValue(item, BitConverter.ToInt64(dna, offset));
					break;
				case "Single": field.SetValue(item, (float)BitConverter.ToUInt32(dna, offset) / uint.MaxValue);
					break;
				case "Double": field.SetValue(item, (double)BitConverter.ToUInt64(dna, offset) / ulong.MaxValue);
					break;
				case "UInt16": field.SetValue(item, BitConverter.ToUInt16(dna, offset));
					break;
				case "UInt32": field.SetValue(item, BitConverter.ToUInt32(dna, offset));
					break;
				case "UInt64": field.SetValue(item, BitConverter.ToUInt64(dna, offset));
					break;
				default: throw new ArgumentException("Attempted to set the value of a field of unsupported type: " + field.FieldType.FullName);
			}
		}

		private static int Len
		{
			get
			{
				if (_length == 0)
				{
					if (_fields.Any((f) => { return !SupportedType(f.FieldType);}))
						throw new ArgumentException("The type \"" + typeof(T).FullName + "\" contains a field that is not of a supported type."
							+ Environment.NewLine + "Supported types are: byte, sbyte, bool, char, short, int, long, float, double, ushort, uint, and ulong.");
					_length = _fields.Aggregate(0, (i, f) => { return i + SizeOf(f.FieldType); });
				}
				return _length;
			}
		}

		public static implicit operator T(DNA<T> dna)
		{
			return dna._cast;
		}
	}
}
