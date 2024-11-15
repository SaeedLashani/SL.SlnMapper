using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using SL.Domain.Models;
using log4net;


namespace SL.Application.Services.Mappers
{
    public class DynamicTypeBuilder
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DynamicTypeBuilder));
        public static Type CreateType(TypeDefinition typeDef)
        {
            try
            {
                var assemblyName = new AssemblyName("DynamicTypes");
                var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
                var typeBuilder = moduleBuilder.DefineType(typeDef.Name, TypeAttributes.Public | TypeAttributes.Class);

                if (typeDef?.Properties is null)
                    throw new ArgumentException("Invalid TypeDefinition: Properties cannot be null.");

                foreach (var property in typeDef.Properties)
                {
                    try
                    {
                        Type propertyType = Type.GetType("System." + property.Value, true, true);
                        CreateProperty(typeBuilder, property.Key, propertyType);
                    }
                    catch (TypeLoadException ex)
                    {
                        // Handle specific exception if type cannot be found
                        throw new InvalidOperationException($"Failed to create type '{typeDef.Name}': Invalid type specified for property '{property.Key}'");
                    }
                    catch (Exception ex)
                    {
                        // General catch for any other unexpected exceptions
                        throw new InvalidOperationException($"Failed to create type '{typeDef.Name}' due to an error in property '{property.Key}'", ex);
                    }
                }

                return typeBuilder.CreateType();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            try
            {
                var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

                var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

                var getterMethod = typeBuilder.DefineMethod("get_" + propertyName,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    propertyType, Type.EmptyTypes);

                var getterIL = getterMethod.GetILGenerator();
                getterIL.Emit(OpCodes.Ldarg_0);
                getterIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getterIL.Emit(OpCodes.Ret);

                var setterMethod = typeBuilder.DefineMethod("set_" + propertyName,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    null, new Type[] { propertyType });

                var setterIL = setterMethod.GetILGenerator();
                setterIL.Emit(OpCodes.Ldarg_0);
                setterIL.Emit(OpCodes.Ldarg_1);
                setterIL.Emit(OpCodes.Stfld, fieldBuilder);
                setterIL.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getterMethod);
                propertyBuilder.SetSetMethod(setterMethod);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
