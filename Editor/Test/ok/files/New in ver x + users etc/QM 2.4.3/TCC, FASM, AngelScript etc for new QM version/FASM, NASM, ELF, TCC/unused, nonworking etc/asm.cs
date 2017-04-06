; Disassembly of file: Q:\app\Release\TestObj.obj
; Tue Oct 14 21:19:06 2014
; Mode: 32 bits
; Syntax: YASM/NASM
; Instruction set: 80386


global zo:function
global TestK2:function
global ??_C@_06JBKEBCGB@except?$AA@
global ??_C@_03FIKCJHKP@abc?$AA@
global TestK:function

extern _EH_prolog                                      ; near
extern __CxxFrameHandler3                              ; near
extern strlen                                          ; near
extern GetKeyState                             ; dword
extern wsprintfA                                 ; dword
extern OutputDebugStringA                      ; dword

@feat.00 equ 00000001H                                  ; 1
@comp.id equ 00847809H                                  ; 8681481


SECTION .text                         ; section number 3, code
;  Communal section not supported by YASM

zo:; Function begin
        push    ebp                                     ; 0000 _ 55
        mov     ebp, esp                                ; 0001 _ 8B. EC
        mov     eax, dword [ebp+8H]                     ; 0003 _ 8B. 45, 08
        sub     esp, 1028                               ; 0006 _ 81. EC, 00000404
        test    eax, eax                                ; 000C _ 85. C0
        jz      _001                                   ; 000E _ 74, 1C
        cmp     byte [eax], 0                           ; 0010 _ 80. 38, 00
        jz      _001                                   ; 0013 _ 74, 17
        lea     ecx, [ebp+0CH]                          ; 0015 _ 8D. 4D, 0C
        push    ecx                                     ; 0018 _ 51
        push    eax                                     ; 0019 _ 50
        lea     eax, [ebp-404H]                         ; 001A _ 8D. 85, FFFFFBFC
        push    eax                                     ; 0020 _ 50
        call    near [wsprintfA]                 ; 0021 _ FF. 15, 00000000(d)
        add     esp, 12                                 ; 0027 _ 83. C4, 0C
        jmp     _002                                   ; 002A _ EB, 0E

_001:  mov     byte [ebp-404H], 32                     ; 002C _ C6. 85, FFFFFBFC, 20
        mov     byte [ebp-403H], 0                      ; 0033 _ C6. 85, FFFFFBFD, 00
_002:  lea     eax, [ebp-404H]                         ; 003A _ 8D. 85, FFFFFBFC
        push    eax                                     ; 0040 _ 50
        call    near [OutputDebugStringA]      ; 0041 _ FF. 15, 00000000(d)
        leave                                           ; 0047 _ C9
        ret                                             ; 0048 _ C3
; zo End of function


SECTION .text                         ; section number 5, code
;  Communal section not supported by YASM

TestK2:; Function begin
        mov     eax, _ehhandler$?TestK2@@YAHXZ         ; 0000 _ B8, 00000000(d)
        call    _EH_prolog                             ; 0005 _ E8, 00000000(rel)
        push    ecx                                     ; 000A _ 51
        push    ecx                                     ; 000B _ 51
        push    ebx                                     ; 000C _ 53
        push    esi                                     ; 000D _ 56
        push    edi                                     ; 000E _ 57
        mov     dword [ebp-10H], esp                    ; 000F _ 89. 65, F0
        push    ??_C@_03FIKCJHKP@abc?$AA@               ; 0012 _ 68, 00000000(d)
        call    strlen                                 ; 0017 _ E8, 00000000(rel)
        pop     ecx                                     ; 001C _ 59
        mov     dword [ebp-14H], eax                    ; 001D _ 89. 45, EC
        and     dword [ebp-4H], 00H                     ; 0020 _ 83. 65, FC, 00
        push    145                                     ; 0024 _ 68, 00000091
        call    near [GetKeyState]             ; 0029 _ FF. 15, 00000000(d)
        test    al, 01H                                 ; 002F _ A8, 01
        jz      _003                                   ; 0031 _ 74, 0D
        mov     eax, dword [ebp-14H]                    ; 0033 _ 8B. 45, EC
        cdq                                             ; 0036 _ 99
        idiv    dword [g_div]                          ; 0037 _ F7. 3D, 00000000(d)
        mov     dword [ebp-14H], eax                    ; 003D _ 89. 45, EC
_003:  or      dword [ebp-4H], 0FFFFFFFFH              ; 0040 _ 83. 4D, FC, FF
$LN8_2_:   mov     eax, dword [ebp-14H]                    ; 0044 _ 8B. 45, EC
        mov     ecx, dword [ebp-0CH]                    ; 0047 _ 8B. 4D, F4
; Note: Absolute memory address without relocation
        mov     dword [fs:0H], ecx                      ; 004A _ 64: 89. 0D, 00000000
        pop     edi                                     ; 0051 _ 5F
        pop     esi                                     ; 0052 _ 5E
        pop     ebx                                     ; 0053 _ 5B
        leave                                           ; 0054 _ C9
        ret                                             ; 0055 _ C3
; TestK2 End of function

_catch$?TestK2@@YAHXZ$0:; Local function
        push    ??_C@_06JBKEBCGB@except?$AA@            ; 0056 _ 68, 00000000(d)
        call    near [OutputDebugStringA]      ; 005B _ FF. 15, 00000000(d)
        or      dword [ebp-4H], 0FFFFFFFFH              ; 0061 _ 83. 4D, FC, FF
        mov     eax, $LN8_2_                               ; 0065 _ B8, 00000000(d)
        ret                                             ; 006A _ C3


SECTION .rdata progbits alloc noexec nowrite align=4                       ; section number 7, const
;  Communal section not supported by YASM

??_C@_06JBKEBCGB@except?$AA@:                           ; byte
        db 65H, 78H, 63H, 65H, 70H, 74H, 00H            ; 0000 _ except.


SECTION .bss                       ; section number 8, bss

g_div:                                                 ; dword
        resd    1                                       ; 0000


SECTION .rdata progbits alloc noexec nowrite align=4                       ; section number 9, const
;  Communal section not supported by YASM

??_C@_03FIKCJHKP@abc?$AA@:                              ; byte
        db 61H, 62H, 63H, 00H                           ; 0000 _ abc.


SECTION .text                         ; section number 10, code
;  Communal section not supported by YASM

_ehhandler$?TestK2@@YAHXZ:; Local function
        mov     eax, _ehfuncinfo$?TestK2@@YAHXZ        ; 0000 _ B8, 00000000(d)
        jmp     __CxxFrameHandler3                     ; 0005 _ E9, 00000000(rel)


SECTION .rdata                      ; section number 11, const
;  Communal section not supported by YASM

_catchsym$?TestK2@@YAHXZ$2:                            ; byte
        db 00H, 00H, 00H, 00H, 00H, 00H, 00H, 00H       ; 0000 _ ........
        db 00H, 00H, 00H, 00H                           ; 0008 _ ....
        dd _catch$?TestK2@@YAHXZ$0                     ; 000C _ 00000000 (d)

_unwindtable$?TestK2@@YAHXZ:                           ; byte
        db 0FFH, 0FFH, 0FFH, 0FFH, 00H, 00H, 00H, 00H   ; 0010 _ ........
        db 0FFH, 0FFH, 0FFH, 0FFH, 00H, 00H, 00H, 00H   ; 0018 _ ........

_tryblocktable$?TestK2@@YAHXZ:                         ; byte
        db 00H, 00H, 00H, 00H, 00H, 00H, 00H, 00H       ; 0020 _ ........
        db 01H, 00H, 00H, 00H, 01H, 00H, 00H, 00H       ; 0028 _ ........
        dd _catchsym$?TestK2@@YAHXZ$2                  ; 0030 _ 00000000 (d)

_ehfuncinfo$?TestK2@@YAHXZ:                            ; byte
        db 22H, 05H, 93H, 19H, 02H, 00H, 00H, 00H       ; 0034 _ ".......
        dd _unwindtable$?TestK2@@YAHXZ                 ; 003C _ 00000000 (d)
        dd 00000001H                                    ; 0040 _ 1 
        dd _tryblocktable$?TestK2@@YAHXZ               ; 0044 _ 00000000 (d)
        dd 00000000H, 00000000H                         ; 0048 _ 0 0 
        dd 00000000H, 00000000H                         ; 0050 _ 0 0 


SECTION .rdata                       ; section number 12, const

        db 21H, 00H, 00H, 00H                           ; 0000 _ !...


SECTION .text                         ; section number 13, code
;  Communal section not supported by YASM

TestK:; Function begin
        jmp     TestK2                          ; 0000 _ E9, 00000000(rel)
; TestK End of function


