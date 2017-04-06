 /
function hwnd

Acc a.Find(hwnd "" "" "" 16|32 0 0 "" &sub.Callback_Acc_Find)


#sub Callback_Acc_Find
function# Acc&a level cbParam

 Callback function for Acc.Find or acc.
 Read more about <help #IDP_ENUMWIN>callback functions</help>.

 a - the found object.
 level - its level in the hierarchy. If class or id is specified, it is level beginning from that child window. When searching in web page (flag 0x2000), it is level from the root object of the web page (DOCUMENT or PANE).
 cbParam - cbParam passed to Acc.Find, or y passed to acc.

 Return:
 0 - stop. Let a will be the found object.
 1 - continue.
 2 - continue, but skip children of a.


str role name value
a.Role(role)
name=a.Name; err
value=a.Value
out "%.*m%s  ''%s''  ''%s''" level 9 role name value
ret 1
