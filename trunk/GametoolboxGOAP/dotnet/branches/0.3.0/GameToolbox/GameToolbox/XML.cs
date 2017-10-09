using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace GameToolbox.IO
{
	/// <summary>
	/// Static class which takes care of saving and loading objects in XML format. To use for a file type,
	/// create a class which contains public properties for whatever is desired to be in the file (both getters
	/// and setters must be public). Some types for properties will not work, such as Dictionary(TKey, TValue).
	/// Make sure the class has a default constructor (one which takes no parameters) that initializes all of
	/// its properties to default values (List(T) objects can be used, but should be initialized to contain no
	/// elements; otherwise the elements will be duplicated over multiple save/load operations). Any properties
	/// which don't meet these conditions should have the [XmlIgnore] attribute. If subclasses which may be
	/// put into the XML file have the same name (i.e. are in different namespaces), use the [XmlType("samplename")]
	/// attribute to differentiate the like-named types.
	/// 
	/// Note: You cannot serialize a subtype of the given type to XML using this. To serialize the subtype, use
	/// that subtype as the type parameter for this class. Also, this will not allow any type to be serialized by
	/// using the Object type. For this to work, Object cannot be the type of any properties or a type parameter
	/// for any generic types to be serialized.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class XML<T>
		where T : new()
	{
		private static XmlSerializer _xml = null;

		/// <summary>
		/// Prepares to be able to read and write data of type T in XML format.
		/// </summary>
		public static void Initialize()
		{
			if (_xml != null)
				return;
			IEnumerable<Type> allTypes = new List<Type>();
			try
			{
				allTypes =
					from a in AppDomain.CurrentDomain.GetAssemblies()
					from t in a.GetTypes()
					select t;
			}
			catch
			{
			}
			_xml = new XmlSerializer(typeof(T), allTypes.FindAllUsableSubtypes(typeof(T)).ToArray());
		}

		/// <summary>
		/// Checks whether the given file is a valid XML file for the given type. Returns true if the directory
		/// does not exist of the file does not exist as long as they are still valid directory and file names,
		/// since the Load function will still complete successfully in these circumstances (creating the file and
		/// directory if necessary).
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns>True if an object of the given type can be loaded from the file, otherwise false.</returns>
		public static bool IsValid(string fileName)
		{
			try
			{
				using (FileStream stream = new FileStream(fileName, FileMode.Open))
				{
					return IsValid(stream);
				}
			}
			catch (DirectoryNotFoundException)
			{
				if ((fileName.IndexOfAny(Path.GetInvalidPathChars()) < 0) && (Path.GetFileName(fileName).Length > 0)
					&& (Path.GetFileName(fileName).IndexOfAny(Path.GetInvalidFileNameChars()) < 0))
					return true;
			}
			catch (FileNotFoundException)
			{
				if ((fileName.IndexOfAny(Path.GetInvalidPathChars()) < 0) && (Path.GetFileName(fileName).Length > 0)
					&& (Path.GetFileName(fileName).IndexOfAny(Path.GetInvalidFileNameChars()) < 0))
					return true;
			}
			catch
			{
			}
			return false;
		}

		/// <summary>
		/// Checks whether the given stream is a valid XML format for the given type.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns>True if an object of the given type can be loaded from the stream, otherwise false.</returns>
		public static bool IsValid(Stream stream)
		{
			using (XmlTextReader reader = new XmlTextReader(stream))
			{
				return _xml.CanDeserialize(reader);
			}
		}

		/// <summary>
		/// Checks whether the given TextReader is a valid XML format for the given type.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns>True if an object of the given type can be loaded from the stream, otherwise false.</returns>
		public static bool IsValid(TextReader reader)
		{
			using (XmlTextReader rdr = new XmlTextReader(reader))
			{
				return _xml.CanDeserialize(rdr);
			}
		}

		/// <summary>
		/// Attempts to load an object of the given type from the given file. If the file does not
		/// exist, it is created with default values for the given type.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static T Load(string fileName)
		{
			T t = new T();
			try
			{
				using (FileStream stream = new FileStream(fileName, FileMode.Open))
				{
					t = Load(stream);
				}
			}
			catch
			{
			}
			Save(fileName, t);
			return t;
		}

		/// <summary>
		/// Attempts to load an object of the given type from the stream, and returns the result.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static T Load(Stream stream)
		{
			return (T)_xml.Deserialize(stream);
		}

		/// <summary>
		/// Attempts to load an object of the given type from the TextReader, and returns the result.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static T Load(TextReader reader)
		{
			return (T)_xml.Deserialize(reader);
		}

		/// <summary>
		/// Attempts to load an object of the given type from the XmlReader, and returns the result.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static T Load(XmlReader reader)
		{
			return (T)_xml.Deserialize(reader);
		}

		/// <summary>
		/// Attempts to save an object of the given type to the given file.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="obj"></param>
		/// <exception cref="IOException">The directory specified by <i>path</i> is read-only.</exception>
		/// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
		/// <exception cref="ArgumentException"><i>path</i> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="InvalidPathChars"/>.
		/// -or-
		/// path is prefixed with, or contains only a colon character (:).</exception>
		/// <exception cref="ArgumentNullException"><i>path</i> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.</exception>
		/// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
		/// <exception cref="NotSupportedException"><i>path</i> contains a colon character (:) that is not part of a drive label ("C:\").</exception>
		public static void Save(string fileName, T obj)
		{
			try
			{
				using (FileStream stream = new FileStream(fileName, FileMode.Create))
				{
					Save(stream, obj);
				}
			}
			catch (DirectoryNotFoundException)
			{
				Directory.CreateDirectory(Path.GetDirectoryName(fileName));
				Save(fileName, obj);
			}
		}

		/// <summary>
		/// Attempts to save the object as XML to the given stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="obj"></param>
		public static void Save(Stream stream, T obj)
		{
			_xml.Serialize(stream, obj);
		}

		/// <summary>
		/// Attempts to save the object as XML to the given TextWriter.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="obj"></param>
		public static void Save(TextWriter writer, T obj)
		{
			_xml.Serialize(writer, obj);
		}

		/// <summary>
		/// Attempts to save the object as XML to the given TextWriter.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="obj"></param>
		public static void Save(XmlWriter writer, T obj)
		{
			_xml.Serialize(writer, obj);
		}
	}

	internal static class XMLExtensions
	{

		/// <summary>
		/// Finds usable (public, non-abstract, and with a public default constructor) subtypes of the given type and all
		/// its type parameters.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static IEnumerable<Type> FindAllUsableSubtypes(this IEnumerable<Type> allTypes, Type type)
		{
			List<Type> subtypes = new List<Type>();

			if ((type == typeof(object)) || (type.GetInterface("IXMLSerializable") != null))
				return subtypes;

			if (type.IsGenericType)
			{
				foreach (Type typeParameter in type.GetGenericArguments())
				{
					if ((!typeParameter.IsGenericParameter) && (typeParameter != typeof(object))
						&& (typeParameter.GetInterface("IXmlSerializable", true) == null))
						subtypes.AddRange(allTypes.FindUsableSubtypes(typeParameter));
				}
			}
			foreach (Type interfaceType in type.GetInterfaces())
			{
				if (interfaceType.IsGenericType || interfaceType.IsGenericTypeDefinition)
				{
					foreach (Type typeParameter in interfaceType.GetGenericArguments())
					{
						if ((!typeParameter.IsGenericParameter) && (typeParameter != typeof(object))
							&& (typeParameter.GetInterface("IXmlSerializable", true) == null))
							subtypes.AddRange(allTypes.FindUsableSubtypes(typeParameter));
					}
				}
			}
			foreach (PropertyInfo property in type.GetProperties())
			{
				if (property.CanRead && property.CanWrite
					&& property.GetGetMethod().IsPublic && property.GetSetMethod().IsPublic)
				{
					subtypes.AddRange(allTypes.FindUsableSubtypes(property.PropertyType));
				}
			}

			subtypes.AddRange(allTypes.FindAllUsableSubtypes(type.BaseType));

			return subtypes.Distinct();
		}

		/// <summary>
		/// Finds usable (public, non-abstract, and with a public default constructor) subtypes of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static IEnumerable<Type> FindUsableSubtypes(this IEnumerable<Type> allTypes, Type type)
		{
			return
				from t in allTypes
				where t.IsPublic && t.IsSubclassOf(type) && !t.IsAbstract
					&& t.GetConstructor(Type.EmptyTypes) != null && t.GetConstructor(Type.EmptyTypes).IsPublic
				select t;
		}
	}
}
