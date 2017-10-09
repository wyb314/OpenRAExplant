using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GameToolbox.DataStructures
{
	/// <summary>
	/// Implements the Flyweight pattern, acting as a pool of shared instances of each qualifying subclass of T
	/// (and the main T class). When instantiated, by default creates an instance of T and one of each of its
	/// subclasses which exists in any currently-loaded assembly. These instances are then shared when an action
	/// of the that type is retrieved.
	/// </summary>
	public class FlyweightPool<T>
	{
		protected Dictionary<Type, T> _instances = new Dictionary<Type, T>();

		/// <summary>
		/// Constructor. Creates an instance of T and each subclass of T available in currently-loaded assemblies.
		/// </summary>
		public FlyweightPool()
			:this(true)
		{
		}

		/// <summary>
		/// Constructor. Allows the user to specify that class instances should not be preloaded.
		/// </summary>
		/// <param name="preload"></param>
		public FlyweightPool(bool preload)
		{
			if (!preload)
				return;
			Type typeT = typeof(T);
			if (typeT.IsPublic && !typeT.IsAbstract
				&& typeT.GetConstructor(Type.EmptyTypes) != null && typeT.GetConstructor(Type.EmptyTypes).IsPublic)
				_instances[typeT] = (T)typeT.GetConstructor(Type.EmptyTypes).Invoke(null);

			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					var types =
						from t in assembly.GetTypes()
						where t.IsPublic && t.IsSubclassOf(typeT) && !t.IsAbstract
							&& t.GetConstructor(Type.EmptyTypes) != null && t.GetConstructor(Type.EmptyTypes).IsPublic
						select t;
					foreach (Type type in types)
					{
						try
						{
							_instances[type] = (T)type.GetConstructor(Type.EmptyTypes).Invoke(null);
						}
						catch
						{
						}
					}
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// Gets a T of the given type from the pool.
		/// </summary>
		/// <typeparam name="U">The type of T to get.</typeparam>
		/// <returns>A T instance of the given type.</returns>
		public U Get<U>()
			where U : T, new()
		{
			if (!_instances.ContainsKey(typeof(U)))
				_instances[typeof(U)] = new U();
			return (U)_instances[typeof(U)];
		}
	}
}
