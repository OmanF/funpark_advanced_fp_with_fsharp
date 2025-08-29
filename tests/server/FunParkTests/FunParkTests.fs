open Expecto
open Shared
open FunPark.Rides
open FSharp.Data.UnitSystems.SI.UnitSymbols

module DomainTests =
    let ridesTests =
        testList
            "Rides tests"
            [ testCase "Create a ride with all valid parameters"
              <| fun _ ->
                  let ride =
                      Ride.create
                          { Name = "Roller Coaster"
                            MinAge = PositiveNonZeroInt.create 8<yr>
                            MinHeight = PositiveNonZeroInt.create 90<cm>
                            WaitTime = PositiveNonZeroInt.create 25<s>
                            Online = Some Online
                            Tags = [ FamilyFriendly; Thrilling ] }

                  Expect.equal (Ride.value ride).Name "Roller Coaster" "Expected ride name to match"

              testCase "Create a ride with (at least one) invalid parameter(s)"
              <| fun _ ->
                  let ride =
                      Ride.create
                          { Name = "Invalid Ride"
                            MinAge = PositiveNonZeroInt.create -88<yr>
                            MinHeight = PositiveNonZeroInt.create 90<cm>
                            WaitTime = PositiveNonZeroInt.create 25<s>
                            Online = Some Online
                            Tags = [ FamilyFriendly; Thrilling ] }

                  Expect.equal (Ride.value ride).MinAge 8<yr> "Expected ride min age to match" ]

[<EntryPoint>]
let main args =
    runTestsWithCLIArgs [] args DomainTests.ridesTests
