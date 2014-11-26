module ParsingSchemaTests

open Args
open Rop
open NUnit.Framework
open Swensen.Unquote

[<Test>]
let ``Empty schema is valid``() =
    let actual = parseSchema ""
    let expected : SchemeParsingResult = Success Map.empty
    test <@ expected = actual @>

[<Test>]
let ``Bool argument type definition``() =
    let actual = parseSchema "x"
    let expected = Success(Map.empty.Add('x', Bool))
    test <@ expected = actual @>

[<Test>]
let ``Non letter schema is invalid``() =
    let actual = parseSchema "*"
    let expected : SchemeParsingResult = Failure(InvalidArgumentName '*')
    test <@ expected = actual @>

[<Test>]
let ``Invalid argument format``() =
    let actual = parseSchema "f~"
    let expected : SchemeParsingResult = Failure(InvalidArgumentFormat('f', "~"))
    test <@ expected = actual @>

[<Test>]
let ``String argument type definition``() =
    let actual = parseSchema "x*"
    let expected = Success(Map.empty.Add('x', String))
    test <@ expected = actual @>

[<Test>]
let ``StringList argument type definition``() =
    let actual = parseSchema "x**"
    let expected = Success(Map.empty.Add('x', StringList))
    test <@ expected = actual @>

[<Test>]
let ``Int argument type definition``() =
    let actual = parseSchema "x#"
    let expected = Success(Map.empty.Add('x', Int))
    test <@ expected = actual @>

[<Test>]
let ``Double argument type definition``() =
    let actual = parseSchema "x##"
    let expected = Success(Map.empty.Add('x', Double))
    test <@ expected = actual @>

[<Test>]
let ``Multiple arguments schema``() =
    let actual = parseSchema "x*,y"
    let expected = Success(Map.empty.Add('x', String).Add('y', Bool))
    test <@ expected = actual @>

[<Test>]
let ``Spaces in format``() =
    let actual = parseSchema "x*, y"
    let expected = Success(Map.empty.Add('x', String).Add('y', Bool))
    test <@ expected = actual @>
