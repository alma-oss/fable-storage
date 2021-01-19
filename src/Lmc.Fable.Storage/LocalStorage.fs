namespace Lmc.Fable.Storage

[<RequireQualifiedAccess>]
module LocalStorage =
    open Thoth.Json

    let inline loadWith (decoder: Decoder<'Data>) key: Result<'Data, string> =
        let o = Browser.WebStorage.localStorage.getItem key
        if isNull o then
            "No item found in local storage with key " + key |> Error
        else
            Decode.fromString decoder o

    let inline load<'Data> key: Result<'Data, string> =
        key |> loadWith (Decode.Auto.generateDecoder<'Data>())

    let delete key =
        Browser.WebStorage.localStorage.removeItem(key)

    let inline saveWith (encode: 'Data -> string) key (data: 'Data) =
        Browser.WebStorage.localStorage.setItem(key, encode data)

    let inline save key (data: 'Data) =
        data |> saveWith (fun data -> Encode.Auto.toString(0, data)) key
