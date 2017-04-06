str se="$desktop$\tcccon.exe"
__Tcc x.Compile("" se 1)

run se "/aaa"
 RunConsole2 _s.from(se " /aaa")

#ret

#include <stdio.h>

//will craete console app if there is function main
int main(int argc, char **argv)
{
int i;
printf("Console exe, %s().\n", __func__);
for(i=0; i<argc; i++) printf("\targv[%i]=%s\n", i, argv[i]);
Sleep(2000);
return 0;
}
