****************************************
* Getting Started With LINQ to Twitter *
****************************************

Purpose
-------
This document explains how to get LINQ to Twitter and provides information on the various versions of LINQ to Twitter and how to choose the right one.

What is LINQ to Twitter?
------------------------
LINQ to Twitter is a 3rd party LINQ Provider for the Twitter micro-blogging service. It uses standard LINQ syntax for queries and includes method calls for changes via the Twitter API. LINQ to Twitter is hosted on CodePlex.com at:

	http://linqtotwitter.codeplex.com/

How To Get LINQ to Twitter
--------------------------
There are a few ways to get the latest version of LINQ to Twitter:

1. Via NuGet, which is the easiest way to create a reference in Visual Studio

	http://nuget.org/packages/linqtotwitter

Through Visual Studio, you can type the following command in the NuGet Package Manager Console:
	
	PM> Install-Package linqtotwitter 
	
2. By visiting the LINQ to Twitter Releases page on the Downloads tab at

	http://linqtotwitter.codeplex.com/

3. By downloading the source code from the Source tab at

	http://linqtotwitter.codeplex.com/
	
Referencing LINQ to Twitter DLLs
--------------------------------
NuGet is the easiest method of referencing LINQ to Twitter because it automatically figures out which DLL you need. If you don't use NuGet, then you can read the following information to learn what version of LINQ to Twitter to use in your project. Each DLL is located in a folder within the downloadable binary, explained below:

*\net35\LinqToTwitter.dll

	Use with .NET v3.5 Full Profile. This includes ASP.NET WebForms, ASP.NET MVC, or any other type of regular .NET Framework.  Warning: When you create a new client application and reference this DLL, you'll receive an error because the default profile on Console and WPF applications is Client profile.  You can either change the profile to full profile or use the client profile dll, explained next.

*\net35-client\LinqToTwitterCP.dll

	Use only with projects set to .NET v3.5 Client Profile, which is the default for Console and WPF applications.

*\net40\LinqToTwitter.dll

	Use with .NET v4.0 Full Profile. This includes ASP.NET WebForms, ASP.NET MVC, or any other type of regular .NET Framework.  Warning: When you create a new client application and reference this DLL, you'll receive an error because the default profile on Console and WPF applications is Client profile.  You can either change the profile to full profile or use the client profile dll, explained next.

*\net40-client\LinqToTwitterCP.dll

	Use only with projects set to .NET v4.0 Client Profile, which is the default for Console and WPF applications.

*\net45\LinqToTwitter.dll

	Use with .NET v4.5 Full Profile. This includes ASP.NET WebForms, ASP.NET MVC, or any other type of regular .NET Framework.

*\net45-client\LinqToTwitterCP.dll

	Use only with projects set to .NET v4.5 Client Profile. Note: Unlike earlier versions of Visual Studio, VS 2012 does not set the default framework to Client Profile on client applications, such as Console and WPF.

*\sl4-windowsphone71\LinqToTwitterWP.dll

	This version is for Windows Phone v7.1. This version also works on Windows Phone 8, which is supported.

*\sl4\LinqToTwitterAg.dll

	This version supports Silverlight 4.0. Important: Twitter does not offer either a crossdomain.xml or clientaccesspolicy.xml file, which means you can't make requests to them directly.  Therefore, you must copy the HttpHandler proxy in *\LinqToTwitterProxy to the root of your Silverlight Web Application.

*\winrt45\LinqToTwitterRT.dll

	LINQ to Twitter supports Windows 8 and this is the DLL you'll need to use for Windows 8 Metro style applications.

Note: LINQ to Twitter works on Windows Azure too. Just select the .NET version you need.

How to Get More Help
--------------------
1. You can learn how to use LINQ to Twitter by reading the API reference on the Documentation page at

	http://linqtotwitter.codeplex.com/

	The documentation also includes a FAQ, which is a very important resource if you encounter HTTP 401 errors.

2. For help with LINQ to Twitter, you can visit the Discussion forum at

	http://linqtotwitter.codeplex.com/
		
3. For the latest information, follow @JoeMayo on Twitter.