#r "../../src/Sast/bin/Debug/Sast.dll"

open GenerativeTypeProviderExample
                        

type Example = Provided.TypeProvider<5> //.TypeProvider<5>

let t = Example()

let res = t.ExampleMethWithStaticParam<4>()

