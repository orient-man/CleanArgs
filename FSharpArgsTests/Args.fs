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
type ParsingResult = Result<Map<char, ArgValue>, ErrorCode>

let BoolMarshaller c args = Success((c, BoolValue true), args)

let getMarshaller = function
    | Bool -> BoolMarshaller
    | _ -> failwith "Not implemented"

let (|ValidArgument|_|) arg =
    match List.ofSeq arg with | '-'::c::_ -> Some c | _ -> None

let parseArgument (schema : SchemaInfo) = function
    | ValidArgument c::args -> args |> getMarshaller schema.[c] c |> map Some
    | args -> Success None

let parseArgs (schema : String) args : ParsingResult =
    let rec parse (schema : SchemaInfo) (values, args) =
        let append = function
            | Some(value, args) -> (value::values, args)
            | None -> (values, args |> List.tail)

        match args with
        | [] -> Success values
        | _ -> parseArgument schema args |> map append >>= (parse schema)

    parseSchema schema >>= (fun schema -> parse schema ([], args)) |> map Map.ofList