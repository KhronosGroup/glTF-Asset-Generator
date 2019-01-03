# glTF Asset Generator Code Style Guide

The following sections are mostly general C# conventions, but come up often in this project. If something isn't covered then refer to the following links, which are used by Microsoft to develop samples and documentation.
+ [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
+ [Identifier names](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/identifier-names)
+ [C# programming guide](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/index)

## Cross-platform Compatibility
Make build paths and other strings so that separators are platform neutral.
+ Use `System.IO.Path.Combine` to create file paths, instead of explicitly typing the path separators.

## Arrays
Add linebreaks so that an array will look like the structure it represents. 
For example:
+ After every four values in a 4x4 matrix
+ After every three indices in triangle lists.
```C#
Indices = new List<int>
{
    0, 1, 2, 
    0, 2, 3, 
    4, 5, 6,
    4, 6, 7,
},
```

Format values so the decimal points line up, to increase readability.
+ When adding a list of vectors, or other floats, add zeros where reasonable. 
+ Leave a extra space before positive numbers when there are also negative numbers in the same column. 
```C#
new List<Vector2>
{
    new Vector2( 0.00f,  0.75f),
    new Vector2( 0.25f, -0.75f),
    new Vector2( 0.25f, -0.50f),
    new Vector2(-0.00f,  0.50f),
}
```

## Identifier Names
An identifier is the name you assign to a type (class, interface, struct, delegate, or enum), member, variable, or namespace.
+ Use Pascal case for type names, namespaces, and all public members. Use lower camel case for all other identifiers.
+ Be detailed when naming variables. Longer variable names are preferred over abbreviations.

## Implicit Typing
Use implicit typing for local variables when the type of the variable is obvious from the right side of the assignment, or when the precise type is not important.
```C#
var var1 = "This is clearly a string.";
var var2 = 27;
var var3 = Convert.ToInt32(Console.ReadLine());
```
Use implicit typing to determine the type of the loop variable in `for` and `foreach` loops.
```C#
for (var i = 0; i < 10; i++)
{
    Console.WriteLine(i);
}
```
Use implicit typing for object instantiation.
```C#
var instance1 = new ExampleClass();
```
