namespace FunPark

open System
open Shared
open FSharp.Data.UnitSystems.SI.UnitSymbols

module Rides =
    type RideTags =
        | FamilyFriendly
        | Thrilling
        | Educational

    type RideStatus =
        | Online
        | Offline

    [<CustomEquality; CustomComparison>]
    type Ride =
        private
            { Id: Guid
              Name: string
              MinAge: int<yr>
              MinHeight: int<cm>
              WaitTime: int<s>
              Online: RideStatus
              Tags: Set<RideTags>
              LastChanged: DateTime }

        override this.Equals otherRide =
            match otherRide with
            | :? Ride as other -> this.Id = other.Id
            | _ -> false

        override this.GetHashCode() = hash this.Id

        interface IComparable with
            member this.CompareTo otherRide =
                match otherRide with
                | :? Ride as other -> this.Id.CompareTo other.Id
                | _ -> invalidArg "otherRide" "Can't compare values of different types"

    type RideView =
        { Id: Guid
          Name: string
          MinAge: int<yr>
          MinHeight: int<cm>
          WaitTime: int<s>
          Online: RideStatus
          Tags: Set<RideTags> }

    module Ride =
        type RideConstructor =
            { Name: ContentfulString option
              MinAge: Natural<yr> option
              MinHeight: Natural<cm> option
              WaitTime: Natural<s> option
              Online: RideStatus option
              Tags: RideTags list }

        type RideUpdate =
            { Id: Guid option
              Name: ContentfulString option
              MinAge: Natural<yr> option
              MinHeight: Natural<cm> option
              WaitTime: Natural<s> option
              Online: RideStatus option
              Tags: RideTags list option }

        let create
            ({ Name = name
               MinAge = minAge
               MinHeight = minHeight
               WaitTime = waitTime
               Online = online
               Tags = tags }: RideConstructor)
            : Ride =
            { Id = Guid.NewGuid()
              Name = defaultArg (Option.map (fun (n: ContentfulString) -> n.Value) name) "Generic ride!"
              MinAge = defaultArg (Option.map Natural.value minAge) 8<yr>
              MinHeight = defaultArg (Option.map Natural.value minHeight) 100<cm>
              WaitTime =
                if online = Some Offline then
                    0<s>
                else
                    defaultArg (Option.map Natural.value waitTime) 60<s>
              Online = defaultArg online Online
              Tags = Set.ofList tags
              LastChanged = DateTime.UtcNow }

        let value (ride: Ride) : RideView =
            { Id = ride.Id
              Name = ride.Name
              MinAge = ride.MinAge
              MinHeight = ride.MinHeight
              WaitTime = ride.WaitTime
              Online = ride.Online
              Tags = ride.Tags }

        let update (ride: Ride) (update: RideUpdate) =
            { ride with
                // Using `Option.defaultValue` instead of `defaultArg` to show the stylistic choices in F# (since F#6, in this case)
                Id = update.Id |> Option.defaultValue ride.Id
                Name =
                    update.Name
                    |> Option.map (fun (n: ContentfulString) -> n.Value)
                    |> Option.defaultValue ride.Name
                MinAge = update.MinAge |> Option.map Natural.value |> Option.defaultValue ride.MinAge
                MinHeight =
                    update.MinHeight
                    |> Option.map Natural.value
                    |> Option.defaultValue ride.MinHeight
                WaitTime = update.WaitTime |> Option.map Natural.value |> Option.defaultValue ride.WaitTime
                Online = update.Online |> Option.defaultValue ride.Online
                Tags =
                    // Adds to the existing tags (i.e., `ride.Tags`), or wipes the tags set clean completely
                    // Depending on the value of `update.Tags`:
                    // `None` - wipes the tags set clean, i.e., new tags set is the empty set!
                    // `Some []` - retains `ride.Tags`, making no changes
                    // `Some [tag1; tag2...] - **Adds** tags from the list not already on `ride.Tags`
                    // This is done to make **adding** new tags easier - just pass the new tags in a list, without needing to know, or pass in, the existing tags
                    // The tradeoff is that removing certain tag(s) is more cumbersome: first wipe the set clean, then pass the **entire new set**, but that is a less common operation - tags should be assigned after some thought
                    update.Tags
                    |> Option.map (fun ts -> Set.union (Set.ofList ts) ride.Tags)
                    |> Option.defaultValue Set.empty
                LastChanged = DateTime.UtcNow }
