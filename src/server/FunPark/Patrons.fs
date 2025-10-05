namespace FunPark

open System
open Shared
open FunPark.FreePasses

module Patrons =
    [<Measure>]
    type rp // Reward points

    type TicketTier =
        | Standard
        | Premium
        | VIP

    [<CustomEquality; CustomComparison>]
    type Patron =
        private
            { Id: Guid
              Name: string
              Age: int<yr>
              Height: int<cm>
              RewardPoints: int<rp>
              TicketTier: TicketTier
              FreePasses: Set<FreePass>
              Likes: Set<string>
              Dislikes: Set<string> }

        override this.Equals otherPatron =
            match otherPatron with
            // Equality is based on both `Id` and `Name`, same reasoning as in `Ride` type, and for the same educational purpose
            | :? Patron as other -> this.Id = other.Id && this.Name = other.Name
            | _ -> false

        override this.GetHashCode() =
            HashCode.Combine(hash this.Id, hash this.Name)

        interface IComparable with
            member this.CompareTo otherPatron =
                match otherPatron with
                // Ordering over `Name` only, as `Id` has no intrinsic ordering value, it's just a unique identifier
                | :? Patron as other -> this.Name.CompareTo other.Name
                | _ -> invalidArg "otherPatron" "Can't compare values of different types"

    // Public view type for `Patron`, exposes all fields for dot-access
    type PatronView =
        { Id: Guid
          Name: string
          Age: int<yr>
          Height: int<cm>
          RewardPoints: int<rp>
          TicketTier: TicketTier
          FreePasses: Set<FreePassView>
          Likes: Set<string>
          Dislikes: Set<string> }

    module Patron =
        // Utility type for constructing a `Patron`: allows for named parameters to be used as input for the constructor
        // Either alternative, tuple or curried parameters, require positional arguments, with no tags, making it harder to understand the purpose of each argument, and their correct order
        type PatronConstructor =
            { Name: ContentfulString option
              Age: Natural<yr> option
              Height: Natural<cm> option
              RewardPoints: int<rp>
              TicketTier: TicketTier option
              FreePasses: FreePass list
              Likes: string list
              Dislikes: string list }

        // Auxiliary type for the following `update` function, because I dislike passing anonymous records as parameters
        type PatronUpdate =
            { Id: Guid option
              Name: ContentfulString option
              Age: Natural<yr> option
              Height: Natural<cm> option
              RewardPoints: int<rp> option
              TicketTier: TicketTier option
              FreePasses: FreePass list option
              Likes: string list option
              Dislikes: string list option }

        // Annotating the function output's type as `Patron`, the private type, is required since `Patron` and `PatronView` have the same fields, and `PatronView` comes later, unless the function is annotated it will be assigned the wrong type, the public `PatronView` type
        // The input's type is inferred as `PatronConstructor` since it's missing the `Id` field
        let create
            ({ Name = name
               Age = minAge
               Height = minHeight
               RewardPoints = rewardPoints
               TicketTier = ticketTier
               FreePasses = freePasses
               Likes = likes
               Dislikes = dislikes }: PatronConstructor)
            : Patron =
            { Id = Guid.NewGuid()
              Name =
                name
                |> Option.map (fun (n: ContentfulString) -> n.Value)
                |> Option.defaultValue "Generic patron!"
              Age = defaultArg (Option.map Natural.value minAge) 30<yr>
              Height = minHeight |> Option.map Natural.value |> Option.defaultValue 190<cm>
              RewardPoints =
                defaultArg
                    (match rewardPoints > 0<rp> with
                     | true -> Some rewardPoints
                     | false -> None)
                    0<rp>
              TicketTier = defaultArg ticketTier Standard
              FreePasses = freePasses |> Set.ofList
              Likes = likes |> Set.ofList
              Dislikes = dislikes |> Set.ofList }

        // Annotation both required: since `Patron` and `PatronView` have the same fields, and `PatronView` comes later, unless `patron` is annotated, the compiler will assign its type as `PatronView` which will result in a logic error
        // But, also helpful in distinguishing between the fact input is a `Patron`, the private type, and the output is `PatronView`, the publicly accessible type
        let value (patron: Patron) : PatronView =
            { Id = patron.Id
              Name = patron.Name
              Age = patron.Age
              Height = patron.Height
              RewardPoints = patron.RewardPoints
              TicketTier = patron.TicketTier
              FreePasses = patron.FreePasses |> Set.map FreePass.value
              Likes = patron.Likes
              Dislikes = patron.Dislikes }

        // Annotation NOT required, `PatronUpdate` differs from `PatronConstructor` by the `Id` field, but as always, explicit is better in a complex domain
        // `PatronUpdate` required since, as with the tests, `Patron` is a private type and we can't **directly** assign it, or its members, to any constructor function, only a public clone of it
        // Is it likely, not to mention smart, that a `Patron`'s `Id` would change? No, but an `update` function should be able to update any field, including the `Id`, even if it's not a common operation
        let update (patron: Patron) (update: PatronUpdate) =
            { patron with
                // Using `Option.defaultValue` instead of `defaultArg` to show the stylistic choices in F# (since F#6, in this case)
                Id = update.Id |> Option.defaultValue patron.Id
                Name =
                    update.Name
                    |> Option.map (fun (n: ContentfulString) -> n.Value)
                    |> Option.defaultValue patron.Name
                Age = update.Age |> Option.map Natural.value |> Option.defaultValue patron.Age
                Height = update.Height |> Option.map Natural.value |> Option.defaultValue patron.Height
                TicketTier = update.TicketTier |> Option.defaultValue patron.TicketTier
                RewardPoints =
                    // Update the reward points only if the new value is positive
                    update.RewardPoints
                    |> Option.bind (fun rp -> if rp > 0<rp> then Some rp else None)
                    |> Option.defaultValue patron.RewardPoints
                // For `FreePasses`, `Likes` and `Dislikes`, like in `Ride` type:
                // Depending on the value passed:
                // `None` - wipes the respective set clean
                // `Some []` - leaves the respective set unchanged
                // `Some [x1; x2; ...]` - adds the new items to the existing set
                // See logic in `Ride`'s `update` function's `tags` member
                FreePasses =
                    update.FreePasses
                    |> Option.map (fun fps -> Set.union (Set.ofList fps) patron.FreePasses)
                    |> Option.defaultValue patron.FreePasses
                Likes =
                    update.Likes
                    |> Option.map (fun ls -> Set.union (Set.ofList ls) patron.Likes)
                    |> Option.defaultValue patron.Likes
                Dislikes =
                    update.Dislikes
                    |> Option.map (fun ds -> Set.union (Set.ofList ds) patron.Dislikes)
                    |> Option.defaultValue patron.Dislikes }
