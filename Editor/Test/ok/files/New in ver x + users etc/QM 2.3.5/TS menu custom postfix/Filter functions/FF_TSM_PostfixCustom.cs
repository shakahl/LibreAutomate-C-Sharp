 /
function# iid FILTER&f

 Use with TS menus to define custom postfix characters.
 This is a template function. When you create a new function from it, edit postfixChars string.
 Assign such filter function to a TS menu to make it work only if the user-typed postfix character is one of characters in postfixChars string.


lpstr postfixChars=" [9][13]'''>}]).,?!:;-" ;;edit this. Special characters: [9] Tab, [13] Enter, '' ".

 __________________________________________________

int ch; TriggerInfoTsMenu 0 ch 0 0 f ;;get postfix character
if(!ch) ret iid ;;Ctrl
if(findc(postfixChars ch)>=0) ret iid
ret -2

 ret iid	;; run the TS menu item.
 ret 0		;; don't run any items.
 ret -2		;; don't run this item. Matching items of other TS menus can run.
