out
  Declare Variables
ARRAY(str) removeNamesList, SANamesCheckList
int counter1, counter2

ExcelSheet esData.Init("Sheet2") ;;Initialize an ExcelSheet object
ExcelSheet esVCC.Init("Sheet1") ;;Initialize an ExcelSheet object

  Initialize all variables to be sure
counter1=0
counter2=0

 Obtain 2 Excel lists(dynamic ranges) into arrays
esData.CellsToArray(removeNamesList "M:M") ;;Obtain list of names from 'Data' Sheet that have to be replaced/removed - Master List
esVCC.CellsToArray(SANamesCheckList "G:G") ;;Obtain list of name from 'VCC Release Sheet' that need to be checked against Master List
out removeNamesList.len
out SANamesCheckList.len

 arrayCheckRoutine
  Check one array against another and change those not in Master list
for counter1 1 SANamesCheckList.len
	out SANamesCheckList[0 counter1]
	for counter2 1 removeNamesList.len ;;Loop through Master List Array
		out "    %s" removeNamesList[0 counter2]
		if SANamesCheckList[0 counter1]=removeNamesList[0 counter2] ;;Check if current SANamesCheck is in SANamesMaster list
			goto obtainSANameFromAL
			break

ret ;;Ends Macro as the 'SANamesCheck' list is now fully checked.

 obtainSANameFromAL
out "--------"
  Check Autoline system for the correct SA Name and replace in Excel
out SANamesCheckList[0 counter1]