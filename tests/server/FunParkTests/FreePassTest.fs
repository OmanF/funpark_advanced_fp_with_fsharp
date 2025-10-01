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
            [ testCase "Create a FreePass with all valid parameters - `FreePass` type happy path"
              <| fun _ ->
                  printfn "Testing FreePass"

                  let freePass =
                      FreePass.create
                          { Ride = referenceRide
                            ValidFrom = ValidFreePassStartDate.Create System.DateTime.UtcNow }

                  Expect.equal
                      (FreePass.value freePass)
                      { Ride = Ride.value referenceRide
                        ValidFrom = (FreePass.value freePass).ValidFrom
                        Id = (FreePass.value freePass).Id }
                      "Expected FreePass record to match"

              //   testCase "Create a FreePass with an invalid ValidFrom date (in the future)"
              //   <| fun _ ->
              //       let ride =
              //           Ride.create
              //               { Name = ContentfulString.Create "Roller Coaster"
              //                 MinAge = Natural.create 5<yr>
              //                 MinHeight = Natural.create 90<cm>
              //                 WaitTime = Natural.create 25<s>
              //                 Online = Some Online
              //                 Tags = [ FamilyFriendly ] }

              //       let invalidDate = System.DateTime.UtcNow.AddHours(1)

              //       Expect.isNone
              //           (ValidFreePassStartDate.Create invalidDate)
              //           (sprintf "Expected None for invalid ValidFrom date: %A" invalidDate)
              ]
