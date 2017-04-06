---- Enter New Customer data using Amanda's Spreadsheet ----
str browser
str controls = "1001"
str cb1001Bro
cb1001Bro="&Chrome[]Opera[]Firefox[]Internet Explorer[]"
if(!ShowDialog("New_CustomerWSAG" 0 &controls)) ret
browser=cb1001Bro
out browser

   edit the path in the next statement to point to the desired spreadsheet
ExcelSheet es.Init("Customers" 8 "C:\Macro_Test\test_0502.xlsx")
ARRAY(str) cust
es.CellsToArray(cust "")
int c w r rr
DateTime t0 t1 tt

str id1="a:id=emailAddressInput"
str id2="a:id=billingEmailAddressInput"
int cn=0

if browser="0 Chrome"
w=act(win("10-4 Marketplace * Google Chrome" "" "" 1))
else if browser="2 Firefox"
w=act(win("10-4 Marketplace * Mozilla Firefox" "" "" 1))
else if browser="3 Internet Explorer"
w=act(win("10-4 Marketplace * Internet Explorer" "" "" 1))
else if browser="1 Opera"
w=act(win("10-4 Marketplace * Opera" "" "" 1))
out browser
for rr 1 50
if cust[0 rr]=""
goto endmacro
out rr
spe 500
opt slowkeys 1
Acc siteName.Find(w "" "" "a:id=customerNameInput")
siteName.Select(3)
spe 200
'F"{cust[0 rr]}" T ;; Customer Name
if cust[1 rr]="Broker" ;; Customer Type
'DTST F"{cust[2 rr]}" ;; MC#
else if cust[1 rr]="Shipper"
'DDTST F"{cust[3 rr]}" ;; EIN#
'T
'F"{cust[4 rr]}" T ;; Website URL
'F"{cust[5 rr]}" T ;; Open credit Amount
spe 100
'F"{cust[6 rr]}" T ;; Customer Description
spe 200
'F"{cust[7 rr]}" T ;; Order Status Email
'F"{cust[8 rr]}" T ;; Work Address Line 1
'F"{cust[9 rr]}" T ;; Work Address Line 2
'F"{cust[10 rr]}" T ;; Work Postal Code
0.5 ;; wait to find postal code
'F"{cust[11 rr]}" T ;; Work City
'F"{cust[12 rr]}" T ;; Work State
Acc emailWork.Find(w "" "" id1)
emailWork.Select(3)
'F"{cust[13 rr]}" T ;; Work Email
'F"{cust[14 rr]}" T ;; Work Phone
'F"{cust[15 rr]}" T ;; Work Phone Extension
'F"{cust[16 rr]}" T ;; Work Fax
if cust[17 rr]="Yes"
'VT ;; Billing Address Same as Work Address
goto billsame
else
'T
'F"{cust[18 rr]}" T ;; Billing Address Line 1
'F"{cust[19 rr]}" T ;; Billing Address Line 2
'F"{cust[20 rr]}" T ;; Billing Postal Code
0.5 ;; wait to find postal code
'F"{cust[21 rr]}" T ;; Billing City
'F"{cust[22 rr]}" T ;; Billing State
Acc emailBilling.Find(w "" "" id2)
emailBilling.Select(3)
'F"{cust[23 rr]}" T ;; Billing Email
'F"{cust[24 rr]}" T ;; Billing Phone
'F"{cust[25 rr]}" T ;; Billing Phone Extention
'F"{cust[26 rr]}" T ;; Billing Fax
billsame
spe 250
'TV ;; Add Customer
0.5

Acc cnd.Find(w "PUSHBUTTON" "Create New Customer" "a:class=btn btn-default btn-primary[]a:data-bind=click: function ()")
cnd.Mouse(1)

endmacro
end



BEGIN DIALOG
0 "" 0x90C80AC8 0x0 0 0 150 100 "Select Browser"
1001 ComboBox 0x54230243 0x0 24 14 96 213 "Browser"
1 Button 0x54030001 0x4 22 80 48 14 "OK"
2 Button 0x54030000 0x4 78 80 48 14 "Cancel"
END DIALOG
DIALOG EDITOR: "" 0x2030605 "*" "0" "" ""
