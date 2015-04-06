This is a collection of projects that all build to generate an exe whose primary usage is imagined
to be in conjunction with a keyboard shortcut program.  I'm using Clavier+.  Combined, a keyboard
shortcut tool and a cleverly designed exe can make for rapid execution of repetitive tedious tasks
along with a familiar C# syntax which gives the advantage of easy task modification and
implementations.  Using this project, one can mimic the already existing projects and reuse/share
logic and MSBuild steps.  All this servers the purpose of allowing the rapid generation of a new
exe coded for a specific task that only you (the lonesome inventive developer) may see the need for.

EXAMPLE SCENARIO:
I google something 100+ times a day.  Therefore, I have set up my (winkey) + ` keyboard shortcut
to open "cmd".  I have a collection of .cmd commandlets files in an
"added-to-%PATH%-environment-variable" folder.  So in order for me to google anything that comes
to my mind, I type the below:

1. (winkey) + `
    This opens up a cmd prompt via Clavier+ in my case.

2. g whatever I want to search 
    "g" is the name of the commandlet file that calls GoogleIt.exe.  The words after it are the
    parameters that get passed to the exe.  The below text is the content of my "g.cmd" file.

        start "" "C:\source\Git\mikesocha3\ExeLibrary\GoogleIt\bin\Debug\GoogleIt.exe" %1 %2 %3 %4 %5 %6 %7 %8 %9
        exit

Voila! This is how I can rapidly google a very specific topic with out moving my arm to the mouse!