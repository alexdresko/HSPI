Developer Workflow
==================

Always start with an issue
--------------------------

If you just want to help, find something in the current milestone_ or any open issue with the |helpwanted|_ tag. 

Otherwise, please don't start any new work without first checking to see if there's an existing issue describing what you want to do. If there is not an existing issue, please create an issue first, and wait for feedback from the core team. 


Creating issues
---------------------------------

Use this information to determine if creating an issue is the correct thing to do: 

* **Have a question about how HSPI works?** Ask on Stack Overflow and tag with “HSPI". This is for questions about how HSPI works, not for reporting bugs or feature requests. If you have an opinion, or want someone else's opinion, it's probably better to use the HSPI chat link. 
 http://stackoverflow.com/questions/ask?tags=HSPI 

    *Using Stack Overflow, as opposed to the HomeSeer forums, allow us to more easily track questions related to HSPI and their status.*
* **Github issues** This is not for asking questions, providing an opinion, or requesting an opinion. For questions about how HSPI works, use the Stack Overflow link. For opinion stuff, use the HSPI chat link. 
 https://github.com/alexdresko/HSPI/issues 

    *Using the Github issues, as opposed to the HomeSeer forums, allows us to more easily track issues and their status.*
* **HSPI chat**  This channel is great for giving and requesting feedback. Use Stack Overflow for questions about how HSPI works, and Github issues for bugs and feature requests. 
 https://gitter.im/HSPI/Lobby

When you *do* need to create an issue, it should be easy now. Just create an issueas your normally would in Github and you'll be prompted for any required information. 

Beginners and n00bs
-------------------

**Please** read the entire :doc:`how-to-contribute` section of this site. 

Also, we love n00bs. If you need help getting started, head on over to our chat_ and we'll try to help. 

If you've never contributed to a github project, you might find the following links helpful:

    How to Contribute to Open Source | Open Source Guides
    https://opensource.guide/how-to-contribute/

    Understanding the GitHub Flow · GitHub Guides
    https://guides.github.com/introduction/flow/

    Hello World · GitHub Guides
    https://guides.github.com/activities/hello-world/

    Forking Projects · GitHub Guides
    https://guides.github.com/activities/forking/

    Getting your project on GitHub · GitHub Guides
    https://guides.github.com/introduction/getting-your-project-on-github/

    Mastering Markdown · GitHub Guides
    https://guides.github.com/features/mastering-markdown/

Otherwise, here are the basic steps:

#. Fork the HSPI repo using the ``Fork`` button.
 .. image:: _static/fork-button.png
#. Clone the repo. You might find the ``Clone`` button useful.
 .. image:: _static/clone-button.png
#. Switch to the ``Dev`` branch. How you do this and the remaining steps will depend on your git stack. 
#. Create a new branch for your changes. 
#. Commit your changes.
#. Push your changes.
#. Create a pull request against the `Dev` branch. 

Note that it's entirely possible to perform all of the above steps without ever leaving Github, though we recommend only doing that for simple changes to HSPI. 



Git branching strategy
----------------------

We use a sloppy rendition of `Gitflow <http://datasift.github.io/gitflow/IntroducingGitFlow.html>`_. Basically, all new work should generally branch from `Dev`. When we feel like we've got enough work to consitute a release, we'll merge `Dev` into `Master`. The primary reason for using this workflow is to prevent frequent builds on the master branch. Every time we build on the master branch, the public nuget package and Visual Studio template are updated. It would be very annoying to HSPI users if they were constantly asked to upgrade HSPI. 

Creating pull requests
----------------------
When you're ready to create a pull request, it should be simple. Just create a pull request as your normally would in Github and you'll be prompted for any required information. 

ZenHub
------

We use ZenHub_ to enhance the Github issue management process. It's free for public Github repos, so grab the extension if you want to help maintain HSPI. 

Our ZenHub configuration uses the default pipeline:

#. **New Issues** This is where new issues go. 
#. **Icebox** This is for issues that need a little more time to simmer. 
#. **Backlog** This is for approved issues. Generally, I try to only work on issues that are in the Backlog _and_ assigned to the current milestone. Any issues in the Backlog with the "up for grabs" tag can be tackled by any community contributer at any time. 
#. **Blocked** This is for issues that can't be worked on for whatever reason. The reason should be detailed in the issue description. 
#. **In Progress** This is for issues that are in progress. 
#. **Review/Q** This is for issues associated to a PR that is being reviewed. 
#. **Done** This is for issues where the corresponding code has been merged into the Dev branch. 
#. **Closed** This is for issues where the corresponding code has been merged into Master. 

The following link might prove helpful in understanding how we use ZenHub:

ZenHub - Agile GitHub Project Management: https://www.zenhub.com/guides/agile-concepts-in-github


I also super highly recommend reading this book if you have the time -- It's not at all necessary -- It's just a really good book, and it further explains how ZenHub integrates with HSPI.

ZenHub - GitHub Project Management Book - Download Free: https://www.zenhub.com/book/github-project-management


Obivously, the HSPI project isn't Agile in the truest sense, but it's easier for me if we stick with the terminology as much as possible. 