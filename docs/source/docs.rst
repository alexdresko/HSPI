Documentation
#############

Summary
-------

Please help write HSPI's documentation! This project started with the vision of combining and simplifying a lot of work that was already out there. None of that existing work had documentation, so we started with nothing. Any time HSPI confuses you is a good opportunity for documentation. If you can't contribute to HSPI's documentation, at least voice your confusion in our `chat room`_ so someone else can clarify things. 

This document explains how to set up your environment to make changes to HSPI's documentation. If you're a new contributer to HSPI, be sure to read everything in :doc:`how-to-contribute`.

HSPI's documentation is hosted on https://readthedocs.org/ (RTD). The documentation is written using Sphinx_, so you'll need Python_.  You'll also want gulp, and therefore Node if you want to take advantage of some of the utilities I wrote to make writing documentation for HSPI easier. 

If you're starting with a completely new development environment, here are the steps I'd go through to get things set up. If you already have some of these tools, or prefer to do things differently, you're on your own. :) 

Installing the software
-----------------------

(On some installations of Windows, you may need to replace ``pip`` with ``py -m pip`` in the instructions below)

#. Install Chocolatey_.  At any point in the following steps, you may need to restart your console for the newly installed tool to become available in your console session. 
#. Install git via ``choco install git -y``
#. Install python via ``choco install python -y``
#. Install VS Code via ``choco install visualstudiocode -y``. I prefer to write the documentation in VS Code simply because I like VS Code. 
#. Install Nodejs via ``choco install nodejs -y``
#. Install Sphinx via ``pip install sphinx``
#. Install Sphinx autobuild via ``pip install sphinx-autobuild``
#. Install gulp via ``npm install -g gulp-cli``
#. ``cd`` to the directory where you cloned HSPI from git
#. Install the npm dependencies via ``npm install``
#. Install the RTD Sphinx template via ``pip install sphinx_rtd_theme``
#. Fire up the live preview by running ``gulp watch`` from the root of the repo. This will launch a web browser on http://localhost:3000/. Any changes you make to any of the rst files in HSPI will usually automatically refresh the browser, allowing you to see the changes you made as they'll appear on http://hspi.readthedocs.io. You might sometimes have to run ``gulp build`` to perform a full build of all RST files if it seems like the files aren't updating properly. Also, keep an eye on this console window between saves to make sure you don't have any errors in your RST syntax.  

That's it. I just tested those instructions on my wife's laptop, in the car while she's in Walmart, tethered to my phone, while writing this very page. Clearly it works. 

To learn more about Sphinx and restructuredText, go to http://www.sphinx-doc.org/en/stable/contents.html.  It takes a while to get used to rst, but it's much more powerful than Markdown. 

To learn more about ReadTheDocs, go to http://docs.readthedocs.io/en/latest/

Side by Side
------------

HSPI documentation is stored alongside the HSPI source code. That's great because it allows the documentation and code to stay up to date more easily. If you make a change to code, it only takes a few moments to update the documentation.

Another advantage here is that you can directly include source code from HSPI into the documentation. Here's ``HSPI\Option.cs`` for example. 

.. literalinclude:: ..\..\HSPI\Options.cs
    :language: C#
    :caption: HSPI\\Option.cs
    :encoding: UTF-8
    
That's neat. 

Check out the code for this page in the repository to see how that's done. And then go here for more information: http://www.sphinx-doc.org/en/stable/markup/code.html

Note that Sphinx has trouble applying syntax highlighting to some C# files. 

Common Substitutions
--------------------
The ``rst_epilog`` variable in ``conf.py`` contains a list of common subsitutions. At the time of writing, it looks like the list below, but it's expected to grow quickly.

.. code-block:: python

    rst_epilog = """
    .. _Python: https://www.python.org/
    .. _Sphinx: http://sphinx-doc.org/latest/install.html
    .. _Chocolatey: https://chocolatey.org/install
    .. _chat room: https://gitter.im/HSPI/Lobby
    .. |ad| replace:: Alex Dresko
    .. _ad: http://www.alexdresko.com
    .. _`Contributor Covenant`: http://contributor-covenant.org
    """