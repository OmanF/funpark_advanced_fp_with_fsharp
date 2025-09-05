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

                    let! name =
                        if size < 5 then Gen.constant "Mr. O"
                        elif size < 10 then Gen.constant (fakerName.FirstName())
                        else Gen.constant (fakerName.FullName())

                    return name |> ContentfulString.Create
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
                let rec repeatedAccumulation n f acc =
                    if n <= 0 then
                        acc
                    else
                        repeatedAccumulation (n - 1) f (f () @ acc)

                let tagSelector () =
                    typeof<RideTags>
                    |> Reflection.FSharpType.GetUnionCases
                    |> Array.fold
                        (fun acc case ->
                            match System.Random().NextDouble() <= 0.5 with
                            | true ->
                                let tag = Reflection.FSharpValue.MakeUnion(case, [||]) :?> RideTags
                                tag :: acc
                            | false -> acc)
                        []

                Gen.frequency
                    [ // 2-in-3: one pass, creating tags probabilistically
                      2, tagSelector () |> Gen.constant

                      // 1-in-3: three passes, creating tags probabilistically
                      1, repeatedAccumulation 3 tagSelector [] |> Gen.constant ]

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
    let propMinAgeNatural =
        property {
            let! ride = genRide
            let rideView = Ride.value ride

            if rideView.MinAge > 0<yr> then
                ()
            else
                failwithf "Ride.MinAge is not positive: %A" rideView.MinAge
        }

    let propMinHeightNatural =
        property {
            let! ride = genRide
            let rideView = Ride.value ride

            if rideView.MinHeight > 0<cm> then
                ()
            else
                failwithf "Ride.MinHeight is not positive: %A" rideView.MinHeight
        }

    let propMinWaitTimeNatural =
        property {
            let! ride = genRide
            let rideView = Ride.value ride
            where (rideView.Online <> Offline) // This test is only true for online rides! Rides that are offline have wait time of 0, tested in another test in this suite

            if rideView.WaitTime > 0<s> then
                ()
            else
                failwithf "Ride.WaitTime is not positive: %A" rideView.WaitTime
        }

    let propNoDuplicateTags =
        property {
            let! ride = genRide
            let rideView = Ride.value ride

            if List.distinct rideView.Tags |> List.length = List.length rideView.Tags then
                ()
            else
                failwithf "Ride.Tags contains duplicates: %A" rideView.Tags
        }

    let propOfflineWaitTimeZero =
        property {
            let! ride = genRide
            let rideView = Ride.value ride

            if rideView.Online = Offline then
                if rideView.WaitTime = 0<s> then
                    ()
                else
                    failwithf "Ride.WaitTime is not zero for an offline ride: %A" rideView.WaitTime
            else
                ()
        }
