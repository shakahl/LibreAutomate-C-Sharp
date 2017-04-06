 /
function [~kur] [noEnter]

if(_winver!=0x501) ret ;;the database is only on XP

if(!kur.len) inp- kur "" "kur?"
str si sp
if(!PasswordFromId(kur si sp)) ret
AutoPassword si sp !noEnter|4
err
