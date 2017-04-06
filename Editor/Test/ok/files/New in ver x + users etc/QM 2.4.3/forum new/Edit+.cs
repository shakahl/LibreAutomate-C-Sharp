 trigger: @11
Disable mou ''...'' lines :_s.getmacro; if(_s.replacerx("(?m)^mou ''.+[]" " $0")) _s.setmacro
Enable mou ''...'' lines :_s.getmacro; if(_s.replacerx("(?m)^ (mou ''.+[])" "$1")) _s.setmacro
Delete disabled mou ''...'' lines :_s.getmacro; if(_s.replacerx("(?m)^ mou ''.+[]")) _s.setmacro
Delete all mou ''...'' lines :_s.getmacro; if(_s.replacerx("(?m)^ ?mou ''.+[]")) _s.setmacro
