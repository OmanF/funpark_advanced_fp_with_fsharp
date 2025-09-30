namespace FunParkHedgehog.Tests

open Shared
open System
open Hedgehog
open FunPark.FreePasses

[<AutoOpen>]
module FreePassGenerator =
    let genFreePass =
        gen {
            let now = DateTime.UtcNow
            let! ride = genRide
            let! validFromGenerator = Gen.dateTime (Range.constant (now.AddMinutes -5) now)
            let validFrom = validFromGenerator |> ValidFreePassStartDate.Create

            return FreePass.create { Ride = ride; ValidFrom = validFrom }
        }

[<AutoOpen>]
module FreePassProperties =
    let propFreePassNotInFuture =
        property {
            let! freePass = genFreePass
            let freePassView = FreePass.value freePass

            if freePassView.ValidFrom <= DateTime.UtcNow then
                ()
            else
                failwithf "FreePass.ValidFrom is in the future: %A" freePassView.ValidFrom
        }
