using H.Necessaire.Models.Branding;
using System.Reflection;

namespace H.Necessaire.Runtime.UI.CLI.BLL
{
    class ColorPaletteFinder
    {
        public OperationResult<ColorPalette> Find(string className, string memberName, string assemblyName = null)
        {
            if (className.IsEmpty())
                return OperationResult.Fail("ClassName is empty").WithoutPayload<ColorPalette>();

            if (memberName.IsEmpty())
                return OperationResult.Fail("MemberName is empty").WithoutPayload<ColorPalette>();

            Type colorPaletteClassType = Type.GetType(className);
            if (colorPaletteClassType is not null)
            {
                return LoadColorPalette(colorPaletteClassType, memberName);
            }

            Assembly[] assembliesToLookInto = null;
            if (!assemblyName.IsEmpty())
            {
                Assembly loadedAssembly = null;
                new Action(() => loadedAssembly = Assembly.Load(assemblyName)).TryOrFailWithGrace();
                if (loadedAssembly is not null)
                    assembliesToLookInto = [loadedAssembly];
            }

            assembliesToLookInto = assembliesToLookInto ?? AppDomain.CurrentDomain.GetNonCoreAssemblies();


            foreach (Assembly assembly in assembliesToLookInto)
            {
                colorPaletteClassType = Type.GetType($"{className}, {assembly.FullName}");
                if (colorPaletteClassType is not null)
                    break;
            }

            colorPaletteClassType = colorPaletteClassType ?? assembliesToLookInto.SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.Name.Is(className));

            if (colorPaletteClassType is null)
                return OperationResult.Fail($"Color Palette class {className} cannot be found").WithoutPayload<ColorPalette>();

            return LoadColorPalette(colorPaletteClassType, memberName);
        }

        OperationResult<ColorPalette> LoadColorPalette(Type colorPaletteClassType, string memberName)
        {
            MemberInfo memberInfo = colorPaletteClassType.GetMember(memberName, BindingFlags.Public | BindingFlags.Static).SingleOrDefault();
            if (memberInfo is null)
                return OperationResult.Fail($"A public static member with name {memberName} cannot be found in class {colorPaletteClassType.FullName}").WithoutPayload<ColorPalette>();

            OperationResult<ColorPalette> result = OperationResult.Fail("Not yet parsed").WithoutPayload<ColorPalette>();
            new Action(() => {

                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Field:
                        result = ((memberInfo as FieldInfo).GetValue(null) as ColorPalette).ToWinResult();
                        break;
                    case MemberTypes.Property:
                        result = ((memberInfo as PropertyInfo).GetValue(null) as ColorPalette).ToWinResult();
                        break;
                    case MemberTypes.Method:
                        result = ((memberInfo as MethodInfo).Invoke(null, []) as ColorPalette).ToWinResult(); 
                        break;
                    default:
                        result = OperationResult.Fail($"Member {memberName} which is a {memberInfo.MemberType} is not supported, it cannot be interpreted. It must be a field, property or argless-method").WithoutPayload<ColorPalette>();
                        break;
                }

            }).TryOrFailWithGrace(
                onFail: ex => result 
                    = OperationResult.Fail(
                        ex,
                        $"Error occurred while trying to read the color palette value from {colorPaletteClassType.Name}.{memberName}." +
                        $" Reason: {ex.Message}")
                    .WithoutPayload<ColorPalette>()
                );

            return result;
        }
    }
}
