ClipCode (0.0.1) Documentation
==============================

by Mark Nelson

This is a Visual Studio add-in that adds a Ctrl+Alt+C command to your environment which will copy the selected code as raw html formatting into the clipboard.
There are many add-ins out there that already do this very thing - but this one is mine. It is small, quick, dirty, has NO OPTIONS (just hit Ctrl+Alt+C - no additional windows pop up or anything), requires no additional CSS to be added (will be given an inline style), no muss, no fuss, just paste crap into your blog and forget about it.

How To Install:
---------------
* Copy `ClipCode.dll` along with the `ClipCode.AddIn` file and place it in your Visual Studio addins folder at `%documents%/Visual Studio 2012/Add-Ins` (you may need to create this folder)
* Poof! Next time you start Visual Studio will have a "Copy to HTML" feature in your "Edit" menu.

How To Use:
-----------
* Open the document containing the code you would like to post to your blog.
* Select the code you would like to post to your blog.
* Copy the code you would like to post to your blog using Ctrl+Alt+C.
* Past the code you would like to post to your blog IN YOUR BLOG HTML EDITOR!
* BAM! The code you would like to post to your blog IS IN YOUR BLOG!!!! OMG!!!!

How It Works:
-------------
This is painfully simple and retarded, but...
* It calls the normal "Copy" (Ctrl+C)
* It reads the RTF that is placed in the clipboard and turns it into raw HTML
* It puts the HTML back into the clipboard
* It waits...