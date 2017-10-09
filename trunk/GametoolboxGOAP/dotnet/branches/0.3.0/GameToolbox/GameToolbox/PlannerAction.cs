using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using GameToolbox.IO;
using GameToolbox.Reflection;

namespace GameToolbox.Planner
{
	/// <summary>
	/// An action for use with a planner.
	/// </summary>
	[Serializable]
	public abstract class PlannerAction
	{
		private string _name;
		private PlannerStateCondition _prerequisites = new PlannerStateCondition();
		private PlannerState _effects = new PlannerState();

		/// <summary>
		/// The name of this action.
		/// </summary>
		public string Name { get { return _name; } }

		/// <summary>
		/// The name of the action for display purposes.
		/// </summary>
		[XmlIgnore]
		public abstract string DisplayName { get; }

		/// <summary>
		/// Gets the cost of this action given the current state.
		/// </summary>
		/// <param name="currentState">The current planner state.</param>
		/// <param name="parameters">Parameters for this action.</param>
		public abstract double Cost(PlannerState currentState, params IPlannerStateSymbol[] parameters);

		/// <summary>
		/// Represents the symbols which must be set to a specific value in order for this action to be valid.
		/// </summary>
		[XmlIgnore]
		public PlannerStateCondition Prerequisites { get { return _prerequisites; } }

		/// <summary>
		/// Represents the symbols which are set to a specific value as a result of this action.
		/// </summary>
		[XmlIgnore]
		public PlannerState Effects { get { return _effects; } }

		/// <summary>
		/// Takes a PlannerState and performs any modifications which are not covered in the Effects state.
		/// </summary>
		/// <param name="currentState">The current planner state.</param>
		public virtual PlannerState SymbolicExecute(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			return currentState;
		}

		/// <summary>
		/// Takes a PlannerState and reverses all modifications made by SymbolicExecute.
		/// </summary>
		/// <param name="currentState">The current planner state.</param>
		/// <param name="parameters">Parameters for this action.</param>
		public virtual PlannerState SymbolicRevert(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			return currentState;
		}

		/// <summary>
		/// The names of the state symbols to use as parameters for this action. These are symbols which
		/// this action affects; i.e. symbols which this action sets, but which this action can set to
		/// whatever value is needed for the plan. The planner will use values for the parameters which will
		/// complete the plan. These parameters will be passed into this action's functions in the same order
		/// they appear in this property.
		/// 
		/// Example: a GoTo action would have as a parameter the symbol "Location". When the planner needs to
		/// solve for the "Location" symbol it will find the GoTo action and pass as its parameter the "Location"
		/// value it is attempting to reach. IsValid will be checked to see if the "Location" value can be reached
		/// from the current state, and Cost will be used to determine the cost of reaching that "Location" value
		/// from the current state.
		/// 
		/// Note: there is no need to include symbols here which this action wil not change, as they will already
		/// be passed to this action as part of the current state.
		/// </summary>
		public virtual IEnumerable<string> ParameterSymbols { get { yield break; } }

		/// <summary>
		/// Use this to specify symbols affected by this action which do not show up in the
		/// Effects state. These symbols must be affected in the SymbolicExecute and SymbolicReverse functions.
		/// </summary>
		public virtual IEnumerable<string> OtherAffectedSymbols { get { yield break; } }

		/// <summary>
		/// Checks any prerequisites not specified in the Prerequisites state. This function is important if
		/// there are one or more parameter symbols and not all possible values for them can be reached from
		/// all planner states. In such cases, this function should return false.
		/// </summary>
		/// <param name="currentState">The current planner state.</param>
		/// <param name="parameters">Parameters for this action.</param>
		public abstract bool IsValid(PlannerState currentState, params IPlannerStateSymbol[] parameters);

		/// <summary>
		/// Performs the action. Not used by the planner.
		/// </summary>
		/// <param name="parameters">The parameters for this action.</param>
		public abstract void Execute(params IPlannerStateSymbol[] parameters);

		/// <summary>
		/// Constructor.
		/// </summary>
		public PlannerAction()
		{
			_name = this.GetType().FullName;
		}

		/// <summary>
		/// Gets the list of symbols this action affects.
		/// </summary>
		public IEnumerable<string> AffectedSymbols
		{
			get
			{
				foreach (var symbol in Effects)
					yield return symbol.Name;
				if ((ParameterSymbols != null) && (ParameterSymbols.Count() > 0))
				{
					foreach (var symbolName in ParameterSymbols)
						if (!Effects.Contains(symbolName))
							yield return symbolName;
				}
				if ((ParameterSymbols != null) && (OtherAffectedSymbols.Count() > 0))
				{
					foreach (var symbolName in OtherAffectedSymbols)
						if ((!Effects.Contains(symbolName)) && (!ParameterSymbols.Contains(symbolName)))
							yield return symbolName;
				}
				yield break;
			}
		}
	}

	public class PlannerActionInstance : IXmlSerializable
	{
		public PlannerAction Action { get; set; }
		public List<IPlannerStateSymbol> Parameters { get; set; }

		public PlannerActionInstance()
		{
			Parameters = new List<IPlannerStateSymbol>();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is PlannerActionInstance))
				return false;
			return GetHashCode() == obj.GetHashCode();
		}

		public override int GetHashCode()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(Action.Name);
			foreach (IPlannerStateSymbol parameter in Parameters)
			{
				builder.Append(parameter.Name);
				builder.Append(parameter.Value);
			}
			return builder.ToString().GetHashCode();
		}

		#region IXmlSerializable Members

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			reader.Read();
			if (reader.MoveToAttribute("type"))
			{
				Type actionType = TypeHelper.FindType(reader.Value);
				if(actionType != null)
					Action = (PlannerAction)actionType.GetConstructor(Type.EmptyTypes).Invoke(null);
			}

			Type xmlTypeDef = typeof(XML<>);
			Type xmlType = null;
			while (reader.ReadToNextSibling("Parameter"))
			{
				try
				{
					reader.MoveToAttribute("type");
					string symbolTypeName = reader.Value;
					Type symbolType = TypeHelper.FindType(symbolTypeName);
					xmlType = xmlTypeDef.MakeGenericType(symbolType);
					xmlType.GetMethod("Initialize").Invoke(null, null);
					reader.ReadStartElement();
					reader.MoveToContent();
					Parameters.Add((IPlannerStateSymbol)xmlType.GetMethod("Load",
						new Type[] { typeof(XmlReader) }).Invoke(null, new object[] { reader }));
				}
				catch
				{
					Console.WriteLine("Error converting symbol.");
					continue;
				}
			}
			reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(typeof(PlannerAction).Name);
			if(Action != null)
				writer.WriteAttributeString("type", Action.GetType().FullName);
			writer.WriteEndElement();

			if (Parameters == null)
				return;

			Type xmlTypeDef = typeof(XML<>);
			Type xmlType = null;
			foreach (IPlannerStateSymbol symbol in Parameters)
			{
				Type type = symbol.GetType();
				if (type == typeof(object))
					continue;
				try
				{
					xmlType = xmlTypeDef.MakeGenericType(type);
					xmlType.GetMethod("Initialize").Invoke(null, null);

					writer.WriteStartElement("Parameter");
					writer.WriteAttributeString("type", type.FullName);

					xmlType.GetMethod("Save",
						new Type[] { typeof(XmlWriter), type }).Invoke(null, new object[] { writer, symbol });
					writer.WriteEndElement();
				}
				catch
				{
					Console.WriteLine("Error converting symbol:");
					Console.WriteLine(symbol.ToString());
					continue;
				}
			}
		}

		#endregion
	}
}
