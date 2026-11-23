using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Horror.Tests
{
    /// <summary>
    /// The game scripts live in Assembly-CSharp, which test assemblies cannot reference,
    /// so the play-mode tests reach them by name.
    /// </summary>
    internal static class GameTypes
    {
        public static Type Get(string typeName)
        {
            Type type = Type.GetType(typeName + ", Assembly-CSharp");
            if (type == null) throw new InvalidOperationException($"Type {typeName} not found in Assembly-CSharp.");
            return type;
        }

        public static Component Find(string typeName) => (Component)Object.FindObjectOfType(Get(typeName));

        public static object Call(object target, string method, params object[] args)
        {
            Type type = target as Type ?? target.GetType();
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            foreach (MethodInfo candidate in type.GetMethods(flags))
            {
                if (candidate.Name == method && candidate.GetParameters().Length == args.Length)
                {
                    return candidate.Invoke(target is Type ? null : target, args);
                }
            }
            throw new MissingMethodException(type.Name, method);
        }

        public static object Get(object target, string property)
        {
            Type type = target as Type ?? target.GetType();
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            PropertyInfo info = type.GetProperty(property, flags);
            if (info != null) return info.GetValue(target is Type ? null : target);
            FieldInfo field = type.GetField(property, flags);
            if (field != null) return field.GetValue(target is Type ? null : target);
            throw new MissingMemberException(type.Name, property);
        }
    }
}
