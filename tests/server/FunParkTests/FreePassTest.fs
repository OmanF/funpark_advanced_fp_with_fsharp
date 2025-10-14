namespace FunParkExpecto.Tests

open Expecto
open Shared
open FunPark.Rides
open FunPark.FreePasses
open FSharp.Data.UnitSystems.SI.UnitSymbols

[<AutoOpen>]
module FreePassTests =
    let freePassTests =
        let referenceRide =
            Ride.create
                { Name = ContentfulString.Create "Roller Coaster"
                  MinAge = Natural.create 5<yr>
                  MinHeight = Natural.create 90<cm>
                  WaitTime = Natural.create 25<s>
                  Online = Some Online
                  Tags = [ FamilyFriendly ] }

        testList
            "FreePass tests"
            // There is no "Happy path" test for FreePass creation because the only way to create a FreePass is via the `FreePass.create` function, which only takes already validated parameters, so there is no way to create an invalid FreePass at runtime
            [ testCase "Create a FreePass with an invalid ValidFrom date (in the future)"
              <| fun _ ->
                  let invalidDate = System.DateTime.UtcNow.AddHours(1)

                  let testFreePass =
                      FreePass.create
                          { Ride = referenceRide
                            ValidFrom = ValidFreePassStartDate.Create invalidDate }

                  Expect.equal
                      (FreePass.value testFreePass)
                      { Ride = referenceRide
                        Id = (FreePass.value testFreePass).Id
                        ValidFrom = (FreePass.value testFreePass).ValidFrom }
                      "Expected ValidFrom to be `DateTime.UtcNow`" ]
