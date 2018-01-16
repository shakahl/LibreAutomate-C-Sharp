#include "stdafx.h"
#include "cpp.h"
#include "acc.h"

namespace outproc
{
int AccEnableChrome(HWND w, bool checkClassName);
}

namespace {

//Returns: 1 Chrome, 2 Java, 3 OpenOffice, 0 none.
int _IsSpecWnd(HWND w, bool onlyChrome)
{
	if(onlyChrome) return wnd::ClassNameIs(w, L"Chrome*");
	return wnd::ClassNameIs(w, { L"Chrome*", L"SunAwt*", L"SALFRAME" });
}

bool _IsLink(IAccessible* par)
{
	switch(ao::get_accRole(par)) { case ROLE_SYSTEM_LINK: case ROLE_SYSTEM_PUSHBUTTON: return true; }
	return false;
}

void _FromPoint_GetLink(ref IAccessible*& a, ref long& elem)
{
	IAccessible* parent = null;
	if(elem != 0) parent = a; else if(0 != ao::get_accParent(a, out parent)) return;
	if(_IsLink(parent)) {
		if(elem != 0) elem = 0; else util::Swap(ref a, ref parent);
	}
	if(parent != a) parent->Release();
	//note: ignore role. Because it can be anything, not only TEXT, STATICTEXT, GRAPHIC.
	//rejected: support 2 levels, eg youtube right-side list.
	//	Can be even more levels, and multiple children of LINK, some of them may be useful for automation.
	//	Often they have default action "jump" and value=URL.
	//	Then also usually have state LINKED. But many objects don't have this state.
}
} //namespace

//flags: 1 get UIA, 2 prefer LINK.
EXPORT HRESULT Cpp_AccFromPoint(POINT p, int flags, out Cpp_Acc& aResult)
{
	//Perf.First();
	aResult.Zero();

	IAccessiblePtr iacc; long elem = 0;

	HWND wFP = WindowFromPoint(p), wTL = GetAncestor(wFP, GA_ROOT);
	if(!wTL) return 1; //let the caller retry

	bool screenReader = false;
	eAccMiscFlags miscFlags = (eAccMiscFlags)0;
	int specWnd = _IsSpecWnd(wTL, wTL != wFP);
	if(specWnd) {
		if(specWnd == 2) { //Java
			WINDOWINFO wi; wi.cbSize = sizeof(wi);
			if(GetWindowInfo(wTL, &wi) && PtInRect(&wi.rcClient, p)) {
				auto ja = AccJavaFromPoint(p, wTL);
				if(ja != null) {
					iacc.Attach(ja);
					miscFlags |= eAccMiscFlags::Java;
					flags &= ~2;
				}
			}
		} else if(!(WinFlags::Get(wTL)&eWinFlags::AccEnableMask)) {
			if(specWnd == 1) { //Chrome
				outproc::AccEnableChrome(wTL, false);
				//note: now can get wrong AO, although the above func waits for new good DOCUMENT (max 1.5 s).
				//	Chrome updates web page AOs lazily. The speed depends on web page. Can get wrong AO even after loong time.
				//	Or eg can be good AO, but some its properties are still not set.
				//	This func doesn't know what AO must be there, and cannot wait.
				//	Instead, where need such reliability, the caller script can eg wait for certain AO (role LINK etc) at that point.
			} else { //OpenOffice
				screenReader = true;
				//OpenOffice bug: crashes on exit if AccessibleObjectFromPoint or AccessibleObjectFromWindow called with SPI_GETSCREENREADER.
				//	Could not find a workaround.
				//	Inspect.exe too.
				//	Does not if SPI_GETSCREENREADER was when starting OO.
				//	Does not if we get certain AO (eg DOCUMENT), eiher from point or when searching, now or later. Crashes eg if the AO is a toolbar button.
				//	Tested only with Writer.
				//	tested: inproc does not help.
			}
		}
	}

	int role = 0;
	if(!iacc) {
		ao::TempSetScreenReader tsr; if(screenReader) tsr.Set(wTL);
		if(flags & 1) { //UIA
			HRESULT hr = AccUiaFromPoint(p, &iacc);
			if(hr) return hr;
			miscFlags |= eAccMiscFlags::UIA;
		} else {
			VARIANT v;
			HRESULT hr = AccessibleObjectFromPoint(p, &iacc, &v);
			if(hr == 0 && !iacc) hr = E_FAIL;
			if(hr != 0) return hr;
			assert(v.vt == VT_I4 || v.vt == 0);
			elem = v.vt == VT_I4 ? v.lVal : 0;

			//CONSIDER: inproc. Because now the AO has the bugs (toolbar button name, focus).
			//	Also maybe faster eg with Firefox. Although now not too slow.

			//UIA?
			if(elem == 0 && specWnd == 0) {
				//Perf.Next();
				role = ao::get_accRole(iacc);
				if(role == ROLE_SYSTEM_CLIENT || role == ROLE_SYSTEM_PANE) { //info: Edge web page is PANE
					//Perf.Next();
					IAccessiblePtr auia; long x, y, wid1, hei1, wid2, hei2;
					if(0 == AccUiaFromPoint(p, &auia)) { //speed: similar to AOFP
						//Perf.Next();
						//if auia is smaller than iacc, use auia. Dirty, but in most cases works well.
						ao::VE ve;
						if(0 == iacc->accLocation(&x, &y, &wid1, &hei1, ve) && 0 == auia->accLocation(&x, &y, &wid2, &hei2, ve)) {
							__int64 sq1 = (__int64)wid1*hei1, sq2 = (__int64)wid2*hei2;
							if(sq2 < sq1 && sq2>0) {
								iacc.Release();
								iacc = auia.Detach();
								miscFlags |= eAccMiscFlags::UIA;
							}
							//Perf.NW();
						}
					}
				}
			}
		}
	}

	if(flags & 2) _FromPoint_GetLink(ref (IAccessible*&)iacc, ref elem);

	aResult.acc = iacc.Detach(); aResult.elem = elem; aResult.misc.flags = miscFlags;
	aResult.SetRole(role);
	return 0;
}

//w - must be the focused control or window.
//flags: 1 get UIA.
EXPORT HRESULT Cpp_AccGetFocused(HWND w, int flags, out Cpp_Acc& aResult)
{
	aResult.Zero();

	HWND wTL = GetAncestor(w, GA_ROOT);
	//if(!wTL || wTL != GetForegroundWindow()) return 1; //return quickly, anyway would fail. No, does not work with Edge.
	if(!wTL) return 1;

	bool screenReader = false;
	int specWnd = _IsSpecWnd(wTL, wTL != w);
	if(specWnd) {
		if(specWnd == 2) { //Java
			auto ja = AccJavaFromWindow(w, true);
			if(ja != null) {
				aResult.acc = ja;
				aResult.misc.flags = eAccMiscFlags::Java;
				return 0;
			}
		} else if(!(WinFlags::Get(w)&eWinFlags::AccEnableMask)) {
			if(specWnd == 1) { //Chrome
				outproc::AccEnableChrome(w, false);
			} else { //OpenOffice
				screenReader = true;
				//OpenOffice bug: crashes. More info in Cpp_AccFromPoint.
			}
		}
	}

	if(flags & 1) { //UIA
		ao::TempSetScreenReader tsr; if(screenReader) tsr.Set(w);
		HRESULT hr = AccUiaFocused(&aResult.acc);
		if(hr) return hr;
		aResult.misc.flags = eAccMiscFlags::UIA;

		//Edge bug: gets wrong element. Without this flag works well.
	} else {
		IAccessiblePtr aw;
		HRESULT hr = ao::AccFromWindow(w, OBJID_CLIENT, &aw, screenReader); if(hr) return hr;

		AccRaw a1(aw, 0), a2; bool isThis;
		hr = a1.DescendantFocused(out a2, out isThis); if(hr) return hr;
		if(isThis) {
			aw.Detach();
			aResult = a1;
		} else {
			aResult = a2;

			//surprise: works with Edge too.
			//	The AO is in w, which is not in the main Edge window. It is in another process.
			//	It means that Edge has AOs, but they are not in the main Edge window, therefore Find and FromXY don't see them.

			//never mind: cannot get focused AO of other UIA-only windows, eg Java FX.
		}
	}
	return 0;
}
