Fable.Storage
==================

> Fable library for a working with Browser storages.

---

## Install

Add following into `paket.dependencies`
```
source https://nuget.pkg.github.com/almacareer/index.json username: "%PRIVATE_FEED_USER%" password: "%PRIVATE_FEED_PASS%"
# LMC Nuget dependencies:
nuget Alma.Fable.Storage
```

NOTE: For local development, you have to create ENV variables with your github personal access token.
```sh
export PRIVATE_FEED_USER='{GITHUB USERNANME}'
export PRIVATE_FEED_PASS='{TOKEN}'	# with permissions: read:packages
```

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
