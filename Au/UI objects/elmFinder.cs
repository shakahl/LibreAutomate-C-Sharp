namespace Au
{
	/// <summary>
	/// Contains UI element properties and is used to find it.
	/// </summary>
	/// <remarks>
	/// Can be used instead of <see cref="elm.find"/>.
	/// </remarks>
	/// <example>
	/// Find window that contains certain UI element, and get the UI element too.
	/// <code><![CDATA[
	/// var f = new elmFinder("BUTTON", "Apply");
	/// wnd w = wnd.find(cn: "#32770", also: t => f.Find(t));
	/// print.it(w);
	/// print.it(f.Result);
	/// ]]></code>
	/// </example>
	public unsafe class elmFinder
	{
		string _role, _name, _prop, _navig;
		EFFlags _flags;
		int _skip;
		Cpp.AccCallbackT _also;
		char _resultProp;

		/// <summary>
		/// The found UI element.
		/// null if not found. null if used <see cref="ResultGetProperty"/>.
		/// </summary>
		public elm Result { get; private set; }

		/// <summary>
		/// The requested propery of the found UI element, depending on <see cref="ResultGetProperty"/>.
		/// null if: 1. UI element not found. 2. <b>ResultGetProperty</b> not used or is '-'. 3. Failed to get the property.
		/// </summary>
		/// <remarks>
		/// The type depends on the property. Most properties are String. Others: <see cref="elm.Rect"/>, <see cref="elm.State"/>, <see cref="elm.WndContainer"/>, <see cref="elm.HtmlAttributes"/>.
		/// </remarks>
		public object ResultProperty { get; private set; }

		/// <summary>
		/// Set this when you need only some property of the UI element (name, etc) and not the UI element itself.
		/// The value is a character, the same as with <see cref="elm.GetProperties"/>, for example 'n' for Name. Use '-' if you don't need any property.
		/// </summary>
		/// <exception cref="ArgumentException">Used parameter <i>also</i> or <i>navig</i>.</exception>
		public char ResultGetProperty {
			set {
				if (_also != null) throw new ArgumentException("ResultGetProperty cannot be used with parameter 'also'.");
				if (_navig != null) throw new ArgumentException("ResultGetProperty cannot be used with parameter 'navig'.");
				_resultProp = value;
			}
		}

		/// <summary>
		/// true if used parameter <i>navig</i> and the intermediate UI element was found but the navigation did not find the final UI element.
		/// </summary>
		public bool NavigFailed { get; private set; }

		void _ClearResult() { Result = null; ResultProperty = null; NavigFailed = false; }

		/// <summary>
		/// Stores the specified UI element properties in this object. Reference: <see cref="elm.find"/>.
		/// </summary>
		public elmFinder(string role = null,
			[ParamString(PSFormat.wildex)] string name = null,
			string prop = null, EFFlags flags = 0, Func<elm, bool> also = null, int skip = 0, string navig = null) {
			_role = role;
			_name = name;
			_prop = prop;
			_flags = flags;
			_skip = skip;
			_navig = navig;
			if (also != null) _also = (Cpp.Cpp_Acc ca) => also(new elm(ca)) ? 1 : 0;
		}

		/// <summary>
		/// Finds UI element in the specified control of window w.
		/// Returns true if found. The <see cref="Result"/> property will be the found UI element.
		/// </summary>
		/// <param name="w">Window that contains the control.</param>
		/// <param name="controls">Control properties. This functions searches in all matching controls.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Find(wnd)"/>.</exception>
		/// <remarks>
		/// Alternatively you can specify control class name or id in role. How this function is different: 1. Allows to specify more control properties. 2. Works better/faster when the control is of a different process or thread than the parent window; else slightly slower.
		/// </remarks>
		public bool Find(wnd w, wndChildFinder controls) {
			w.ThrowIfInvalid();
			foreach (var c in controls.FindAll(w)) {
				try {
					if (_FindOrWait(c, 0, false)) {
						controls.Result = c;
						return true;
					}
				}
				catch (AuException ex) when (!c.IsAlive) { Debug_.Print(ex.Message); } //don't throw AuWndException/AuException if the window or a control is destroyed while searching, but throw AuException if eg access denied
			}
			return false;
		}

		/// <summary>
		/// Finds UI element in window w, like <see cref="elm.find"/>.
		/// Returns true if found. The <see cref="Result"/> property will be the found UI element.
		/// </summary>
		/// <param name="w">Window or control that contains the UI element.</param>
		/// <exception cref="ArgumentException">
		/// - <i>role</i> is "" or invalid.
		/// - <i>name</i> is invalid wildcard expression (<c>"**options "</c> or regular expression).
		/// - <i>prop</i> has invalid format or contains unknown property names or invalid wildcard expressions.
		/// - flag <see cref="EFFlags.UIA"/> when searching in web page (role prefix "web:" etc).
		/// </exception>
		/// <exception cref="AuWndException">Invalid window.</exception>
		/// <exception cref="AuException">Failed. For example, window of a higher [](xref:uac) integrity level process.</exception>
		public bool Find(wnd w) {
			return _FindOrWait(w, 0, false);
		}

		/// <summary>
		/// Finds UI element in UI element, like <see cref="elm.Find(string, string, string, EFFlags, Func{elm, bool}, int, string)"/>.
		/// Returns true if found. The <see cref="Result"/> property will be the found UI element.
		/// </summary>
		/// <param name="e">Direct or indirect parent UI element.</param>
		/// <exception cref="ArgumentNullException"><i>e</i> is null.</exception>
		/// <exception cref="ArgumentException">
		/// - <i>role</i> is "" or invalid or has a prefix ("web:" etc).
		/// - <i>name</i> is invalid wildcard expression (<c>"**options "</c> or regular expression).
		/// - <i>prop</i> has invalid format or contains unknown property names or invalid wildcard expressions.
		/// - flag <see cref="EFFlags.UIA"/>.
		/// - <see cref="elm.SimpleElementId"/> is not 0.
		/// </exception>
		/// <exception cref="AuException">Failed. For example, window of a higher [](xref:uac) integrity level process.</exception>
		public bool Find(elm e) {
			return _FindOrWait(e, 0, false);
		}

		/// <summary>
		/// Finds UI element in window w.
		/// The same as <see cref="Find(wnd)"/>, but waits until the UI element is found or the given time expires.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="w">Window or control that contains the UI element.</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Find(wnd)"/>.</exception>
		public bool Wait(double secondsTimeout, wnd w) {
			return _FindOrWait(w, secondsTimeout, true);
		}

		/// <summary>
		/// Finds UI element in UI element.
		/// The same as <see cref="Find(elm)"/>, but waits until the UI element is found or the given time expires.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="e">Direct or indirect parent UI element.</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Find(elm)"/>.</exception>
		public bool Wait(double secondsTimeout, elm e) {
			return _FindOrWait(e, secondsTimeout, true);
		}

		bool _FindOrWait(wnd w, double secondsTimeout, bool isWaitFunc) {
			w.ThrowIfInvalid();
			return _Find(w, default, secondsTimeout, isWaitFunc);
		}

		bool _FindOrWait(elm e, double secondsTimeout, bool isWaitFunc) {
			if (e == null) throw new ArgumentNullException();
			e.ThrowIfDisposed_();
			if (e.SimpleElementId != 0) throw new ArgumentException("SimpleElementId is not 0.");
			if (_flags.HasAny(EFFlags.UIA | EFFlags.ClientArea)) throw new ArgumentException("Cannot use flags UIA and ClientArea when searching in elm.");

			Cpp.Cpp_Acc aParent = e;
			var R = _Find(default, &aParent, secondsTimeout, isWaitFunc);
			GC.KeepAlive(e);
			return R;
		}

		bool _Find(wnd w, Cpp.Cpp_Acc* aParent, double secondsTimeout, bool isWaitFunc) {
			if (_flags.Has(EFFlags.UIA | EFFlags.ClientArea)) throw new ArgumentException("Cannot use flags UIA and ClientArea together.");

			_ClearResult();

			EFFlags flags = _flags;
			if (aParent != null) {
				if (!aParent->misc.flags.Has(EMiscFlags.InProc)) flags |= EFFlags.NotInProc;
			}

			bool inProc = !flags.Has(EFFlags.NotInProc);

			if (!isWaitFunc) {
				Debug.Assert(secondsTimeout == 0.0);
				secondsTimeout = -1; //for WaitChromeDisabled
			}

			var ap = new Cpp.Cpp_AccParams(_role, _name, _prop, flags, _skip, _resultProp);

			var to = new wait.Loop(secondsTimeout, new OWait(period: inProc ? 10 : 40));
			for (bool doneUAC = false, doneThread = false; ;) {
				var hr = Cpp.Cpp_AccFind(w, aParent, in ap, _also, out var ca, out var sResult);

				if (hr == 0) {
					switch (_resultProp) {
					case '\0':
						var res = new elm(ca);
						if (_navig != null) {
							res = res.Navigate(_navig);
							if (NavigFailed = (res == null)) {
								if (isWaitFunc && to.Sleep()) continue;
								return false;
							}
						}
						Result = res;
						break;
					case 'r':
					case 's':
					case 'w':
					case '@':
						if (sResult == null) break;
						unsafe {
							fixed (char* p = sResult) {
								switch (_resultProp) {
								case 'r': ResultProperty = *(RECT*)p; break;
								case 's': ResultProperty = *(EState*)p; break;
								case 'w': ResultProperty = (wnd)(*(int*)p); break;
								case '@': ResultProperty = elm.AttributesToDictionary_(p, sResult.Length); break;
								}
							}
						}
						break;
					default:
						ResultProperty = sResult;
						break;
					}
					return true;
				}

				if (hr == Cpp.EError.InvalidParameter) throw new ArgumentException(sResult);
				if ((hr == Cpp.EError.WindowClosed) || (!w.Is0 && !w.IsAlive)) return false; //FUTURE: check if a is disconnected etc. Or then never wait.

				if (!doneUAC) {
					doneUAC = true;
					w.UacCheckAndThrow_(); //CONSIDER: don't throw. Maybe show warning.
				}

				//print.it(hr > 0 ? $"hr={hr}" : $"hr={(int)hr:X}");
				switch (hr) {
				case Cpp.EError.NotFound:
					if (!isWaitFunc) return false;
					break;
				case Cpp.EError.WaitChromeDisabled:
					//print.it("WaitChromeDisabled");
					if (to.TimeRemaining < 3000) to.TimeRemaining += (long)(to.Period * 15 / 16);
					//normally waits ~10 times longer, eg 10 s instead of 1
					break;
				default:
					Debug.Assert(!Cpp.IsCppError((int)hr));
					if (hr == (Cpp.EError)Api.RPC_E_SERVER_CANTMARSHAL_DATA && !_flags.Has(EFFlags.NotInProc))
						throw new AuException((int)hr, "For this UI element need flag NotInProc");
					throw new AuException((int)hr);
				}

				if (!doneThread) {
					doneThread = true;
					if (!w.Is0 && w.IsOfThisThread) return false;
				}

				if (!to.Sleep()) return false;
			}
		}
	}
}
