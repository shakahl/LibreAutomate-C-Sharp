 int w1=child("" "SHELLDLL_DefView" win("Program Manager" "Progman"))
 act w1
 men 31062 w1

 int w1=win("3. Kate Bush - Army Dreams - Winamp" "Winamp v1.x")
 men 40048 w1 ;;Next              
 men 40046 win(" - Winamp" "Winamp v1.x") ;;Next

int w=win("Program Manager" "Progman")
act w
int c=id(1 w) ;;list 'Desktop'
SendClickMessage 0 0 c 0 2
