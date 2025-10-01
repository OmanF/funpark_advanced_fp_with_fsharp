open Expecto
open FunParkExpecto.Tests

[<EntryPoint>]
let main args =
    let tests = testList "All FunPark example-based tests" [ freePassTests; ridesTests ]
    runTestsWithCLIArgs [] args tests
