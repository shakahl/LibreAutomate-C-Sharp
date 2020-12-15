// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"

#include <string>
#include <vector>

HMODULE s_hModule;

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch(ul_reason_for_call) {
	case DLL_PROCESS_ATTACH:
		s_hModule = hModule;
		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

#define EXPORT extern "C" __declspec(dllexport)
#define lenof(x) (sizeof(x) / sizeof(*(x)))


//////////ComPointer//////////////////////////

template <class T> class ComPointer
{
	T* i;
public:
	ComPointer() { i = nullptr; }

	~ComPointer() { if(i) i->Release(); }

	//Calls from->QueryInterface(__uuidof(T), this). Releases previous self if need.
	HRESULT QI(IUnknown* from)
	{
		IUnknown* u = (IUnknown*)i; i = nullptr;
		HRESULT hr = from->QueryInterface(__uuidof(T), (void**)&i);
		if(u) u->Release();
		return hr;
	}

	//Calls CoCreateInstance. Releses previous self if need.
	HRESULT CreateInstance(REFCLSID clsid, DWORD context = CLSCTX_INPROC_SERVER)
	{
		IUnknown* u = (IUnknown*)i; i = 0;
		HRESULT hr = CoCreateInstance(clsid, 0, context, __uuidof(T), (void**)&i);
		if(u) u->Release();
		return hr;
	}

	//If not nullptr, calls Release and sets = nullptr.
	void Clear() { if(i) { i->Release(); i = 0; } }

	void Swap(T*& ii) { T* _i = i; i = ii; ii = _i; }

	//Same as this=from in scripting. Calls from->AddRef if not nullptr and this->Release if not nullptr.
	void Assign(T* from)
	{
		T* u = i; i = nullptr;
		if(from) { from->AddRef(); i = from; }
		if(u) u->Release();
	}

	T* operator->() { return i; }
	operator T* () { return i; } //!! don't use with =
	T** operator&() { return &i; } //note: then cannot not use parameters ComPointer<IFace>*, instead use IFace**. This operator is more useful.
};

////////////

#pragma comment(lib, "taskschd.lib")

HRESULT _GetTS(ComPointer<ITaskService>& ts)
{
	HRESULT hr = ts.CreateInstance(CLSID_TaskScheduler); if(hr!=0) return hr;
	VARIANT vd = {}; vd.vt = VT_ERROR; vd.scode = DISP_E_PARAMNOTFOUND;
	return ts->Connect(vd, vd, vd, vd);
}

HRESULT CreateSchedulerTask(LPCWSTR dir)
{
	std::wstring xml = LR"(<?xml version='1.0' encoding='UTF-16'?>
<Task version='1.3' xmlns='http://schemas.microsoft.com/windows/2004/02/mit/task'>

<RegistrationInfo>
<Author>Au</Author>
</RegistrationInfo>

<Principals>
<Principal id='Author'>
<UserId>S-1-5-18</UserId>
</Principal>
</Principals>

<Settings>
<MultipleInstancesPolicy>Parallel</MultipleInstancesPolicy>
<DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>
<StopIfGoingOnBatteries>false</StopIfGoingOnBatteries>
<ExecutionTimeLimit>PT0S</ExecutionTimeLimit>
<Priority>5</Priority>
</Settings>

<Actions Context='Author'>
<Exec>
<Command>)";
	xml += dir;
	xml += LR"(Au.CL.exe</Command>
<Arguments>/s $(Arg0)</Arguments>
</Exec>
</Actions>

</Task>
)";

	ComPointer<ITaskService> ts;
	HRESULT hr = _GetTS(ts); if(hr != 0) return hr;

	_bstr_t folder = L"Au";
	_bstr_t task = L"Aedit";
	_variant_t sddl = L"D:AI(A;;FA;;;SY)(A;;FA;;;BA)(A;;GRGX;;;WD)"; //everyone read execute
	VARIANT vd = {}; vd.vt = VT_ERROR; vd.scode = DISP_E_PARAMNOTFOUND;

	ComPointer<ITaskFolder> tf;
	hr = ts->GetFolder(folder, &tf);
	if(hr == 0) {
		tf->DeleteTask(task, 0); //we use DeleteTask/TASK_CREATE, because TASK_CREATE_OR_UPDATE does not update task file's security
	} else {
		ComPointer<ITaskFolder> tf0;
		hr = ts->GetFolder(nullptr, &tf0);
		if(hr == 0) hr = tf0->CreateFolder(folder, sddl, &tf);
		if(hr != 0) return hr;
	}

	ComPointer<IRegisteredTask> rt;
	hr = tf->RegisterTask(task, _bstr_t(xml.c_str()), TASK_CREATE, vd, vd, TASK_LOGON_SERVICE_ACCOUNT, sddl, &rt);

	return hr;
}

HRESULT DeleteSchedulerTask()
{
	ComPointer<ITaskService> ts;
	HRESULT hr = _GetTS(ts); if(hr != 0) return hr;

	ComPointer<ITaskFolder> tf;
	hr = ts->GetFolder(_bstr_t(L"Au"), &tf);
	if(hr == 0) hr = tf->DeleteTask(_bstr_t(L"Aedit"), 0);
	return hr == 0x80070002 ? 0 : hr; //0x80070002 if file or folder does not exists
}

////////////

//Unloads AuCpp.dll from all processes except.
//flags: 1 wait less.
EXPORT void Cpp_Unload(DWORD flags = 0)
{
	int less = flags & 1 ? 5 : 1;
	DWORD_PTR res;
	std::vector<HWND> a;

	//close acc agent windows
	for(HWND w = 0; w = FindWindowExW(HWND_MESSAGE, w, L"AuCpp_IPA_1", nullptr); ) a.push_back(w);
	int n = a.size();
	if(n > 0) {
		for(int i = 0; i < n; i++) SendMessageTimeout(a[i], WM_CLOSE, 0, 0, SMTO_ABORTIFHUNG, 5000 / less, &res);
		a.clear();
		Sleep(n * 50);
	}

	//unload from processes where loaded by the clipboard hook
	SendMessageTimeout(HWND_BROADCAST, 0, 0, 0, SMTO_ABORTIFHUNG, 1000 / less, &res);
	for(HWND w = 0; w = FindWindowExW(HWND_MESSAGE, w, nullptr, nullptr); ) a.push_back(w);
	for(int i = 0; i < a.size(); i++) SendMessageTimeout(a[i], 0, 0, 0, SMTO_ABORTIFHUNG, 1000 / less, &res);
	Sleep(500 / less);
}

////////////

EXPORT HRESULT Cpp_Install(int step, LPCWSTR dir)
{
	switch(step) {
	case 0:
		Cpp_Unload();
		return 0;
	case 2:
		return CreateSchedulerTask(dir);
	}

	return 1;
}

EXPORT HRESULT Cpp_Uninstall(int step)
{
	switch(step) {
	case 0:
		Cpp_Unload();
		return 0;
	case 1:
		return DeleteSchedulerTask();
	}

	return 1;
}
