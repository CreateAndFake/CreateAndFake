# Create & Fake

[![NuGet](https://img.shields.io/badge/nuget-v1.1-blue.svg?style=flat)](https://www.nuget.org/packages/CreateAndFake) [![Appveyor Build status](https://ci.appveyor.com/api/projects/status/rmbj86u333y66hfw?svg=true)](https://ci.appveyor.com/project/Werebunny/createandfake) [![Travis Build Status](https://travis-ci.org/CreateAndFake/CreateAndFake.svg?branch=master)](https://travis-ci.org/CreateAndFake/CreateAndFake) [![CodeCov Coverage](https://codecov.io/gh/CreateAndFake/CreateAndFake/branch/master/graph/badge.svg)](https://codecov.io/gh/CreateAndFake/CreateAndFake) [![Codacy Grade](https://api.codacy.com/project/badge/Grade/cc753a1417c24f6dba43e2386e89005a)](https://www.codacy.com/app/Werebunny/CreateAndFake?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=CreateAndFake/CreateAndFake&amp;utm_campaign=Badge_Grade)

A C# class library for .NET Standard 2.0 and .NET Core 2.0 that handles mocking, test data generation, and validation. Designed to handle the bulk of test setup quickly and easily so that developers can focus on the behavior to test, making tests easier to develop and maintain. The handful of tools provided are both easy to use and customizable should the need arise:

* `Faker` - Creates mocks and stubs.
* `Randomizer` - Creates random instances of any type.
* `Randiffer` - Creates random variants of objects.
* `Duplicator` - Creates deep clones of objects.
* `Valuer` - Compares objects by value.
* `Asserter` - Handles common test scenarios.

A key benefit of the library is in how the tools are logically integrated with each other. For example, the `Randomizer` will use stubs for interfaces that have no known implementations in the code. Or the mocks created by the `Faker` utilize value equality in matching arguments.

Visit the [documentation site](https://createandfake.github.io/CreateAndFake/) for more information and how to get started.

## Installation

Use a NuGet package reference inside your project with the name "[CreateAndFake](https://www.nuget.org/packages/CreateAndFake)." Alternatively, binaries can be pulled from the build server linked above under artifacts or compiled manually.

## Documentation

The documentation site is located here: https://createandfake.github.io/CreateAndFake/

The raw files can be viewed from the doc folder or built into a local site using Jekyll.

## Contributing

If you're looking to contribute, thanks for your interest. Feel free to submit reports for any issues you can find, or request potential features you'd like to see [here](../../issues). If you wish to contribute code to the project, refer to the contributing guidelines [here](docs/CONTRIBUTING.md). Please follow the [code of conduct](docs/CODE_OF_CONDUCT.md) when contributing.

## License

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

View the [license](LICENSE.txt) file for more details.
