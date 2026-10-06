---
name: fable-storage
description: Use whenever generating or reviewing F# (Fable) code that reads from or writes to browser LocalStorage — calls to LocalStorage.save, LocalStorage.saveWith, LocalStorage.load, LocalStorage.loadWith, LocalStorage.loadItem, or LocalStorage.delete. Trigger on mentions of Alma.Fable.Storage, browser storage persistence in Fable apps, Thoth.Json (Decode/Encode) serialization of stored values, Result-based load handling, or custom Thoth decoders/encoders for persisted data.
---

# Fable-Storage

Library: [alma-oss/fable-storage](https://github.com/alma-oss/fable-storage)
NuGet: `Alma.Fable.Storage`

## Purpose

A small Fable (F#→JavaScript) library that provides typed save/load/delete operations over the browser `localStorage` API. Values are serialized to and from JSON via Thoth.Json, with loads returning a `Result` so deserialization and missing-key failures are explicit.

## When to Use

- Persisting typed F# values (records, unions, primitives) to browser `localStorage` in a Fable application.
- Reading typed values back with automatic or custom Thoth.Json decoders.
- Removing or probing raw entries by key.

## When NOT to Use

- Server-side or any non-browser .NET code — this is Fable/browser-only.
- `sessionStorage`, `IndexedDB`, cookies, or any non-`localStorage` mechanism — not covered.
- Cross-tab reactive state or storage-event subscriptions — out of scope.

## Main Concepts

- `LocalStorage` — `[<RequireQualifiedAccess>]` module; the entire public surface.
- `loadItem` — fetches the raw stored string for a key as a `string option` (no decoding).
- `loadWith` — decodes a stored value using an explicit `Thoth.Json.Decoder<'T>`, returning `Result<'T, string>`.
- `load<'T>` — like `loadWith` but builds the decoder automatically via `Decode.Auto`; returns `Result<'T, string>`.
- `saveWith` — serializes a value with a caller-supplied `'T -> string` encoder and stores it.
- `save` — like `saveWith` but serializes automatically via `Encode.Auto`.
- `delete` — removes the entry for a key.
- `inline` — the generic functions are `inline` so Thoth's auto coders can resolve types at the call site.

## Related Libraries

- `Thoth.Json` — supplies the `Decode`/`Encode` primitives and `Decode.Auto`/`Encode.Auto` used for serialization.
- `Fable.Browser.WebStorage` — provides the underlying `localStorage` binding.

## Keywords for Search

Alma.Fable.Storage, fable-storage, LocalStorage, localStorage, browser storage, save, saveWith, load, loadWith, loadItem, delete, Thoth.Json, Decode.Auto, Encode.Auto, Decoder, Result, Fable, F#, JSON serialization, persistence

## Reference Files

- For composition principles, recommended API usage, error handling, integration with Thoth.Json, and testing guidance, read `references/preferred-patterns.md`.
- For known pitfalls, the outdated `Decoder.for*` API, and incorrect assumptions, read `references/anti-patterns.md`.
- For worked, self-contained code examples ordered by complexity, read `references/examples.md`.
