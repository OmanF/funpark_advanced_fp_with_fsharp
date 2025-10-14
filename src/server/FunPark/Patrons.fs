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
              Dislikes: Set<string>
              LastChanged: DateTime }

        override this.Equals otherPatron =
            match otherPatron with
            // Educational: by asserting equality on two (or more) members, we must use `HashCode.Combine` in `GetHashCode` override
            | :? Patron as other -> this.Id = other.Id && this.Name = other.Name
            | _ -> false

        override this.GetHashCode() =
            // Educational: `HashCode.Combine` is used to combine multiple hash codes into a single hash code
            HashCode.Combine(hash this.Id, hash this.Name)

        interface IComparable with
            member this.CompareTo otherPatron =
                match otherPatron with
                | :? Patron as other -> compare (this.Id, this.Name) (other.Id, other.Name)
                | _ -> invalidArg "otherPatron" "Can't compare values of different types"

    type PatronView =
        { Id: Guid
          Name: string
          Age: int<yr>
          Height: int<cm>
          RewardPoints: int<rp>
          TicketTier: TicketTier
          FreePasses: Set<FreePass>
          Likes: Set<string>
          Dislikes: Set<string> }

    module Patron =
        type PatronConstructor =
            { Name: ContentfulString option
              Age: Natural<yr> option
              Height: Natural<cm> option
              RewardPoints: int<rp>
              TicketTier: TicketTier option
              FreePasses: FreePass list
              Likes: string list
              Dislikes: string list }

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
              Dislikes = dislikes |> Set.ofList
              LastChanged = DateTime.UtcNow }

        let value (patron: Patron) : PatronView =
            { Id = patron.Id
              Name = patron.Name
              Age = patron.Age
              Height = patron.Height
              RewardPoints = patron.RewardPoints
              TicketTier = patron.TicketTier
              FreePasses = patron.FreePasses
              Likes = patron.Likes
              Dislikes = patron.Dislikes }

        let update (patron: Patron) (update: PatronUpdate) =
            { patron with
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
                    |> Option.defaultValue patron.Dislikes
                LastChanged = DateTime.UtcNow }
