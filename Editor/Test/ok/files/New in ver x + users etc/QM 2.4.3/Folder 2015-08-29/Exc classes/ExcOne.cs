out "ExcOne: %i" &this
 out "ExcOne: %i, in %s" &this _s.getmacro(getopt(itemid 1) 1) ;;error: QM item not found
 end
 min 0
 if(mes("make error?" "" "YN")='Y') min 0
