 /
function!

 Use this function in exe if you want to add qmgrid.dll to the exe file.
 Returns: 1 success, 0 failed.

 REMARKS
 qmgrid.dll is required for <help #IDP_QMGRID>QM_Grid control</help>, <help #IDP_QMCOMBOBOX>QM_ComboBox control, QM_Edit control and ShowDropdownList</help>. It is a QM dll, unavailable on computers where QM is not installed.
 When making exe, this function adds qmgrid.dll to exe.
 When exe runs, this function extracts the dll to $temp qm$ folder, and loads.
 Call this function once, somewhere at the beginning of exe code. Not error to call multiple times or if the dll is already loaded somehow.
 This function does nothing if the macro runs in QM (not in exe) or using .qmm file.
 Don't use this function if you want the dll to be a separate file.

 You can use this function as a template to create similar functions for other dlls that you want to add to exe easily.
 Notes for creating such functions:
   1. If you use functions from the dll, declare them with dll- (delay-load).
   2. In the dll declaration should be dll name without path. QM 2.4.3: can be with path, then QM will use the loaded dll if the full-path file does not exist.
   3. Use unique resource id, not 21079. Also use unique variable, not ___eqgd.

 Added in: QM 2.3.3.

 EXAMPLE
 ExeQmGridDll ;;somewhere at the beginning of exe code


#if EXE=1
#exe addfile "$qm$\qmgrid.dll" 21079
int+ ___eqgd
if !___eqgd
	lock
	if !___eqgd
		if !GetModuleHandle("qmgrid.dll")
			_s.expandpath(F"$temp qm$\ver 0x{QMVER}\qmgrid.dll")
			if !FileExists(_s)
				if(!ExeExtractFile(21079 _s)) ret ;;also creates folders.
			if(!LoadLibraryW(@_s)) ret
		___eqgd=1
#endif
ret 1
