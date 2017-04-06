use32

;push 5
;mov eax, <#Empty#>
;call eax

;lea eax, s1
;push eax
push dword s1
;lea eax, [s1]
;push eax
mov eax, <#printf#>
call eax
pop eax

ret

;SECTION ".data"
s1 db 'test',0
