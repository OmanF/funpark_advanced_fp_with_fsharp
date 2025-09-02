namespace FunParkHedgehog.Tests

open Shared
open Hedgehog
open FunPark.Rides
open FSharp.Data.UnitSystems.SI.UnitSymbols

[<AutoOpen>]
module RideGenerator =
    let genRide =
        gen {
            // Integrate with `Bogus` library to produce reproducible (by seed), human names
            let! name =
                gen {
                    let! fakerName = genBogus (fun faker -> faker.Name)
                    let! size = Gen.int32 (Range.linear 1 50)

                    return!
                        if size < 5 then Gen.constant "Mr. O"
                        elif size < 10 then Gen.constant (fakerName.FirstName())
                        else Gen.constant (fakerName.FullName())
                }

            let! minAge =
                Gen.int32 (Range.linear -20 90)
                |> Gen.map (fun i -> Natural.create (LanguagePrimitives.Int32WithMeasure<yr> i))

            let! minHeight =
                Gen.int32 (Range.linear 50 220)
                |> Gen.map (fun i -> Natural.create (LanguagePrimitives.Int32WithMeasure<cm> i))

            let! waitTime =
                Gen.int32 (Range.linear 30 180)
                |> Gen.map (fun i -> Natural.create (LanguagePrimitives.Int32WithMeasure<s> i))

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

[<AutoOpen>]
module RideProperties =
    let propMinAgePositiveNonZero =
        property {
            let! ride = genRide
            let rideView = Ride.value ride

            if rideView.MinAge > 0<yr> then
                ()
            else
                failwithf "Ride.MinAge is not positive: %A" rideView.MinAge
        }

    let propMinHeightPositiveNonZero =
        property {
            let! ride = genRide
            let rideView = Ride.value ride

            if rideView.MinHeight > 0<cm> then
                ()
            else
                failwithf "Ride.MinHeight is not positive: %A" rideView.MinHeight
        }

    let propMinWaitTimePositiveNonZero =
        property {
            let! ride = genRide
            let rideView = Ride.value ride

            if rideView.WaitTime > 0<s> then
                ()
            else
                failwithf "Ride.WaitTime is not positive: %A" rideView.WaitTime
        }
