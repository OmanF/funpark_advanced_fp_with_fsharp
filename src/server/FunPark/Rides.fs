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
              Tags: Set<RideTags> }

        override this.Equals otherRide =
            match otherRide with
            // Equality is based on both `Id` and `Name`, to avoid accidental equality of two different rides with the same name (e.g., "Ferris Wheel")
            // Indeed, `Id` alone suffices, that's the definition of the `Id` field, but I'm adding `Name` to show, and use, `System.HashCode.Combine` in the override of `GetHashCode()` - an educational opportunity
            | :? Ride as other -> this.Id = other.Id && this.Name = other.Name
            | _ -> false

        override this.GetHashCode() =
            // `System.HashCode.Combine` combines multiple hash codes into one
            // Used internally by .Net to generate hash codes for multi-part data structures (e.g., tuples, records with multiple fields, etc.), it should be used to generate hash codes for custom equality implementations taking more than one field into account
            HashCode.Combine(hash this.Id, hash this.Name)

        interface IComparable with
            member this.CompareTo otherRide =
                match otherRide with
                // Ordering over `Name` only, since `Id` has no intrinsic ordering value, it's just a unique identifier
                | :? Ride as other -> this.Name.CompareTo other.Name
                | _ -> invalidArg "otherRide" "Can't compare values of different types"

    // Public view type for `Ride`, exposes all fields for dot-access
    type RideView =
        { Id: Guid
          Name: string
          MinAge: int<yr>
          MinHeight: int<cm>
          WaitTime: int<s>
          Online: RideStatus
          Tags: Set<RideTags> }

    module Ride =
        // Utility type for constructing a `Ride`: allows for named parameters to be used as input for the constructor
        // Either alternative, tuple or curried parameters, require positional arguments, with no tags, making it harder to understand the purpose of each argument, and their correct order
        type RideConstructor =
            { Name: ContentfulString option
              MinAge: Natural<yr> option
              MinHeight: Natural<cm> option
              WaitTime: Natural<s> option
              Online: RideStatus option
              Tags: RideTags list }

        // Auxiliary type for the following `update` function, because I dislike passing anonymous records as parameters
        type RideUpdate =
            { Id: Guid option
              Name: ContentfulString option
              MinAge: Natural<yr> option
              MinHeight: Natural<cm> option
              WaitTime: Natural<s> option
              Online: RideStatus option
              Tags: RideTags list option }

        // Annotating the function output's type as `Ride`, the private type, is required since `Ride` and `RideView` have the same fields, and `RideView` comes later, unless the function is annotated it will be assigned the wrong type, the public `RideView` type
        // The input's type is inferred as `RideConstructor` since it's missing the `Id` field
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
              Tags = Set.ofList tags }

        // Annotation both required: since `Ride` and `RideView` have the same fields, and `RideView` comes later, unless `ride` is annotated, the compiler will assign its type as `RideView` which will result in a logic error
        // But, also helpful in distinguishing between the fact input is a `Ride`, the private type, and the output is `RideView`, the publicly accessible type
        let value (ride: Ride) : RideView =
            { Id = ride.Id
              Name = ride.Name
              MinAge = ride.MinAge
              MinHeight = ride.MinHeight
              WaitTime = ride.WaitTime
              Online = ride.Online
              Tags = ride.Tags }

        // Annotation NOT required, `RideUpdate` differs from `RideConstructor` by the `Id` field, but as always, explicit is better in a complex domain
        // `RideUpdate` required since, as with the tests, `Ride` is a private type and we can't **directly** assign it, or its members, to any constructor function, only a public clone of it
        // Is it likely, not to mention smart, that a `Ride`'s `Id` would change? No, but an `update` function should be able to update any field, including the `Id`, even if it's not a common operation
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
                    |> Option.defaultValue Set.empty }
