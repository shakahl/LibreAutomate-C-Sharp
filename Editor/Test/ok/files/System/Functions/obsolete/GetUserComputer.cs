 /
function# str&user [str&computer]

 Obsolete. Use <help>GetUserInfo</help>.


if(&user && !GetUserInfo(&user)) ret
if(&computer && !GetUserInfo(&computer 1)) ret
ret 1
