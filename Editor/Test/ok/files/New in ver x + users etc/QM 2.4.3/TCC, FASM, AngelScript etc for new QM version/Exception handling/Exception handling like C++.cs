
out
dll "qm.exe"
	EhProlog
	EhEpilog
	#CallC fa
ARRAY(int) a.create(2)
a[0]=&EhProlog
a[1]=&EhEpilog
__Tcc2 x.Compile("" "Test" 0 "" "EhProlog[]EhEpilog" &a[0])
_hresult=100
 out call(x.f)
out CallC(x.f)


#ret

typedef struct { void*prev; int handler; void*labelErr,*labelFin; int _esp,_ebx,_esi,_edi; } EXCEPTION_REGISTRATION;
#define TRY(label) asm("movl %%esp,-16(%%ebp);movl %0,-24(%%ebp)"::"m"(&&label));
#define CATCH(label) label:asm("movl $0,-24(%ebp)");
#define OUTESP asm("movl %%esp,%0":"=r"(_esp)); printf("%X",_esp);

int g_div = 0;
int Test2(); void MakeException();

int Test()
{
	EXCEPTION_REGISTRATION r;
	//const void* __el__[1]={&&g1};
	//reg.test=__el__;
	EhProlog();
	int _esp;
	int j = 10;
	//double d1=3.0, d2=4.5; d1+=d2; printf("%g", d1);
	
	//OUTESP
TRY(g1)
	//printf("%X %X %X %X %X", r.prev, r.handler, r.labelErr, r.labelFin, r._esp);
	j/=g_div;  //Exception
	//Test2();
	printf("Test: no exception");
CATCH(g1)
	//OUTESP
	printf("Test: %i", j);
//	goto gr;
//gr:
	EhEpilog();
	return j;
}




int Test2()
{
	EXCEPTION_REGISTRATION r;
	EhProlog();
	r.labelFin=&&finally;
	
	//double d1=3.0, d2=4.5; d1+=d2; printf("%g", d1);
	
	//asm("mov $1,%ebx;mov $2,%esi;mov $3,%edi;");
	int j = 10;
	j/=g_div;  //Exception
	//MakeException();
	printf("Test2: no exception, j=%i", j);
finally:
	printf("Test2: finally");
	//j/=g_div;  //Exception
	EhEpilog();
	return j;
}


void MakeException()
{
	int j = 10;
	j/=g_div;  //Exception
}
