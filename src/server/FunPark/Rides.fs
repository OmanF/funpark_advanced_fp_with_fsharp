namespace FunPark

open System
open Shared
open FSharp.Data.UnitSystems.SI.UnitSymbols

module Rides =
    type RideStatus =
        | Online
        | Offline

    type Ride =
        private
            { Id: Guid
              Name: string
              MinAge: int<yr>
              MinHeight: int<cm>
              WaitTime: int<s>
              Online: RideStatus
              Tags: Tags list }

    module Ride =
        type RideConstructor =
            { Name: string
              MinAge: PositiveNonZeroInt<yr> option
              MinHeight: PositiveNonZeroInt<cm> option
              WaitTime: PositiveNonZeroInt<s> option
              Online: RideStatus
              Tags: Tags list }

        let create
            { Name = name
              MinAge = minAge
              MinHeight = minHeight
              WaitTime = waitTime
              Online = online
              Tags = tags }
            =
            { Id = Guid.NewGuid()
              Name = name
              MinAge = defaultArg (Option.map PositiveNonZeroInt.value minAge) 8<yr>
              MinHeight = defaultArg (Option.map PositiveNonZeroInt.value minHeight) 100<cm>
              WaitTime = defaultArg (Option.map PositiveNonZeroInt.value waitTime) 60<s>
              Online = online
              Tags = tags }
