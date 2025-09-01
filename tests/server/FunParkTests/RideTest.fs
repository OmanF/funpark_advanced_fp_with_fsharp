namespace FunParkExpecto.Tests

open Expecto
open Shared
open FunPark.Rides
open FSharp.Data.UnitSystems.SI.UnitSymbols

[<AutoOpen>]
module RideTests =
    let ridesTests =
        let referenceRide: RideView =
            { Id = new System.Guid()
              Name = "Roller Coaster"
              MinAge = 8<yr>
              MinHeight = 100<cm>
              WaitTime = 60<s>
              Online = Online
              Tags = [ FamilyFriendly; Thrilling ] }

        testList
            "Rides tests"
            [ testCase "Create a ride with all valid parameters - `Ride` type happy path"
              <| fun _ ->
                  let ride =
                      Ride.create
                          { Name = "Roller Coaster"
                            MinAge = PositiveNonZeroInt.create 5<yr>
                            MinHeight = PositiveNonZeroInt.create 90<cm>
                            WaitTime = PositiveNonZeroInt.create 25<s>
                            Online = Some Online
                            Tags = [ FamilyFriendly ] }

                  Expect.equal
                      (Ride.value ride)
                      { referenceRide with
                          Id = (Ride.value ride).Id
                          MinAge = 5<yr>
                          MinHeight = 90<cm>
                          WaitTime = 25<s>
                          Tags = [ FamilyFriendly ] }
                      "Expected ride record to match"

              testCase "Create a ride with an invalid name"
              <| fun _ ->
                  Expect.throws
                      (fun () ->
                          Ride.create
                              { Name = ""
                                MinAge = PositiveNonZeroInt.create 8<yr>
                                MinHeight = PositiveNonZeroInt.create 100<cm>
                                WaitTime = PositiveNonZeroInt.create 60<s>
                                Online = Some Online
                                Tags = [ FamilyFriendly; Thrilling ] }
                          |> ignore)
                      "Expected an exception for empty ride name"

              testCase "Create a ride with an invalid minimal age"
              <| fun _ ->
                  let ride =
                      Ride.create
                          { Name = "Roller Coaster"
                            MinAge = PositiveNonZeroInt.create -10<yr>
                            MinHeight = PositiveNonZeroInt.create 100<cm>
                            WaitTime = PositiveNonZeroInt.create 60<s>
                            Online = Some Online
                            Tags = [ FamilyFriendly; Thrilling ] }

                  Expect.equal
                      (Ride.value ride)
                      { referenceRide with
                          Id = (Ride.value ride).Id
                          MinAge = 8<yr> }
                      "Expected ride record to match"

              testCase "Create a ride with an invalid minimal height"
              <| fun _ ->
                  let ride =
                      Ride.create
                          { Name = "Roller Coaster"
                            MinAge = PositiveNonZeroInt.create 8<yr>
                            MinHeight = PositiveNonZeroInt.create -30<cm>
                            WaitTime = PositiveNonZeroInt.create 60<s>
                            Online = Some Online
                            Tags = [ FamilyFriendly; Thrilling ] }

                  Expect.equal
                      (Ride.value ride)
                      { referenceRide with
                          Id = (Ride.value ride).Id
                          MinHeight = 100<cm> }
                      "Expected ride record to match"

              testCase "Create a ride with an invalid wait time"
              <| fun _ ->
                  let ride =
                      Ride.create
                          { Name = "Roller Coaster"
                            MinAge = PositiveNonZeroInt.create 8<yr>
                            MinHeight = PositiveNonZeroInt.create 100<cm>
                            WaitTime = PositiveNonZeroInt.create -25<s>
                            Online = Some Online
                            Tags = [ FamilyFriendly; Thrilling ] }

                  Expect.equal
                      (Ride.value ride)
                      { referenceRide with
                          Id = (Ride.value ride).Id
                          WaitTime = 60<s> }
                      "Expected ride record to match"

              testCase "Create a ride with an invalid online status"
              <| fun _ ->
                  let ride =
                      Ride.create
                          { Name = "Roller Coaster"
                            MinAge = PositiveNonZeroInt.create 8<yr>
                            MinHeight = PositiveNonZeroInt.create 100<cm>
                            WaitTime = PositiveNonZeroInt.create 60<s>
                            Online = None
                            Tags = [ FamilyFriendly; Thrilling ] }

                  Expect.equal
                      (Ride.value ride)
                      { referenceRide with
                          Id = (Ride.value ride).Id }
                      "Expected ride record to match"

              testCase "Create a ride with an varying tags list"
              <| fun _ ->
                  let ride =
                      Ride.create
                          { Name = "Roller Coaster"
                            MinAge = PositiveNonZeroInt.create 8<yr>
                            MinHeight = PositiveNonZeroInt.create 100<cm>
                            WaitTime = PositiveNonZeroInt.create 60<s>
                            Online = Some Online
                            Tags = [] }

                  Expect.equal
                      (Ride.value ride)
                      { referenceRide with
                          Id = (Ride.value ride).Id
                          Tags = [] }
                      "Expected ride record to match" ]
