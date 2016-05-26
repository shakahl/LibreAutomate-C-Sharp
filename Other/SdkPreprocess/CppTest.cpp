
#define OOO(k) (k>0)

//const int pa = 4;
#define pa 4

#pragma pack(push)

auto g_x = 8;
static int* pp;

extern "C" {
	void Fuu();
}

struct Moo
{
#pragma pack(4)
	int c;
};

enum Moon {
};

int Moon;

//__if_not_exists(g_kk) {
void  Test()
{
	int ka = 1'0'00;
	double uu=1'0'0.0'5E1'2;
	double ux = 0x10E2;
	int kb = 0b1'0'00;
	double du = 09.7;
	double ki = 09E2;
	//int cc = 'A'u;
	int nn = 5i64;

	if (OOO(5));

	int k = 5u;
	k = 5U;
	k = 36'000'000;
	k = 5l;
	k = 5L;
	__int64 m;
	m = 5LL;
	m = 5ll;
	m = 5ULL;
	m = 5ull;
	m = 5i64;
	m = 5ui64;
	double d;
	d = 10.5;
	d = 10.;
	d = 10.5f;
	d = 10.5L;
	d = 10.5E3;
	d = 10.5E3f;
	d = 10.5E3L;
	d = 10.5E-3;
	d = 10.5e+3;
	int hex = 0x10;
	int bin = 0b11;

	char c = 'a';
	wchar_t wc = L'A';
	char16_t c1 = u'A';
	char32_t c2 = U'A';
	//c = u8'c'; //no
	//c = R'(c)'; //no

	char* s = "simple";
	wchar_t* ws = L"aaaa";
	//wchar_t* ws = L "aaaa"; //error
	char16_t* s1 = u"aaaa";
	char32_t* s2 = U"mmm";
	char* utf = u8"UTF";
	//utf = U8"UTF"; //no, prefixes must match case
	char* raw = R"(ab"cd)"; //must be with ()
	s1 = uR"(aaaa)";
	raw = R"xyz()")xyz"; //delimiter xyz
	//s = "sss"s; //error, need to include something because it is a user-defined literal operator


	auto x = 7;
	//int __unaligned * p;
	int __based(pp)* b;
	__w64 int w;

}
//}

#define RAW1 R"(bbb"nnn)"
#define RAW2 R"(bbb
nnn)"

const wchar_t* su1 = L"<??>";
//const wchar_t* su2 = u8"<??>";

typedef struct {
	int x;
} MDT;

#include <stdio.h>
//#define WIN32_LEAN_AND_MEAN
//#include <windows.h>

void main()
{
	//MessageBoxW(0, su1, L"test", 0);

	printf(R"(test\n)");
	printf(RAW1);
	printf(RAW2);
	printf("\n");
}
