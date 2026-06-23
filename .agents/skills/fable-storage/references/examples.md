# Examples

All example code for this skill lives here. Examples are ordered by increasing complexity and each is self-contained.

## Basic — primitive round-trip

```fsharp
open Alma.Fable.Storage

// Save a string
"hello" |> LocalStorage.save "greeting"

// Load it back (Result<string, string>)
match "greeting" |> LocalStorage.load<string> with
| Ok value -> printfn "loaded: %s" value
| Error err -> printfn "failed: %s" err

// Remove it
LocalStorage.delete "greeting"
```

## Realistic — typed record and single-case union (auto coders)

```fsharp
open Alma.Fable.Storage

type Identifier = Identifier of string

type Account = {
    Id: Identifier
    Label: string
}

let account = { Id = Identifier "a-1"; Label = "Demo" }

// Save with the automatic encoder
account |> LocalStorage.save "account"

// Load with the automatic decoder
let loaded : Result<Account, string> =
    "account" |> LocalStorage.load<Account>
```

## Probing presence with loadItem

```fsharp
open Alma.Fable.Storage

// loadItem returns the raw string option, without decoding
match LocalStorage.loadItem "account" with
| Some _raw -> printfn "an entry exists"
| None -> printfn "nothing stored under this key"
```

## Custom Coders — explicit Thoth.Json decoder/encoder

```fsharp
open Alma.Fable.Storage
open Thoth.Json

type Settings = {
    Theme: string
    Volume: int
}

let private encode (s: Settings) =
    Encode.object [
        "theme", Encode.string s.Theme
        "volume", Encode.int s.Volume
    ]
    |> Encode.toString 0

let private decoder : Decoder<Settings> =
    Decode.object (fun get -> {
        Theme = get.Required.Field "theme" Decode.string
        Volume = get.Required.Field "volume" Decode.int
    })

// Save with the custom encoder
{ Theme = "dark"; Volume = 7 } |> LocalStorage.saveWith encode "settings"

// Load with the matching custom decoder
let settings : Result<Settings, string> =
    "settings" |> LocalStorage.loadWith decoder
```

## Round-trip Test

```fsharp
open Alma.Fable.Storage

type Item = { Name: string; Count: int }

let ``save then load returns the same value`` () =
    let original = { Name = "widget"; Count = 3 }
    original |> LocalStorage.save "item"

    match "item" |> LocalStorage.load<Item> with
    | Ok loaded -> loaded = original   // expected: true
    | Error _ -> false                 // expected: not reached
```

## Full Workflow — typed helper module

```fsharp
open Alma.Fable.Storage

type Profile = { DisplayName: string }

[<RequireQualifiedAccess>]
module ProfileStore =
    let private key = "profile"

    let save (profile: Profile) = profile |> LocalStorage.save key

    let tryLoad () : Profile option =
        match LocalStorage.load<Profile> key with
        | Ok profile -> Some profile
        | Error _ -> None

    let clear () = LocalStorage.delete key

// Usage
ProfileStore.save { DisplayName = "Demo User" }
let current = ProfileStore.tryLoad ()
ProfileStore.clear ()
```
