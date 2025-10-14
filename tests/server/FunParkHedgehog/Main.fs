open Hedgehog
open FunParkHedgehog.Tests

[<EntryPoint>]
let main _ =
    let testsNames =
        [ "Ride - Minimal age is a Natural number"
          "Ride - Minimal height is a Natural number"
          "Ride - Minimal wait time is a Natural number"
          "Ride - Offline wait time is zero for offline rides"
          "FreePass - FreePass initial validation data is current to issuing" ]

    let testsFunctions =
        [ propMinAgeNatural
          propMinHeightNatural
          propMinWaitTimeNatural
          propOfflineWaitTimeZero
          propFreePassNotInFuture ]

    let testsZipped = List.zip testsNames testsFunctions

    printfn "Starting property-based testing. Unit, (), results means no failures!"

    testsZipped
    |> List.iter (fun (name, prop) -> printfn $"Testing %s{name}: %A{Property.check prop}")

    0
