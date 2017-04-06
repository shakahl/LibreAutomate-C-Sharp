act "Excel"
ExcelSheet es.Init
IDispatch x=es.ws.Application
 now insert Excel macro and append x. to the beginning of every line

x.Range("B2").Select
x.ActiveSheet.Pictures.Insert("C:\Documents and Settings\All Users\Documents\My Pictures\avatar_bird.gif").Select
