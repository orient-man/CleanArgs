module Args

open System
open Rop

type SchemaElement = | Bool | String | StringList | Int | Double
type SchemaInfo = Map<char, SchemaElement>
type ErrorCode =
    | InvalidArgumentName of char
    | InvalidArgumentFormat of char * string
type SchemeParsingResult = Result<SchemaInfo, ErrorCode>

let (|SupportedFormat|_|) = function
    | "" -> Some Bool
    | "*" -> Some String
    | "**" -> Some StringList
    | "#" -> Some Int
    | "##" -> Some Double
    | _ -> None

let parseElement = function
    | (arg, _) when not(Char.IsLetter arg) -> Failure(InvalidArgumentName arg)
    | (arg, SupportedFormat format) -> Success(arg, format)
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
    |> map Map.ofList

type ArgValue = | BoolValue of bool

let getMarshaller = function
    | Bool -> (fun c args -> Success((c, BoolValue true), args))
    | _ -> failwith "Not implemented"

let (|ValidArgument|_|) arg =
    match List.ofSeq arg with | '-'::c::_ -> Some c | _ -> None

let parseArgument (schema : SchemaInfo) = function
    | ValidArgument c::args -> args |> getMarshaller schema.[c] c |> map Some
    | args -> Success(None)

let rec readValues schema (values, args) =
    let append = function
        | Some(value, args) -> (value::values, args)
        | None -> (values, args |> List.tail)

    match args with
    | [] -> Success(values)
    | _ -> parseArgument schema args |> map append >>= (readValues schema)

let parseArgs schema args =
    schema
    |> parseSchema
    >>= (fun schema -> readValues schema ([], args))
    |> map Map.ofList