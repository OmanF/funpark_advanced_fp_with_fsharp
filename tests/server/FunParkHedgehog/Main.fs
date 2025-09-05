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

    printfn "Testing: All Ride's Tags have no duplicates"
    let noDuplicateTagsResult = Property.check propNoDuplicateTags
    printfn "%A" noDuplicateTagsResult

    printfn "Testing: All offline Ride's WaitTime is zero"
    let offlineWaitTimeZeroResult = Property.check propOfflineWaitTimeZero
    printfn "%A" offlineWaitTimeZeroResult

    0
