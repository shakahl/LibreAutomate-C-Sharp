 /
function VARIANT'number str&s [localeId]
 function VARIANT'number str&s [localeId] [NUMBERFMT&nf]

 Formats a number for current or specified locale.
 Fixed number of digits is always added after decimal.
 number must not be too big to be represented as string without E.

 EXAMPLE
 str s
 FormatNumber 2234567.544 s
 out s


def LOCALE_USER_DEFAULT 0x400
type NUMBERFMT NumDigits LeadingZero Grouping $lpDecimalSep $lpThousandSep NegativeOrder
dll kernel32 #GetNumberFormat Locale dwFlags $lpValue NUMBERFMT*lpFormat $lpNumberStr cchNumber

if(localeId=0) localeId=LOCALE_USER_DEFAULT
str ss=number
s.all(ss.len+30)
s.fix(GetNumberFormat(localeId 0 ss 0 s s.nc)-1)
 s.fix(GetNumberFormat(localeId 0 ss &nf s s.nc)-1)
