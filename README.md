# whiteMath + whiteStructs

whiteMath is an open-source, programmer-friendly C# .NET mathematical/algorithmic library.

Current version: 0.2.0

The source code of the libary is licensed under GNU GPL v.3.

This library should not by any means be treated as complete in terms of functionality and documentation. 
It does not look like it will ever be complete in terms of functionality; unfortunately, it is likely that 
the documentation isn't going to cover everything, either.

## Description

This is a mathematics / algorithms / data structures project that was initially designed to assist me in 
my student endeavours at the Higher School of Economics, Moscow, Russia. What is it now? You decide.

It can already do something:

- Generic mathematical functions for arbitrary numeric types using the Numeric<T, C> wrapper. 

You just write an `ICalc` calculator for your brand-new `MyComplex`, `MyInt` or `Foo` numeric type 
(it can even be a `string`) that has basic operations and constants defined, such as zero, sum, subtraction, 
multiplication etc. Then – poof, magic! These generic functions start working for you. They can even do 
trigonometry and exponentiation using Taylor series expansion.

All the statistical functions, such as `SampleAverage`, `MovingAverage` etc. are also (hopefully) going to work 
with arbitrary numeric types.

- Long integer arithmetic with arbitrary numeric bases for digit arrays (something `BigInt` doesn't have).

- Matrix operations (also implemented using `Numeric<T, C>`).

- A not-so-terrible grapher library (only 2D graphs are currently supported). Surprisingly, it really looks 
better than some of the graphers available out there. 

- A nice collection of pseudo-random number generators and respective interfaces.

- A cryptographic sub-library currently containing only the RSA algorithm.

- Primality tests.

- And more, which I hope to describe someday.

## Tools used

1. IDE: Microsoft Visual Studio (Windows) and Xamarin Studio (Mac)
2. Help/Documentation: SandCastle, a tool building HTML/CHM files from XML comments (https://github.com/EWSoftware/SHFB)
3. Unit testing: NUnit 3.0 (http://www.nunit.org/)

We used to use the Design By Contract approach (using .NET Code Contracts), but due to terrible Mono support the 
contracts have been dropped since 0.2.0. Currently, an internal validation instrument, `whiteStructs.Conditions`, 
is being used and developed.

## Why?

Because I believe that shareable ideas ought to be shared and expanded upon.

I wanted to open-source this ages years ago, but thought to write a thorough documentation and brush it all up before 
making it public. This never happened, as you could expect.

So why now? Because today I stopped caring. Perhaps this goal is more reachable together.

## Declaration

I understand that this probably isn't a very good readme for an open-source math library. It will eventually get better, 
probably not without your help.

Also, the code isn't too consistent in structure and perhaps not very self-explanatory. Something can be learnt from the 
XML doc comments in the source files, and something should be discussed in order to be worked on together.

## Bad documentation

Currently, there are two types of hardly understandable and badly documented code in the library:

1. "How in the world this thing works?"
2. "What in the world this thing does?"

The second type should be urgently reported using appropriate media (e.g. the issues channel). Code of the first type 
can also be reported, but the author does not guarantee that he would remember and say anything intelligent in response.
You see, this library began in the far 2008, when the originator hardly knew any C# (the artifacts of this prehistoric 
period can still be found scattered throughout the library).

At the moment, the author tries to document everything thoroughly using XML and in-line comments. Some of these comments 
are still in Russian, but the situation is changing for the better.

## Bugs / Issues

I suspect there are lots of them. Most of the functions were in real-life use from one to several times, and after that 
were happily abandoned.

We have introduced unit tests in 0.1.0, so please report and contribute meaningful tests.

## Important Copyright Notes

- The idea of `Numeric<T, C>` appears to be stolen / rewritten from somewhere.
- `DoubleInfoExtraction.cs` is also taken from somewhere, I will happily give credit if someone points out the author.
- There are numerous other bits of code which I didn't care about when I took them from the Web, but it is understood that all their authors deserve at least their name to be mentioned. 

So please drop me an email if you find anything that should be given credit for.