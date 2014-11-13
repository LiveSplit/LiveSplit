Redistribution
--------------
Noesis.Javascript.dll needs the Microsoft C Runtime Libraries.
The exact version required is specified in a manifest file automatically
included inside the DLL.  You can extract it using 
MT.exe (from the Windows SDK):

> mt -inputresource:Noesis.Javascript.dll;2 -out:t.manifest
> type t.manifest

If you don't include the correct version of the runtime libraries
when you redistribute Noesis.Javascript.dll then you will get errors
when loading the DLL on some users machines.  (Many, but not all users
will already have it.)

Visual Stdio 2010 is more flexible about where it finds its DLLs 
(http://mariusbancila.ro/blog/2010/03/24/visual-studio-2010-changes-for-vc-part-5/)
so you need not worry about the manifest, but you should still redistribute the
runtime library because the user may not have it.


Building from Source
--------------------
We no longer release a source ZIP file because you need to use Subversion in 
order to get the v8 source code, so you may as well use it for the 
javascriptdotnet source too.

1. You need to have Python (32-bit CPython 2.4+, but not 3.x) installed.
   http://python.org/ftp/python/2.7.2/python-2.7.2.msi
   Problems have been reported with the 64-bit Python distributions.
   
2. You need to have the Win32 extensions for Python (pywin32) installed,
   matching your Python version and architecture.
   http://downloads.sourceforge.net/project/pywin32/pywin32/Build%20217/pywin32-217.win32-py2.7.exe
   
3. You need to have scons installed.  I had to run the installation program
   as an administrator to get it to install without errors.
   http://prdownloads.sourceforge.net/scons/scons-2.1.0.win32.exe
   
4. Check out the javascriptdotnet source:
   C:\> svn checkout https://javascriptdotnet.svn.codeplex.com/svn javascriptdotnet
   
5. Check out a recent v8 tag (the tagged releases are more stable than the 
   trunk) and move the v8 directory under your javascriptdotnet checkout.
   C:\> svn checkout http://v8.googlecode.com/svn/tags/3.11.6.2/ v8
   C:\> move v8 javascriptdotnet
   
6. Run build.bat to build v8 for your preferred architecture and build
   environment.
   C:\javascriptdotnet> buildv8 ia32 vs2010
   
7. Load the Visual Studio Solution file corresponding to your version of
   Visual Studio.
