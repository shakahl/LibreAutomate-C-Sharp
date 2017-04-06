 /permute
function $s str&sout

 EXAMPLE
 str s
 permute("abcd" s)
 out s


sout.all

int f1(&perm_append)
__Tcc+ __tcc_perm
if !__tcc_perm.f
	__tcc_perm.Compile("" "perm" 0 "" "perm_append" &f1)

str s2=s ;;the func modifies the input string
call(__tcc_perm.f s2 0 s2.len &sout)

#ret

#include <string.h>

/* swap(s,t): swap characters.
* Swaps characters *s and *t. */
void swap(char *s, char *t)
{
  int c;

  /* Rotate through temp variable c. */
  c = (int) *s; *s = *t; *t = (char) c;
}

/* perm(s,n,l): print permutations.
* Formats string s for all permutations of characters
* in positions n through l-1.
*/
void perm(char *s, int n, int l, void* param)
{
 int i;

 if (n == l)
 {
   /* No characters to permute . . print string. */
   perm_append(param, s);
 }
 else
 {
  /* Work through all characters of string. */
  for (i = n; i < l; i++)
  {
    /* Swap this character with the first one. */
    swap(s + i, s + n);
    /* Permute remainder of string. */
    perm(s, n+1, l, param);
    /* Swap back again. */
    swap(s + i, s + n);
  }
 }
}
