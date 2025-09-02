namespace Shared

[<AutoOpen>]
module Domain =
    [<Measure>]
    type cm // Non-SI unit: centimeter

    [<Measure>]
    type yr // Years

    type Natural<[<Measure>] 'u> = private Natural of int<'u>

    module Natural =
        let create x =
            if x > 0<_> then Some(Natural x) else None

        let value (Natural x) = x
