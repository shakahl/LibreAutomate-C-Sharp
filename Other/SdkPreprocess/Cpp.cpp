
struct ARRAYMEMBER {
	int a[10];
	int b[5], c[2], d;
	int e, f[5][6];
	int* p;
	__wchar_t* s;
};
//

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

struct SIMPLE {
	int x;
	__wchar_t* s;
	const __wchar_t* cs;
	char* cp;
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

typedef short KEYARRAY[128];
typedef long KEYARRAY2[3][4];
typedef short ARR5[5], ARR6[6];
typedef int(*FUUARR)(int p[8]);
typedef int(*FUUARRSTR)(__wchar_t p[8]);

//
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

typedef int(*FUU12)(int, __declspec(deprecated) int(*cmp)(int a, int b));
typedef int(*FUU13)(int, struct SIMPLE(*cmp)(int a, int b));
typedef struct Node Node;
struct Node {
	int data;
	Node *nextptr;
};

//
UNK* p;

struct FORWARD; //ok
struct SIMPLE; //ok
typedef int(*FUU2)(int* p); //not error if same definition
typedef const __wchar_t* LPCWSTR; //ok
//typedef __wchar_t* LPCWSTR; //error

//typedef int(*FUU13)(int, inline int(*cmp)(int a, int b)); //error

//void CallForward(FORWARD f) //error if not pointer. But not error in func typedef parameters and return type. Not error in simple typedef too.
//{
//}

//typedef enum EFO; //warning: typedef ignored

//typedef int(*FUU9)(struct { int x } p); //error: type definition is not allowed
//typedef int(*FUU10)(int, struct SIMPLE k(int, int)); //error: incomplete type is not allowed


struct BASE {
	int b;
};

enum ENBASE { nnn, mmm }; //cannot be base of struct
typedef BASE *PBASE; //cannot be base of struct
typedef BASE BASE2; //can be base of struct

struct FORWARD :BASE{
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
	DRAWF3 d3=d1; //ok

}

typedef struct :BASE{
	int x;
} CONSTRUCT;

using ALIAS = int;
using ALIAS2 = int*;
using ALIASF =
int(*)(int);

struct Stru1
{
	int x;
	int y;
};



typedef int MINT, *PMINT;

enum Enum1 {
	A1 = 1,
	B1 = 2
};

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

typedef enum {
	A6 = 1,
	B6 = 2
} Enum6;

typedef enum tagEnum7 {
	A7 = 1,
	B7 = 2
} Enum7;

typedef int TD;

enum Enum8 :TD {
	A8 = 1,
	B8 = 2
};
