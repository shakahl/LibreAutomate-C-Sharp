#include "stdafx.h"
#include "util.h"

#pragma comment(lib, "oleacc.lib")

const int MaxChildren = 10000;

namespace acc
{
	struct _Acc
	{
		IAccessible* a;
		long elem;

		_Acc() : a(nullptr), elem(0) {}
		_Acc(IAccessible* a, int elem) : a(a), elem(elem) {}

		//note: calls Relese only if elem is 0. Else a is considered not owned by this.
		~_Acc() { if(a != nullptr && elem == 0) a->Release(); }

		void Dispose() {
			if(a != nullptr) { a->Release(); a = nullptr; }
			elem = 0;
		}
	};

	/// <summary>
	/// Calls QueryInterface to get IAccessible from IDispatch idisp. Releases idisp.
	/// Returns HRESULT.
	/// </summary>
	/// <param name="idisp">If nullptr, returns E_FAIL.</param>
	/// <param name="iacc">Result.</param>
	HRESULT FromIDispatch(IDispatch* idisp, OUT IAccessible*& iacc)
	{
		iacc = nullptr;
		if(idisp == nullptr) return E_FAIL;
		HRESULT hr = idisp->QueryInterface(IID_IAccessible, (void**)&iacc);
		idisp->Release();
		if(hr == 0 && iacc == nullptr) hr = E_FAIL;
		return hr;
	}

	HRESULT get_accChild(IAccessible* a, long elem, OUT IAccessible*& aChild)
	{
		aChild = nullptr;
		IDispatch* idisp = nullptr;
		HRESULT hr = a->get_accChild(_variant_t(elem), &idisp);
		if(hr == 0) hr = FromIDispatch(idisp, aChild);
		return hr;
	}

	static HRESULT FromVARIANT(IAccessible* parent, IN VARIANT& v, OUT _Acc&a, bool tryGetObjectFromId = false)
	{
		assert(a.a == nullptr && a.elem == 0);
		int hr = 0;
		assert(v.pdispVal != nullptr); //bug in our code or AO
		if(v.pdispVal == nullptr) hr = E_FAIL;
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


}

class AccFinder
{

	_bstr_t _name;

public:
	AccFinder(LPCWSTR name)
	{
		MaxLevel = 1000;
		_name = name;
	}

	IAccessiblePtr Result;

	int MaxLevel;

	bool FindIn(HWND w)
	{
		return _FindIn(w);



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
		for(;;) {
			acc::_Acc a;
			if(!c.GetNext(a)) break;
			bool skipChildren;
			if(_Match(a.a, a.elem, skipChildren, level)) return true;
			if(skipChildren) continue;
			if(level >= MaxLevel) continue;
			if(_FindIn(a.a, level + 1)) return true;
		} //now a.a is released if a.elem==0
		return false;
	}

	bool _Match(IAccessible* a, long e, OUT bool& skipChildren, int level)
	{
		skipChildren = e!=0;
		//Print(level);
		_bstr_t b;
		if(0 != a->get_accName(_variant_t(e), b.GetAddress())) return false;
		//Print(b);
		if(_name != b) return false;

		Result.Attach(a, true);
		return true;
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
			_v = nullptr;
			_count = -1;
			_i = 0;
			_startAtIndex = startAtIndex;
			_exactIndex = exactIndex;
			_reverse = reverse;
		}

		~_Children()
		{
			if(_v != nullptr) {
				while(_count > 0) VariantClear(&_v[--_count]); //info: it's OK to dispose variants for which FromVARIANT was called because then vt is 0 and Dispose does nothing
				auto t = _v; _v = nullptr; free(t);
			}
		}


		bool GetNext(OUT acc::_Acc& a)
		{
			assert(a.a == nullptr && a.elem == 0);
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
			//note: don't call get_accChildCount here.
			//	With Firefox and some other apps it makes almost 2 times slower. With others same speed.

			const int nStack = 100; //info: fast even with 10 or 7, but 5 makes slower. Just slightly faster with 100. Not faster with 30 etc.
			auto v = (VARIANT*)_alloca(sizeof(VARIANT) * nStack);
			long n = 0;
			int hr = AccessibleChildren(_parent, 0, nStack, v, &n);
			if(hr < 0) { /*Debug_.PrintHex(hr);*/ n = 0; } //never noticed

			if(n == nStack) { //probably there are more children
				_parent->get_accChildCount(&n); //note: some objects return 0 or 1, ie less than AccessibleChildren, and HRESULT is usually 0. Noticed this only in IE, when c_acBufferSize<10.
				if(n != nStack) { //yes, more children
					for(int i = nStack; i > 0;) VariantClear(&v[--i]);
					if(n > MaxChildren) { //protection from AO such as LibreOffice Calc TABLE that has 1073741824 children. Default 10000.
						n = 0;
					} else {
						if(n < nStack) n = 1000; //get_accChildCount returned error or incorrect value
						_v = (VARIANT*)calloc(n, sizeof(VARIANT));
						hr = AccessibleChildren(_parent, 0, n, _v, &n); //note: iChildStart must be 0, else not always gets all children
						if(hr < 0) { /*Debug_.PrintHex(hr);*/ n = 0; }
					}
				}
			}

			if(n > 0 && _v == nullptr) {
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


LRESULT TestAcc(HWND w, LPCWSTR name)
{
	AccFinder f(name);
	if(!f.FindIn(w)) return 0;

	auto R = LresultFromObject(IID_IAccessible, 0, f.Result);
	//f.Result.Release(); //probably don't need
	return R;
}

//LRESULT TestAcc(HWND w)
//{
//	IAccessiblePtr a;
//	if(AccessibleObjectFromWindow(w, OBJID_CLIENT, IID_IAccessible, (void**)(IAccessible**)&a)) return 0;
//
//	auto R = LresultFromObject(IID_IAccessible, 0, a);
//	a.Release();
//	return R;
//}
