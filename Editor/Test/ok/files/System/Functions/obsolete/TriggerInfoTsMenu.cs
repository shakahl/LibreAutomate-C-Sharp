 /
function! [str&userText] [int&postfixChar] [str&itemText] [int&itemFlags] [FILTER&f]

 Gets autotext trigger information.
 Obsolete. Use <help>TriggerInfoAutotext</help>. It gets some more info. Parameters are the same.

 f - in filter function must be f.

 Added in: QM 2.3.3.


ret __TriggerInfoAutotext(&userText &postfixChar 0 &itemText &itemFlags)
err+
