# Contributing to ENMARCHA

You can contribute to ENMARCHA with ideas, suggestions, issues and pull requests (PRs).

Creating new code and suggest changes using pull requests (PRs) is a great, awesome and very welcome way to contribute.

Nevertheless, simply filing issues for problems you encounter is welcome as well.

## Reporting Issues

We always welcome bug reports, API proposals and overall feedback. Here are a few
tips on how you can make reporting your issue as effective as possible.

### Where to Report

New issues can be reported by email to ENMARCHA owners or in the Azure DevOps of the project.

Before filing a new issue, please search the list of issues to make sure it does
not already exist.

If you do find an existing issue for what you wanted to report, please include
your own feedback in the discussion.

### Writing a Good Bug Report

Good bug reports make it easier for maintainers to verify and root cause the underlying problem.

The better a bug report, the faster the problem will be resolved. Ideally, a bug report should contain the following information:

- A high-level and detailed description of the problem.
- A _minimal reproduction_, i.e. the smallest size of code or configuration required
  to reproduce the wrong behavior.
- A description of the _expected behavior_, contrasted with the _actual behavior_ observed.
- Information on the environment: operative system, CPU architecture, SDK version, etc.
- Additional information, e.g. Is it a regression from previous versions? Are there
  any known workarounds?

## Contributing Changes

Project maintainers will merge accepted code changes from contributors.

### DOs and DON'Ts

DO's:

 - **DO** follow the standard coding conventions from [.NET](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions).
 - **DO** give priority to the current style of the project or file you're changing if it diverges from the general guidelines.
 - **DO** include tests when adding new features. When fixing bugs, start with adding a test that highlights how the current behavior is broken. Changes without tests might not get their pull requests approved.
 - **DO** keep the discussions focused. When a new or related topic comes up it's often better to create new issue than to side track the discussion.
 - **DO** clearly state on an issue that you are going to take on implementing it.
 
DON'Ts:

 - **DON'T** surprise us with big pull requests. Instead, file an issue and start a discussion so we can agree on a direction before you invest a large amount of time.
 - **DON'T** commit code that you didn't write. If you find code that you think is a good fit to add to ENMARCHA, file an issue and start a discussion before proceeding.
 - **DON'T** submit PRs that alter licensing related files or headers. If you believe there's a problem with them, file an issue and we'll be happy to discuss it.
 - **DON'T** make new APIs without filing an issue and discussing with us first.

### Breaking Changes

Contributions must maintain API signature and behavioral compatibility.

Contributions that include breaking changes will be rejected unless they are properly argued. Please file an issue to discuss your idea or change if you believe that a breaking change is warranted. Breaking changes are innevitable, but it is always good to know and understand why.

### Development scripts

The scripts below are used to build, test, and lint within the project.

 - .NET:
   - PublishToLocal.ps1 → Publishes NuGet packages to a local package repository that you can reference in your projects to test implementations before a final pull request.
   - PublishToExternalDistribution.ps1 → Creates NuGet packages in a local package repository that are suitable to distribute freely to ENCAMINA's customers. These NuGet packages does not have [Source Link](https://github.com/dotnet/sourcelink) configured, and also does not generate [symbols packages](https://learn.microsoft.com/en-us/nuget/create-packages/symbol-packages-snupkg), thus preventing easy reverse engineering, disclosing implementation details or ENCAMINA's Intellecutal Property.

### Suggested Workflow

We recommend the following workflow:

1. Create an issue for your work.
   - You can skip this step for trivial changes.
   - Reuse an existing issue on the topic, if there is one.
   - Get agreement from the team and the community that your proposed change is a good one.
   - Clearly state that you are going to take on implementing it, if that's the case. You can request that the issue be assigned to you. Note: The issue filer and the implementer don't have to be the same person.
2. Create a personal fork of the repository (if you don't already have one).
    - ENMARCHA is provided as releases versions. We try to support as many versions as possible. When creating a branch, base it on the release where you are implementing the chante. For example: `release6.0.2` or `release6.0.3`.
    - If you want to create a branch in the repository of ENMARCHA, use your username as a prefix of the branch as a marker of ownership. For example `@username/release6.0.2/chage_description`.
    - In your fork, create a branch off the release branch.
    - Name the branch so that it clearly communicates your intentions, such as
     "issue-123" or "githubhandle-issue".
3. Make and commit your changes to your branch.
    - Provide rich descriptions on each commit message.
4. Add new tests corresponding to your changes.
5. Create a PR against the repository's release branch.
   - State in the description what issue or improvement your change is addressing.
   - Verify that all the Continuous Integration checks are passing.
6. Wait for feedback or approval of your changes from the code maintainers.
7. When area owners have signed off, and all checks are green, your PR will be merged.

### PR - CI Process

The continuous integration (CI) system will automatically perform the required builds and run tests (including the ones you are expected to provide and run) for PRs. Builds and test runs must be clean.

If the CI build fails for any reason, the PR issue will be updated with a link that can be used to determine the cause of the failure.

It is your responsability as Contributor to monitor and manage the created PR.

Abandoned branches or PRs will be removed without notice after a reasonable period of time.
