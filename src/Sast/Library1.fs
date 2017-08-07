namespace GenerativeTypeProviderExample

// Outside namespaces and modules
open FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations
open ProviderImplementation.ProvidedTypes // open the providedtypes.fs file
open System.Reflection // necessary if we want to use the f# assembly
open System.IO
open FSharp.Data
// ScribbleProvider specific namespaces and modules
open GenerativeTypeProviderExample.TypeGeneration
open System.Text.RegularExpressions


[<TypeProvider>]
type GenerativeTypeProvider(config : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces ()

    let tmpAsm = Assembly.LoadFrom(config.RuntimeAssembly)

    let generateTypes (name:string) (parameters:obj[]) = 

        let someInteger = parameters.[0]  :?> int

        let ty = name |> createProvidedType tmpAsm
                      |> addCstor ( <@@ () @@> |> createCstor [])
            
        let staticParams = [ProvidedStaticParameter("Count", typeof<int>)]
        let exampleMethWithStaticParams =  
            let m = ProvidedMethod("ExampleMethWithStaticParam", [ ], typeof<int>, IsStaticMethod = false)
            m.DefineStaticParameters(staticParams, (fun nm args ->
                let arg = args.[0] :?> int
                let m2 = 
                    ProvidedMethod(nm, [] , typeof<int>, IsStaticMethod = false,
                                    InvokeCode = fun args -> <@@ arg + someInteger @@>)
                ty.AddMember m2
                m2))
            m

        let ty = ty |> addMethod exampleMethWithStaticParams

        ty.SetAttributes(TypeAttributes.Public ||| TypeAttributes.Class)
        ty.HideObjectMethods <- true

        // If Uncommented the 3 next lines seem to create a compilation error :
        // no invoker for ExampleMethWithStaticParam on type GenerativeTypeProviderExample.Provided.Example
        // Comment the 3 following lines to have a TP compiling but throwing a runtime error 
        // "internal error: null: convTypeRefAux"
        let assemblyPath = Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".dll")
        let assembly = ProvidedAssembly assemblyPath
        assembly.AddTypes [ty]
        
        ty

           
    let providedType = TypeGeneration.createProvidedType tmpAsm "TypeProvider"
    
    let parameters   = [ProvidedStaticParameter("SomeInteger",typeof<int>)]

    do 
        providedType.DefineStaticParameters(parameters,generateTypes)
        
        this.AddNamespace(ns, [providedType])

[<assembly:TypeProviderAssembly>]
    do()