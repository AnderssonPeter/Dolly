<p align="center">
  <a href="https://github.com/AnderssonPeter/Dolly">
    <img src="icon_white.svg" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">Dolly</h3>

  <p align="center">
    Clone .net objects using source generation
    <br />
    <br />
    ·
    <a href="https://github.com/AnderssonPeter/Dolly/issues">Report Bug</a>
    ·
    <a href="https://github.com/AnderssonPeter/Dolly/issues">Request Feature</a>
    ·
  </p>
</p>
<br />

[![NuGet version](https://badge.fury.io/nu/Dolly.svg)](https://www.nuget.org/packages/Dolly)
[![Nuget](https://img.shields.io/nuget/dt/Dolly)](https://www.nuget.org/packages/Dolly)
[![GitHub license](https://img.shields.io/badge/license-Apache%202-blue.svg)](https://raw.githubusercontent.com/AnderssonPeter/Dolly/main/LICENSE)

[![unit tests](https://img.shields.io/github/actions/workflow/status/AnderssonPeter/Dolly/test.yml?branch=main&style=flat-square&label=unit%20tests)](https://github.com/AnderssonPeter/Dolly/actions/workflows/test.yml)
[![Testspace tests](https://img.shields.io/testspace/tests/AnderssonPeter/AnderssonPeter:Dolly/main?style=flat-square)](https://anderssonpeter.testspace.com/spaces/293120/result_sets)
[![Coverage Status](https://img.shields.io/coveralls/github/AnderssonPeter/Dolly?style=flat-square)](https://coveralls.io/github/AnderssonPeter/Dolly)

## Table of Contents
* [About the Project](#about-the-project)
* [Getting Started](#getting-started)
* [Example](#example)
* [Benchmarks](#Benchmarks)

## About The Project
Generate c# code to clone objects on the fly.

## Getting Started
* Add the `Dolly` nuget and add `[Clonable]` attribute to a class and ensure that the class is marked as `partial`.
* Add `[CloneIgnore]` to any property or field that you don't want to include in the clone.
* Call `DeepClone()` or `ShallowClone()` on the object.

### Example
```C#
[Clonable]
public partial class SimpleClass
{
    public string First { get; set; }
    public int Second { get; set; }
    [CloneIgnore]
    public float DontClone { get; set; }
}
```
Should generate
```C#
partial class SimpleClass : IClonable<SimpleClass>
{
    
    object ICloneable.Clone() => this.DeepClone();

    public SimpleClass DeepClone() =>
        new SimpleClass()
        {
            First = First,
            Second = Second
        };

    public SimpleClass ShallowClone() =>
        new SimpleClass()
        {
            First = First,
            Second = Second
        };
}
```

## Benchmarks

| Method          | Mean         | Error      | StdDev       | Median       | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------------- |-------------:|-----------:|-------------:|-------------:|-------:|--------:|-------:|----------:|------------:|
| FastCloner      |     64.48 ns |   1.323 ns |     3.145 ns |     64.11 ns |   0.46 |    0.03 | 0.0094 |     472 B |        0.78 |
| Dolly           |    140.85 ns |   2.835 ns |     7.165 ns |    139.39 ns |   1.00 |    0.07 | 0.0119 |     608 B |        1.00 |
| DeepCloner      |    501.48 ns |  13.753 ns |    37.879 ns |    490.50 ns |   3.57 |    0.32 | 0.0277 |    1392 B |        2.29 |
| CloneExtensions |    694.84 ns |  11.787 ns |    11.576 ns |    694.27 ns |   4.94 |    0.25 | 0.0296 |    1504 B |        2.47 |
| NClone          |  4,970.65 ns |  91.412 ns |   245.572 ns |  4,948.11 ns |  35.37 |    2.41 | 0.1678 |    8584 B |       14.12 |
| FastDeepCloner  | 17,968.60 ns | 356.017 ns |   462.923 ns | 17,988.80 ns | 127.88 |    6.86 | 0.1221 |    6944 B |       11.42 |
| AnyClone        | 22,256.06 ns | 443.712 ns | 1,001.532 ns | 22,134.10 ns | 158.39 |   10.31 | 0.7935 |   41256 B |       67.86 |
