# Anti-Patterns

Each entry is **mistake → why → fix**.

## Outdated `Decoder.for*` API

- **Mistake:** Calling `LocalStorage.loadWith Decoder.forString` or `LocalStorage.loadWith Decoder.forData<'T>`.
- **Why:** The `Decoder` module was removed in v3.0.0 as irrelevant; these symbols no longer exist and will not compile. Older READMEs/snippets still show them.
- **Fix:** Pass a real `Thoth.Json` decoder — `Decode.string` for a string, `Decode.Auto.generateDecoder<'T>()` for an auto decoder, or a hand-built decoder for custom shapes. For the auto case, prefer `load<'T>` which builds the decoder for you.

## Dropping the `inline` keyword on wrappers

- **Mistake:** Writing a generic helper that forwards a type parameter to `load`/`save` but declaring it without `inline`.
- **Why:** Thoth's `Decode.Auto`/`Encode.Auto` resolve the concrete type at the call site; without `inline` the type information is lost and resolution fails.
- **Fix:** Mark any generic forwarding wrapper `inline`, matching the library's own `load`/`save`/`loadWith`/`saveWith` signatures.

## Ignoring the `Result`

- **Mistake:** Treating `load`/`loadWith` as if they return the value directly (or force-unwrapping the `Ok`).
- **Why:** Both return `Result<'T, string>`; the `Error` case occurs for a missing key or a decode mismatch and will be silently wrong or crash if ignored.
- **Fix:** Match on the `Result` and handle `Error` with a default, retry, or surfaced message.

## Confusing absence with a decode failure

- **Mistake:** Inferring "the key does not exist" from any `Error` returned by `load`.
- **Why:** The `Error` string is the same shape whether the key is missing or the stored JSON does not match the type, so you cannot reliably distinguish them from the `Result` alone.
- **Fix:** Call `loadItem` first when you must tell absence from malformed data — `None` means absent, `Some _` followed by a decode error means malformed.

## Mismatched encoder/decoder pairing

- **Mistake:** Writing with a custom `saveWith` encoder but reading with `load` (or vice versa).
- **Why:** A custom JSON shape will not match the auto decoder's expectations, producing decode errors.
- **Fix:** Keep the pair symmetric — auto-with-auto (`save`/`load`) or custom-with-matching-custom (`saveWith`/`loadWith`).

## Assuming non-`localStorage` backends

- **Mistake:** Expecting `sessionStorage`, cookies, or IndexedDB support from this module.
- **Why:** Only `localStorage` is implemented; there is no `SessionStorage` module despite what some overview docs imply.
- **Fix:** Use `LocalStorage` for `localStorage` only; reach for another library for other backends.

## Using the library outside the browser

- **Mistake:** Calling `LocalStorage` functions from server-side .NET or a non-browser host.
- **Why:** The functions call the browser `localStorage` binding through Fable; there is no such object off the browser.
- **Fix:** Restrict usage to Fable-compiled browser code; in tests, inject a `localStorage` shim.

## Unqualified module calls

- **Mistake:** Calling `save`/`load` without the `LocalStorage.` prefix after opening the namespace.
- **Why:** The module is `[<RequireQualifiedAccess>]`, so its members are not brought into scope unqualified.
- **Fix:** Always qualify: `LocalStorage.save`, `LocalStorage.load`, etc.
