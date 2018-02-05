using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
using System.Xml.Linq;
//using System.Xml.XPath;

using Au;
using static Au.NoClass;

namespace Au.Types
{
	/// <summary>
	/// Builds ICondition (<msdn>IUIAutomationCondition</msdn>) from one or more properties, where all properties must match.
	/// </summary>
	/// <example>
	/// <code><![CDATA[
	/// Wnd w = Wnd.Find("Options").OrThrow();
	/// var ew = AElement.Factory.ElementFromHandle(w);
	/// var e = ew.FindFirst(UIA.TreeScope.Descendants, new ACondition().Name("Apply").Type(UIA.TypeId.Button).Enabled(true).Condition);
	/// Print(e.Name);
	/// ]]></code>
	/// </example>
	public struct ACondition
	{
		UIA.ICondition _c;

		static UIA.IUIAutomation _f => AElement.Factory;

		/// <summary>
		/// Gets or sets current ICondition (<msdn>IUIAutomationCondition</msdn>). It contains all added conditions.
		/// </summary>
		public UIA.ICondition Condition { get => _c; set => _c = value; }

		/// <summary>
		/// Adds a condition as an ICondition object.
		/// Algorithm: <c>Condition = Condition AND cond</c>.
		/// </summary>
		/// <remarks>
		/// If this variable is empty, sets Condition = cond. Else calls <msdn>IUIAutomation.CreateAndCondition</msdn>.
		/// </remarks>
		public ACondition AddIC(UIA.ICondition cond)
		{
			_c = _c == null ? cond : _f.CreateAndCondition(_c, cond);
			return this;
		}

		/// <summary>
		/// Adds NOT condition of an ICondition object.
		/// Algorithm: <c>Condition = Condition AND NOT cond</c>.
		/// </summary>
		/// <remarks>
		/// Calls <msdn>IUIAutomation.CreateNotCondition</msdn> and <see cref="AddIC"/>.
		/// </remarks>
		public ACondition AddNotIC(UIA.ICondition cond)
		{
			return AddIC(_f.CreateNotCondition(cond));
		}

		/// <summary>
		/// Adds OR condition from 2 or more conditions.
		/// Algorithm: <c>Condition = Condition AND (conditions[0] OR conditions[1] OR ...)</c>.
		/// </summary>
		/// <remarks>
		/// Calls <msdn>IUIAutomation.CreateOrConditionFromArray</msdn> and <see cref="AddIC"/>.
		/// </remarks>
		public ACondition AddOrIC(params UIA.ICondition[] conditions)
		{
			if(conditions.Length < 2) throw new ArgumentException();
			return AddIC(_f.CreateOrConditionFromArray(conditions));
		}

		/// <summary>
		/// Adds NOT OR condition from 2 or more conditions.
		/// Algorithm: <c>Condition = Condition AND NOT (conditions[0] OR conditions[1] OR ...)</c>.
		/// </summary>
		/// <remarks>
		/// Calls <msdn>IUIAutomation.CreateOrConditionFromArray</msdn>, <msdn>IUIAutomation.CreateNotCondition</msdn> and <see cref="AddIC"/>.
		/// </remarks>
		public ACondition AddNotOrIC(params UIA.ICondition[] conditions)
		{
			if(conditions.Length < 2) throw new ArgumentException();
			return AddIC(_f.CreateNotCondition(_f.CreateOrConditionFromArray(conditions)));
		}

		/// <summary>
		/// Adds a property condition.
		/// Algorithm: <c>Condition = Condition AND (property == value)</c>.
		/// </summary>
		/// <remarks>
		/// Calls <msdn>IUIAutomation.CreatePropertyConditionEx</msdn> and <see cref="AddIC"/>.
		/// </remarks>
		public ACondition Add(UIA.PropertyId propertyId, object value, bool ignoreCase = false)
		{
			return AddIC(_f.CreatePropertyConditionEx(propertyId, value, ignoreCase ? UIA.PropertyConditionFlags.IgnoreCase : 0));
		}

		/// <summary>
		/// Adds NOT condition of a property.
		/// Algorithm: <c>Condition = Condition AND (property != value)</c>.
		/// </summary>
		/// <param name="propertyId"></param>
		/// <param name="value"></param>
		/// <param name="ignoreCase">When string, case-insensitive.</param>
		/// <remarks>
		/// Calls <msdn>IUIAutomation.CreatePropertyConditionEx</msdn> and <see cref="AddNotIC"/>.
		/// </remarks>
		public ACondition AddNot(UIA.PropertyId propertyId, object value, bool ignoreCase = false)
		{
			return AddNotIC(_f.CreatePropertyConditionEx(propertyId, value, ignoreCase ? UIA.PropertyConditionFlags.IgnoreCase : 0));
		}

		/// <summary>
		/// Adds OR condition from 2 or more values of a property.
		/// Algorithm: <c>Condition = Condition AND ((property == values[0]) OR (property == values[1]) OR ...)</c>.
		/// </summary>
		/// <remarks>
		/// Calls <msdn>IUIAutomation.CreatePropertyConditionEx</msdn>, <msdn>IUIAutomation.CreateOrConditionFromArray</msdn> and <see cref="AddIC"/>.
		/// </remarks>
		public ACondition AddOr(UIA.PropertyId propertyId, params object[] values)
		{
			return AddIC(_Or(false, propertyId, values));
		}

		/// <summary>
		/// Adds OR condition from 2 or more values of a property.
		/// Algorithm: <c>Condition = Condition AND ((property == values[0]) OR (property == values[1]) OR ...)</c>.
		/// </summary>
		/// <remarks>
		/// Calls <msdn>IUIAutomation.CreatePropertyConditionEx</msdn>, <msdn>IUIAutomation.CreateOrConditionFromArray</msdn> and <see cref="AddIC"/>.
		/// </remarks>
		public ACondition AddOr(bool ignoreCase, UIA.PropertyId propertyId, params object[] values)
		{
			return AddIC(_Or(ignoreCase, propertyId, values));
		}

		static UIA.ICondition _Or(bool ignoreCase, UIA.PropertyId propertyId, object[] values)
		{
			UIA.PropertyConditionFlags flags = ignoreCase ? UIA.PropertyConditionFlags.IgnoreCase : 0;
			int n = values.Length; if(n < 2) throw new ArgumentException();
			if(n == 2) return _f.CreateOrCondition(_f.CreatePropertyConditionEx(propertyId, values[0], flags), _f.CreatePropertyConditionEx(propertyId, values[1], flags));
			var a = new UIA.ICondition[n];
			for(int i = 0; i < n; i++) a[i] = _f.CreatePropertyConditionEx(propertyId, values[i], flags);
			return _f.CreateOrConditionFromArray(a);
		}

		/// <summary>
		/// Adds NOT OR condition from 2 or more values of a property.
		/// Algorithm: <c>Condition = Condition AND NOT ((property == values[0]) OR (property == values[1]) OR ...)</c>.
		/// </summary>
		/// <remarks>
		/// Calls <msdn>IUIAutomation.CreatePropertyConditionEx</msdn>, <msdn>IUIAutomation.CreateOrConditionFromArray</msdn> and <see cref="AddNotIC"/>.
		/// </remarks>
		public ACondition AddNotOr(UIA.PropertyId propertyId, params object[] values)
		{
			return AddNotIC(_Or(false, propertyId, values));
		}

		/// <summary>
		/// Adds NOT OR condition from 2 or more values of a property.
		/// Algorithm: <c>Condition = Condition AND NOT ((property == values[0]) OR (property == values[1]) OR ...)</c>.
		/// </summary>
		/// <remarks>
		/// Calls <msdn>IUIAutomation.CreatePropertyConditionEx</msdn>, <msdn>IUIAutomation.CreateOrConditionFromArray</msdn> and <see cref="AddNotIC"/>.
		/// </remarks>
		public ACondition AddNotOr(bool ignoreCase, UIA.PropertyId propertyId, params object[] values)
		{
			return AddNotIC(_Or(ignoreCase, propertyId, values));
		}

		/// <summary>
		/// Calls <see cref="Add"/> with UIA.PropertyId.Name.
		/// </summary>
		public ACondition Name(string value, bool ignoreCase = false)
		{
			return Add(UIA.PropertyId.Name, value, ignoreCase);
		}

		/// <summary>
		/// Calls <see cref="Add"/> with UIA.PropertyId.ControlType.
		/// </summary>
		public ACondition Type(UIA.TypeId value)
		{
			return Add(UIA.PropertyId.ControlType, value);
		}

		/// <summary>
		/// Calls <see cref="Add"/> with UIA.PropertyId.BoundingRectangle.
		/// </summary>
		public ACondition Rect(RECT value)
		{
			return Add(UIA.PropertyId.BoundingRectangle, _f.RectToVariant(value));
		}

		/// <summary>
		/// Calls <see cref="Add"/> with UIA.PropertyId.IsOffscreen.
		/// </summary>
		public ACondition Offscreen(bool value)
		{
			return Add(UIA.PropertyId.IsOffscreen, value);
		}

		/// <summary>
		/// Calls <see cref="Add"/> with UIA.PropertyId.IsEnabled.
		/// </summary>
		public ACondition Enabled(bool value)
		{
			return Add(UIA.PropertyId.IsEnabled, value);
		}

		/// <summary>
		/// Calls <see cref="Add"/> with UIA.PropertyId.LegacyIAccessibleRole.
		/// </summary>
		public ACondition LARole(AccROLE value)
		{
			return Add(UIA.PropertyId.LegacyIAccessibleRole, value);
		}

		//rejected:
		///// <summary>
		///// Calls <see cref="Add"/> with UIA.PropertyId.LegacyIAccessibleName.
		///// </summary>
		//public ACondition LAName(string value, bool ignoreCase = false)
		//{
		//	return Add(UIA.PropertyId.LegacyIAccessibleName, value, ignoreCase);
		//}

		///// <summary>
		///// Calls <see cref="Add"/> with UIA.PropertyId.LegacyIAccessibleState.
		///// </summary>
		//public ACondition LAState(AccSTATE value)
		//{
		//	return Add(UIA.PropertyId.LegacyIAccessibleState, value);
		//}
	}
}
