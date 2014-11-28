#load "Rop.fs"
#load "Args.fs"
open Args

parseElement ('a', "");;
parseElement ('b', "#");;
parseElement ('c', "%");;
parseElement ('$', "#");;
parseSchema "l,p#,x*"
BoolMarshaller 'x' ["-y"];;
StringMarshaller 'x' ["a"; "-y"];;
StringListMarshaller 'x' ["a"; "b"];;