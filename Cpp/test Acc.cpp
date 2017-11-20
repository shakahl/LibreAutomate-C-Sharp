#include "stdafx.h"
#include "cpp.h"

#pragma comment(lib, "oleacc.lib")

namespace wnd
{
	bool ClassName(HWND w, std::wstring& s)
	{
		WCHAR b[260];
		int n = GetClassNameW(w, b, 260);
		if(n == 0) {
			s.clear();
			return 0;
		}
		s.assign(b, n);
		return true;
	}
}

void DoEvents()
{
	MSG m;
	while(PeekMessageW(&m, 0, 0, 0, PM_REMOVE)) {
	//while(PeekMessageW(&m, 0, 0, 0, PM_REMOVE| PM_QS_SENDMESSAGE)) { //does not work
		//Print(m.message);
		//std::wstring s; if(wnd::ClassName(m.hwnd, s)) Print(s);
		if(m.message == WM_QUIT) { PostQuitMessage((int)m.wParam); return; }
		TranslateMessage(&m);
		DispatchMessageW(&m);
	}
}

//void DoEvents2()
//{
//	DWORD signaledIndex;
//	auto hr = CoWaitForMultipleHandles(0, 0, 0, null, &signaledIndex); //fails, invalid parameter
//	Printx(hr);
//}

void SleepDoEvents(int milliseconds)
{
	if(milliseconds == 0) { DoEvents(); return; }
	for(;;) {
		ULONGLONG t = 0;
		int timeSlice = 100; //we call API in loop with small timeout to make it respond to Thread.Abort
		if(milliseconds > 0) {
			if(milliseconds < timeSlice) timeSlice = milliseconds;
			t = GetTickCount64();
		}

		DWORD k = MsgWaitForMultipleObjectsEx(0, null, timeSlice, QS_ALLINPUT, MWMO_ALERTABLE);
		//info: k can be 0 (message etc), WAIT_TIMEOUT, WAIT_IO_COMPLETION, WAIT_FAILED.
		if(k == WAIT_FAILED) return; //unlikely, because not using handles
		if(k == 0) DoEvents();

		if(milliseconds > 0) {
			milliseconds -= (int)(GetTickCount64() - t);
			if(milliseconds <= 0) break;
		}
	}
}

const int MaxChildren = 10000;

namespace acc
{
	struct _Acc
	{
		IAccessible* a;
		long elem;

		_Acc() : a(null), elem(0) {}
		_Acc(IAccessible* a, int elem) : a(a), elem(elem) {}

		//note: calls Relese only if elem is 0. Else a is considered not owned by this.
		~_Acc() { if(a != null && elem == 0) a->Release(); }

		void Dispose() {
			if(a != null) { a->Release(); a = null; }
			elem = 0;
		}
	};

	/// <summary>
	/// Calls QueryInterface to get IAccessible from IDispatch idisp. Releases idisp.
	/// Returns HRESULT.
	/// </summary>
	/// <param name="idisp">If null, returns E_FAIL.</param>
	/// <param name="iacc">Result.</param>
	HRESULT FromIDispatch(IDispatch* idisp, out IAccessible*& iacc)
	{
		iacc = null;
		if(idisp == null) return E_FAIL;
		HRESULT hr = idisp->QueryInterface(IID_IAccessible, (void**)&iacc);
		idisp->Release();
		if(hr == 0 && iacc == null) hr = E_FAIL;
		return hr;
	}

	HRESULT get_accChild(IAccessible* a, long elem, out IAccessible*& aChild)
	{
		aChild = null;
		IDispatch* idisp = null;
		HRESULT hr = a->get_accChild(_variant_t(elem), &idisp);
		if(hr == 0) hr = FromIDispatch(idisp, aChild);
		return hr;
	}

	static HRESULT FromVARIANT(IAccessible* parent, IN VARIANT& v, out _Acc&a, bool tryGetObjectFromId = false)
	{
		assert(a.a == null && a.elem == 0);
		int hr = 0;
		assert(v.pdispVal != null); //bug in our code or AO
		if(v.pdispVal == null) hr = E_FAIL;
		else {
			switch(v.vt) {
			case VT_DISPATCH:
				hr = FromIDispatch(v.pdispVal, a.a);
				break;
			case VT_I4: //info: AccessibleChildren does not AddRef
				a.elem = v.lVal;
				if(a.elem == 0) hr = E_FAIL;
				else if(tryGetObjectFromId && 0 == get_accChild(parent, a.elem, a.a)) a.elem = 0;
				else a.a = parent;
				//Printf(L"FromVARIANT: elem=%i, tryGetObjectFromId=%i", a.elem, tryGetObjectFromId);
				break;
			default:
				assert(false);
				VariantClear(&v);
				hr = E_FAIL;
				break;
			}
		}
		v.vt = 0;
		//Debug_.PrintIf(hr != 0, $"0x{hr:X}");
		return hr;
	}

	const STR s_roles[] = { L"0", L"TITLEBAR", L"MENUBAR", L"SCROLLBAR", L"GRIP", L"SOUND", L"CURSOR", L"CARET", L"ALERT", L"WINDOW", L"CLIENT", L"MENUPOPUP", L"MENUITEM", L"TOOLTIP", L"APPLICATION", L"DOCUMENT", L"PANE", L"CHART", L"DIALOG", L"BORDER", L"GROUPING", L"SEPARATOR", L"TOOLBAR", L"STATUSBAR", L"TABLE", L"COLUMNHEADER", L"ROWHEADER", L"COLUMN", L"ROW", L"CELL", L"LINK", L"HELPBALLOON", L"CHARACTER", L"LIST", L"LISTITEM", L"OUTLINE", L"OUTLINEITEM", L"PAGETAB", L"PROPERTYPAGE", L"INDICATOR", L"GRAPHIC", L"STATICTEXT", L"TEXT", L"PUSHBUTTON", L"CHECKBUTTON", L"RADIOBUTTON", L"COMBOBOX", L"DROPLIST", L"PROGRESSBAR", L"DIAL", L"HOTKEYFIELD", L"SLIDER", L"SPINBUTTON", L"DIAGRAM", L"ANIMATION", L"EQUATION", L"BUTTONDROPDOWN", L"BUTTONMENU", L"BUTTONDROPDOWNGRID", L"WHITESPACE", L"PAGETABLIST", L"CLOCK", L"SPLITBUTTON", L"IPADDRESS", L"OUTLINEBUTTON" };

	static STR RoleToString(VARIANT& role)
	{
		switch(role.vt) {
		case VT_BSTR:
			return role.bstrVal;
		case VT_I4:
			if((UINT)role.lVal < _countof(s_roles))
				return s_roles[role.lVal];
		default: return null;
		}
	}
}

class AccFinder
{
	AccCallback* _callback;
	bool _findAll;
	CComBSTR _role, _name;

public:
	AccFinder(AccCallback* callback, bool findAll, STR role, STR name)
	{
		MaxLevel = 1000;
		_callback = callback;
		_findAll = findAll;
		_role = role;
		_name = name;
	}

	//IAccessiblePtr ResultAcc;
	//long ResultElem;

	int MaxLevel;

	bool FindIn(HWND w)
	{
		bool found = _FindIn(w);
		if(!found || _findAll) _callback->Finished();
		return found;
	}

	bool FindIn(IAccessible* iacc)
	{
		bool found = _FindIn(iacc, 0);
		if(!found || _findAll) _callback->Finished();
		return found;
	}

private:

	bool _FindIn(HWND w)
	{
		IAccessiblePtr a;
		if(AccessibleObjectFromWindow(w, OBJID_WINDOW, IID_IAccessible, (void**)(IAccessible**)&a)) return false;
		return _FindIn(a, 0);
	}

	bool _FindIn(IAccessible* iacc, int level)
	{
		_Children c(iacc);
		int i = 0;
		while(_callback->IsClientStillWaiting(false)) {
			acc::_Acc a;
			if(!c.GetNext(a)) break;
			bool skipChildren;
			if(_Match(a.a, a.elem, skipChildren, level)) return true;
			//Perf.First();
			//if(_findAll) DoEvents(); //TODO
			//Perf.NW();
			//if(_findAll) SleepDoEvents(1); //TODO
			if(skipChildren) continue;
			if(level >= MaxLevel) continue;
			if(_FindIn(a.a, level + 1)) return true;
		} //now a.a is released if a.elem==0
		return false;
	}

	bool _Match(IAccessible* a, long e, out bool& skipChildren, int level)
	{
		skipChildren = e != 0;

#if false
		_variant_t varElem(e), varRole, varState;

		HRESULT hr = 0;
		hr = a->get_accRole(varElem, &varRole);
		if(0 != hr) {
			Printf(L"failed to get role.  hr=0x%X  elem=%i", hr, e);
			return false;
		}

		if(!skipChildren && varRole.vt == VT_I4) {
			switch(varRole.lVal) {
			case ROLE_SYSTEM_SCROLLBAR:
			case ROLE_SYSTEM_MENUBAR:
			case ROLE_SYSTEM_TITLEBAR:
			case ROLE_SYSTEM_GRIP:
				skipChildren = true;
				return false;
			}
		}

		if(_role != null) {
			STR s = acc::RoleToString(ref varRole);
			if(_role != s) return false;
		}

		//Printf(L"%i %s", level, acc::RoleToString(ref varRole));
		//{
		if(_name != null) {
			CComBSTR b; LPWSTR s = L"";
			if(0 == a->get_accName(varElem, &b) && b.m_str != null) s = b;
			//Printf(L"%i %s    \"%s\"", level, acc::RoleToString(ref varRole), s);
			//Print(b);
			if(_name != b) return false;
		}

		hr = a->get_accState(varElem, &varState);
		if(hr == 0 && varState.vt == VT_I4 && varState.lVal&STATE_SYSTEM_INVISIBLE) return false;
#endif
		//ResultAcc.Attach(a, true);
		//ResultElem = e;
		_callback->WriteResult(a, e);

		return !_findAll;
	}

	class _Children
	{
		IAccessible* _parent;
		VARIANT* _v;
		int _count, _i, _startAtIndex;
		bool _exactIndex, _reverse;

	public:
		_Children(IAccessible* parent, int startAtIndex = 0, bool exactIndex = false, bool reverse = false)
		{
			_parent = parent;
			_v = null;
			_count = -1;
			_i = 0;
			_startAtIndex = startAtIndex;
			_exactIndex = exactIndex;
			_reverse = reverse;
		}

		~_Children()
		{
			if(_v != null) {
				while(_count > 0) VariantClear(&_v[--_count]); //info: it's OK to dispose variants for which FromVARIANT was called because then vt is 0 and Dispose does nothing
				auto t = _v; _v = null; free(t);
			}
		}


		bool GetNext(out acc::_Acc& a)
		{
			assert(a.a == null && a.elem == 0);
			if(_count < 0) _Init();
			if(_count == 0) return false;
			if(_exactIndex) {
				int i = _startAtIndex; _startAtIndex = -1;
				return i >= 0 && 0 == acc::FromVARIANT(_parent, _v[i], a);
			}
		g1:
			if(_startAtIndex < 0) { //_startAtIndex is -1 if not used
				if(_i >= _count) return false;
				int i = _i++; if(_reverse) i = _count - i - 1;
				if(0 != acc::FromVARIANT(_parent, _v[i], a)) goto g1;
			} else { //_startAtIndex is in _count range
				int i = _startAtIndex + _i;
				if(i < 0 || i >= _count) return false; //no more
				//calculate next i
				if(_i >= 0) {
					_i = -(_i + 1);
					if(_startAtIndex + _i < 0) _i = -_i;
				} else {
					_i = -_i;
					if(_startAtIndex + _i >= _count) _i = -(_i + 1);
				}
				if(0 != acc::FromVARIANT(_parent, _v[i], a)) goto g1;
			}
			return true;
		}

	private:
		void _Init()
		{
			//IEnumVARIANTPtr ev; long cc = 0;
			//Perf.First();
			//auto hrrr = _parent->QueryInterface(&ev);
			//Perf.Next();
			//if(0==hrrr) {
			//	//Print("is EV");
			//	ev.Release();
			//} else if(0 == _parent->get_accChildCount(&cc)) {
			//	//Print(cc);
			//	Perf.Next();
			//} //else Print("failed");
			//Perf.Write();

			//Perf.First();
			//IEnumVARIANTPtr ev; int n5 = 0;
			//if(0 == _parent->QueryInterface(&ev)) {
			//	ev->Reset();
			//	VARIANT v; ULONG nf;
			//	while(0 == ev->Next(1, &v, &nf) && nf == 1) {
			//		n5++;
			//		VariantClear(&v);
			//	}
			//	ev.Release();
			//}
			//Perf.NW(); //not faster
			//long cc = 0; _parent->get_accChildCount(&cc);
			//Printf(L"%i %i", n5, cc);

			//note: don't call get_accChildCount here.
			//	With Firefox and some other apps it makes almost 2 times slower. With others same speed.

			const int nStack = 100; //info: fast even with 10 or 7, but 5 makes slower. Just slightly faster with 100. Not faster with 30 etc.
			VARIANT v[nStack];
			long n = 0;
			int hr = AccessibleChildren(_parent, 0, nStack, v, &n);
			if(hr < 0) { PRINTX(hr); n = 0; } //never noticed

			if(n == nStack) { //probably there are more children
				_parent->get_accChildCount(&n); //note: some objects return 0 or 1, ie less than AccessibleChildren, and HRESULT is usually 0. Noticed this only in IE, when c_acBufferSize<10.
				if(n != nStack) { //yes, more children
					for(int i = nStack; i > 0;) VariantClear(&v[--i]);
					if(n > MaxChildren) { //protection from AO such as LibreOffice Calc TABLE that has 1073741824 children. Default 10000.
						n = 0;
					} else {
						if(n < nStack) n = 1000; //get_accChildCount returned error or incorrect value
						_v = (VARIANT*)malloc(n * sizeof(VARIANT));
						hr = AccessibleChildren(_parent, 0, n, _v, &n); //note: iChildStart must be 0, else not always gets all children
						if(hr < 0) { PRINTX(hr); n = 0; }
					}
				}
			}

			if(n > 0 && _v == null) {
				int memSize = n * sizeof(VARIANT);
				_v = (VARIANT*)malloc(memSize);
				memcpy(_v, v, memSize);
			}

			_count = n;
			if(n > 0 && _startAtIndex != 0) {
				if(_startAtIndex < 0) _startAtIndex = n + _startAtIndex; else _startAtIndex--; //if < 0, it is index from end
				int i = _startAtIndex; if(i < 0) i = 0; else if(i >= n) i = n - 1;
				if(_exactIndex && i != _startAtIndex) _startAtIndex = -1; else _startAtIndex = i;
			} else _startAtIndex = -1; //not used

			//speed: AccessibleChildren same as IEnumVARIANT with array. IEnumVARIANT.Next(1, ...) much slower.
			//	get_accChild is tried only when IEnumVARIANT not supported. Else it often fails or is slower.

			//50% AO passed to this function have 0 children. Then we don't allocate _v.
			//	20% have 1 child. Few have more than 7.

			//Acc CONSIDER: if we know it's Firefox, try to just IEnumVARIANT, because it's documented that if no IEnumVARIANT then there are no children.
			//	Now AccessibleChildren also calls get_accChildCount, which probably makes slower.
		}
	};
};

void TestAccFindInWnd(AccCallback* callback, bool findAll, HWND w, STR role, STR name)
{
	//Printf(L"%p %s", w, name);

#if true
	AccFinder f(callback, findAll, role, name);
	bool found = f.FindIn(w);
	//Print(found ? "found" : "not found");
#else
	IAccessiblePtr ap;
	AccessibleObjectFromWindow(w, 0, IID_IAccessible, (void**)&ap);
	write(ap, 0);
#endif
}

//void TestAccFindInAcc(fWriteResult write, bool findAll, IAccessible* iaccParent, STR role, STR name)
//{
//	//Printf(L"%p %s", w, name);
//
//	AccFinder f(write, findAll, name);
//	bool found = f.FindIn(iaccParent);
//	//Print(found ? "found" : "not found");
//
//	return 0;
//}
