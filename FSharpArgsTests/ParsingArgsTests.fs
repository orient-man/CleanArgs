module ParsingArgsTests

open Args
open Rop
open NUnit.Framework
open Swensen.Unquote

[<Test>]
let ``Bool argument``() =
    let actual = parseArgs "x" ["-x"]
    let expected = Success(Map.empty.Add('x', BoolValue true))
    test <@ expected = actual @>

[<Test>]
let ``Multiple bool arguments``() =
    let actual = parseArgs "x,y" ["-x"; "-y"]
    let expected = Success(Map.empty.Add('x', BoolValue true).Add('y', BoolValue true))
    test <@ expected = actual @>

[<Test>]
let ``Empty args list``() =
    let actual = parseArgs "" []
    let expected : ParsingResult = Success(Map.empty)
    test <@ expected = actual @>

[<Test>]
let ``Skips non args``() =
    let actual = parseArgs "x" ["not_an_arg"; "-x"]
    let expected = Success(Map.empty.Add('x', BoolValue true))
    test <@ expected = actual @>

[<Test>]
let ``String argument``() =
    let actual = parseArgs "x*" ["-x"; "string"]
    let expected = Success(Map.empty.Add('x', StringValue "string"))
    test <@ expected = actual @>

[<Test>]
let ``Missing string argument``() =
    let actual = parseArgs "x*" ["-x"]
    let expected : ParsingResult = Failure(MissingString 'x')
    test <@ expected = actual @>

[<Test>]
let ``String list argument``() =
    let actual = parseArgs "x**" ["-x"; "a"; "b"]
    let expected = Success(Map.empty.Add('x', StringListValue ["a"; "b"]))
    test <@ expected = actual @>

[<Test>]
let ``Empty string list argument``() =
    let actual = parseArgs "x**" ["-x"]
    let expected = Success(Map.empty.Add('x', StringListValue []))
    test <@ expected = actual @>

[<Test>]
let ``String list argument with following argument``() =
    let actual = parseArgs "x**,y#" ["-x"; "a"; "-y"; "5"]
    let expected = Success(Map.empty.Add('x', StringListValue ["a"]).Add('y', IntValue 5))
    test <@ expected = actual @>

[<Test>]
let ``Int argument``() =
    let actual = parseArgs "x#" ["-x"; "15"]
    let expected = Success(Map.empty.Add('x', IntValue 15))
    test <@ expected = actual @>

[<Test>]
let ``Missing int argument``() =
    let actual = parseArgs "x#" ["-x"]
    let expected : ParsingResult = Failure(MissingInt 'x')
    test <@ expected = actual @>

[<Test>]
let ``Invalid int argument``() =
    let actual = parseArgs "x#" ["-x"; "wrong"]
    let expected : ParsingResult = Failure(InvalidInt('x', "wrong"))
    test <@ expected = actual @>

[<Test>]
let ``Double argument``() =
    let actual = parseArgs "x##" ["-x"; "3.14"]
    let expected = Success(Map.empty.Add('x', DoubleValue 3.14))
    test <@ expected = actual @>

[<Test>]
let ``Missing double argument``() =
    let actual = parseArgs "x##" ["-x"]
    let expected : ParsingResult = Failure(MissingDouble 'x')
    test <@ expected = actual @>

[<Test>]
let ``Invalid double argument``() =
    let actual = parseArgs "x##" ["-x"; "wrong"]
    let expected : ParsingResult = Failure(InvalidDouble('x', "wrong"))
    test <@ expected = actual @>
