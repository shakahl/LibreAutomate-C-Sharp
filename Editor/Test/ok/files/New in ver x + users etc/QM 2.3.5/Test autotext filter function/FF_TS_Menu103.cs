 /
function# iid FILTER&f

 Use with autotext lists to define custom postfix characters.
 FFT_Autotext_PostfixCustom is a template function. When you create a new function from it, edit postfixChars string.
 Assign such filter function to an autotext list to make it work only if the user-typed postfix character is one of characters in postfixChars string.


lpstr postfixChars=" [9][13]'''" ;;edit this. Special characters: [9] Tab, [13] Enter, '' ".

 __________________________________________________

int ch; TriggerInfoAutotext 0 ch ;;get postfix character
if(!ch) ret iid ;;Ctrl
if(findc(postfixChars ch)>=0) ret iid
ret -2

 ret iid	;; run the item.
 ret 0		;; don't run any items.
 ret -2		;; don't run this item. Matching items of other autotext lists can run.
