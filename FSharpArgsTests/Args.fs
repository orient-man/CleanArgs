module Args

open System
open System.Globalization
open Rop

type SchemaElement = Bool | String | StringList | Int | Double
type SchemaInfo = Map<char, SchemaElement>
type ErrorCode =
    | InvalidArgumentName of char
    | InvalidArgumentFormat of char * string
    | MissingString of char
    | MissingInt of char
    | InvalidInt of char * string
    | MissingDouble of char
    | InvalidDouble of char * string
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

type ArgValue =
    | BoolValue of bool
    | StringValue of string
    | StringListValue of string list
    | IntValue of int
    | DoubleValue of double
type ParsingResult = Result<Map<char, ArgValue>, ErrorCode>
type MarshallingResult = Result<((char * ArgValue) * string list), ErrorCode>
type Marshaller = char -> string list -> MarshallingResult

let (|ValidArgument|_|) arg =
    match List.ofSeq arg with | '-'::c::_ -> Some c | _ -> None

let BoolMarshaller arg tail = Success((arg, BoolValue true), tail)

let StringMarshaller arg = function
    | value::tail -> Success((arg, StringValue value), tail)
    | _ -> Failure(MissingString arg)

let StringListMarshaller arg tail : MarshallingResult =
    let rec collectValues acc args =
        match args with
        | ValidArgument _::_ -> (acc, args)
        | value::tail -> collectValues (value::acc) tail
        | _ -> (acc, [])
    let value, tail = collectValues [] tail
    Success((arg, StringListValue (List.rev value)), tail)

let IntMarshaller arg = function
    | value::tail ->
        match Int32.TryParse value with
        | (true, value) -> Success((arg, IntValue value), tail)
        | _ -> Failure(InvalidInt(arg, value))
    | _ -> Failure(MissingInt arg)

let DoubleMarshaller arg = function
    | value::tail ->
        match Double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture) with
        | (true, value) -> Success((arg, DoubleValue value), tail)
        | _ -> Failure(InvalidDouble(arg, value))
    | _ -> Failure(MissingDouble arg)

let getMarshaller elem : Marshaller =
    match elem with
    | Bool -> BoolMarshaller
    | String -> StringMarshaller
    | StringList -> StringListMarshaller
    | Int -> IntMarshaller
    | Double -> DoubleMarshaller

let parseArgument (schema : SchemaInfo) = function
    | ValidArgument c::args -> args |> getMarshaller schema.[c] c |> map Some
    | args -> Success None

let parseArgs (schema : string) args : ParsingResult =
    let rec parse (schema : SchemaInfo) (values, args) =
        let append = function
            | Some(value, args) -> (value::values, args)
            | None -> (values, args |> List.tail)

        match args with
        | [] -> Success values
        | _ -> parseArgument schema args |> map append >>= (parse schema)

    parseSchema schema >>= (fun schema -> parse schema ([], args)) |> map Map.ofList