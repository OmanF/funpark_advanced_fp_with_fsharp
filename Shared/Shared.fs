namespace Shared

[<AutoOpen>]
module Domain =
    [<Measure>]
    type cm // Non-SI unit: centimeter

    [<Measure>]
    type yr // Years

    type PositiveNonZeroInt<[<Measure>] 'u> = private PositiveNonZeroInt of int<'u>

    module PositiveNonZeroInt =
        let create x =
            if x > 0<_> then Some(PositiveNonZeroInt x) else None

        let value (PositiveNonZeroInt x) = x
