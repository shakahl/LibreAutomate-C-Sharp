#define _CRT_SECURE_NO_WARNINGS
#pragma warning(disable: 6386 6385 6255 6305 6011 28251)

//#include <stdlib.h>
#include <string.h>
#include <string>
#include <Windows.h>
#include "coreclrhost.h"

//min supported Core version.
//SHOULDDO: somehow auto-update, else will forget this when updating Core version of C# projects (<TargetFramework>netcoreapp3.1</TargetFramework>).
#define COREVER1 3
#define COREVER2 1
#define COREVER3 0

//#define PRINT
#ifdef PRINT
template<typename ... Args>
void Print(LPCSTR frm, Args ... args)
{
	HWND w = FindWindowW(L"QM_Editor", nullptr); if(w == 0) return;
	size_t size = sizeof...(Args) > 0 ? snprintf(nullptr, 0, frm, args ...) : 0;
	char* buf = size > 0 ? (char*)malloc(++size) : nullptr;
	if(buf != nullptr) {
		snprintf(buf, size, frm, args ...);
		frm = (LPCSTR)buf;
	} else {
		if(frm == nullptr) frm = "";
		size = strlen(frm) + 1;
	}
	auto u = (wchar_t*)malloc(size * 2);
	MultiByteToWideChar(CP_UTF8, 0, frm, (int)size, u, (int)size);
	SendMessage(w, WM_SETTEXT, -1, (LPARAM)u);
	free(u);
	if(buf != nullptr) free(buf);
}
#else
#define Print __noop
#endif

#define lenof(x) (sizeof(x) / sizeof(*(x)))
#define is32bit (sizeof(void*) == 4)

int toUtf8(LPCWSTR utf16, int lenUtf16, LPSTR utf8, int lenUtf8) {
	return WideCharToMultiByte(CP_UTF8, 0, utf16, lenUtf16, utf8, lenUtf8, nullptr, nullptr);
}

int toUtf16(LPCSTR utf8, int lenUtf8, LPWSTR utf16, int lenUtf16) {
	return MultiByteToWideChar(CP_UTF8, 0, utf8, lenUtf8, utf16, lenUtf16);
}

bool regOpenKey(HKEY hkey, LPCWSTR subkey, HKEY& hkRet) {
	return 0 == ::RegOpenKeyExW(hkey, subkey, 0, KEY_READ | (is32bit ? 0 : KEY_WOW64_32KEY), &hkRet);
}

bool regGetUtf8(HKEY hkey, LPCWSTR name, LPSTR value, DWORD& size) {
	auto w = (LPWSTR)_alloca(size);
	DWORD size2 = size;
	bool R = 0 == RegQueryValueExW(hkey, name, nullptr, nullptr, (LPBYTE)w, &size2);
	if(R) {
		int n = toUtf8(w, size2 / 2, value, size) - 1;
		if(n > 0) size = n; else R = false;
	}
	if(!R) size = 0;
	return R;
}

bool regGetUtf8(HKEY hkey, LPCWSTR subkey, LPCWSTR name, LPSTR value, DWORD& size) {
	bool R = regOpenKey(hkey, subkey, hkey);
	if(R) {
		R = regGetUtf8(hkey, name, value, size);
		RegCloseKey(hkey);
	} else size = 0;
	return R;
}

struct _PATHS {
	char appDir[1000], netCore[1000], netDesktop[1000], asmDll[1000];
	wchar_t coreclrDll[300];
	bool isPrivate;
};

//HINSTANCE s_hInstance;
char s_asmName[800] = "\0hi7yl8kJNk+gqwTDFi7ekQ";

bool GetPaths(_PATHS& p) {
	//https://github.com/dotnet/designs/blob/master/accepted/install-locations.md

	wchar_t w[300];
	char version[100];

	//get exe full path
	DWORD len = ::GetModuleFileNameW(0, w, lenof(w));
	len = toUtf8(w, len + 1, p.appDir, lenof(w) - 100) - 1; if(len < 7 || p.appDir[len - 4] != '.') return false;

	//get asmDll if it is not exe created from script
	if(s_asmName[0] == 0) {
		bool asmSuffixed = is32bit && w[len - 6] == '3' && w[len - 5] == '2'; //if exe is "name32.exe", dll is "name.dll"
		strcpy(p.asmDll, p.appDir); strcpy(p.asmDll + len - (asmSuffixed ? 6 : 4), ".dll");
	}

	//get appDir
	while(p.appDir[len - 1] != '\\') len--;
	p.appDir[len] = 0;

	//get asmDll if it is exe created from script. See code in Compiler.cs, function _AppHost.
	if(s_asmName[0] != 0) {
		strncpy(p.asmDll, p.appDir, len);
		strcpy(p.asmDll + len, s_asmName);
	}

	//is coreclr.dll in app dir?
	wcscpy(w + len, L"coreclr.dll");
	if(p.isPrivate = (0xffffffff != GetFileAttributesW(w))) {
		strcpy(p.netCore, p.appDir);
		strcpy(p.netDesktop, p.appDir);
		wcscpy(p.coreclrDll, w);
		return true;
	}

	//get Core root path, like "C:\Program Files\dotnet"
	wchar_t key[] = LR"(SOFTWARE\dotnet\Setup\InstalledVersions\x64)"; if(is32bit) { key[41] = '8'; key[42] = '6'; }
	HKEY hk1;
	if(!regOpenKey(HKEY_LOCAL_MACHINE, key, hk1)) return false;
	if(!regGetUtf8(hk1, L"InstallLocation", p.netCore, len = sizeof(p.netCore) - 150)) { //Core 3.0.1+ does not set InstallLocation; assume default location.
		int len1 = ExpandEnvironmentStringsW(L"%ProgramFiles%\\dotnet", w, lenof(w)); //info: if is32bit, expands to "C:\Program Files (x86)"
		toUtf8(w, len1, p.netCore, sizeof(p.netCore) - 150);
	}

	//get latest installed Core runtime version, like "3.1.0\"
	bool ok = regGetUtf8(hk1, L"hostfxr", L"Version", version, len = sizeof(version) - 2);
	if(ok) {
		LPSTR v = version; if(strtol(v, &v, 10) < COREVER1 || strtol(++v, &v, 10) < COREVER2 || strtol(++v, &v, 10) < COREVER3) return false;
		version[len] = '\\'; version[len + 1] = 0;
	}
	RegCloseKey(hk1);
	if(!ok) return false;

	//get netCore and netDesktop
	strcat(p.netCore, "\\shared\\Microsoft.");
	strcpy(p.netDesktop, p.netCore);
	strcat(p.netCore, "NETCore.App\\"); strcat(p.netCore, version);
	strcat(p.netDesktop, "WindowsDesktop.App\\"); strcat(p.netDesktop, version);

	//get coreclrDll
	len = toUtf16(p.netCore, -1, p.coreclrDll, lenof(p.coreclrDll) - 12) - 1; if(len < 0) return false;
	wcscpy(p.coreclrDll + len, L"coreclr.dll");
	if(0xffffffff == GetFileAttributesW(p.coreclrDll)) return false;

	//is Desktop installed?
	len = toUtf16(p.netDesktop, -1, w, lenof(w)) - 2; if(len < 0)return false;
	w[len] = 0;
	if(0xffffffff == GetFileAttributesW(w)) return false;

	return true;
}

void BuildTpaList(LPCSTR dir, std::string& tpaList)
{
	std::string wild = dir; wild += "*.dll";
	WIN32_FIND_DATAA findData;
	HANDLE h = FindFirstFileA(wild.c_str(), &findData);
	if(h != INVALID_HANDLE_VALUE) {
		do {
			char* s = findData.cFileName;
			int four = *(int*)s;
			if(four == '-ipa') continue; //api-ms-
			//Print("%s", s);
			tpaList += dir;
			tpaList += s;
			tpaList += ";";
		} while(FindNextFileA(h, &findData));
		FindClose(h);
	}

	//note: don't use stringstream, it makes file size *= 2.
}

const char** ArgsUtf8(int& nArgs) {
	int na = __argc - 1;
	auto a = __wargv + 1;
	int size = 0; for(int i = 0; i < na; i++) size += (int)wcslen(a[i]) * 3 + 1;
	auto r = (const char**)malloc(na * sizeof(LPSTR) + size);
	LPSTR p = (LPSTR)(r + na);
	for(int i = 0; i < na; i++) {
		int k = toUtf8(a[i], -1, p, size);
		if(k == 0) exit(-1);
		r[i] = p;
		p += k;
		size -= k;
	}
	nArgs = na;
	return r;
}

int APIENTRY wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR s, int nCmdShow)
{
#if false
	LARGE_INTEGER c;
	QueryPerformanceCounter(&c);
	return c.LowPart;

#else

	//s_hInstance = hInstance;

	_PATHS p = {};
	if(!GetPaths(p)) { //fast
		std::string s1 = "Please install or update .NET Core Desktop Runtime x";
		s1 += is32bit ? "86" : "64";
		s1 += ".\r\n\r\nhttps://dotnet.microsoft.com/download";
		MessageBoxA(0, s1.c_str(), nullptr, MB_ICONERROR);
		return -1;
	}

	//Print("appDir=%s", p.appDir);
	//Print("netCore=%s", p.netCore);
	//Print("netDesktop=%s", p.netDesktop);
	//Print("coreclrDll=%S", p.coreclrDll);
	//Print("asmDll=%s", p.asmDll);
	//return 0;

	HMODULE hm = LoadLibraryW(p.coreclrDll); //~3 ms
	if(hm == NULL) return -2;
	coreclr_initialize_ptr coreclr_initialize = (coreclr_initialize_ptr)GetProcAddress(hm, "coreclr_initialize");
	coreclr_execute_assembly_ptr coreclr_execute_assembly = (coreclr_execute_assembly_ptr)GetProcAddress(hm, "coreclr_execute_assembly");
	//coreclr_create_delegate_ptr coreclr_create_delegate = (coreclr_create_delegate_ptr)GetProcAddress(hm, "coreclr_create_delegate");
	coreclr_shutdown_ptr coreclr_shutdown = (coreclr_shutdown_ptr)GetProcAddress(hm, "coreclr_shutdown");

	//LARGE_INTEGER t1, t2;
	//QueryPerformanceCounter(&t1);

	std::string tpaList; //~2 ms total
	if(!p.isPrivate) {
		BuildTpaList(p.netDesktop, tpaList); //note: must be first, else eg WPF does not work because netCore dir contains WindowsBase too, and it is invalid
		BuildTpaList(p.netCore, tpaList);
	}
#if false
	BuildTpaList(p.appDir, tpaList);

	const char* propertyKeys[] = { "TRUSTED_PLATFORM_ASSEMBLIES", "APP_PATHS" };
	const char* propertyValues[] = { tpaList.c_str(), strcat(p.appDir, "Libraries") };
#else
	else BuildTpaList(p.appDir, tpaList);

	std::string ap(p.appDir);
	ap[ap.length() - 1] = ';'; ap += p.appDir; ap += "Libraries";
	std::string nd(p.netCore);
	nd[nd.length() - 1] = ';'; nd += p.netDesktop; nd[nd.length() - 1] = 0;

	const char* propertyKeys[] = { "TRUSTED_PLATFORM_ASSEMBLIES", "APP_PATHS", "NATIVE_DLL_SEARCH_DIRECTORIES", "APP_CONTEXT_BASE_DIRECTORY" };
	const char* propertyValues[] = { tpaList.c_str(), ap.c_str(), nd.c_str(), p.appDir };
	//Print("TPA:"); Print("%s", propertyValues[0]);
	//Print("APP:"); Print("%s", propertyValues[1]);
	//Print("ND:"); Print("%s", propertyValues[2]);
	//Print("ABD:"); Print("%s", propertyValues[3]);
#endif
	//QueryPerformanceCounter(&t2);
	//Print(L"%i", (t2.LowPart - t1.LowPart) / 10);

	SetEnvironmentVariable(L"COMPlus_legacyCorruptedStateExceptionsPolicy", L"1");

	void* hostHandle;
	unsigned int domainId;

	int hr = coreclr_initialize(p.appDir, "main", lenof(propertyKeys), propertyKeys, propertyValues, &hostHandle, &domainId); //~15 ms
	if(hr < 0) {
		return -3;
	}

	int nArgs = 0;
	const char* args0[1] = {};
	const char** args = (s != nullptr && s[0] != 0) ? ArgsUtf8(nArgs) : args0;

	unsigned int ec = 0;
	hr = coreclr_execute_assembly(hostHandle, domainId, nArgs, args, p.asmDll, &ec); //6 ms

	coreclr_shutdown(hostHandle, domainId);

	if(hr < 0) {
		return -4;
	}
	return ec;
#endif
}
