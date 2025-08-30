namespace FunPark

open System
open Rides

module FreePasses =
    type FreePass =
        private
            { Id: Guid
              Ride: Ride
              ValidFrom: DateTime }

    // Public view type for FreePass, exposes all fields for dot-access
    type FreePassView =
        { Id: Guid
          Ride: RideView
          ValidFrom: DateTime }

    module FreePass =
        // Utility type for constructing a FreePass: allows for named parameters to be used as input for the constructor
        // Either alternative, tuple or curried parameters, require positional arguments, with no "nametag", making it harder to understand the purpose of each argument, and their correct order
        type FreePassConstructor = { Ride: Ride; ValidFrom: DateTime }

        // Annotating the function output's type as `FreePass`, the private type, is required since `FreePass` and `FreePassView` have the same fields, and `FreePassView` comes later, unless the function is annotated it will be assigned the wrong type, the public `FreePassView` type
        // The input's type is inferred as `FreePassConstructor` since it's missing the `Id` field
        let create { Ride = ride; ValidFrom = validFrom } : FreePass =
            { Id = Guid.NewGuid()
              Ride = ride
              ValidFrom =
                match abs (validFrom - DateTime.UtcNow).TotalMinutes < 1.0 with
                | true -> validFrom
                | false ->
                    raise
                    <| ArgumentException
                        "Invalid FreePass validation date: must be within one minute of the current time" }

        // Annotation both required: since `FreePass` and `FreePassView` have the same fields, and `FreePassView` comes later, unless `freePass` is annotated, the compiler will assign its type as `FreePassView` which will result in a logic error
        // But, also helpful in distinguishing between the fact input is a `FreePass`, the private type, and the output is `FreePassView`, the publicly accessible type
        let value (freePass: FreePass) : FreePassView =
            { Id = freePass.Id
              Ride = Ride.value freePass.Ride
              ValidFrom = freePass.ValidFrom }
