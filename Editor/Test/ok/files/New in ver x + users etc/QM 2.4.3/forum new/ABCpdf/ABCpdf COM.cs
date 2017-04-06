IDispatch theDoc._create("ABCpdf9.Doc")
theDoc.FontSize = 96
theDoc.AddText("Quick Macros")
theDoc.Save(_s.expandpath("$desktop$\QM ABCpdf COM.pdf"))

 or

typelib ABCpdf {9F74D25B-CA59-48F9-B374-26CE7A34EF1B} 9.1
ABCpdf.Doc theDoc2._create
theDoc2.FontSize = 96
theDoc2.AddText("Quick Macros")
theDoc2.Save(_s.expandpath("$desktop$\QM ABCpdf COM 2.pdf"))
