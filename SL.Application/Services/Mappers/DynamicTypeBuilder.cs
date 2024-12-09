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
            if (typeDef == null || typeDef.Properties == null || !typeDef.Properties.Any())
                throw new ArgumentException("Invalid TypeDefinition: Properties must not be null or empty.");

            var assemblyName = new AssemblyName("DynamicTypes");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            var typeBuilder = moduleBuilder.DefineType(typeDef.Name, TypeAttributes.Public | TypeAttributes.Class);

            foreach (var property in typeDef.Properties)
                CreateProperty(typeBuilder, property.Key, property.Value);

            return typeBuilder.CreateType();
        }

        private static void CreateProperty(TypeBuilder typeBuilder, string propertyName, string propertyType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrWhiteSpace(propertyType))
                    throw new ArgumentException("Property name and type must not be null or empty.");

                var type = Type.GetType($"System.{propertyType}", throwOnError: true, ignoreCase: true);
                AddGetterSetter(typeBuilder, propertyName, type);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error creating property '{propertyName}'", ex);
            }
        }

        private static void AddGetterSetter(TypeBuilder typeBuilder, string propertyName, Type propertyType)
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
    }
}
