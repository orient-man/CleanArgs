module Playground

// pattern matching + recursion
let rec sumList list =
    match list with
    | [] -> 0
    | n::rest -> n + sumList rest

// generalized function
let rec foldr f x = function
    | [] -> x
    | n::rest -> f n (foldr f x rest)

let sum = foldr (+) 0
let product = foldr (*) 1
let anytrue = foldr (||) false
let alltrue = foldr (&&) true
let cons x y = x::y
let copy list = foldr cons [] list
let append a b = foldr cons b a
let count _ n = n + 1
let length list = foldr count 0 list
// using partial application
let double = (*) 2
let half x = x / 2 
// gluing functions
//let fandcons f = f >> cons
//let doubleandcons = fandcons double
//let doubleall = foldr doubleandcons []
//let doubleall = foldr (double >> cons) []
let map f = foldr (f >> cons) []
let doubleall = map double
let summatrix = map sum >> sum

type Tree<'a> = | Node of 'a * Tree<'a> list

let rec foldtree f g a tree =
    let rec foldchildren f g a = function
        | [] -> a
        | tree::rest -> g (foldtree f g a tree) (foldchildren f g a rest)

    match tree with
    | Node(label, subtrees) -> f label (foldchildren f g a subtrees)

let sumtree = foldtree (+) (+) 0
let labels tree = foldtree cons append [] tree
let node x y = Node(x, y)
let maptree f = foldtree (f >> node) cons []
let (.+.) g f = f >> g