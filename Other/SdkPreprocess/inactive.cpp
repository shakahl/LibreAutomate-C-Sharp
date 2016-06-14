
#if 0

enum MENUM {
	A, B
};

enum MENUM2 :char {
	A2, B2
};
enum MENUM3 {
	A = 0x10, B
};


struct BASE {
	int a;
	int b;
};

struct INHER :BASE {
	int c, d;
};

__interface IUnknown
{
	void AddRef();
};

__interface IBase : IUnknown
{
	void BaseFunc();
};

__interface IInher : IBase
{
	void OwnFunc();
};

typedef int ARR[5];

//



__interface IInterface
{
public:
	int Func();
	virtual int* PureVirtual() = 0;
};





typedef int DWORD, UINT, BOOL, LONG;
typedef short WORD, SHORT;
typedef char BYTE;
typedef __int64 DWORD64, ULONGLONG;
struct IntLong { void* x; };

union PSAPI_WORKING_SET_BLOCK {
	IntLong Flags;
	struct {
		IntLong Protection : 5;
		IntLong ShareCount : 3;
		IntLong Shared : 1;
		IntLong Reserved : 3;
		IntLong VirtualPage : 52;
	};

	__int64 i1 : 3, i2 : 5, i3 : 40;
};

struct IMAGE_ARCHITECTURE_HEADER {
	u$int AmaskValue : 1;
	int : 7;
	u$int AmaskShift : 8;
	int : 16;
	DWORD * FirstEntryRVA;
};

struct BITF {
	union {
		DWORD clear;
		struct {
			BYTE a : 7, b : 2;
		};
	};
};

struct SHELLFLAGSTATE {
	BOOL fShowAllObjects : 1;
	BOOL fShowExtensions : 1;
	BOOL fNoConfirmRecycle : 1;
	BOOL fShowSysFiles : 1;
	BOOL fShowCompColor : 1;
	BOOL fDoubleClickInWebView : 1;
	BOOL fDesktopHTML : 1;
	BOOL fWin95Classic : 1;
	BOOL fDontPrettyPath : 1;
	BOOL fShowAttribCol : 1;
	BOOL fMapNetDrvBtn : 1;
	BOOL fShowInfoTip : 1;
	BOOL fHideIcons : 1;
	BOOL fAutoCheckSelect : 1;
	BOOL fIconsOnly : 1;
	UINT fRestFlags : 1;
	UINT more : 25;
};

struct COMSTAT {
	DWORD fCtsHold : 1;
	DWORD fDsrHold : 1;
	DWORD fRlsdHold : 1;
	DWORD fXoffHold : 1;
	DWORD fXoffSent : 1;
	DWORD fEof : 1;
	DWORD fTxim : 1;
	DWORD fReserved : 25;
	DWORD cbInQue;
	DWORD cbOutQue;
};

union RATE_QUOTA_LIMIT {
	DWORD RateData;
	struct {
		DWORD RatePercent : 7;
		DWORD Reserved0 : 25;
	};
};

struct LDT_ENTRY {
	WORD LimitLow;
	WORD BaseLow;
	union {
		struct {
			BYTE BaseMid;
			BYTE Flags1;
			BYTE Flags2;
			BYTE BaseHi;
		} Bytes;
		struct {
			DWORD BaseMid : 8;
			DWORD Type : 5;
			DWORD Dpl : 2;
			DWORD Pres : 1;
			DWORD LimitHi : 4;
			DWORD Sys : 1;
			DWORD Reserved_0 : 1;
			DWORD Default_Big : 1;
			DWORD Granularity : 1;
			DWORD BaseHi : 8;
		} Bits;
	} HighWord;
};

struct XSTATE_CONFIGURATION {
	DWORD64 EnabledFeatures;
	DWORD64 EnabledVolatileFeatures;
	DWORD Size;
	DWORD OptimizedSave : 1;
	DWORD CompactionEnabled : 1;
	int Features[(64)];
	DWORD64 EnabledSupervisorFeatures;
	DWORD64 AlignedFeatures;
	DWORD AllFeatureSize;
	DWORD AllFeatures[(64)];
};

struct PROCESSOR_IDLESTATE_POLICY {
	WORD Revision;
	union {
		WORD AsWORD;
		struct {
			WORD AllowScaling : 1;
			WORD Disabled : 1;
			WORD Reserved : 14;
		};
	} Flags;
	DWORD PolicyCount;
	int Policy[0x3];
};

struct PROCESSOR_POWER_POLICY_INFO {
	DWORD TimeCheck;
	DWORD DemoteLimit;
	DWORD PromoteLimit;
	BYTE DemotePercent;
	BYTE PromotePercent;
	BYTE Spare[2];
	DWORD AllowDemotion : 1;
	DWORD AllowPromotion : 1;
	DWORD Reserved : 30;
};

struct PROCESSOR_PERFSTATE_POLICY {
	DWORD Revision;
	BYTE MaxThrottle;
	BYTE MinThrottle;
	BYTE BusyAdjThreshold;
	union {
		BYTE Spare;
		union {
			BYTE AsBYTE;
			struct {
				BYTE NoDomainAccounting : 1;
				BYTE IncreasePolicy : 2;
				BYTE DecreasePolicy : 2;
				BYTE Reserved : 3;
			};
		} Flags;
	};
	DWORD TimeCheck;
	DWORD IncreaseTime;
	DWORD DecreaseTime;
	DWORD IncreasePercent;
	DWORD DecreasePercent;
};

struct IMAGE_TLS_DIRECTORY32 {
	DWORD StartAddressOfRawData;
	DWORD EndAddressOfRawData;
	DWORD AddressOfIndex;
	DWORD AddressOfCallBacks;
	DWORD SizeOfZeroFill;
	union {
		DWORD Characteristics;
		struct {
			DWORD Reserved0 : 20;
			DWORD Alignment : 4;
			DWORD Reserved1 : 8;
		};
	};
};

struct IMAGE_RESOURCE_DIRECTORY_ENTRY {
	union {
		struct {
			DWORD NameOffset : 31;
			DWORD NameIsString : 1;
		};
		DWORD Name;
		WORD Id;
	};
	union {
		DWORD OffsetToData;
		struct {
			DWORD OffsetToDirectory : 31;
			DWORD DataIsDirectory : 1;
		};
	};
};

struct IMAGE_CE_RUNTIME_FUNCTION_ENTRY {
	DWORD FuncStart;
	DWORD PrologLen : 8;
	DWORD FuncLen : 22;
	DWORD ThirtyTwoBit : 1;
	DWORD ExceptionFlag : 1;
};

struct FPO_DATA {
	DWORD ulOffStart;
	DWORD cbProcSize;
	DWORD cdwLocals;
	WORD cdwParams;
	WORD cbProlog : 8;
	WORD cbRegs : 3;
	WORD fHasSEH : 1;
	WORD fUseBP : 1;
	WORD reserved : 1;
	WORD cbFrame : 2;
};

struct IMPORT_OBJECT_HEADER {
	WORD Sig1;
	WORD Sig2;
	WORD Version;
	WORD Machine;
	DWORD TimeDateStamp;
	DWORD SizeOfData;
	union {
		WORD Ordinal;
		WORD Hint;
	};
	WORD Type : 2;
	WORD NameType : 3;
	WORD Reserved : 11;
};

union SLIST_HEADER {
	struct {
		ULONGLONG Alignment;
		ULONGLONG Region;
	};
	struct {
		ULONGLONG Depth : 16;
		ULONGLONG Sequence : 48;
		ULONGLONG Reserved : 4;
		ULONGLONG NextEntry : 60;
	} HeaderX64;
};

struct SHELLSTATEA {
	BOOL fShowAllObjects : 1;
	BOOL fShowExtensions : 1;
	BOOL fNoConfirmRecycle : 1;
	BOOL fShowSysFiles : 1;
	BOOL fShowCompColor : 1;
	BOOL fDoubleClickInWebView : 1;
	BOOL fDesktopHTML : 1;
	BOOL fWin95Classic : 1;
	BOOL fDontPrettyPath : 1;
	BOOL fShowAttribCol : 1;
	BOOL fMapNetDrvBtn : 1;
	BOOL fShowInfoTip : 1;
	BOOL fHideIcons : 1;
	BOOL fWebView : 1;
	BOOL fFilter : 1;
	BOOL fShowSuperHidden : 1;
	BOOL fNoNetCrawling : 1;
	DWORD dwWin95Unused;
	UINT uWin95Unused;
	LONG lParamSort;
	int iSortDirection;
	UINT version;
	UINT uNotUsed;
	BOOL fSepProcess : 1;
	BOOL fStartPanelOn : 1;
	BOOL fShowStartPage : 1;
	BOOL fAutoCheckSelect : 1;
	BOOL fIconsOnly : 1;
	BOOL fShowTypeOverlay : 1;
	BOOL fShowStatusBar : 1;
	UINT fSpareFlags : 9;
};

struct PDH_BROWSE_DLG_CONFIG_HW {
	DWORD bIncludeInstanceIndex : 1,
		bSingleCounterPerAdd : 1,
		bSingleCounterPerDialog : 1,
		bLocalCountersOnly : 1,
		bWildCardInstances : 1,
		bHideDetailBox : 1,
		bInitializePath : 1,
		bDisableMachineSelection : 1,
		bIncludeCostlyObjects : 1,
		bShowObjectBrowser : 1,
		bReserved : 22;
	DWORD dwDefaultDetailLevel;
};

struct PERSISTENT_RESERVE_COMMAND {
	DWORD Version;
	DWORD Size;
	union {
		struct {
			BYTE ServiceAction : 5;
			BYTE Reserved1 : 3;
			WORD AllocationLength;
		} PR_IN;
		struct {
			BYTE ServiceAction : 5;
			BYTE Reserved1 : 3;
			BYTE Type : 4;
			BYTE Scope : 4;
			BYTE ParameterList[0];
		} PR_OUT;
	};
};



//

struct TUOP {
	struct fw5;
	typedef int(*mmm)(fw5* p);
	typedef double MPOU, *PMPU;

	struct NESTED { int x; };
	struct { MPOU x; };
	struct { int y; };
	struct NESTED2 { int x; } a;
	struct { int z; } b;

	enum EEE1 { OO1, MM1 };
	enum { OOO1, MMM1 };
	enum EEE2 { OO2, MM2 } e1;
	enum { OOO2, MMM2 } e2;

	int last;
};

enum Enum1 {
	A1 = 1,
	B1 = 2
};


typedef int MINT, *PMINT;
struct FW;
typedef int(*kkk)(const struct FW*);


//

enum Enum2 :char {
	A2 = 1,
	B2 = 2
};

enum class Enum3 {
	A3 = 0x1,
	B3 = 0x2
};

enum {
	A4 = 1,
	B4 = 2
};

enum :short {
	A5 = 1,
	B5 = 2
};

typedef int TD;

enum Enum8 :TD {
	A8 = 1,
	B8 = 2
};

struct Stru1
{
	int x;
	int y;
};

struct BASE {
	int b;
};

struct CONSTRUCT :BASE {
	int x;
};

struct Stru4
{
	int x;
	int y;
	Stru4* p;
};

class Cla1
{
	int x;
public:
	int y;
};

union UN {
	int i;
	double d;
};

typedef struct Boo tagBoo, *PBoo;
struct Boo {
	int x;
};

struct {
	int y;
} gVar;

struct {
	int y;
};

//

struct Morq2 {
	int j;
};

struct MOUT {
	struct Forq;
	Forq* p;
	struct Forq* m;
	struct Morq* k;
	struct Morq2* k2;

	void Mem()
	{
		k2->j = 5;
	}
};
struct MOUT2 {
	//Forq* p;
	Morq* k;
	//typedef int IMT;
};

static_assert(
	1 == 1,
	"string - literal"
	);

typedef short KEYARRAY[128];
typedef long KEYARRAY2[3][4];
typedef short ARR5[5], ARR6[6];
typedef int(*FUUARR)(int p[8]);
typedef int(*FUUARRSTR)(__wchar_t p[8]);

enum ENUM_FORWARD;
struct FORWARD;

typedef FORWARD ALIAS1;

typedef const __wchar_t* LPCWSTR;



typedef int(*FUU)(int, __wchar_t*, struct UNK* p, const __wchar_t* x);
typedef int(*FUU2)(int* p);
typedef int(__stdcall*FUU3)(int** p);
typedef int*(*FUU4)();
typedef int(*FUU5)(int, int(*cmp)(int a, int b));
typedef int(*FUU52)(int, int(*f1)(int** a), int(*f2)(int a, int b));
typedef int(*FUU6)(int, int*(*)(int a, int b));
typedef FORWARD(*FUU7)(FORWARD* p, ENUM_FORWARD k, FORWARD n);

typedef void DRAWF(int, int);
typedef void __stdcall DRAWF2(int, int);

typedef int(*FUU8)(int, void k(int, int));
typedef int(*FUU8S)(int, void __stdcall k(int, int));

typedef int(*FUU11)(int, __interface INo* p);

typedef int(*FUU13)(int, struct SIMPLE(*cmp)(int a, int b));

typedef struct Node Node;
typedef struct Node *PNode;
typedef struct Node Node2, *PNode2;

struct Node {
	int data;
	Node *nextptr;
};

struct SIMPLE {
	int x;
	__wchar_t* s;
	const __wchar_t* cs;
	char* cp;
	void(__fastcall*fun)(int k);
};
//
struct ARRAYMEMBER {
	int a[10];
	int b[5], c[2], d;
	int e, f[5][6];
	int* p;
	__wchar_t* s;
};

struct ACCESS {
private:
	int x, y;
public:
	const __wchar_t* s, *s2;
};

struct COMMA {
	int x, y;
	const __wchar_t* s, *s2;
};

struct SIMPLEEX :SIMPLE {
	int added;
};

struct SIMPLEEXP :public SIMPLE {
	int added;
};

struct VAVA {
	double d;
};

struct SIMPLEEX2 :SIMPLE, VAVA {
	int added;
};

struct SIMPLEEX2P :public SIMPLE, VAVA {
	int added;
};

//

struct FORWARD; //ok
struct SIMPLE; //ok
typedef int(*FUU2)(int* p); //not error if same definition
typedef const __wchar_t* LPCWSTR; //ok

struct S1 {
	int i;
	typedef S1* PS1;
	struct FORW;
	FORW* p;

	//namespace UU {} //error
	//extern "C++" { } //error
	//extern int ei; //error
	const int coi;
};

//

typedef int DWORD;
typedef short WORD, SHORT;
typedef char BYTE;

typedef struct _LIST_ENTRY {
	struct _LIST_ENTRY *Flink;
	struct _LIST_ENTRY *Blink;
} LIST_ENTRY, *PLIST_ENTRY, *PRLIST_ENTRY;
//

typedef struct _SCOPE_TABLE_AMD64 {
	int Count;
	struct {
		int BeginAddress;
		int EndAddress;
		int HandlerAddress;
		int JumpTarget;
	} ScopeRecord[1];
} SCOPE_TABLE_AMD64, *PSCOPE_TABLE_AMD64;

typedef struct _JOBOBJECT_CPU_RATE_CONTROL_INFORMATION {
	DWORD ControlFlags;
	union {
		DWORD CpuRate;
		DWORD Weight;
		struct {
			WORD MinRate;
			WORD MaxRate;
		};
	};
} JOBOBJECT_CPU_RATE_CONTROL_INFORMATION, *PJOBOBJECT_CPU_RATE_CONTROL_INFORMATION;

typedef struct _NT_TIB {
	struct _EXCEPTION_REGISTRATION_RECORD *ExceptionList;
	void* StackBase;
	void* StackLimit;
	void* SubSystemTib;
	union {
		void* FiberData;
		int Version;
	};
	void* ArbitraryUserPointer;
	struct _NT_TIB *Self;
} NT_TIB;

typedef struct _KNONVOLATILE_CONTEXT_POINTERS {
	union {
		int FloatingContext[16];
		struct {
			int Xmm0;
			int Xmm1;
		};
	};
	union {
		int IntegerContext[16];
		struct {
			int Rax;
			int Rcx;
		};
	};
} KNONVOLATILE_CONTEXT_POINTERS, *PKNONVOLATILE_CONTEXT_POINTERS;















//
struct BITS {
	int BaseMid : 8;
	int Type : 5;
	int Dpl : 2;
	int Pres : 1;
	int LimitHi : 4;
	int Sys : 1;
	int Reserved_0 : 1;
	int Default_Big : 1;
	int Granularity : 1;
	int BaseHi : 8;
};

typedef struct _LDT_ENTRY {
	short LimitLow;
	short BaseLow;
	union {
		struct {
			char BaseMid;
			char Flags1;
			char Flags2;
			char BaseHi;
		} Bytes;
		struct {
			int BaseMid : 8;
			int Type : 5;
			int Dpl : 2;
			int Pres : 1;
			int LimitHi : 4;
			int Sys : 1;
			int Reserved_0 : 1;
			int Default_Big : 1;
			int Granularity : 1;
			int BaseHi : 8;
		} Bits;
	} HighWord;
} LDT_ENTRY, *PLDT_ENTRY;

typedef struct _PROCESS_MITIGATION_ASLR_POLICY {
	union {
		DWORD Flags;
		struct {
			DWORD EnableBottomUpRandomization : 1;
			DWORD EnableForceRelocateImages : 1;
			DWORD EnableHighEntropy : 1;
			DWORD DisallowStrippedImages : 1;
			DWORD ReservedFlags : 28;
		};
	};
} PROCESS_MITIGATION_ASLR_POLICY, *PPROCESS_MITIGATION_ASLR_POLICY;

struct _SYSTEM_CPU_SET_INFORMATION {
	DWORD Size;
	union {
		struct {
			DWORD Id;
			WORD Group;
			union {
				BYTE AllFlags;
				struct {
					BYTE Parked : 1;
					BYTE Allocated : 1;
					BYTE AllocatedToTargetProcess : 1;
					BYTE RealTime : 1;
					BYTE ReservedFlags : 4;
				};
			};
			DWORD Reserved;
			int AllocationTag;
		} CpuSet;
	};
};

typedef struct _REPARSE_GUID_DATA_BUFFER {
	DWORD ReparseTag;
	WORD ReparseDataLength;
	struct {
		BYTE DataBuffer[1];
	} GenericReparseBuffer;
} REPARSE_GUID_DATA_BUFFER, *PREPARSE_GUID_DATA_BUFFER;

typedef struct {
	WORD Revision;
	union {
		WORD AsWORD;
		struct {
			WORD AllowScaling : 1;
			WORD Disabled : 1;
			WORD Reserved : 14;
		};
	} Flags;
	DWORD PolicyCount;
	int Policy[0x3];
} PROCESSOR_IDLESTATE_POLICY, *PPROCESSOR_IDLESTATE_POLICY;

typedef union _IMAGE_AUX_SYMBOL {
	struct {
		DWORD TagIndex;
		union {
			struct {
				WORD Linenumber;
				WORD Size;
			} LnSz;
			DWORD TotalSize;
		} Misc;
		union {
			struct {
				DWORD PointerToLinenumber;
				DWORD PointerToNextFunction;
			} Function;
			struct {
				WORD Dimension[4];
			} Array;
		} FcnAry;
		WORD TvIndex;
	} Sym;
	struct {
		BYTE Name[18];
	} File;
	struct {
		DWORD Length;
		WORD NumberOfRelocations;
		WORD NumberOfLinenumbers;
		DWORD CheckSum;
		SHORT Number;
		BYTE Selection;
		BYTE bReserved;
		SHORT HighNumber;
	} Section;
	int TokenDef;
	struct {
		DWORD crc;
		BYTE rgbReserved[14];
	} CRC;
} IMAGE_AUX_SYMBOL;











//

//
UNK* p;
//typedef __wchar_t* LPCWSTR; //error

//typedef int(*FUU13)(int, inline int(*cmp)(int a, int b)); //error

//void CallForward(FORWARD f) //error if not pointer. But not error in func typedef parameters and return type. Not error in simple typedef too.
//{
//}

//typedef enum EFO; //warning: typedef ignored

//typedef int(*FUU9)(struct { int x } p); //error: type definition is not allowed
//typedef int(*FUU10)(int, struct SIMPLE k(int, int)); //error: incomplete type is not allowed


enum ENBASE { nnn, mmm }; //cannot be base of struct
typedef BASE *PBASE; //cannot be base of struct
typedef BASE BASE2; //can be base of struct

struct FORWARD :BASE {
	int k;
};
//struct FORWARD { //error: type redefinition
//	int k;
//};

enum ENUM_FORWARD { Zero, One };
//enum ENUM_FORWARD { Zero, One }; //error: type redefinition

typedef void(*DRAWF3)(int, int);

void main2()
{
	FUU7 k = 0;
	ENUM_FORWARD ef;
	FORWARD hi;
	k(0, ef, hi);

	DRAWF d1;
	//DRAWF2 d2=d1; //error
	DRAWF3 d3 = d1; //ok

}

using ALIAS = int;
using ALIAS2 = int*;
using ALIASF =
int(*)(int);

#endif
