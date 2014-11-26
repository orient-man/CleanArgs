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
    | Bool -> (fun args -> Success(true, args))
    | _ -> failwith "Not implemented"

type ArgValue = | Flag of bool

let parse (schema : string) args =
    schema.Split ','
    |> Seq.ofArray
    |> Seq.filter (fun s -> s.Length > 0)
    |> Seq.map (fun s -> (s.[0], Flag true))
    |> List.ofSeq