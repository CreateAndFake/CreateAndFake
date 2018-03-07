# Overview

Welcome to the documentation source for developers.

## Functionality

The library provides a handful of tools that are both easy to use and customizable should the need arise:

* Randomizer - Creates random instances of any type.
* Randiffer - Creates random variants of objects.
* Duplicator - Creates deep clones of objects.
* Valuer - Compares objects by value.
* Faker - Creates mocks and stubs.
* Asserter - Handles common test scenarios.

One of the benefits of this library is that the tools are logically integrated with each other. For example, the randomizer will use stubs for interfaces that have no known implementations in the code. Or the mocks created by the faker utilize value equality in matching arguments.
