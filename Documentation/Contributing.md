# Contributing

Hi, and thanks for considering to help out. Don't be intimidated by the size of this project as there are multiple strategies in place to control complexity. If there is are any questions/don't know where to start/need help setting up, ask me `@Jonathan Burns` in slack. I'd be happy to help.

## Core Principles

I try to maintain a few principles to make developing/using Experience Capture easier.

- Everything as code
- Cross-platform
- Transparency

### Everything As Code

This means that not only is the application stuff (Server, Clients) in code, but as much as possible everything else too. This means the deploy, development process, documentation, etc are all done in some type of programming or configuration language. This means all changes can be tracked under version control, in this case git, so that its easy to capture the system at a point in time. [Here is more explanation](https://hackernoon.com/everything-as-code-explained-0ibg32a3).

### Cross-platform

The reason for this a given, and it is achieved two ways. First, games in Unity are naturally cross-platform just by building for a different target. Second, all of the non-game parts of Experience Capture are dockerized make it so only one requirement, docker is needed run anything on a different computer.

### Transparency 

 It is important to make sure that the data captured is valid, which means transparency of how this functions is a priority. This is implemented through everything as code as mentioned before and a discrete build system for the server. What that means is given a copy of the Server code, it should identically recreatable without anything irregular coming in to play.

## Open A Pull-Request

In order to actually contribute, open a [Pull-Request (PR)](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/about-pull-requests) to the master branch. After opening a PR, Continuous Integration (CI) will automatically be triggered. The CI is system to make sure our code both builds and confirms to style rules. Both of these factors make maintaining, adding features, and finding bugs easier. Our CI uses [GitHub Actions](https://github.com/features/actions). Here is the full process:

1. A PR opens.
1. GitHub Actions checks which code paths are changed and only runs various [workflows](https://help.github.com/en/actions/automating-your-workflow-with-github-actions/configuring-a-workflow) based on that.
1. Each workflow run in a container independently, either succeeding or failing.
1. If all workflows pass, then the PR can be merged. If not, then more commits can be added to the PR and the workflows are reran until it passes.

[Here is more information of the benefit of CI.](https://martinfowler.com/articles/continuousIntegration.html)