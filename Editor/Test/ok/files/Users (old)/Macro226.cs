ExcelSheet es.Init
Excel.Range ra=es.ws.Application.Selection
ra.NumberFormat="General"
ARRAY(VARIANT) a
a=ra.Value
ra.Value=a
