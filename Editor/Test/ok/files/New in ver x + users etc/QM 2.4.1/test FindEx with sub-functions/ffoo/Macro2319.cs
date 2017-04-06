 mac "Function286"
mac "sub.Sub"
0.1
 out IsThreadRunning("Function286")
out IsThreadRunning("Macro2319:Sub")
out IsThreadRunning("\ffoo\Macro2319:Sub")
out IsThreadRunning("<00003>Sub")
out IsThreadRunning("\ffoo\<00003>Sub")


#sub Sub
out __FUNCTION__
mes 1
