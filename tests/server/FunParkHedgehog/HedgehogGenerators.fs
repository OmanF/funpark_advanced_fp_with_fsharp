namespace FunParkHedgehog.Tests

open Bogus
open System
open Hedgehog

[<AutoOpen>]
module HedgehogGenerators =
    let private fakerFromSeed seed =
        let faker = new Faker("en") // Default setting for Bogus is English, this is just a nicety to make it explicit
        faker.Random <- new Randomizer(seed)
        faker

    let genBogus (f: Faker -> 'T) : Gen<'T> =
        Gen.int32 (Range.linear Int32.MinValue Int32.MaxValue)
        |> Gen.map (fun seed -> f (fakerFromSeed seed))

    let genBogusSized (f: int -> Faker -> 'T) : Gen<'T> =
        Gen.sized (fun size ->
            Gen.int32 (Range.linear Int32.MinValue Int32.MaxValue)
            |> Gen.map (fun seed -> f size (fakerFromSeed seed)))
