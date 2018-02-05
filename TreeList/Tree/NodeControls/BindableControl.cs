using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Aga.Controls.Tree.NodeControls
{
	public abstract class BindableControl :NodeControl
	{
		private struct MemberAdapter
		{
			private object _obj;
			private PropertyInfo _pi;
			private FieldInfo _fi;

			public static readonly MemberAdapter Empty = new MemberAdapter();

			public Type MemberType
			{
				get
				{
					if(_pi != null)
						return _pi.PropertyType;
					else if(_fi != null)
						return _fi.FieldType;
					else
						return null;
				}
			}

			public object Value
			{
				get
				{
					if(_pi != null && _pi.CanRead)
						return _pi.GetValue(_obj, null);
					else if(_fi != null)
						return _fi.GetValue(_obj);
					else
						return null;
				}
				set
				{
					if(_pi != null && _pi.CanWrite)
						_pi.SetValue(_obj, value, null);
					else if(_fi != null)
						_fi.SetValue(_obj, value);
				}
			}

			public MemberAdapter(object obj, PropertyInfo pi)
			{
				_obj = obj;
				_pi = pi;
				_fi = null;
			}

			public MemberAdapter(object obj, FieldInfo fi)
			{
				_obj = obj;
				_fi = fi;
				_pi = null;
			}
		}

		#region Properties

		private string _propertyName;
		[DefaultValue(null), Category("Data")]
		public string DataPropertyName
		{
			get { return _propertyName; }
			set
			{
				_propertyName = value;
			}
		}

		//private bool _incrementalSearchEnabled = false;
		//[DefaultValue(false)]
		//public bool IncrementalSearchEnabled
		//{
		//	get { return _incrementalSearchEnabled; }
		//	set { _incrementalSearchEnabled = value; }
		//}

		#endregion

		public virtual object GetValue(TreeNodeAdv node)
		{
			if(DataPropertyName == null) {
				return OnValueNeeded(node);
			} else {
				try {
					return GetMemberAdapter(node).Value;
				}
				catch(TargetInvocationException ex) {
					if(ex.InnerException != null)
						throw new ArgumentException(ex.InnerException.Message, ex.InnerException);
					else
						throw new ArgumentException(ex.Message);
				}
			}
		}

		public virtual void SetValue(TreeNodeAdv node, object value)
		{
			if(DataPropertyName == null) {
				OnValuePushed(node, value);
			} else {
				try {
					MemberAdapter ma = GetMemberAdapter(node);
					ma.Value = value;
				}
				catch(TargetInvocationException ex) {
					if(ex.InnerException != null)
						throw new ArgumentException(ex.InnerException.Message, ex.InnerException);
					else
						throw new ArgumentException(ex.Message);
				}
			}
		}

		public Type GetPropertyType(TreeNodeAdv node)
		{
			return GetMemberAdapter(node).MemberType;
		}

		private MemberAdapter GetMemberAdapter(TreeNodeAdv node)
		{
			if(node.Tag != null && !string.IsNullOrEmpty(DataPropertyName)) {
				Type type = node.Tag.GetType();
				PropertyInfo pi = type.GetProperty(DataPropertyName);
				if(pi != null)
					return new MemberAdapter(node.Tag, pi);
				else {
					FieldInfo fi = type.GetField(DataPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if(fi != null)
						return new MemberAdapter(node.Tag, fi);
				}
			}
			return MemberAdapter.Empty;
		}

		public override string ToString()
		{
			if(string.IsNullOrEmpty(DataPropertyName))
				return GetType().Name;
			else
				return string.Format("{0} ({1})", GetType().Name, DataPropertyName);
		}

		public Func<TreeNodeAdv, object> ValueNeeded;
		private object OnValueNeeded(TreeNodeAdv node)
		{
			return ValueNeeded?.Invoke(node);
		}

		public Action<TreeNodeAdv, object> ValuePushed;
		private void OnValuePushed(TreeNodeAdv node, object value)
		{
			ValuePushed?.Invoke(node, value);
		}
	}
}
