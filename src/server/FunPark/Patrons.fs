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
        type PatronConstructor =
            { Name: string
              Age: PositiveNonZeroInt<yr> option
              Height: PositiveNonZeroInt<cm> option
              RewardPoints: int<rp>
              TicketTier: TicketTier option
              FreePasses: FreePass list
              Likes: string list
              Dislikes: string list }

        let create
            { Name = name
              Age = minAge
              Height = minHeight
              RewardPoints = rewardPoints
              TicketTier = ticketTier
              FreePasses = freePasses
              Likes = likes
              Dislikes = dislikes }
            : Patron =
            { Id = Guid.NewGuid()
              Name = name
              Age = defaultArg (Option.map PositiveNonZeroInt.value minAge) 30<yr>
              Height = defaultArg (Option.map PositiveNonZeroInt.value minHeight) 190<cm>
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
