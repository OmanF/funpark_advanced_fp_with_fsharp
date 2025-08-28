namespace FunPark

open System
open Shared
open FunPark.FreePasses

module Patrons =
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

    module Patron =
        type PatronConstructor =
            { Name: string
              Age: PositiveNonZeroInt<yr> option
              Height: PositiveNonZeroInt<cm> option
              RewardPoints: int<rp>
              TicketTier: TicketTier
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
            =
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
              TicketTier = ticketTier
              FreePasses = freePasses
              Likes = likes
              Dislikes = dislikes }
