open Hedgehog
open FunParkHedgehog.Tests

[<EntryPoint>]
let main _ =
    printfn "Testing: All Ride's MinAge is positive"
    let minAgeResult = Property.check propMinAgeNatural
    printfn "%A" minAgeResult

    printfn "Testing: All Ride's MinHeight is positive"
    let minHeightResult = Property.check propMinHeightNatural
    printfn "%A" minHeightResult

    printfn "Testing: All Ride's MinWaitTime is positive"
    let minWaitTimeResult = Property.check propMinWaitTimeNatural
    printfn "%A" minWaitTimeResult

    0
