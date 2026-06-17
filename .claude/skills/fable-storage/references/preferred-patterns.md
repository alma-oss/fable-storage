# Preferred Patterns

## Core Principles

- Treat every load as fallible. `load` and `loadWith` return `Result<'T, string>`; the `Error` case covers both a missing key and a decode failure, so always branch on it.
- Pair each `save`/`load` (auto) or `saveWith`/`loadWith` (manual) symmetrically: a value written with `Encode.Auto` must be read with `Decode.Auto`/`load`, and a value written with a custom encoder must be read with the matching custom decoder.
- Keep keys as stable string constants in one place rather than scattering literals across call sites.
- Open the module namespace and call through the `LocalStorage.` qualifier; the module is `[<RequireQualifiedAccess>]`, so unqualified calls will not resolve.

## Recommended API Usage

- Use `load<'T>` / `save` for ordinary records, unions, and primitives where Thoth's automatic coders are sufficient — the lowest-ceremony path. See `examples.md` → Basic and Realistic.
- Use `loadWith` / `saveWith` when you need control over the JSON shape, custom field naming, versioning, or types the auto coders cannot handle. See `examples.md` → Custom Coders.
- Use `loadItem` only when you genuinely want the raw stored `string` (or its presence) without decoding — for example to check existence. It returns `string option`, not a `Result`.
- Use `delete` to remove an entry; it is a no-op when the key is absent.

## Error Handling

- Destructure the `Result` immediately and supply a fallback for the `Error` branch (default value, re-fetch, or surfacing the error string). Never assume the `Ok` case.
- Because the `Error` string conflates "missing" and "malformed", use `loadItem` first when you specifically need to distinguish absence from a decode failure.

## Composition

- Build small helpers that close over a typed key and delegate to `load`/`save`, so call sites stay free of stringly-typed keys and type annotations.
- For optional persisted state, map the `Result` to an `option` at the boundary (`Result.toOption` or a match) rather than propagating the error string deeper than necessary.

## Integration with Other Libraries

- Custom coders are plain `Thoth.Json` values: construct decoders with `Decode.object` / `Decode.field` / `Decode.string` etc., and encoders with `Encode.object` / `Encode.string` etc. Pass the decoder to `loadWith` and an encoding function (`encoder >> Encode.toString 0`) to `saveWith`.
- The generic functions are `inline` because Thoth's `Decode.Auto`/`Encode.Auto` resolve the target type at the call site; keep your own wrapper functions `inline` too when they forward a generic type parameter to `load`/`save`.

## Naming Conventions

- Mirror the library's own naming: the `*With` suffix denotes the explicit-coder variant (`loadWith`, `saveWith`); the bare name denotes the auto-coder variant (`load`, `save`).
- Name key constants after the data they hold, not after the storage mechanism.

## Testing Recommendations

- Round-trip test: `save` (or `saveWith`) a value, then `load` (or `loadWith`) it back and assert equality on the `Ok` value. See `examples.md` → Round-trip Test.
- Cover the `Error` path explicitly by loading an absent key and by loading a key whose stored content does not match the target type.
- In a non-browser test runner, provide a `localStorage` shim, since the library calls the real browser binding.
