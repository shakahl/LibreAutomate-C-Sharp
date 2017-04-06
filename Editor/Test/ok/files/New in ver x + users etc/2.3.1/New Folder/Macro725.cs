ref unrar "unrar"
SetCurDir "$program files$\UnrarDLL"

unrar.RAROpenArchiveData d
 then set d members, call RAROpenArchive, etc
unrar.RAROpenArchive(&d)
