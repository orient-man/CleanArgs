#load "Rop.fs"
#load "Args.fs"
open Args

parseElement ('a', "");;
parseElement ('b', "#");;
parseElement ('c', "%");;
parseElement ('$', "#");;

StringMarshaller 'x' ["a"];;
StringListMarshaller 'x' ["a"; "b"];;