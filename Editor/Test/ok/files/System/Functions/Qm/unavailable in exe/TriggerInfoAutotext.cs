 /
function! [str&userText] [int&postfixChar] [int&prefixChar] [str&itemText] [int&itemFlags]

 Gets autotext trigger information.
 Call this function from an autotext list filter function or item code.
 Returns: 1 success, 0 failed.

 userText - receives user-typed text that triggered the autotext list item. Without postfix character.
   It may be not exactly the same as itemText because of the case insensitive option.
 postfixChar - receives user-typed postfix character.
   It is Unicode UTF-16 <help #IDP_ASCII>character code</help>. Enter is 13.
   It is 0 if used Ctrl or don't need a postfix.
 prefixChar - receives user-typed character before userText.
   It is Unicode UTF-16 character code. Enter is 13.
   It is 0 if before userText was click, Ctrl or other event that resets QM autotext buffer.
 itemText - receives autotext list item text.
 itemFlags - receives item options that are set in autotext list text:
   1 - /s (select)
   2 - /b (erase)
   4 - /i (case insensitive)
   8 - /c (capitalize)
   16 - /p1 (postfix delimiter)
   32 - /p2 (postfix Ctrl)
   64 - /m (confirm)

 Added in: QM 2.3.5. In older QM use TriggerInfoTsMenu.


ret __TriggerInfoAutotext(&userText &postfixChar &prefixChar &itemText &itemFlags)
err
