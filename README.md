![Logo](Art/Logo150x150.png "Logo")

# Genesis.Ensure

[![Build status](https://ci.appveyor.com/api/projects/status/lu38d2kn4tmclwv4?svg=true)](https://ci.appveyor.com/project/kentcb/genesis-ensure)

## What?

> All Genesis.* projects are formalizations of small pieces of functionality I find myself copying from project to project. Some are small to the point of triviality, but are time-savers nonetheless.
 
**Genesis.Ensure** is a simple library for application authors (_not_ library authors) to facilitate asserting runtime expectations. It is delivered as a PCL targeting a wide range of platforms, including:

* .NET 4.5
* Windows 8
* Windows Store
* Windows Phone 8
* Xamarin iOS
* Xamarin Android

## Why?

[Code Contracts]() delivers similar (though much richer) functionality than **Genesis.Ensure**. The reasons you might consider **Genesis.Ensure** over Code Contracts include:

 * Code Contracts requires extraneous tooling to translate contracts into executable code. This tooling is not supported on all platforms, such as Xamarin
 * **Genesis.Ensure** has a simpler API

## Where?

The easiest way to get **Genesis.Ensure** is via [NuGet](http://www.nuget.org/packages/Genesis.Ensure/):

```PowerShell
Install-Package Genesis.Ensure
```

## How?

**Genesis.Ensure** is super simple to use. It provides only a single, `static` class called `Ensure`. Using the `Ensure` class, you can validate arguments and other conditions.

Here are some examples:

```C#
public void SomeMethod(string key, object value)
{
    Ensure.ArgumentNotNullOrEmpty(key, nameof(key));
    Ensure.ArgumentNotNull(value, nameof(value));

    ...
}

public void SomeMethod(DayOfWeek day)
{
    Ensure.ArgumentIsValidEnum(day, nameof(day));
}
```

**IMPORTANT:** unless your consuming assembly defines the `ENSURE` compiler symbol, all calls to `Ensure` will be elided from the built assembly. This makes it very simple to optimize out these calls for performance-critical scenarios, such as mobile applications and embedded development. 

## Who?

**Genesis.Ensure** is created and maintained by [Kent Boogaart](http://kent-boogaart.com). Issues and pull requests are welcome.