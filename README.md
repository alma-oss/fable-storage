Fable.Storage
==================

[![NuGet](https://img.shields.io/nuget/v/Alma.Fable.Storage.svg)](https://www.nuget.org/packages/Alma.Fable.Storage)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Alma.Fable.Storage.svg)](https://www.nuget.org/packages/Alma.Fable.Storage)
[![Tests](https://github.com/alma-oss/fable-storage/actions/workflows/tests.yaml/badge.svg)](https://github.com/alma-oss/fable-storage/actions/workflows/tests.yaml)

> Fable library for a working with Browser storages.

---

## Install

Add following into `paket.references`
```
Alma.Fable.Storage
```

## Usage

### Simple `string`
```fs
open Alma.Fable.Storage

// Saving data to the storage
"data" |> LocalStorage.save "key"

// Loading data from the storage
let data = "key" |> LocalStorage.load<string>
let data = "key" |> LocalStorage.loadWith Decoder.forString
```

### Complex object
```fs
open Alma.Fable.Storage

type Username = Username of string

type User = {
    Username: Username
    Name: string
}

// Saving data to the storage
{
    Username = Username "admin"
    Name = "admin"
}
|> LocalStorage.save "user"

// Loading data from the storage
let user = "user" |> LocalStorage.loadWith Decoder.forData<User>
let user = "user" |> LocalStorage.load<User>
```

## Release
1. Increment version in `Alma.Fable.Storage.fsproj`
2. Update `CHANGELOG.md`
3. Commit new version and tag it

## Development
### Requirements
- [dotnet core](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial)

### Build
```bash
./build.sh build
```

### Tests
```bash
./build.sh -t tests
```
