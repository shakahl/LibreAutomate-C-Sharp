 Shows how to use Windows Search (WS) from QM, using SQL query.
 WS is a Windows component that is used to find files.
 For example, Windows uses it when you type in the search boxes in Start menu and Windows Explorer. You can access and change WS settings from there.
 It is in Vista/7. For XP can be downloaded from Microsoft.

 WS documentation: http://msdn.microsoft.com/en-us/library/ff628790%28v=VS.85%29.aspx
 WS SQL reference: http://msdn.microsoft.com/en-us/library/bb231256%28v=VS.85%29.aspx
 Shell properties (System.XXX) reference: http://msdn.microsoft.com/en-us/library/dd561977%28VS.85%29.aspx

 This code is converted from the 'WSFromScript' VBScript sample.
 Also there are other ways to use WS. For example use typelib MSSCTLB {9E175B60-F52A-11D8-B9A5-505054503030} 1.0.

out
IDispatch objConnection objRecordSet

objConnection._create("ADODB.Connection")
objRecordSet._create("ADODB.Recordset")

 This is the Windows Search connection string to use
objConnection.Open("Provider=Search.CollatorDSO;Extended Properties='Application=Windows';")

 SQL SELECT statement specifies what properties to return, you can add more if you want
     FROM - use SystemIndex for a local query or MACHINENAME.SystemIndex for remote
     WHERE - specify restrictions including SCOPE and other conditions that must be true

 This is a very simple query over the whole index. To add scope restriction append "WHERE SCOPE='file:c:/users'" to the query string.
objRecordSet.Open("SELECT TOP 10 System.ItemPathDisplay, System.ItemTypeText, System.Size FROM SystemIndex" objConnection) ;;display the first 10 indexed files; remove the 'TOP 10' to display all.
 objRecordSet.Open("SELECT System.ItemPathDisplay, System.ItemTypeText, System.Size FROM SystemIndex WHERE SCOPE='file:Q:\Downloads'" objConnection) ;;search single folder and its subfolders; to skip subfolders, use DIRECTORY instead of SCOPE.
 objRecordSet.Open("SELECT System.ItemPathDisplay, System.ItemTypeText, System.Size FROM SystemIndex WHERE CONTAINS('macro')" objConnection) ;;search all containing word "macro"
 objRecordSet.Open("SELECT System.ItemPathDisplay, System.ItemTypeText, System.Size FROM SystemIndex WHERE CONTAINS(System.Author,'G')" objConnection) ;;search all where author is "G"

objRecordSet.MoveFirst
rep
	if(objRecordSet.EOF) break
	 Access the column values that were specified in the SELECT statement here
	str s1 s2; int i
	s1=objRecordSet.Fields.Item("System.ItemPathDisplay")
	s2=objRecordSet.Fields.Item("System.ItemTypeText")
	i=objRecordSet.Fields.Item("System.Size"); err i=0
	out "'%s'  '%s'  '%i'" s1 s2 i
	objRecordSet.MoveNext
