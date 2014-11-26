module ParsingArgsTests

open Args
open Rop
open NUnit.Framework
open Swensen.Unquote

[<Test>]
let ``Simple Bool argument``() =
    let actual = parseArgs "x" ["-x"]
    let expected = Success(Map.empty.Add('x', BoolValue true))
    test <@ expected = actual @>

[<Test>]
let ``Multiple Bool argument``() =
    let actual = parseArgs "x,y" ["-x"; "-y"]
    let expected = Success(Map.empty.Add('x', BoolValue true).Add('y', BoolValue true))
    test <@ expected = actual @>
