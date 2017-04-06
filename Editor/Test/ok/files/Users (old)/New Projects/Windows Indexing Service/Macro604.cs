out
typelib CIODMLib {3BC4F393-652A-11D1-B4D4-00C04FC2DB8D} 1.0
typelib Cisso {4E469DD1-2B6F-11D0-BFBC-0020F8008024} 1.0 0x409

Cisso.CissoQuery q._create
q.Catalog="HtmlHelp"
 q.Columns="Filename"
q.Columns="DocTitle, hitcount"
 out q.Dialect ;;2
 q.Dialect="1"
 out q.OptimizeFor
 q.Query="#filename *.html"
 q.Query="accessible"
 q.Query="$contents ''accessible''"
 q.Query="$contents ''activated''"
 q.Query="$contents activated"
 q.Query="$contents active"
  q.Query="$contents {generate method=inflect} active {/generate}"
 q.Query="$contents object**"
 q.Query="$contents {generate method=inflect} work {/generate}"
 q.Query="$contents works"
q.Query="$contents apply**"
ADO.Recordset rs=q.CreateRecordset("nonsequential")
out rs.RecordCount
 ret

int r f nr nf
ADO.Field field
rs.MoveFirst
nr=rs.RecordCount; nf=rs.Fields.Count
 out nr
 a.create(nf nr)
for r 0 nr
	f=0
	foreach(field rs.Fields)
		 a[f r]=field.Value; err
		out field.Value; err
		f+1
	rs.MoveNext

