module FSharpArgsTests

open System
open FsUnit
open NUnit.Framework
open Rop
open Swensen.Unquote

type SchemaElement = | Bool
type SchemaInfo = (char * SchemaElement) list
type ErrorCode = | InvalidArgumentName of char
type SchemeParsingResult = Result<SchemaInfo, ErrorCode>

let rec parseSchemaElements schema = function
    | [] -> Success(schema)
    | (name, param)::tail when Char.IsLetter name ->
        parseSchemaElements ((name, param)::schema) tail
    | (name, _)::_ -> Failure(InvalidArgumentName name)

let parseSchema (schema : string) : SchemeParsingResult =
    schema.Split ','
    |> List.ofArray
    |> List.map (fun s -> s.Trim())
    |> List.filter (fun s -> s.Length > 0)
    |> List.map (fun s -> (s.[0], s.Substring(1)))
    |> parseSchemaElements []
    |> Rop.map (List.map (fun (name, _) -> (name, Bool)))

type ArgValue = | Flag of bool

let parse (schema : string) args =
    schema.Split ','
    |> Seq.ofArray
    |> Seq.filter (fun s -> s.Length > 0)
    |> Seq.map (fun s -> (s.[0], Flag true))
    |> List.ofSeq

[<Test>]
let ``Parses empty schema``() =
    let actual = parseSchema ""
    let expected : SchemeParsingResult = Success []
    test <@ expected = actual @>

[<Test>]
let ``Parses simple bool schema``() =
    let actual = parseSchema "x"
    let expected : SchemeParsingResult = Success [('x', Bool)]
    test <@ expected = actual @>

[<Test>]
let ``Invalid argument name``() =
    let actual = parseSchema "1"
    let expected : SchemeParsingResult = Failure (InvalidArgumentName '1')
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
