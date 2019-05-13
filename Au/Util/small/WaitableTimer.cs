﻿using System;
using System.Collections.Generic;
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
//using System.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au.Util
{
	/// <summary>
	/// Wraps a waitable timer handle. Allows to create, open, set and wait.
	/// More info: API <msdn>CreateWaitableTimer</msdn>.
	/// Note: will need to dispose.
	/// </summary>
	public class WaitableTimer : WaitHandle
	{
		WaitableTimer(IntPtr h) => SafeWaitHandle = new Microsoft.Win32.SafeHandles.SafeWaitHandle(h, true);

		/// <summary>
		/// Calls API <msdn>CreateWaitableTimer</msdn> and creates a WaitableTimer object that wraps the timer handle.
		/// </summary>
		/// <param name="manualReset"></param>
		/// <param name="timerName">Timer name. If a timer with this name already exists, opens it if possible. If null, creates unnamed timer.</param>
		/// <exception cref="AException">Failed. For example, a non-timer kernel object with this name already exists.</exception>
		public static WaitableTimer Create(bool manualReset = false, string timerName = null)
		{
			var h = Api.CreateWaitableTimer(Api.SECURITY_ATTRIBUTES.ForLowIL, manualReset, timerName);
			if(h.Is0) throw new AException(0, "*create timer");
			return new WaitableTimer(h);
		}

		/// <summary>
		/// Calls API <msdn>OpenWaitableTimer</msdn> and creates a WaitableTimer object that wraps the timer handle.
		/// </summary>
		/// <param name="timerName">Timer name. Fails if it does not exist; to open-or-create use <see cref="Create"/>.</param>
		/// <param name="access">.See <msdn>Synchronization Object Security and Access Rights</msdn>. The default value TIMER_MODIFY_STATE|SYNCHRONIZE allows to set and wait.</param>
		/// <exception cref="AException">Failed. For example, a non-timer kernel object with this name already exists.</exception>
		/// <param name="inheritHandle"></param>
		/// <param name="noException">If fails, return null, don't throw exception. Supports <see cref="WinError"/>.</param>
		/// <exception cref="AException">Failed. For example, the timer does not exist.</exception>
		public static WaitableTimer Open(string timerName, uint access = Api.TIMER_MODIFY_STATE | Api.SYNCHRONIZE, bool inheritHandle = false, bool noException = false)
		{
			var h = Api.OpenWaitableTimer(access, inheritHandle, timerName);
			if(h.Is0) {
				var e = WinError.Code;
				if(noException) {
					WinError.Code = e;
					return null;
				}
				throw new AException(e, "*open timer");
			}
			return new WaitableTimer(h);
		}

		/// <summary>
		/// Calls API <msdn>SetWaitableTimer</msdn>.
		/// Returns false if fails. Supports <see cref="WinError"/>.
		/// </summary>
		/// <param name="dueTime">
		/// The time after which the state of the timer is to be set to signaled. It is relative time (from now).
		/// If positive, in milliseconds. If negative, in 100 nanosecond intervals (microseconds*10), see <msdn>FILETIME</msdn>.
		/// Also can be 0, to set minimal time.</param>
		/// <param name="period">The period of the timer, in milliseconds. If 0, the timer is signaled once. If greater than 0, the timer is periodic.</param>
		/// <exception cref="OverflowException">dueTime*10000 is greater than long.MaxValue.</exception>
		public bool Set(long dueTime, int period = 0)
		{
			if(dueTime > 0) dueTime = -checked(dueTime * 10000);
			return Api.SetWaitableTimer(this.SafeWaitHandle.DangerousGetHandle(), ref dueTime, period, default, default, false);
		}

		/// <summary>
		/// Calls API <msdn>SetWaitableTimer</msdn>.
		/// Returns false if fails. Supports <see cref="WinError"/>.
		/// </summary>
		/// <param name="dueTime">The UTC date/time at which the state of the timer is to be set to signaled.</param>
		/// <param name="period">The period of the timer, in milliseconds. If 0, the timer is signaled once. If greater than 0, the timer is periodic.</param>
		public bool SetAbsolute(DateTime dueTime, int period = 0)
		{
			var t = dueTime.ToFileTimeUtc();
			return Api.SetWaitableTimer(this.SafeWaitHandle.DangerousGetHandle(), ref t, period, default, default, false);
		}
	}
}