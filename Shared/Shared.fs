namespace Shared

[<AutoOpen>]
module Domain =
    [<Measure>]
    type cm

    [<Measure>]
    type yr

    type PositiveNonZeroInt<[<Measure>] 'u> = private PositiveNonZeroInt of int<'u>

    module PositiveNonZeroInt =
        let create x =
            if x > 0<_> then Some(PositiveNonZeroInt x) else None

        let value (PositiveNonZeroInt x) = x

    type Tags =
        | FamilyFriendly
        | Thrilling
        | Educational
