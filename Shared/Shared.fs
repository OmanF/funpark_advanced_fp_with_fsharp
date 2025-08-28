namespace Shared

[<AutoOpen>]
module Domain =
    [<Measure>]
    type cm // Non-SI unit: centimeter

    [<Measure>]
    type yr // Years

    [<Measure>]
    type rp // Reward points

    type Tags =
        | FamilyFriendly
        | Thrilling
        | Educational

    type TicketTier =
        | Standard
        | Premium
        | VIP

    type PositiveNonZeroInt<[<Measure>] 'u> = private PositiveNonZeroInt of int<'u>

    module PositiveNonZeroInt =
        let create x =
            if x > 0<_> then Some(PositiveNonZeroInt x) else None

        let value (PositiveNonZeroInt x) = x
