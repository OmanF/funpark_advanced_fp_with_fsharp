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

    type Patron =
        private
            { Id: Guid
              Name: string
              Age: int<yr>
              Height: int<cm>
              RewardPoints: int<rp>
              TicketTier: TicketTier
              FreePasses: FreePass list
              Likes: string list
              Dislikes: string list }

    // Public view type for Patron, exposes all fields for dot-access
    type PatronView =
        { Id: Guid
          Name: string
          Age: int<yr>
          Height: int<cm>
          RewardPoints: int<rp>
          TicketTier: TicketTier
          FreePasses: FreePassView list
          Likes: string list
          Dislikes: string list }

    module Patron =
        // Utility type for constructing a Patron: allows for named parameters to be used as input for the constructor
        // Either alternative, tuple or curried parameters, require positional arguments, with no "nametag", making it harder to understand the purpose of each argument, and their correct order
        type PatronConstructor =
            { Name: ContentfulString option
              Age: Natural<yr> option
              Height: Natural<cm> option
              RewardPoints: int<rp>
              TicketTier: TicketTier option
              FreePasses: FreePass list
              Likes: string list
              Dislikes: string list }

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
              Name = defaultArg (Option.map (fun (n: ContentfulString) -> n.Value) name) "Generic patron!"
              Age = defaultArg (Option.map Natural.value minAge) 30<yr>
              Height = defaultArg (Option.map Natural.value minHeight) 190<cm>
              RewardPoints =
                defaultArg
                    (match rewardPoints > 0<rp> with
                     | true -> Some rewardPoints
                     | false -> None)
                    0<rp>
              TicketTier = defaultArg ticketTier Standard
              FreePasses = freePasses
              Likes = likes
              Dislikes = dislikes }

        // Annotation both required: since `Patron` and `PatronView` have the same fields, and `PatronView` comes later, unless `patron` is annotated, the compiler will assign its type as `PatronView` which will result in a logic error
        // But, also helpful in distinguishing between the fact input is a `Patron`, the private type, and the output is `PatronView`, the publicly accessible type
        let value (patron: Patron) : PatronView =
            { Id = patron.Id
              Name = patron.Name
              Age = patron.Age
              Height = patron.Height
              RewardPoints = patron.RewardPoints
              TicketTier = patron.TicketTier
              FreePasses = List.map FreePass.value patron.FreePasses
              Likes = patron.Likes
              Dislikes = patron.Dislikes }
