__Tcc x.Compile("" "flute")
double r
call(x.f 1.1 2.2 &r)
out r

#ret
void flute(double x, double y, double* r)
{
*r=x+y;
}