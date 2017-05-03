From guidelines:

DO NOT return error codes. Use exceptions.
DO NOT have public members that can either throw or not based on some option.
CONSIDER 'Tested-Doer' pattern (if(x.CanDoIt()) x.DoIt()) or 'Try-Parse' pattern (if(!x.TryDoIt()) ...).
Other guidelines are obvious.

See also (many useful info):
https://msdn.microsoft.com/en-us/library/system.exception.aspx

____________________________

AVOID USING

Exception, SystemException:
	DO NOT throw (it's a base class). Avoid catching, except in top-level exception handlers.

ApplicationException:
	DO NOT throw (it's a base class).

NullReferenceException, AccessViolationException:
	DO NOT throw (used by CLR). Do argument checking to avoid throwing these exceptions.

OutOfMemoryException, ComException, SEHException, ExecutionEngineException:
	DO NOT throw (used by CLR).

IndexOutOfRangeException:
	DO NOT throw (used by CLR). Instead use ArgumentOutOfRangeException.

StackOverflowException: DO NOT throw or catch.

____________________________

CAN USE

Source 1:

InvalidOperationException:
	Throw if the object is in an inappropriate state.

ArgumentException, ArgumentNullException, ArgumentOutOfRangeException:
	Throw ArgumentException or one of its subtypes if bad arguments are passed to a member.
	Prefer the most derived exception type, if applicable.
	Set the ParamName property.


Source 2:

ArgumentException: A non-null argument that is passed to a method is invalid.

ArgumentNullException: An argument that is passed to a method is null.

ArgumentOutOfRangeException: An argument is outside the range of valid values.

DirectoryNotFoundException: Part of a directory path is not valid.

DivideByZeroException: The denominator in an integer or Decimal division operation is zero.

DriveNotFoundException: A drive is unavailable or does not exist.

FileNotFoundException: A file does not exist.

FormatException: A value is not in an appropriate format to be converted from a string by a conversion method such as Parse.

InvalidOperationException: A method call is invalid in an object's current state.

KeyNotFoundException: The specified key for accessing a member in a collection cannot be found.

NotImplementedException: A method or operation is not implemented.

NotSupportedException: A method or operation is not supported.

ObjectDisposedException: An operation is performed on an object that has been disposed.

OverflowException: An arithmetic, casting, or conversion operation results in an overflow.

PathTooLongException: A path or file name exceeds the maximum system-defined length.

PlatformNotSupportedException: The operation is not supported on the current platform.

RankException: An array with the wrong number of dimensions is passed to a method.

TimeoutException: The time interval allotted to an operation has expired.

UriFormatException: An invalid Uniform Resource Identifier (URI) is used.

System.ComponentModel.InvalidEnumArgumentException: Invalid enum value.


Source 3:

ArgumentNullException: null reference

ArgumentOutOfRangeException: Outside the allowed range of values (such as an index for a collection or list)
	
ComponentModel.InvalidEnumArgumentException: Invalid enum value

FormatException: Contains a format that does not meet the parameter specifications of a method (such as the format string for ToString(String))

System.ArgumentException: Otherwise invalid

InvalidOperationException: When an operation is invalid for the current state of an object

ObjectDisposedException: When an operation is performed on an object that has been disposed

NotSupportedException: When an operation is not supported (such as in an overridden Stream.Write in a Stream opened for reading)

OverflowException: When a conversion would result in an overflow (such as in a explicit cast operator overload)

For all other situations, consider creating your own type that derives from Exception and throw that.
