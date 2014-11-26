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
    |> Rop.map Map.ofList

type Marshaller<'a> = string list -> Result<'a * string list, ErrorCode>

let getMarshaller = function
    | Bool -> (fun c args -> Success((c, box true), args))
    | _ -> failwith "Not implemented"

type ArgValue = | Flag of bool

let parse (schema : string) args =
    schema.Split ','
    |> Seq.ofArray
    |> Seq.filter (fun s -> s.Length > 0)
    |> Seq.map (fun s -> (s.[0], Flag true))
    |> List.ofSeq

let (|ValidArgument|_|) arg =
    match List.ofSeq arg with | '-'::c::_ -> Some(c) | _ -> None

let parseArgument (schema : SchemaInfo) = function
    | ValidArgument c::args ->
        args |> getMarshaller schema.[c] c |> Rop.map (fun (v, args) -> Some(v, args))
    | args -> Success(None)

let rec readValues schema (values, args) =
    let append = function
        | Some(value, args) -> (value::values, args)
        | None -> (values, args |> List.tail)

    match args with
    | [] -> Success(values)
    | _ -> parseArgument schema args |> Rop.map append >>= (readValues schema)

let parseArgs schema args =
    schema
    |> parseSchema
    >>= (fun schema -> readValues schema ([], args))
    |> map Map.ofList