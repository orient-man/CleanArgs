#load "Rop.fs"
#load "Args.fs"
open Args

parseElement ('a', "");;
parseElement ('b', "#");;
parseElement ('c', "%");;
parseElement ('$', "#");;
