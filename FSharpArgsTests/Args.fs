module FSharpArgsTests

open System
open Rop
open FsUnit
open NUnit.Framework
open Swensen.Unquote

type SchemaElement = | Bool
type SchemaInfo = (char * SchemaElement) list
type ErrorCode =
    | InvalidArgumentName of char
    | InvalidArgumentFormat of char * string
type SchemeParsingResult = Result<SchemaInfo, ErrorCode>

let parseElement = function
    | (arg, _) when not(Char.IsLetter arg) -> Failure(InvalidArgumentName arg)
    | (arg, format) when format = "" -> Success(arg, Bool)
    | (arg, format) -> Failure(InvalidArgumentFormat(arg, format))

let rec parseSchemaElements schema = function
    | [] -> Success(schema)
    | e::tail -> e |> parseElement >>= (fun e -> parseSchemaElements (e::schema) tail)

let parseSchema (schema : string) : SchemeParsingResult =
    schema.Split ','
    |> List.ofArray
    |> List.map (fun s -> s.Trim())
    |> List.filter (fun s -> s.Length > 0)
    |> List.map (fun s -> (s.[0], s.Substring(1)))
    |> parseSchemaElements []

type ArgValue = | Flag of bool

let parse (schema : string) args =
    schema.Split ','
    |> Seq.ofArray
    |> Seq.filter (fun s -> s.Length > 0)
    |> Seq.map (fun s -> (s.[0], Flag true))
    |> List.ofSeq

[<Test>]
let ``Empty schema is valid``() =
    let actual = parseSchema ""
    let expected : SchemeParsingResult = Success []
    test <@ expected = actual @>

[<Test>]
let ``One argument Bool schema``() =
    let actual = parseSchema "x"
    let expected : SchemeParsingResult = Success [('x', Bool)]
    test <@ expected = actual @>

[<Test>]
let ``Non letter schema is invalid``() =
    let actual = parseSchema "*"
    let expected : SchemeParsingResult = Failure(InvalidArgumentName '*')
    test <@ expected = actual @>

[<Test>]
let ``Invalid argument format``() =
    let actual = parseSchema "f~"
    let expected : SchemeParsingResult = Failure(InvalidArgumentFormat ('f', "~"))
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
