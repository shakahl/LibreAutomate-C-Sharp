 /
function# iid FILTER&f

 Use with autotext lists to define custom postfix characters.
 FFT_Autotext_PostfixCustom is a template function. When you create a new function from it, edit postfixChars string.
 Assign such filter function to an autotext list to make it work only if the user-typed postfix character is one of characters in postfixChars string.


 __________________________________________________

int ch1 ch2; TriggerInfoAutotext 0 ch2 ch1 ;;get postfix character
if((!ch2 or ch2=';') and (!ch1 or ch1='-')) ret iid
ret -2

 ret iid	;; run the item.
 ret 0		;; don't run any items.
 ret -2		;; don't run this item. Matching items of other autotext lists can run.
