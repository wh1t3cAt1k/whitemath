# whiteMath + whiteStructs

whiteMath is an open-source and (hopefully) programmer-friendly C# .NET mathematical/algorithmic library.

Current version: 0.1.0

The source code of the libary is licensed under GNU GPL v.3.

This library should not by any means be treated as complete in terms of functionality and documentation. Hopefully, it will never be complete in terms of functionality; unfortunately, it is likely that the documentation isn't going to cover everything, either.

## -2\. WHAT?

This is a math/algorithms/data structures that was initially designed to assist me in my student projects at the Higher School of Economics, Moscow, Russia. What is it now? You decide.

It can already do something:

- Generic mathematical functions for arbitrary numeric types using the Numeric<T, C> wrapper. 

You just write a calculator (see ICalc<T>) for your brand-new MyComplex, MyInt or Foo numeric type (it can even be a String) that has basic operations and constants defined (such as zero, sum, subtraction, multiplication etc.) - poof, magic! these generic functions start working for you. They can even calculate Sin, Cos and Exp (by using Taylor series expansion).

All the statistical functions, such as SampleAverage, MovingAverage etc. are also (hopefully) going to work with arbitrary numeric types. Written once, runs everything.

- Long integer arithmetic (even though it is now included in the standard .NET package) with arbitrary digit numeric bases (which isn't).

- Matrix operations (also implemented using Numeric<T, C>).

- A not-so-terrible grapher library (only 2D graphs are currently supported). Surprisingly, it really looks better than some of the graphers available on-line. 

- A collection of pseudo-random number generators and respective interfaces.

- A cryptographic sub-library currently containing only the RSA algorithm.

- Primality tests

- And more, which I hope do describe someday.

## -1\. WHY?

Because I believe that the majority of shareable ideas ought to be shared.

I wanted to go open-source more than four years ago, but thought to write a thorough documentation before I would disclose all that stuff. This never happened, as you could expect.

So why now? Because today I stopped caring. Maybe we could do it together?

## 0\. TODO:

I understand that perhaps this isn't a very good readme for an open-source library. It will eventually get better, probably not without your help.

Also, the code isn't too consistent in structure and perhaps not very self-explanatory. Something can be learnt from the XML doc comments in the source files, and something should be discussed together in order to be worked on together.

## 1\. BAD DOCUMENTATION:

Currently, there are two types of hardly understandable and badly documented code in the library:

1. "How in the world this thing works?"
2. "What in the world this thing does?"

The second type should be urgently reported using appropriate media (e.g. the issues channel). Code of the first type can also be reported, but the author does not guarantee that he would remember and say anything intelligent in response. You see, this library began in the far 2008, when the originator hardly knew any C# (the artifacts of this prehistoric period can still be found scattered throughout the library).

Currently, the author tries to document everything thoroughly using XML and in-line comments. Sadly, part of these comments are still in Russian, so feel free to ask for an appropriate translation.

## 2\. BUGS: 

I suspect there are lots of them. Most of the functions were in real-life use from one to several times, and after that were happily abandoned. I have never bothered with thorough randomized testing either.

Please report.

I am also planning on introducing unit testing ASAP.

## 3\. COPYRIGHT NOTES:

- The idea of Numeric<T, C> appears to be stolen/rewritten from somewhere. My conscience eats me for that, but I just can't find the source.
- DoubleInfoExtraction.cs is also taken from somewhere, I will happily give credit if someone points out the author.
- There are numerous other bits of code which I didn't care about when I took them from the Web, but it is understood that all their authors deserve at least their name to be mentioned. 

So please drop me an email if you find anything.
You can also tell me what a bad boy I was, but I seem to already know that.

## 3.5\. TOOLS USED:

3.5.0 - Visual Studio 2012 Ultimate Edition / Xamarin Studio on Mac
3.5.1 - SandCastle for building HTML/CHM files from XML comments: http://shfb.codeplex.com/  
3.5.2 - .NET Code Contracts: http://visualstudiogallery.msdn.microsoft.com/1ec7db13-3363-46c9-851f-1ce455f66970  
3.5.3 - Brain