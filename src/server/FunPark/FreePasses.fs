namespace FunPark

open System
open Shared
open Rides

module FreePasses =
    [<CustomEquality; CustomComparison>]
    type FreePass =
        private
            { Id: Guid
              Ride: Ride
              ValidFrom: DateTime }

        override this.Equals otherFreePass =
            match otherFreePass with
            // Equality is based on both `Ride` and `ValidFrom` since `FreePass` is tied, defined actually, by both
            | :? FreePass as other -> this.Ride = other.Ride && this.ValidFrom = other.ValidFrom
            | _ -> false

        override this.GetHashCode() =
            HashCode.Combine(hash this.Ride, hash this.ValidFrom)

        interface IComparable with
            member this.CompareTo otherFreePass =
                match otherFreePass with
                // Ordering over `ValidFrom` only, but can only compare `FreePasses` for the same `Ride`
                | :? FreePass as other ->
                    if this.Ride <> other.Ride then
                        invalidArg "otherFreePass" "Can't compare FreePasses for different Rides"
                    else
                        this.ValidFrom.CompareTo other.ValidFrom
                | _ -> invalidArg "otherFreePass" "Can't compare values of different types"

    // Public view type for `FreePass`, exposes all fields for dot-access
    type FreePassView =
        { Id: Guid
          Ride: RideView
          ValidFrom: DateTime }

    module FreePass =
        // Utility type for constructing a `FreePass`: allows for named parameters to be used as input for the constructor
        // Either alternative, tuple or curried parameters, require positional arguments, with no tags, making it harder to understand the purpose of each argument, and their correct order
        type FreePassConstructor =
            { Ride: Ride
              ValidFrom: ValidFreePassStartDate option }

        // Auxiliary type for the following `update` function, because I dislike passing anonymous records as parameters
        type FreePassUpdate =
            { Id: Guid option
              Ride: Ride option
              ValidFrom: ValidFreePassStartDate option }

        // Annotating the function output's type as `FreePass`, the private type, is required since `FreePass` and `FreePassView` have the same fields, and `FreePassView` comes later, unless the function is annotated it will be assigned the wrong type, the public `FreePassView` type
        // The input's type is inferred as `FreePassConstructor` since it's missing the `Id` field
        let create { Ride = ride; ValidFrom = validFrom } : FreePass =
            { Id = Guid.NewGuid()
              Ride = ride
              // Prior to F#6+ could only be written as:
              // ValidFrom = defaultArg (Option.map (fun (d: ValidFreePassStartDate) -> d.Value) validFrom) DateTime.UtcNow
              // But now can be simplified to:
              ValidFrom =
                validFrom
                |> Option.map (fun d -> d.Value)
                |> Option.defaultValue DateTime.UtcNow }

        // Annotation both required: since `FreePass` and `FreePassView` have the same fields, and `FreePassView` comes later, unless `freePass` is annotated, the compiler will assign its type as `FreePassView` which will result in a logic error
        // But, also helpful in distinguishing between the fact input is a `FreePass`, the private type, and the output is `FreePassView`, the publicly accessible type
        let value (freePass: FreePass) : FreePassView =
            { Id = freePass.Id
              Ride = Ride.value freePass.Ride
              ValidFrom = freePass.ValidFrom }

        // Annotation NOT required, `FreePassUpdate` differs from `FreePassConstructor` by the `Id` field, but as always, explicit is better in a complex domain
        // `FreePassUpdate` required since, as with the tests, `FreePass` is a private type and we can't **directly** assign it, or its members, to any constructor function, only a public clone of it
        // Is it likely, not to mention smart, that a `FreePass`'s `Id` would change? No, but an `update` function should be able to update any field, including the `Id`, even if it's not a common operation
        let update (freePass: FreePass) (update: FreePassUpdate) =
            { freePass with
                Id = update.Id |> Option.defaultValue freePass.Id // Compare with updating the `Ride` field... both approaches are the same, current one is starting at F#6
                Ride = defaultArg update.Ride freePass.Ride
                ValidFrom =
                    update.ValidFrom
                    |> Option.map (fun d -> d.Value)
                    |> Option.defaultValue freePass.ValidFrom }
