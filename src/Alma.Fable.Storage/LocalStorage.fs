namespace Alma.Fable.Storage

[<RequireQualifiedAccess>]
module LocalStorage =
    open Thoth.Json

    let loadItem key =
        let item = Browser.WebStorage.localStorage.getItem key
        if isNull item then None
        else Some item

    let inline loadWith (decoder: Decoder<'Data>) key: Result<'Data, string> =
        match key |> loadItem with
        | Some item -> Decode.fromString decoder item
        | _ -> "No item found in local storage with key " + key |> Error

    let inline load<'Data> key: Result<'Data, string> =
        key |> loadWith (Decode.Auto.generateDecoder<'Data>())

    let delete key =
        Browser.WebStorage.localStorage.removeItem(key)

    let inline saveWith (encode: 'Data -> string) key (data: 'Data) =
        Browser.WebStorage.localStorage.setItem(key, encode data)

    let inline save key (data: 'Data) =
        data |> saveWith (fun data -> Encode.Auto.toString(0, data)) key
