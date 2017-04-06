NOINLINE int Test() { return 1; }
NOINLINE int Test1(int a) { return -1; }
NOINLINE int Test2(int a, int b) { return a+b; } ;;a+b etc prevents optimization where unused arguments would not be passed at all
NOINLINE int Test3(int a, int b, int c) { return a+b+c; }
NOINLINE int Test4(int a, int b, int c, int d) { return a+b+c+d; }

void TestAll()
{
	Str s; s.Buffer(10000, 2);
	Arr<int> a; a.Alloc_SetLen(10000);

	WakeCPU();
	int i; int j = 1;
	PF;
	//loop
	for (i = 0; i<10000; i++) j *= 2; //*= 2 prevents optimization where the loop would be removed
	PN;
	//if, expression
	for (i = 0; i<10000; i++) { if (j) j = j && (j * 2 + 1 > 1); }
	PN;
	//string iteration
	for (i = 0; i<10000; i++) j += s.p[i];
	PN;
	//array iteration
	for (i = 0; i<10000; i++) j += a[i];
	PN;
	//function call
	for (i = 0; i < 10000; i++)
	{
		j += Test();
		j += Test1(i);
		j += Test2(i, 0);
		j += Test3(i, j, 0);
		j += Test4(i, j, 1000000, 0);
	}
	PN;
	PO;
	zz(j);
}
