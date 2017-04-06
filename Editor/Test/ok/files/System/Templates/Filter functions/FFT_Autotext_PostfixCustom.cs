 /
function# iid FILTER&f

 Autotext lists that have this filter function will work only when the user-typed postfix character is one of characters in the postfixChars string.


lpstr postfixChars=" [9][13]'''>}]).,?!:;-" ;;edit this. Special characters: [9] Tab, [13] Enter, '' ". Use only delimiter characters.

 __________________________________________________

int ch; TriggerInfoAutotext 0 ch ;;get postfix character
if(!ch) ret iid ;;Ctrl
if(findc(postfixChars ch)>=0) ret iid
ret -2

 ret iid	;; run the item.
 ret 0		;; don't run any items.
 ret -2		;; don't run this item. Matching items of other autotext lists can run.
