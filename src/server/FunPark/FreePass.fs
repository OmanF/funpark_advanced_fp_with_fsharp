namespace FunPark

open System
open Rides

module FreePasses =
    type FreePass =
        private
            { Id: Guid
              Ride: Ride
              ValidFrom: DateTime }

    module FreePass =
        type FreePassConstructor = { Ride: Ride; ValidFrom: DateTime }

        let create { Ride = ride; ValidFrom = validFrom } =
            { Id = Guid.NewGuid()
              Ride = ride
              ValidFrom = validFrom }
