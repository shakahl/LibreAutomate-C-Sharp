
out
dll "qm.exe"
	EhProlog
	EhEpilog
ARRAY(int) a.create(2)
a[0]=&EhProlog
a[1]=&EhEpilog
__Tcc2 x.Compile("" "Test" 0 "" "EhProlog[]EhEpilog" &a[0])
_hresult=100
out call(x.f)


#ret

typedef struct { void *prev; int handler; void* label; } EXCEPTION_REGISTRATION;

int g_div = 0;

int myHandler(void *ExcRecord, void * EstablisherFrame, void *ContextRecord, void * DispatcherContext)
{
	printf("In the exception handler");
	//OutputDebugStringA("In the exception handler");
	g_div=1;
	//return 0; //ExceptionContinueExecution
	return 1; //ExceptionContinueSearch
}

int Test()
{
	//const void* __el__[1]={&&g1};
	EXCEPTION_REGISTRATION reg;
	//reg.test=__el__;
	EhProlog();
	
	reg.label=&&g1;
	int j = 10;
	j/=g_div;  //Exception
g1:
//	goto gr;
//gr:
	EhEpilog();
	return j;
}
