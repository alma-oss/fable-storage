Fable.Storage
==================

> Fable library for a working with Browser storages.

---

## Install

Add following into `paket.dependencies`
```
git ssh://git@bitbucket.lmc.cz:7999/archi/nuget-server.git master Packages: /nuget/
# LMC Nuget dependencies:
nuget Lmc.Fable.Storage
```

Add following into `paket.references`
```
Lmc.Fable.Storage
```

## Usage

### Simple `string`
```fs
open Lmc.Fable.Storage

// Saving data to the storage
"data" |> LocalStorage.save "key"

// Loading data from the storage
let data = "key" |> LocalStorage.load<string>
let data = "key" |> LocalStorage.loadWith Decoder.forString
```

### Complex object
```fs
open Lmc.Fable.Storage

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
1. Increment version in `Fable.Storage.fsproj`
2. Update `CHANGELOG.md`
3. Commit new version and tag it
4. Run `$ fake build target release`
5. Go to `nuget-server` repo, run `faket build target copyAll` and push new versions

## Development
### Requirements
- [dotnet core](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial)
- [FAKE](https://fake.build/fake-gettingstarted.html)

### Build
```bash
fake build
```

### Watch
```bash
fake build target watch
```
