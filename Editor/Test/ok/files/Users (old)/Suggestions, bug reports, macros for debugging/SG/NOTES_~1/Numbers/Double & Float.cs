 Double/float bug. Run macro & see notes below
double half_fraction_double hfd
double half_decimal_double hdd
FLOAT half_fraction_float hff
FLOAT half_decimal_float hdf

hfd=1/2
hdd=0.5
hff=1/2
hdf=0.5
out "------------ output: ratio verses decimal ----------"
out hfd
out hdd
out hff
out hdf
double onehfd=(1/2)+(1/2)
double onehdd=(1=2)+(1/2)
out "----------------half + half ------------------------"
out onehfd
out onehdd
out (1/2)+(1/2)
out 0.5+0.5
out "hdd %2.2g hfd %2.2g hdf %2.2g hff %2.2g" hdd hfd hdf hff

out "======================================================="
out "--------------output when outing floats --------------------"
 bugs:
 1. when assigning to a double or float variable, using =(1/2) or =(3/2) etc gives integer result (wrong),
   but using =0.5 etc gives double result (correct).
 2. when printing a float #, then next variable is not output right:

double dx=1
int x=1
out "%2.2g %2.2g %2.2g %i		 OK" dx dx dx x	;; ok
out "%2.2g %2.2g %2.2g %i 		OK" dx hfd hdd x	;; ok
out "%2.2g %2.2g %2.2g %i	 ERROR" dx hfd hff x ;; x is not output right
out "%2.2f %i 		ERROR" hff x					;; ditto

 str s="End"
 out "%2.2g %2.2g %2.2g %2.2g %s" hfd hdd hff hdf s ;;causes exception

 3. when printing a float, its not right:
FLOAT testf=0.5
out "%2.2f			 WRONG" testf
