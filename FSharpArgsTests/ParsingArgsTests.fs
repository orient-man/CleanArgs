module ParsingArgsTests

open Args
open Rop
open NUnit.Framework
open FsUnit
open Swensen.Unquote

[<Test>]
let ``Simple Bool arg``() =
    let actual = parseArgs "x" ["-x"]
    let expected = Success(Map.empty.Add('x', box true))
    test <@ expected = actual @>

[<Test>]
let ``When no schema or arguments result is empty``() =
    parse "" [] |> should haveLength 0

[<Test>]
let ``Bool flag``() =
    parse "x" ["x"] |> should equal [('x', Flag true)]

[<Test>]
let ``Multiple bool flags``() =
    parse "x,y" ["x", "y"] |> should equal [('x', Flag true); ('y', Flag true)]
