#include <oleauto.h>
typedef SAFEARRAY* ARRAY; //QM ARRAY in C++ is SAFEARRAY*

//Creates or resizes 1-dim ARRAY.
//vt - array type VT_ constant. VT_I4 for int, VT_BSTR for BSTR, etc. Used only when creating.
//Sets added elements to 0/empty. Frees removed elements if need (BSTR, VARIANT etc).
extern "C" __declspec(dllexport)
HRESULT ResizeSafeArray(ARRAY& a, int n, int vt)
{
	HRESULT hr;
	SAFEARRAYBOUND b={n};
	//int nOld=0;
	if(a) //resize
	{
		//nOld=a->rgsabound[0].cElements;
		if(hr=SafeArrayRedim(a, &b)) return hr;
	}
	else //create
	{
		a=SafeArrayCreate(vt, 1, &b);
		if(!a) return E_OUTOFMEMORY;
	}

	return 0;
}
