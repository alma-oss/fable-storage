namespace Lmc.Fable.Storage

[<RequireQualifiedAccess>]
module Decoder =

    open Thoth.Json

    let forData<'Data> = Decode.Auto.generateDecoder<'Data>()

    let forString: Decoder<string> = forData<string>
    let forInt: Decoder<int> = forData<int>
    let forBool: Decoder<bool> = forData<bool>
