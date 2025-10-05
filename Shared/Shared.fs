namespace Shared

open System

[<AutoOpen>]
module Domain =
    [<Measure>]
    type cm // Non-SI unit: centimeter

    [<Measure>]
    type yr // Years

    // Take directly from Scott Wlaschin's GH gist on constrained types (minus restricting string's length to 50): https://gist.github.com/swlaschin/54cfff886669ccab895a#file-constrainedtypesexamples-fsx
    type ContentfulString private (str) =
        static member Create str =
            if String.IsNullOrWhiteSpace str then
                None
            else
                Some(ContentfulString str)

        member _.Value = str

    type ValidFreePassStartDate private (date) =
        static member Create date =
            // ValidFrom date must be in the past or present, since it represents an already issued FreePass
            // A FreePass is issued and is "active" since that moment. There is no concept of a FreePass being issued in the future
            if date <= DateTime.UtcNow then
                Some(ValidFreePassStartDate date)
            else
                None

        member _.Value = date

    type Natural<[<Measure>] 'u> = private Natural of int<'u>

    module Natural =
        let create x =
            if x > 0<_> then Some(Natural x) else None

        let value (Natural x) = x
