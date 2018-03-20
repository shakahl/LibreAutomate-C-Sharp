#include "stdafx.h"
#include "cpp.h"
#include "acc.h"
#include "JAB.h"

namespace jab
{
_JApi s_api;

class JAccessible : public IAccessible
{
	long _cRef;
	long _vmID;
	JObject _jo;

public:
#pragma region ctor, dtor
	JAccessible(JObject jo, int vmID)
	{
		//PRINTS(L"JAccessible");
		_cRef = 1;
		_jo = jo; _vmID = vmID;
	}

	~JAccessible()
	{
		//PRINTS(L"~JAccessible");
		s_api.releaseJavaObject(_vmID, _jo);
		_ReleaseObjectInfoCache(); //a new variable may be allocated in the same place soon
	}
#pragma endregion

#pragma region IDispatch
	STDMETHODIMP QueryInterface(REFIID iid, void** ppv)
	{
		if(iid == IID_IAccessible || iid == IID_IDispatch || iid == IID_IUnknown) {
			InterlockedIncrement(&_cRef);
			*ppv = this;
			return 0;
		}
		*ppv = 0;
		return E_NOINTERFACE;
	}

	STDMETHODIMP_(ULONG) AddRef()
	{
		return InterlockedIncrement(&_cRef);
	}

	STDMETHODIMP_(ULONG) Release()
	{
		long ret = InterlockedDecrement(&_cRef);
		if(!ret) delete this;
		return ret;
	}

	STDMETHODIMP GetTypeInfoCount(UINT *pctinfo) { return E_NOTIMPL; }

	STDMETHODIMP GetTypeInfo(UINT iTInfo, LCID lcid, ITypeInfo** ppTInfo) { return E_NOTIMPL; }

	STDMETHODIMP GetIDsOfNames(REFIID riid, LPOLESTR *rgszNames, UINT cNames, LCID lcid, __RPC__out_ecount_full(cNames) DISPID *rgDispId) { return E_NOTIMPL; }

	STDMETHODIMP Invoke(DISPID dispIdMember, REFIID riid, LCID lcid, WORD wFlags, DISPPARAMS *pDispParams, VARIANT *pVarResult, EXCEPINFO *pExcepInfo, UINT *puArgErr) { return E_NOTIMPL; }
#pragma endregion

#pragma region IAccessible
	STDMETHODIMP get_accParent(IDispatch** ppdispParent)
	{
		//PRINTS(__FUNCTIONW__);
		*ppdispParent = null;
		JObject jo;

		//Workaround JAB bug: if eg dialog has owner window, GetAccessibleParentFromContext(dialog) gets a half-valid object of the owner window.
		//Instead we'll get standard WINDOW object. Need it for WindowFromAccessibleObject.
		//Makes slower 2-3 times, but still not too slow.
		//tested: getObjectDepth cannot be used. It then returns 1, not 0. Same speed.
		jo = s_api.getTopLevelObject(_vmID, _jo);
		if(jo != 0) //gets correct object, not of owner window
		{
			if(s_api.isSameObject(_vmID, _jo, jo)) {
				s_api.releaseJavaObject(_vmID, jo);
				HWND w = s_api.getHWNDFromAccessibleContext(_vmID, _jo);
				if(w && 0 == AccessibleObjectFromWindow(w, OBJID_WINDOW, IID_IAccessible, (void**)ppdispParent)) return 0;
				return 1;
			}
			s_api.releaseJavaObject(_vmID, jo);
		}

		if((jo = s_api.getAccessibleParentFromContext(_vmID, _jo)) == 0) return 1;
		*ppdispParent = new JAccessible(jo, _vmID);
		return 0;
	}

	STDMETHODIMP get_accChildCount(long *pcountChildren)
	{
		//PRINTS(__FUNCTIONW__);
		auto k = _GetObjectInfo(); if(k == null) return E_FAIL;
		*pcountChildren = k->childrenCount;
		return 0;
	}

	STDMETHODIMP get_accChild(VARIANT varChild, IDispatch** ppdispChild)
	{
		//PRINTS(__FUNCTIONW__);
		int i = varChild.lVal - 1;
		if(varChild.vt != VT_I4 || i < 0) return E_INVALIDARG;
		auto jo = s_api.getAccessibleChildFromContext(_vmID, _jo, i);
		if(jo == 0) {
			auto k = _GetObjectInfo();
			if(k != null && i >= k->childrenCount) return E_INVALIDARG;
			return E_FAIL;
		}
		*ppdispChild = new JAccessible(jo, _vmID);
		return 0;
	}

	STDMETHODIMP get_accName(VARIANT varChild, out BSTR* pszName)
	{
		//PRINTS(__FUNCTIONW__);
		if(_InvalidVarChildParam(ref varChild)) return E_INVALIDARG;
		WCHAR m[MAX_STRING_SIZE];
		if(!s_api.getVirtualAccessibleName(_vmID, _jo, m, MAX_STRING_SIZE)) return E_FAIL;
		*pszName = SysAllocString(m);
		return 0;

		//getVirtualAccessibleName is better than getAccessibleContextInfo (GACI).
		//It gets names of toolbar buttons. With GACI, name empty, we can use description.
		//It gets name of related control - adjacent label etc.
		//Much faster than GACI.
		//Makes searching ~10% slower when GACI is cached, but not when role used (not called if role does not match).
	}

	STDMETHODIMP get_accValue(VARIANT varChild, out BSTR* pszValue)
	{
		//PRINTS(__FUNCTIONW__);
		if(_InvalidVarChildParam(ref varChild)) return E_INVALIDARG;
		auto k = _GetObjectInfo(); if(k == null) return E_FAIL;
		*pszValue = null;
		if(k->accessibleValue) {
			WCHAR m[MAX_STRING_SIZE];
			if(s_api.getCurrentAccessibleValueFromContext(_vmID, _jo, m, MAX_STRING_SIZE)) //speed: 20 times faster than getAccessibleContextInfo
				*pszValue = SysAllocString(m);
		} else {
			AccessibleTextInfo ati;
			if(k->accessibleText && s_api.getAccessibleTextInfo(_vmID, _jo, out &ati, 0, 0)) { //slow
				auto b = SysAllocStringLen(null, ati.charCount);
				if(ati.charCount == 0 || s_api.getAccessibleTextRange(_vmID, _jo, 0, ati.charCount - 1, b, ati.charCount)) *pszValue = b;
				else SysFreeString(b);
			}
		}
		return *pszValue == null ? 1 : 0;

		//tested: most objects don't have value/text. Value usually is numeric, eg "1" for checked checkbox. Text objects don't have value, therefore we get text instead.
		//AccessibleContextInfo::accessibleInterfaces flags, from AccessibleBridgePackages.h (cAccessibleValueInterface etc):
		//	1 value, 2 actions, 4 component, 8 selection, 0x10 table, 0x20 text, 0x40 hypertext.
	}

	STDMETHODIMP get_accDescription(VARIANT varChild, out BSTR* pszDescription)
	{
		//PRINTS(__FUNCTIONW__);
		if(_InvalidVarChildParam(ref varChild)) return E_INVALIDARG;
		auto k = _GetObjectInfo(); if(k == null) return E_FAIL;
		*pszDescription = k->description.Copy();
		return 0;
	}

	STDMETHODIMP get_accRole(VARIANT varChild, out VARIANT* pvarRole)
	{
		//PRINTS(__FUNCTIONW__);
		if(_InvalidVarChildParam(ref varChild)) return E_INVALIDARG;
		auto k = _GetObjectInfo(); if(k == null) return E_FAIL;
		return k->role.CopyTo(pvarRole);
	}

	STDMETHODIMP get_accState(VARIANT varChild, out VARIANT* pvarState)
	{
		//PRINTS(__FUNCTIONW__);
		if(_InvalidVarChildParam(ref varChild)) return E_INVALIDARG;
		auto k = _GetObjectInfo(); if(k == null) return E_FAIL;
		pvarState->vt = VT_I4;
		pvarState->lVal = k->state;
		return 0;
	}

	STDMETHODIMP get_accHelp(VARIANT varChild, out BSTR* pszHelp)
	{
		return E_NOTIMPL;
	}

	STDMETHODIMP get_accHelpTopic(BSTR* pszHelpFile, VARIANT varChild, long* pidTopic)
	{
		return E_NOTIMPL;
	}

	STDMETHODIMP get_accKeyboardShortcut(VARIANT varChild, out BSTR* pszKeyboardShortcut)
	{
		return E_NOTIMPL;
	}

	STDMETHODIMP get_accFocus(out VARIANT* pvarChild)
	{
		return E_NOTIMPL;
		//Difficult to implement so that would work correctly, ie return direct child even if focused is another descendant.
		//getAccessibleContextWithFocus is unstable. More often does not work than works.
		//Rarely used.
	}

	STDMETHODIMP get_accSelection(out VARIANT* pvarChildren)
	{
		return E_NOTIMPL;
		//Rarely used.
	}

	STDMETHODIMP get_accDefaultAction(VARIANT varChild, out BSTR* pszDefaultAction)
	{
		//PRINTS(__FUNCTIONW__);
		if(_InvalidVarChildParam(ref varChild)) return E_INVALIDARG;
		return _get_accDefaultAction(out *pszDefaultAction, true);
	}

	HRESULT _get_accDefaultAction(out BSTR& b, bool getList)
	{
		b = null;
		auto k = _GetObjectInfo(); if(k == null) return E_FAIL;
		if(!k->accessibleAction) return 1;
		std::unique_ptr<AccessibleActions> actions(new(AccessibleActions)); //130 KB
		HRESULT hr = 1; int n;
		if(s_api.getAccessibleActions(_vmID, _jo, actions.get()) && (n = actions->actionsCount) > 0 && n <= MAX_ACTION_INFO) {
			hr = 0;
			if(!getList || n == 1) b = SysAllocString(actions->actionInfo[0].name);
			else {
				str::StringBuilder t;
				for(int i = 0; i < n; i++) {
					if(i) t << L", ";
					t << actions->actionInfo[i].name;
				}
				b = t.ToBSTR();
			}
		}
		return hr;
	}

	STDMETHODIMP accSelect(long flagsSelect, VARIANT varChild)
	{
		//PRINTS(__FUNCTIONW__);
		if(_InvalidVarChildParam(ref varChild)) return E_INVALIDARG;
		if(flagsSelect&SELFLAG_EXTENDSELECTION) return E_INVALIDARG;
		if(flagsSelect&(SELFLAG_TAKESELECTION | SELFLAG_ADDSELECTION | SELFLAG_REMOVESELECTION)) {
			auto k = _GetObjectInfo(); if(k == null) return E_FAIL;
			auto ap = s_api.getAccessibleParentFromContext(_vmID, _jo); if(ap == 0) return E_FAIL;
			_ReleaseObjectInfoCache(); //the action changes object state
			int i = k->indexInParent;
			if(flagsSelect&SELFLAG_TAKESELECTION) s_api.clearAccessibleSelectionFromContext(_vmID, ap);
			if(flagsSelect&(SELFLAG_TAKESELECTION | SELFLAG_ADDSELECTION)) s_api.addAccessibleSelectionFromContext(_vmID, ap, i);
			if(flagsSelect&SELFLAG_REMOVESELECTION) s_api.removeAccessibleSelectionFromContext(_vmID, ap, i);
			s_api.releaseJavaObject(_vmID, ap);
		}
		if(flagsSelect&SELFLAG_TAKEFOCUS) {
			_ReleaseObjectInfoCache(); //the action changes object state
			if(!s_api.requestFocus(_vmID, _jo)) return E_FAIL;
		}
		return 0;
	}

	STDMETHODIMP accLocation(out long* pxLeft, out long* pyTop, out long* pcxWidth, out long* pcyHeight, VARIANT varChild)
	{
		//PRINTS(__FUNCTIONW__);
		if(_InvalidVarChildParam(ref varChild)) return E_INVALIDARG;
		auto k = _GetObjectInfo(); if(k == null) return E_FAIL;
		*pxLeft = k->x; *pyTop = k->y; *pcxWidth = k->width; *pcyHeight = k->height;
		return 0;
	}

	STDMETHODIMP accNavigate(long navDir, VARIANT varStart, out VARIANT* pvarEndUpAt)
	{
		//Print("accNavigate", navDir, varStart.vt, varStart.value);

		//WindowFromAccessibleObject (WFAO) at first calls this with an undocumented navDir 10.
		//	tested: accNavigate(10) for a standard Windows control returns VARIANT(VT_I4, hwnd).
		//	If we return window handle, WFAO does not call get_accParent. Else also calls it for each ancestor.
		if(navDir == 10 && varStart.vt == VT_I4) {
			HWND w = _GetHWND();
			if(w == 0) return 1;
			pvarEndUpAt->vt = VT_I4;
			pvarEndUpAt->lVal = (int)(LPARAM)w;
			return 0;
		}

		if(navDir < NAVDIR_UP || navDir > NAVDIR_LASTCHILD) return E_INVALIDARG;
		if(_InvalidVarChildParam(ref varStart)) return E_INVALIDARG;
		JObject ac = 0;

		if(navDir >= NAVDIR_NEXT && navDir <= NAVDIR_LASTCHILD) {
			int iChild = -1; JObject ap = 0; _JObjectInfo* k = null;
			if(navDir != NAVDIR_FIRSTCHILD && null == (k = _GetObjectInfo())) return 1;
			switch(navDir) {
			case NAVDIR_FIRSTCHILD: iChild = 0; ap = _jo; break; //GetAccessibleChildFromContext will fail if this does not have children
			case NAVDIR_LASTCHILD: iChild = k->childrenCount - 1; ap = _jo; break;
			case NAVDIR_NEXT: iChild = k->indexInParent + 1; break; //GetAccessibleChildFromContext will fail if this is the last
			case NAVDIR_PREVIOUS: iChild = k->indexInParent - 1; break;
			}
			if(iChild >= 0) {
				if(ap == 0) ap = s_api.getAccessibleParentFromContext(_vmID, _jo);
				if(ap != 0) { ac = s_api.getAccessibleChildFromContext(_vmID, ap, iChild); if(ap != _jo) s_api.releaseJavaObject(_vmID, ap); }
			}
		}

		if(ac == 0) return 1;
		pvarEndUpAt->pdispVal = new JAccessible(ac, _vmID);
		pvarEndUpAt->vt = VT_DISPATCH;
		return 0;
	}

	STDMETHODIMP accHitTest(long xLeft, long yTop, out VARIANT* pvarChild)
	{
		return E_NOTIMPL;
		//Rarely used.
	}

	STDMETHODIMP accDoDefaultAction(VARIANT varChild)
	{
		BSTR name = null; int len = 0; Bstr name_;
		if(varChild.vt == VT_BSTR) {
			len = SysStringLen(name = varChild.bstrVal);
			if(len >= SHORT_STRING_SIZE) return E_INVALIDARG;
		} else if(_InvalidVarChildParam(ref varChild)) return E_INVALIDARG;

		if(len == 0) {
			if(_InvalidVarChildParam(ref varChild)) return E_INVALIDARG;
			int hr = _get_accDefaultAction(out name, false);
			if(hr != 0) return hr == 1 ? DISP_E_MEMBERNOTFOUND : E_FAIL;
			name_.Attach(name);
			len = name_.Length();
		}
		_ReleaseObjectInfoCache(); //the action may change object props
		AccessibleActionsToDo atd; //16 KB
		atd.actionsCount = 1;
		memcpy(atd.actions, name, (len + 1) * 2);
		jint failure;
		if(!s_api.doAccessibleActions(_vmID, _jo, &atd, &failure)) {
			//PRINTI(failure); //0-based index of the first action that failed or is unknown, or -1 if not action-related
			return E_FAIL;
		}
		return 0;
	}

	STDMETHODIMP put_accName(VARIANT varChild, BSTR szName)
	{
		return E_NOTIMPL;
		//Rarely used, deprecated.
	}

	STDMETHODIMP put_accValue(VARIANT varChild, BSTR szValue)
	{
		if(_InvalidVarChildParam(ref varChild)) return E_INVALIDARG;
		if(!s_api.setTextContents(_vmID, _jo, szValue)) return 1;
		_ReleaseObjectInfoCache();
		return 0;
	}
#pragma endregion

#pragma region get object info

	//s_api.getAccessibleContextInfo gets multiple properties (name, state, etc) that are then used by multiple JAccessible functions.
	//It is slow etc. We call it once and store the retrieved/converted properties in a thread-static variable t_joInfo. Not in JAccessible because big.
	//t_joInfo is used for all JAccessible instances, but owned by a single JAccessible instance at a time.

	//internal
	struct _JObjectInfo
	{
		Bstr /*name,*/ description, role;
		int indexInParent, childrenCount;
		int x, y, width, height;
		bool accessibleComponent, accessibleAction, accessibleSelection, accessibleText, accessibleValue;
		int state;
		long time;
		JAccessible* owner;

		void Init(const AccessibleContextInfo& c)
		{
			//name = c.name;
			description = c.description;
			role = c.role_en_US;
			indexInParent = c.indexInParent; childrenCount = c.childrenCount;
			x = c.x; y = c.y; width = c.width; height = c.height;
			accessibleComponent = c.accessibleComponent != 0; accessibleAction = c.accessibleAction != 0; accessibleSelection = c.accessibleSelection != 0; accessibleText = c.accessibleText != 0;
			accessibleValue = (c.accessibleInterfaces & 1) != 0;

			state = STATE_SYSTEM_READONLY | STATE_SYSTEM_UNAVAILABLE | STATE_SYSTEM_INVISIBLE;
			for(STR s = c.states_en_US; *s;) {
				STR se = s; while(*se && *se != ',') se++;
				switch(str::Switch(s, (int)(se - s), { L"busy",L"checked",L"collapsed",L"editable",L"enabled",L"expanded",L"focusable",L"focused",L"indeterminate",L"modal",L"multiselectable",L"pressed",L"resizable",L"selectable",L"selected",L"showing",L"visible" })) {
				case 1: state |= STATE_SYSTEM_BUSY; break;
				case 2: state |= STATE_SYSTEM_CHECKED; break;
				case 3: state |= STATE_SYSTEM_COLLAPSED; break;
				case 4: state &= ~STATE_SYSTEM_READONLY; break;
				case 5: state &= ~STATE_SYSTEM_UNAVAILABLE; break;
				case 6: state |= STATE_SYSTEM_EXPANDED; break;
				case 7: state |= STATE_SYSTEM_FOCUSABLE; break;
				case 8: state |= STATE_SYSTEM_FOCUSED; break;
				case 9: state |= STATE_SYSTEM_INDETERMINATE; break;
				case 10: state |= STATE_SYSTEM_HASPOPUP; break;
				case 11: state |= STATE_SYSTEM_MULTISELECTABLE; break;
				case 12: state |= STATE_SYSTEM_PRESSED; break;
				case 13: state |= STATE_SYSTEM_SIZEABLE; break;
				case 14: state |= STATE_SYSTEM_SELECTABLE; break;
				case 15: state |= STATE_SYSTEM_SELECTED; break;
				case 16: state &= ~STATE_SYSTEM_INVISIBLE; break;
				case 17: if(width > 0 && height > 0) state &= ~STATE_SYSTEM_INVISIBLE; break;
				}
				if(*se == 0) break;
				s = se + 1;
			}
		}
	};

	static _JObjectInfo& __JoInfo() {
		thread_local static _JObjectInfo t_joInfo;
		return t_joInfo;
	}

	_JObjectInfo* _GetObjectInfo()
	{
		_JObjectInfo& k = __JoInfo();
		if(k.owner != this || GetTickCount() - k.time > 10) {
			AccessibleContextInfo c;
			if(!s_api.getAccessibleContextInfo(_vmID, _jo, &c)) return null;
			k.Init(ref c);
			k.time = GetTickCount();
			k.owner = this;
		}
		return &k;
	}

	bool _IsObjectInfoCached()
	{
		_JObjectInfo& k = __JoInfo();
		return k.owner == this && GetTickCount() - k.time <= 10;
	}

	void _ReleaseObjectInfoCache()
	{
		_JObjectInfo& k = __JoInfo();
		if(k.owner == this) k.owner = null;
	}

#pragma endregion

#pragma region private
private:
	bool _InvalidVarChildParam(const VARIANT& v)
	{
		if(v.vt == 0) return false; //forgive
		return v.vt != VT_I4 || v.lVal != 0;
	}

	HWND _GetHWND()
	{
		auto joTL = s_api.getTopLevelObject(_vmID, _jo);
		if(joTL != 0) {
			HWND w = s_api.getHWNDFromAccessibleContext(_vmID, joTL);
			s_api.releaseJavaObject(_vmID, joTL);
			return w;
		}
		return 0;
	}
#pragma endregion
};

int s_inited;

DWORD WINAPI _JabThread(LPVOID param)
{
	//How JAB works:
	//_InitJab loads JAB dll and functions, and calls Windows_run.
	//Windows_run creates a hidden dialog "Access Bridge status" and posts "AccessBridge-FromJava-Hello" messages to all top-level windows.
	//Java JAB-enabled processes also have hidden "Access Bridge status" dialogs. They run in a separate thread.
	//These dialogs, when received our hello message, post back the message to our dialog. wParam is poster dialog hwnd.
	//Only when our dialog receives these messages, we can get accessible context of that Java windows.
	//Therefore we must have a message loop etc. We use this thread for it.
	//Alternatively, each thread could call Windows_run and DoEvents etc, but then we would have multiple dialogs. Also would need DoEvents before each GetAccessibleContextFromHWND etc, or it would fail with Java windows created later.
	//Note: JAB events work only in thread that called Windows_run. Currently not using events. Not tested much. When tested with JavaFerret, most events either didn't work or used to stop working etc.

	UINT m1 = RegisterWindowMessageW(L"AccessBridge-FromWindows-Hello"); //received by our hidden "Access Bridge status" dialog once after _InitJab
	UINT m2 = RegisterWindowMessageW(L"AccessBridge-FromJava-Hello"); //received by the dialog from each JAB-enabled Java window
	ChangeWindowMessageFilter(m1, 1); ChangeWindowMessageFilter(m2, 1);

	s_api.Windows_run(); //the slowest part, ~10 ms when warm CPU, else 10-33 ms

	int i, nmsg; MSG msg;
	for(i = 0; i < 10; i++) {
		for(nmsg = 0; PeekMessageW(&msg, 0, 0, 0, PM_REMOVE); nmsg++) DispatchMessageW(&msg);
		if(nmsg == 0 && i > 0) break; //non-Java windows that use JAB (probably as client) don't post back the message
		Sleep(1);
	}
	assert(i <= 2); //just to test reliability in stress conditions. When testing, was 1 almost always, once 2.

	s_inited = 1;
	SetEvent(param);

	while(GetMessageW(&msg, 0, 0, 0) > 0) DispatchMessageW(&msg);
	s_inited = 0;

	return 0;
}

bool _InitJab()
{
	if(s_inited == 0) {
		static CComAutoCriticalSection _cs; CComCritSecLock<CComAutoCriticalSection> lock(_cs);
		if(s_inited == 0) {
			s_inited = -1;
			s_api.Load();
			if(s_api.IsLoaded()) {
				//FUTURE: Time.LibSleepPrecision.LibTempSet1(1);
				CHandle ev(CreateEventW(null, true, false, null));
				DWORD tid;
				HANDLE ht = CreateThread(null, 0, _JabThread, ev, 0, &tid);
				if(ht) {
					HANDLE ha[2] = { ev, ht };
					WaitForMultipleObjects(2, ha, false, INFINITE);
					CloseHandle(ht);
				}
			}
		}
	}
	return s_inited > 0;
}

bool _NoJab() { return s_inited < 0; }

//Returns true if w process has a JAB hidden dialog ("Access Bridge status", "#32770").
//It can be a Java window or a JAB client, but not a window of this process.
//speed: faster than AccessibleObjectFromWindow. Much faster than _JApi::isJavaWindow, and don't need to load dll etc.
bool _IsJavaWindow(HWND w)
{
	if(GetWindowLong(w, GWL_STYLE)&WS_CHILD) return false;

	auto wf = WinFlags::Get(w);
	if(!!(wf&eWinFlags::AccJavaYes)) return true;
	if(!!(wf&eWinFlags::AccJavaNo)) return false;

	bool yes = false; DWORD pid = 0, pidJAB = 0;
	for(HWND wJAB = 0; wJAB = FindWindowExW(0, wJAB, L"#32770", L"Access Bridge status");) {
		if(pid == 0 && !GetWindowThreadProcessId(w, &pid)) break;
		GetWindowThreadProcessId(wJAB, &pidJAB); if(pidJAB != pid) continue;
		yes = pid != GetCurrentProcessId();
		break;
	}

	WinFlags::Set(w, yes ? eWinFlags::AccJavaYes : eWinFlags::AccJavaNo);
	return yes;
}

JObject _JObjectFromWindow(HWND w, out long& vmID, bool getFocused = false)
{
	vmID = 0;
	if(!_IsJavaWindow(w)) return 0;
	if(!_InitJab()) return 0;
	JObject jo;
	auto f = getFocused ? s_api.getAccessibleContextWithFocus : s_api.getAccessibleContextFromHWND;
	if(!f(w, out &vmID, out &jo)) return 0;
	return jo;
}

IAccessible* AccFromWindow(HWND w, bool getFocused = false)
{
	if(_NoJab()) return null;
	long vmID;
	JObject jo = _JObjectFromWindow(w, out vmID, getFocused); if(jo == 0) return null;
	return new JAccessible(jo, vmID);
}

IAccessible* AccFromPoint(POINT p, HWND w = 0)
{
	if(_NoJab()) return null;
	if(!w) w = WindowFromPoint(p);
	long vmID;
	JObject jo, ap = _JObjectFromWindow(w, out vmID); if(ap == 0) return null;
	bool ok = s_api.getAccessibleContextAt(vmID, ap, p.x, p.y, out &jo);
	if(ok && jo == 0) { //JAB bug: the API returns 0 object if the window never received a mouse message
		//PRINTS(L"workaround");
		PostMessageW(w, WM_MOUSEMOVE, 0, 0);
		for(int i = 0; i < 10; i++) {
			Sleep(15); SendMessageTimeoutW(w, 0, 0, 0, SMTO_ABORTIFHUNG, 1000, null);
			ok = s_api.getAccessibleContextAt(vmID, ap, p.x, p.y, out &jo) && jo != 0;
			if(ok) break;
		}
	}
	s_api.releaseJavaObject(vmID, ap);
	if(!ok) return null;
	return new JAccessible(jo, vmID);
}

} //namespace jab

IAccessible* AccJavaFromWindow(HWND w, bool getFocused /*= false*/)
{
	return jab::AccFromWindow(w, getFocused);
}

//Gets Java accessible object from point.
//p - point in screen coordinates.
//w - window containing the point. If 0, calls WindowFromPoint.
IAccessible* AccJavaFromPoint(POINT p, HWND w /*= 0*/)
{
	return jab::AccFromPoint(p, w);
}
