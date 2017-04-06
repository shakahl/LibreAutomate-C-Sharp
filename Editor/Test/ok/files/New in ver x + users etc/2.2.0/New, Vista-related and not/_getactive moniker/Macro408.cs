out
 Excel.Application a._getactive(0 16 ".")
 out a.Version

 Excel.Workbook b._getactive() ;;error
Excel.Workbook b._getactive(0 16 "*ok?") ;;ok
out b.Name
