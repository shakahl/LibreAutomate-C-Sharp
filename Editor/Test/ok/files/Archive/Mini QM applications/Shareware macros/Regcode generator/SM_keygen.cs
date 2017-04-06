/Dialog_Editor
 This is your generator of registration codes. Stores the regcode in the clipboard.
 Don't include this function with your macros.

 Your unique data that you must change.
lpstr encryption_key="encryption key" ;;Some unique string. Use the same value in your ..._CanRun function.

 Remaining code does not need to be changed. You make your regcode generation method unique
 by just providing an unique encryption key (the value of the encryption_key variable).

 ______________________________________________

str regcode encryptedname

 Input customer's name.
str controls = "5 6"
str e5Nam e6Com
if(!ShowDialog("SM_keygen" 0 &controls)) ret
if(e6Com.len) e5Nam.formata(", <%s>" e6Com)

 Generate regcode. The same algorithm is used in your ...CanRun function.
 The regcode has format "xxxxxxxxxxxxxxxx=Customer's Name", where xxxxxxxxxxxxxxxx is encrypted customer's name, which also can include computer ID.
encryptedname.encrypt(9 e5Nam encryption_key)
encryptedname.fix(16)
e5Nam.escape(9) ;;escape dangerous characters
regcode.from(encryptedname "=" e5Nam)

 Store customer's name to the clipboard.
regcode.setclip

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 131 "Registration code generator"
 3 Static 0x54000000 0x0 10 20 44 13 "Name"
 5 Edit 0x54030080 0x200 56 18 158 14 "Nam"
 4 Static 0x54000000 0x0 10 36 44 13 "Computer ID"
 6 Edit 0x54032000 0x200 56 34 44 14 "Com"
 1 Button 0x54030001 0x4 6 114 48 14 "OK"
 2 Button 0x54030000 0x4 58 114 48 14 "Cancel"
 9 Static 0x54000000 0x0 8 94 214 16 "Click OK to generate registration code and store it to the clipboard."
 8 Static 0x54000000 0x0 10 54 204 26 "Computer ID associates the registration code with the computer. Leave empty to generate registration code that can be used on any computer."
 7 Button 0x54020007 0x0 4 4 216 80 "Customer information"
 END DIALOG
 DIALOG EDITOR: "" 0x2010805 "*" ""
