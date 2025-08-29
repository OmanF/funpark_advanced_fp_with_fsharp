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
        type FreePassConstructor = { Ride: Ride; ValidFrom: DateTime }

        let create { Ride = ride; ValidFrom = validFrom } : FreePass =
            { Id = Guid.NewGuid()
              Ride = ride
              ValidFrom = validFrom }

        let value (freePass: FreePass) : FreePassView =
            { Id = freePass.Id
              Ride = Ride.value freePass.Ride
              ValidFrom = freePass.ValidFrom }
