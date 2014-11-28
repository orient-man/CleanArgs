module Args

open System
open System.Globalization
open Rop

type ErrorCode =
    | InvalidArgumentName of char
    | InvalidArgumentFormat of char * string
    | MissingString of char
    | MissingInt of char
    | InvalidInt of char * string
    | MissingDouble of char
    | InvalidDouble of char * string

// Parsing schema
type SchemaElement = Bool | String | StringList | Int | Double
type SchemaInfo = Map<char, SchemaElement>
type SchemaParsingResult = Result<SchemaInfo, ErrorCode>

let parseElement = function
    | (arg, _) when not(Char.IsLetter arg) -> Failure(InvalidArgumentName arg)
    | (arg, "") -> Success(arg, Bool)
    | (arg, "*") -> Success(arg, String)
    | (arg, "**") -> Success(arg, StringList)
    | (arg, "#") -> Success(arg, Int)
    | (arg, "##") -> Success(arg, Double)
    | elem -> Failure(InvalidArgumentFormat elem)

let rec parseSchemaElements acc = function
    | [] -> Success(acc)
    | e::tail -> parseElement e >>= (fun e -> parseSchemaElements (e::acc) tail)

let parseSchema (schema : string) : SchemaParsingResult =
    schema.Split ','
    |> Seq.ofArray
    |> Seq.map (fun s -> s.Trim())
    |> Seq.filter (fun s -> s.Length > 0)
    |> Seq.map (fun s -> (s.[0], s.Substring(1)))
    |> List.ofSeq
    |> parseSchemaElements []
    |> map Map.ofList

// Marshallers
type ArgValue =
    | BoolValue of bool
    | StringValue of string
    | StringListValue of string list
    | IntValue of int
    | DoubleValue of double
type MarshallingResult = Result<((char * ArgValue) * string list), ErrorCode>
type Marshaller = char -> string list -> MarshallingResult

let (|ValidArgument|_|) arg =
    match List.ofSeq arg with | '-'::c::[] -> Some c | _ -> None

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

type SchemaElement with
    member elem.parse : Marshaller =
        match elem with
        | Bool -> BoolMarshaller
        | String -> StringMarshaller
        | StringList -> StringListMarshaller
        | Int -> IntMarshaller
        | Double -> DoubleMarshaller

// Parsing arguments
type ParsingResult = Result<Map<char, ArgValue>, ErrorCode>

let parseArgs (schema : string) args : ParsingResult =
    let rec parse (schema : SchemaInfo) acc = function
        | [] -> Success(acc |> Map.ofList)
        | ValidArgument arg::tail when schema.ContainsKey arg ->
            let parseTail (value, args) = parse schema (value::acc) tail
            schema.[arg].parse arg tail >>= parseTail
        | _::tail -> parse schema acc tail

    parseSchema schema >>= fun schema -> parse schema [] args