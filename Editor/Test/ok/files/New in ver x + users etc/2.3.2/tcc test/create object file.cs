str se="$desktop$\tcco.o"
__Tcc x.Compile("" se 3)

 RunConsole se "/aaa"

 ________________________________________

#ret

#include <stdio.h>

//will craete console app if there is function main
int main(int argc, char **argv)
{
printf("Console exe, %s().\n", __func__);
int i;
for(i=0; i<argc; i++) printf("\targv[%i]=%s\n", i, argv[i]);
return 0;
}
