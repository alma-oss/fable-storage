[<RequireQualifiedAccess>]
module LocalStorage

open Thoth.Json

let loadWith (decoder: Decoder<'T>) key: Result<'T,string> =
    let o = Browser.WebStorage.localStorage.getItem key
    if isNull o then
        "No item found in local storage with key " + key |> Error
    else
        Decode.fromString decoder o

let load<'Data> key: Result<'Data, string> =
    key |> loadWith Decoder.forData<'Data>

let delete key =
    Browser.WebStorage.localStorage.removeItem(key)

let saveWith (encode: 'Data -> string) key (data: 'Data) =
    Browser.WebStorage.localStorage.setItem(key, encode data)

let save key (data: 'Data) =
    data |> saveWith (fun data -> Encode.Auto.toString(0, data)) key
