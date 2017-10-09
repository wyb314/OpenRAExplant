using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GameToolbox
{
	public static class TypeHelper
	{
		/// <summary>
		/// Finds and returns the Type object for the type with the given full (namespace-qualified) name,
		/// if it is in any currently-loaded assembly. For generic types, this just calls Type.GetType().
		/// </summary>
		/// <param name="typeName">The full (namespace-qualified) name of the type to find.</param>
		/// <returns></returns>
		public static Type FindType(string typeName)
		{
			if(Regex.IsMatch(typeName, @"\`") && Regex.IsMatch(typeName, @"\["))
				return Type.GetType(typeName);
			return
				(from a in AppDomain.CurrentDomain.GetAssemblies()
				 from t in a.GetTypes()
				 where t.FullName == typeName
				 select t).FirstOrDefault();
		}
	}
}
