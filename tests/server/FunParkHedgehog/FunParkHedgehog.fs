module FunParkHedgehog.RideTests

open Shared
open Hedgehog
open FunPark.Rides
open FSharp.Data.UnitSystems.SI.UnitSymbols

let genRideInput =
    gen {
        let! name =
            Gen.string (Range.linear 1 50) Gen.alpha
            |> Gen.filter (fun s -> not (System.String.IsNullOrWhiteSpace s))

        let! minAge =
            Gen.int32 (Range.linear -20 90)
            |> Gen.map (fun i -> PositiveNonZeroInt.create (LanguagePrimitives.Int32WithMeasure<yr> i))


        let! minHeight =
            Gen.int32 (Range.linear 50 220)
            |> Gen.map (fun i -> PositiveNonZeroInt.create (LanguagePrimitives.Int32WithMeasure<cm> i))

        let! waitTime =
            Gen.int32 (Range.linear 30 180)
            |> Gen.map (fun i -> PositiveNonZeroInt.create (LanguagePrimitives.Int32WithMeasure<s> i))

        let! online =
            Gen.frequency
                [ 3, Gen.constant (Some Online)
                  1, Gen.constant (Some Offline)
                  1, Gen.constant None ]

        let! tags =
            typeof<RideTags>
            |> Reflection.FSharpType.GetUnionCases
            |> Array.map (fun case ->
                gen {
                    let! includeTag = Gen.bool

                    return
                        if includeTag then
                            Some(Reflection.FSharpValue.MakeUnion(case, [||]) :?> RideTags)
                        else
                            None
                })
            |> Array.toList
            |> ListGen.sequence
            |> Gen.map (List.choose id)

        return
            Ride.create
                { Name = name
                  MinAge = minAge
                  MinHeight = minHeight
                  WaitTime = waitTime
                  Online = online
                  Tags = tags }
    }

let propMinAgePositiveNonZero =
    property {
        let! ride = genRideInput
        let rideView = Ride.value ride

        if rideView.MinAge > 0<yr> then
            ()
        else
            failwithf "Ride.MinAge is not positive: %A" rideView.MinAge
    }

let propMinHeightPositiveNonZero =
    property {
        let! ride = genRideInput
        let rideView = Ride.value ride

        if rideView.MinHeight > 0<cm> then
            ()
        else
            failwithf "Ride.MinHeight is not positive: %A" rideView.MinHeight
    }

let propMinWaitTimePositiveNonZero =
    property {
        let! ride = genRideInput
        let rideView = Ride.value ride

        if rideView.WaitTime > 0<s> then
            ()
        else
            failwithf "Ride.WaitTime is not positive: %A" rideView.WaitTime
    }

[<EntryPoint>]
let main _ =
    printfn "Testing: All Ride's MinAge is positive"
    let minAgeResult = Property.check propMinAgePositiveNonZero
    printfn "%A" minAgeResult

    printfn "Testing: All Ride's MinHeight is positive"
    let minHeightResult = Property.check propMinHeightPositiveNonZero
    printfn "%A" minHeightResult

    printfn "Testing: All Ride's MinWaitTime is positive"
    let minWaitTimeResult = Property.check propMinWaitTimePositiveNonZero
    printfn "%A" minWaitTimeResult

    0
