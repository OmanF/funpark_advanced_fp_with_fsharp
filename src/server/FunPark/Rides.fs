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

    type Ride =
        private
            { Id: Guid
              Name: string
              MinAge: int<yr>
              MinHeight: int<cm>
              WaitTime: int<s>
              Online: RideStatus
              Tags: RideTags list }

    // Public view type for Ride, exposes all fields for dot-access
    type RideView =
        { Id: Guid
          Name: string
          MinAge: int<yr>
          MinHeight: int<cm>
          WaitTime: int<s>
          Online: RideStatus
          Tags: RideTags list }

    module Ride =
        // Utility type for constructing a Ride: allows for named parameters to be used as input for the constructor
        // Either alternative, tuple or curried parameters, require positional arguments, with no "nametag", making it harder to understand the purpose of each argument, and their correct order
        type RideConstructor =
            { Name: ContentfulString option
              MinAge: Natural<yr> option
              MinHeight: Natural<cm> option
              WaitTime: Natural<s> option
              Online: RideStatus option
              Tags: RideTags list }

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
              Tags = List.distinct tags }

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
