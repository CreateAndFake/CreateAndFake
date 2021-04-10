# Contributing

Thank you for your interest in contributing.

Please first discuss the change you wish to make via issue/request, email, or any other method with the owners of this repository before making changes. Failure to do so will likely lead to a declined pull request regardless of quality.

Make sure to refer to and follow the [code of conduct](CODE_OF_CONDUCT.md).

## Quality Requirements

* No external dependencies.
* 100% test coverage.
* Proper comments.
* Updated documentation.
* Follow the project code style.
* No #region.

Pull requests will be reviewed thoroughly to maintain project quality.

## Windows Dev Environment Recommended Setup

1) Install [VS Community](https://visualstudio.microsoft.com/vs/community/).
2) Clone repo and run from cmd prompt "build/run.cmd test".

## Linux Dev Environment Recommended Setup

1) Install packages for:
    A) [.NET frameworks](https://dotnet.microsoft.com/download/)
    B) [Mono](https://www.mono-project.com/download/stable/#download-lin/)
    C) [VS Code](https://code.visualstudio.com/download/)
2) Install VS Code extensions:
    A) C#
    B) EditorConfig for VS Code
    C) Powershell
    D) Code Spell Checker
3) Clone repo and run from terminal "./build/run.sh test".

## Developer Certificate of Origin (DCO)

To allow contributions to be used by the project, developers must declare that they can and do give rights to use that code. This is done by signing off on commits by appending a message at the end of the commit for all owners in the format:

```
Signed-off-by: FirstName LastName <email@mail.com>
```

This can be accomplished manually or by using -s or --signoff when committing. Github can verify this signature with your GPG key, which you can learn about [here](https://help.github.com/articles/signing-commits-with-gpg/). By signing off your commits to this repository, you're confirming that you've read DCO and agree with it, which can be found at https://developercertificate.org/ or read below:


```
Developer's Certificate of Origin 1.1

By making a contribution to this project, I certify that:

(a) The contribution was created in whole or in part by me and I
    have the right to submit it under the open source license
    indicated in the file; or

(b) The contribution is based upon previous work that, to the best
    of my knowledge, is covered under an appropriate open source
    license and I have the right under that license to submit that
    work with modifications, whether created in whole or in part
    by me, under the same open source license (unless I am
    permitted to submit under a different license), as indicated
    in the file; or

(c) The contribution was provided directly to me by some other
    person who certified (a), (b) or (c) and I have not modified
    it.

(d) I understand and agree that this project and the contribution
    are public and that a record of the contribution (including all
    personal information I submit with it, including my sign-off) is
    maintained indefinitely and may be redistributed consistent with
    this project or the open source license(s) involved.
```
