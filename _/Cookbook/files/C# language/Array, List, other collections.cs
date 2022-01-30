/// An array is a variable that holds multiple elements of the same type. Elements can be accesed by index and can be modified. <google C# arrays>More info<>.

int[] a1 = { 1, 3, 5, 7, 9 }; //array variable a1 has 5 elements of type int
var a2 = new int[10]; //array variable a2 has 10 elements of type int, all 0 (default value of that type)
string[] a3 = { "a", "b" };
int[] a4 = null; //no array
a4 = new[] { 10, 20, 30 };

for (int i = 0; i < a2.Length; i++) a2[i] = i; //set values
for (int i = 0; i < a2.Length; i++) print.it(a2[i]); //get values
foreach (var v in a1) print.it(v); //get values with foreach
print.it(a1[0], a1[^1]); //get the first and the last element
print.it(a1.Contains(7), Array.IndexOf(a1, 7)); //some array functions

/// Arrays can have multiple dimensions. Array elements can be arrays of different length. But these aren't often used.

/// Arrays have fixed size. If need to add/remove elements, use <google C# 'List T class'>List<> instead.

List<int> k1 = new() { 1, 3, 5, 7, 9 }; //list variable k1 has 5 elements of type int
var k2 = new List<int> { 1, 3, 5, 7, 9 }; //the same
List<string> k3 = new(); //create empty list
var k4 = new List<string>(); //the same

k1.Add(11); //append 1 element with value 11
k1.Insert(0, -1); //at index 0 insert 1 element with value -1
k1.RemoveAt(k1.Count - 1); //remove the last element
k1.Clear(); //remove all elements
print.it(k2.Contains(5), k2.IndexOf(5));
int[] a10 = k2.ToArray(); //create new array and copy elements

/// List elements are accessed (get, set) like array elements.

for (int i = 0; i < k1.Count; i++) print.it(k1[i]); //get values

/// If need a stack (add/get/remove the last element), can be used <google C# 'Stack T class'>Stack<>.

var k = new Stack<int>();
k.Push(4); //append 1 element with value 4
while (k.TryPop(out var v)) print.it(v); //get and remove elements starting from the last

/// If need a queue (add last, get/remove first), can be used <google C# 'Queue T class'>Queue<>.

var q = new Queue<int>();
q.Enqueue(4); //append 1 element with value 4
while (q.TryDequeue(out var v)) print.it(v); //get and remove elements starting from the first

/// If need a collection of key-value pairs where values can be accessed by key in a fast way, use <google C# 'Dictionary T class'>Dictionary<>.

var d1 = new Dictionary<string, int>(); //keys of type string, values of type int
d1.Add("one", 1);
d1.Add("two", 2);
//d1.Add("one", 3); //throws exception if the key already exists
if (!d1.TryAdd("one", 3)) print.it("failed to add"); //adds if the key does not exist
d1["one"] = 3; //adds or replaces

print.it(d1["one"], d1["two"], d1.Count); //get some values and the count of elements
//var v1 = d1["three"]; //throws exception, because key "three" does not exist
if (d1.TryGetValue("three", out var v1)) print.it(v1); //if key "three" exists, get its value
if (d1.ContainsKey("two")) print.it("contains"); //can be used when don't need the value

if (!d1.Remove("three")) print.it("OK, but the key did not exist"); //remove if exists
if (d1.Remove("two", out var v2)) print.it(v2); //remove if exists and get value
d1.Clear(); //remove all elements

//string-int dictionary with case-insensitive keys and 3 elements
var d2 = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) {
	{ "January", 1 }, //same as d2.Add("January", 1);
	{ "February", 2 },
	{ "March", 3 },
};
print.it(d2["march"]);

/// If need a collection with unique keys without values, use <google C# 'HashSet T class'>HashSet<>.

/// If need to use a collection in multiple threads, use <+lang lock statement><.k>lock<><> or classes from the <b>System.Collections.Concurrent</b> namespace, such as <b>ConcurrentDictionary</b>.

/// If need a variable that holds multiple values of different types, use one of:
/// - <+lang value tuple types>Tuple<>.
/// - Create a <google C# class>class</google> or <google C# struct>struct</google> with public fields.
/// - If need to access elements by index or key, use array/List/Dictionary/etc of type <.k>object<>.
