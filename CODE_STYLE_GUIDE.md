# glTF Asset Generator Code Style Guide

The following sections are mostly general C# conventions, but come up often in this project. If something isn't covered then refer to the following links, which are used by Microsoft to develop samples and documentation.
+ [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
+ [Identifier names](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/identifier-names)
+ [C# programming guide](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/index)

### Cross-platform Compatibility
Make build paths and other strings so that separators are platform neutral.
+ Use `System.IO.Path.Combine` to create filepaths, instead of explicitly typing the path separators.

### Identifier Names
An identifier is the name you assign to a type (class, interface, struct, delegate, or enum), member, variable, or namespace.
+ Identifiers start with a letter.
+ Attribute types end with the word `Attribute`.
+ Decimal digit characters are at the end of variable names, or are spelled out instead.
+ If a using directive is not included, save and use the namespace qualification as a variable to keep code compact and readable.
+ Use Pascal Case for type names, namespaces, and all public members. Use lower camel case for all other identifiers.
+ Be detailed when naming variables. We'd prefer to have longer variable names whose purpose is easily understood.

### Commenting
Please use comments to leave hints before sections of complex logic, to make the intent easier to understand.
+ Place the comment on a separate line, not at the end of a line of code.
+ Begin comment text with an uppercase letter.
+ End comment text with a period.
Use summary blocks instead of a normal comment for functions.

### Implicit Typing
Use implicit typing for local variables when the type of the variable is obvious from the right side of the assignment, or when the precise type is not important.
```
var var1 = "This is clearly a string.";
var var2 = 27;
var var3 = Convert.ToInt32(Console.ReadLine());
```
Use implicit typing to determine the type of the loop variable in `for` and `foreach` loops.
```
for (var i = 0; i < 10; i++)
{
    Console.WriteLine(i);
}
```
Use the concise form of object instantiation
```
var instance1 = new ExampleClass();
```

### Arrays
Use the concise syntax when you initialize arrays on the declaration line.
```
// Preferred syntax. Note that you cannot use var here instead of string[].
string[] vowels1 = { "a", "e", "i", "o", "u" };

// If you use explicit instantiation, you can use var.
var vowels2 = new string[] { "a", "e", "i", "o", "u" };

// If you specify an array size, you must initialize the elements one at a time.
var vowels3 = new string[5];
vowels3[0] = "a";
vowels3[1] = "e";
// And so on.
```

Add a linebreak after every three indices (A triangle).

When adding a list of vectors, or other floats, add zeros to line up values where reasonable. 
For the same reasons, leave a space before positive numbers when there are also negative numbers in the same column.
```
new List<Vector2>
{
    new Vector2( 0.00f, 0.75f),
    new Vector2( 0.25f,-0.75f),
    new Vector2( 0.25f,-0.50f),
    new Vector2(-0.00f, 0.50f),
}
```
