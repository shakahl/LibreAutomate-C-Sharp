using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.Text;
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
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace LineBreaks
{

	/// <summary>
	/// <see cref="Moo{His}"/>,
	/// <see cref="Moo2{His, Ho}"/>,
	/// <see cref="Moo3{His}"/>,
	/// 
	/// <see cref="Test5{M2}"/>,
	/// <see cref="Test5{M2}.Moo{His}"/>,
	/// 
	/// <see cref="Test6"/>,
	/// <see cref="Test6.Moo{His}"/>,
	/// 
	/// <see cref="Test7.Moo1{His}(His)"/>,
	/// <see cref="Test7.Moo2{His, Ko}(His, Ko)"/>,
	/// <see cref="Test7.Test7(int)"/>,
	/// <see cref="Test7.op_Implicit(Test7)"/>,
	/// <see cref="Test7.operator+(Test7,Test7)"/>,
	/// <see cref="P:Au.Test7.Item(System.Int32)"/>,
	/// <see cref="Test7.Event"/>,
	/// </summary>
	public class Test4
	{
		public void Moo<His>()
		{

		}
		public void Moo2<His, Ho>()
		{

		}
		public void Moo3<His>()
		{

		}
	}

	public class Test5<M2>
	{
		public void Moo<His>()
		{

		}

	}

	public class Test6 :IEnumerable<string>
	{
		public IEnumerator<string> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public void Moo<His>()
		{

		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}

	public class Test7
	{
		public void Moo<His>()
		{

		}
		public void Moo1<His>(His m)
		{

		}
		public void Moo2<His, Ko>(His m, Ko k)
		{

		}

		public Test7(int i) { }
		~Test7() { }
		public static implicit operator int(Test7 a) => 0;
		public static int operator +(Test7 a, Test7 b) => 0;
		public int this[int i] => 0;

		public event EventHandler Event;
	}
}
