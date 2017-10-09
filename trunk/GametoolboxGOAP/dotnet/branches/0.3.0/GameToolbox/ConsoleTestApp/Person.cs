using System;
using System.ComponentModel;

namespace ConsoleTestApp
{
	public enum HairColor { Brown, Blonde, Red };
	public enum EyeColor { Brown, Green, Blue };
	public enum Gender { Male, Female };

	public class Person
	{
		private const byte _byteMaxOverThree = byte.MaxValue / 3;
		private const double _weightVariance = 200;
		private const double _weightMinimum = 90;
		private const double _heightVariance = 2.7;
		private const double _heightMinimum = 4.3;

		private byte _hair;
		private byte _eyes;
		private bool _gender;
		private double _height;
		private double _weight;

		public HairColor HairColor
		{
			get
			{
				if (_hair < _byteMaxOverThree)
					return HairColor.Brown;
				else if (_hair < _byteMaxOverThree * 2)
					return HairColor.Blonde;
				return HairColor.Red;
			}
			set
			{
				switch (value)
				{
					case HairColor.Brown: _hair = _byteMaxOverThree - 1;
						break;
					case HairColor.Blonde: _hair = _byteMaxOverThree * 2 - 1;
						break;
					case HairColor.Red: _hair = byte.MaxValue;
						break;
					default: _hair = 0;
						break;
				}
			}
		}

		public EyeColor EyeColor
		{
			get
			{
				if (_eyes < _byteMaxOverThree)
					return EyeColor.Brown;
				else if (_eyes < _byteMaxOverThree * 2)
					return EyeColor.Green;
				return EyeColor.Blue;
			}
			set
			{
				switch (value)
				{
					case EyeColor.Brown: _eyes = _byteMaxOverThree - 1;
						break;
					case EyeColor.Green: _eyes = _byteMaxOverThree * 2 - 1;
						break;
					case EyeColor.Blue: _eyes = byte.MaxValue;
						break;
					default: _eyes = 0;
						break;
				}
			}
		}

		public Gender Gender
		{
			get { return _gender ? Gender.Male : Gender.Female; }
			set { _gender = value == Gender.Male; }
		}

		public double Height
		{
			get { return _height * _heightVariance + _heightMinimum; }
			set { _height = (value - _heightMinimum) / _heightVariance; }
		}

		public double Weight
		{
			get { return _weight * _weightVariance + _weightMinimum; }
			set { _weight = (value - _weightMinimum) / _weightVariance; }
		}
	}
}
