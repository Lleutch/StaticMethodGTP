module GenerativeTypeProviderExample.TypeGeneration

// Outside namespaces and modules
open Microsoft.FSharp.Quotations
open ProviderImplementation.ProvidedTypes // open the providedtypes.fs file
open System.Reflection // necessary if we want to use the f# assembly
open System.Threading.Tasks

// TYPE PROVIDER'S ASSEMBLY (for generative type provider) + NAMESPACE + BASETYPE 
let internal ns = "GenerativeTypeProviderExample.Provided"
//let asm = ProvidedAssembly(Path.ChangeExtension(Path.GetTempFileName(), ".dll"))
let baseType = typeof<obj>

(******************* TYPE PROVIDER'S HELPERS *******************)

// CREATING TYPES, NESTED TYPES, METHODS, PROPERTIES, CONSTRUCTORS
let internal createProvidedType assembly name = 
    ProvidedTypeDefinition(assembly, ns, name, Some baseType, IsErased=false)

let internal createProvidedIncludedType name = 
    ProvidedTypeDefinition(name,Some baseType, IsErased=false)

let internal createProvidedIncludedTypeChoice typing name =
    ProvidedTypeDefinition(name, Some typing , IsErased=false)

let internal createMethodType name param typing expression =
    ProvidedMethod( name, param, typing, InvokeCode = (fun args -> expression ))

let internal createPropertyType name typing expression =
    ProvidedProperty( name , typing , IsStatic = true, GetterCode = (fun args -> expression ))

let internal createCstor param expression = 
    ProvidedConstructor( parameters = param, InvokeCode = (fun args -> expression ))


// ADDING TYPES, NESTED TYPES, METHODS, PROPERTIES, CONSTRUCTORS TO THE ASSEMBLY AND AS MEMBERS OF THE TYPE PROVIDER
(*let internal addProvidedTypeToAssembly (providedType:ProvidedTypeDefinition)=
    asm.AddTypes([providedType])
    providedType*)

let internal addIncludedTypeToProvidedType nestedTypeToAdd (providedType:ProvidedTypeDefinition) =
    providedType.AddMembers(nestedTypeToAdd)
    providedType

let internal addMethod methodType (providedType:ProvidedTypeDefinition) = 
    providedType.AddMember methodType
    providedType    

let internal addProperty propertyToAdd (providedType:ProvidedTypeDefinition) =
    providedType.AddMember(propertyToAdd)
    providedType

let internal addCstor cstorToAdd (providedType:ProvidedTypeDefinition) =
    providedType.AddMember(cstorToAdd)
    providedType

let internal addMember (memberInfo:#MemberInfo) (providedType:ProvidedTypeDefinition) = 
    providedType.AddMember(memberInfo)
    providedType

let internal addMembers (membersInfo:#MemberInfo list) (providedType:ProvidedTypeDefinition) = 
    providedType.AddMembers(membersInfo)
    providedType