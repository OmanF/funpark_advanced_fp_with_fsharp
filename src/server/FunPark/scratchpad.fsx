#load "/home/ofk/FunPark/Shared/Shared.fs"
#load "/home/ofk/FunPark/src/server/FunPark/Rides.fs"
#load "/home/ofk/FunPark/src/server/FunPark/FreePasses.fs"
#load "/home/ofk/FunPark/src/server/FunPark/Patrons.fs"

#r "nuget: Bogus"

open Bogus
open System
open Shared
open FunPark.Rides
open FunPark.FreePasses
open FunPark.Patrons
open FSharp.Data.UnitSystems.SI.UnitSymbols

let f = Faker()

let myRide =
    Ride.create
        { Name = "Ferris Wheel"
          MinAge = PositiveNonZeroInt.create (f.Random.Int(-25, 6) * 1<yr>)
          MinHeight = PositiveNonZeroInt.create (f.Random.Int(-25, 90) * 1<cm>)
          WaitTime = PositiveNonZeroInt.create (f.Random.Int(-25, 30) * 1<s>)
          Online = Online |> Some
          Tags = [ FamilyFriendly; Thrilling ] }

let myFreePass =
    FreePass.create
        { Ride = myRide
          ValidFrom = DateTime.UtcNow }

let myPatron =
    Patron.create
        { Name = f.Name.FullName()
          Age = PositiveNonZeroInt.create (f.Random.Int(-25, 25) * 1<yr>)
          Height = PositiveNonZeroInt.create (f.Random.Int(-25, 175) * 1<cm>)
          RewardPoints = f.Random.Int(-25, 50) * 1<rp>
          TicketTier = Standard |> Some
          FreePasses = [ myFreePass ]
          Likes = [ "Roller Coaster"; "Ferris Wheel" ]
          Dislikes = [ "Haunted House" ] }

printfn $"Ride's name: %s{(Ride.value myRide).Name}"
printfn $"FreePass is valid starting: %A{(FreePass.value myFreePass).ValidFrom}"
printfn $"Parton's name is: %s{(Patron.value myPatron).Name}"
printfn $"Ride created: %A{myRide}"
printfn $"Free pass created: %A{myFreePass}"
printfn $"Patron created: %A{myPatron}"
