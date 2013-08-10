whiteMath + whiteStructs
========================

An open-source programmer-friendly C# .NET mathematical++ library.
Current version: 1.0.0.
Released under Apache v2 open-source license. 

As of 2013.08.10 - "just gone open-source" (c)

The library should not by any means be treated as something complete in terms
of both functionality and documentation. Hopefully, it will never be complete
in terms of functionality; unfortunately, it is likely that the documentation
isn't going to cover everything, either.

-2. WHAT?

This is a mathematical++ (maths, data structures, helper classes) library initially
designed to assist me in my student projects at the Higher School of Economics, 
Moscow, Russia. What is it now? You decide.

-1. WHY?

Because I believe that the majority of shareableideas ought to be shared.

I wanted to go open-source more than four years ago, but wanted to write
a thorough documentation before I would disclose all that stuff.
This never happened, as you could expect.

So why now? Because today I stopped caring.
Maybe we could do it together?

0. TODO:

I understand that perhaps this isn't a very good readme for an open-source library.
I honestly promise to write one very soon - just don't know where to begin.

The code isn't too consistent in structure and perhaps not very self-explanatory;
still, something can be learnt from the Sandcastle-generated documentation
included in repository.

Stay tuned - and I will really appreciate any help. 

1. BAD DOCUMENTATION:

Currently, there are two types of hardly understandable and badly documented code 
in the library:

1. "How in the world this thing works?"
2. "What in the world this thing does?"

The second type should be urgently reported using appropriate media (e.g. the issues
channel). Code of the first type can also be reported, but the author does not guarantee 
that he would remember and say something intelligent in response. You see, this library 
began in the far 2008, when the originator hardly knew any C# (the artifacts of
this prehistoric period can still be found scattered throughout the library).

Currently, the author tries to document everything thoroughly using XML and in-line
comments. Sadly, part of these comments are still in Russian, so feel free to
ask for an appropriate translation.

2. BUGS: 

I suspect there are lots of them. Most of the functions were in real-life use from one to
several times, and after that was happily abandoned. I have never bothered with thorough
randomized testing either.

Please report.

3. COPYRIGHT NOTES:

- The idea of Numeric<T, C> appears to be stolen/rewritten from somewhere. My conscience 
eats me for that, but I just can't find the source. 
- DoubleInfoExtraction.cs is also taken from somewhere.
- There are other bits of code which I didn't care about when I took them from the Web, but 
it is understood that all their authors deserve at least their name to be mentioned. 

So please report if you find anything.
You can also tell me what bad boy I was - I really know.

3.5. TOOLS USED:

3.5.0 - Visual Studio 2012 Ultimate Edition
3.5.1 - SandCastle for building HTML/CHM files from XML comments: http://shfb.codeplex.com/
3.5.2 - .NET Code Contracts: http://visualstudiogallery.msdn.microsoft.com/1ec7db13-3363-46c9-851f-1ce455f66970
3.5.3 - Brain

4. ALL IN ALL,

It can already do something:

- Generic mathematical functions for arbitrary numeric types using 
the Numeric<T, C> wrapper. 

You just write a calculator (see ICalc<T>) for your brand-new MyComplex,
MyInt or Foo numeric type (it can even be a String), which defines basic 
operations an constants such as zero, sum, subtraction, multiplication etc. - 
and (magic!) these generic functions start working for you. They can even calculate
Sin, Cos and Exp (using Taylor series, sorry about that).

All the statistical functions, such as SampleAverage, MovingAverage etc.
are also (hopefully) going to work. Write once, run everything.

- Long integer arithmetic (even though it is now included in the standard 
.NET package) with arbitrary digit bases (this isn't).

- Matrix operations (also implemented using Numeric<T, C>).

- A not-so-terrible grapher library (only 2D graphs are currently supported).
Surprisingly, it really looks better than some of the graphers available
on-line. 

- A collection of pseudo-random number generators and respective interfaces.

- A cryptographic sub-library currently containing only the RSA algorithm.

- Primality tests

- And more (c).

- And more is coming.

I wish you good luck and hope this thing helps somehow.

