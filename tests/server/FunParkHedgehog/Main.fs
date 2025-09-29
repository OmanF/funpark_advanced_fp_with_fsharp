open Hedgehog
open FunParkHedgehog.Tests

[<EntryPoint>]
let main _ =
    let testsNames =
        [ "MinAgeNatural"
          "MinHeightNatural"
          "MinWaitTimeNatural"
          "NoDuplicateTags"
          "OfflineWaitTimeZero" ]

    let testsFunctions =
        [ propMinAgeNatural
          propMinHeightNatural
          propMinWaitTimeNatural
          propNoDuplicateTags
          propOfflineWaitTimeZero ]

    let testsZipped = List.zip testsNames testsFunctions

    testsZipped
    |> List.iter (fun (name, prop) ->
        printfn $"Testing %s{name} (Unit result means no failures): %A{Property.check prop}")

    0
