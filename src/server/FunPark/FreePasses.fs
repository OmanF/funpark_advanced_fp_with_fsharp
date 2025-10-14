namespace FunPark

open System
open Shared
open Rides

module FreePasses =
    [<CustomEquality; CustomComparison>]
    type FreePass =
        private
            { Id: Guid
              Ride: Ride
              ValidFrom: DateTime
              LastChanged: DateTime }

        override this.Equals otherFreePass =
            match otherFreePass with
            | :? FreePass as other -> this.Id = other.Id
            | _ -> false

        override this.GetHashCode() = hash this.Id

        interface IComparable with
            member this.CompareTo otherFreePass =
                match otherFreePass with
                | :? FreePass as other -> this.Id.CompareTo other.Id
                | _ -> invalidArg "otherFreePass" "Can't compare values of different types"

    type FreePassView =
        { Id: Guid
          Ride: RideView
          ValidFrom: DateTime }

    module FreePass =
        type FreePassConstructor =
            { Ride: Ride
              ValidFrom: ValidFreePassStartDate option }

        type FreePassUpdate =
            { Id: Guid option
              Ride: Ride option
              ValidFrom: ValidFreePassStartDate option }

        let create { Ride = ride; ValidFrom = validFrom } : FreePass =
            { Id = Guid.NewGuid()
              Ride = ride
              ValidFrom =
                validFrom
                |> Option.map (fun d -> d.Value)
                |> Option.defaultValue DateTime.UtcNow
              LastChanged = DateTime.UtcNow }

        let value (freePass: FreePass) : FreePassView =
            { Id = freePass.Id
              Ride = Ride.value freePass.Ride
              ValidFrom = freePass.ValidFrom }

        let update (freePass: FreePass) (update: FreePassUpdate) =
            { freePass with
                Id = update.Id |> Option.defaultValue freePass.Id
                Ride = defaultArg update.Ride freePass.Ride
                ValidFrom =
                    update.ValidFrom
                    |> Option.map (fun d -> d.Value)
                    |> Option.defaultValue freePass.ValidFrom
                LastChanged = DateTime.UtcNow }
