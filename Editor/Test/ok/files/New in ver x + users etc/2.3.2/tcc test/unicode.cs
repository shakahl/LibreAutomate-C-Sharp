str si=
 #define KKK 7
si.setfile("$desktop$\ąčę\incl.h")

str se="$desktop$\ąčę\tccąčę.exe"
__Tcc x.Compile("" se 1 "" "" 0 "" "$desktop$\ąčę")

RunConsole se "/aaa"

 ________________________________________

#ret

#include <stdio.h>
#include "incl.h"

//will craete console app if there is function main
int main(int argc, char **argv)
{
printf("Console exe, %s().\n%i", __func__, KKK);
return 0;
}
