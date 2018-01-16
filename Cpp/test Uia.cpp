#include "stdafx.h"
#include "cpp.h"

_COM_SMARTPTR_TYPEDEF(IUIAutomation, __uuidof(IUIAutomation));
_COM_SMARTPTR_TYPEDEF(IUIAutomationElement, __uuidof(IUIAutomationElement));
_COM_SMARTPTR_TYPEDEF(IUIAutomationCondition, __uuidof(IUIAutomationCondition));
_COM_SMARTPTR_TYPEDEF(IUIAutomationElementArray, __uuidof(IUIAutomationElementArray));

bool ElementFromWindow(IUIAutomationPtr f, HWND w, IUIAutomationElementPtr& e)
{
	if (f->ElementFromHandle(w, &e)) return false;
	return true;
}

class UiaFinder
{
	IUIAutomationPtr _f;
	Bstr _name;
	IUIAutomationConditionPtr _condRaw;
public:
	IUIAutomationElementPtr eFound;

	UiaFinder() {
		if (_f.CreateInstance(__uuidof(CUIAutomation), null, CLSCTX_INPROC_SERVER)) return;
		if (_f->get_RawViewCondition(&_condRaw)) return;
	}

	int Find(HWND w, STR name) {
		if (!_f) return 1;

		IUIAutomationElementPtr ew;
		if (!ElementFromWindow(_f, w, ew)) return 2;

		return Find(ew, name);
	}

	//int Find(IUIAutomationElementPtr ew, STR name) {

	//	IUIAutomationConditionPtr cond;
	//	if (_f->CreatePropertyCondition(UIA_NamePropertyId, _variant_t(name), &cond)) return 3;

	//	if (ew->FindFirst(TreeScope_Descendants, cond, &eFound)) return 4;

	//	return 0;
	//}

	int Find(IUIAutomationElementPtr ew, STR name) {
		if (!_f) return 1;
		_name = name;

		return _Find(ew, 0);
	}

private:
	int _Find(IUIAutomationElementPtr ew, int level) {

		IUIAutomationElementArrayPtr a;
		if (ew->FindAll(TreeScope_Children, _condRaw, &a)) return 4;

		int n = 0;
		if (a->get_Length(&n)) return 5;

		for (int i = 0; i < n; i++)
		{
			IUIAutomationElementPtr e;
			if (a->GetElement(i, &e)) return 6;

			Bstr b;
			if (0 == e->get_CurrentName(&b)) {
				//Printf(L"%i %s", level, (STR)b);
				if (b == _name) {
					eFound.Attach(e.Detach());
					break;
				}
			}

			int k = _Find(e, level + 1);
			if (k) return k;
			if (eFound) break;
		}

		return 0;
	}
};

#if true
LRESULT TestUia(HWND w)
{
	UiaFinder x;
	int err = x.Find(w, L"Untitled");
	if (err) return err;

	if (x.eFound) {
		Bstr b;
		if (x.eFound->get_CurrentName(&b)) return 50;
		Print(b);
	}
	else Print(L"not found");

	return 0;
}
#else
LRESULT TestUia(HWND w)
{
	//Print(L"TestUia");

	//_TODO: CoInitialize if need.

	//IUIAutomationPtr f(__uuidof(CUIAutomation), null, CLSCTX_INPROC_SERVER);
	IUIAutomationPtr f;
	if (f.CreateInstance(__uuidof(CUIAutomation), null, CLSCTX_INPROC_SERVER)) return 1;
	//Print((__int64)f.GetInterfacePtr());

	IUIAutomationElementPtr ew;
	if (!ElementFromWindow(f, w, ew)) return 2;

	IUIAutomationConditionPtr cond;
	if (f->CreatePropertyCondition(UIA_NamePropertyId, _variant_t(L"Untitled"), &cond)) return 3;

	IUIAutomationElementPtr e;
	if (ew->FindFirst(TreeScope_Descendants, cond, &e)) return 4;
	if (!e) return 100;

	_bstr_t b;
	if (e->get_CurrentName(&b.GetBSTR())) return 5;

	Print(b);

	return 0;
}
#endif
