using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Coelum.ECS.Extensions {
	
	public static class ReflectionExtensions {
		
		public static bool IsForPublicNodeUse(this FieldInfo field) {
			if(field.DeclaringType == typeof(Node)) return false;
			if(field.IsInitOnly) return false;
			if(field.IsPrivate
			   || field.IsFamily
			   || field.IsAssembly) return false;

			return true;
		}

		public static bool IsForPublicNodeUse(this PropertyInfo property) {
			if(property.DeclaringType == typeof(Node)) return false;
			if(property.Name == "Owner") return false;
			if(property.GetMethod == null
			   || property.GetMethod.IsPrivate
			   || property.GetMethod.IsFamily
			   || property.GetMethod.IsAssembly) return false;
			if(property.SetMethod == null
			   || property.SetMethod.IsPrivate
			   || property.SetMethod.IsFamily
			   || property.SetMethod.IsAssembly) return false;

			return true;
		}

		private static readonly List<Type> _commonDecimalTypes = new() {
			typeof(float),
			typeof(int),
			typeof(double),
			typeof(long),
			typeof(ulong),
			typeof(uint),
			typeof(byte),
			typeof(sbyte),
			typeof(short),
			typeof(ushort),
		};

		public static Type GetCommonTypeForNodeUse(this Type type, bool useDecimal = true) {
			if(type.IsAssignableTo(typeof(Node))) return typeof(Node);
			else if(type.IsAssignableTo(typeof(IEnumerable))) return typeof(IEnumerable);
			else if(type.IsAssignableTo(typeof(KeyValuePair<object, object>))) return typeof(KeyValuePair<object, object>);

			// <3
			// TODO could this cause resolution/floating-point issues?
			if(useDecimal) {
				if(_commonDecimalTypes.Contains(type)) {
					return typeof(decimal);
				}
			}

			return type;
		}

		public static object ReverseCommonDecimalConversion(this object o, Type type) {
			if(type == typeof(float)) return Convert.ToSingle(o);
			else if(type == typeof(int)) return Convert.ToInt32(o);
			else if(type == typeof(double)) return Convert.ToDouble(o);
			else if(type == typeof(long)) return Convert.ToInt64(o);
			else if(type == typeof(ulong)) return Convert.ToUInt64(o);
			else if(type == typeof(uint)) return Convert.ToUInt32(o);
			else if(type == typeof(byte)) return Convert.ToByte(o);
			else if(type == typeof(sbyte)) return Convert.ToSByte(o);
			else if(type == typeof(short)) return Convert.ToInt16(o);
			else if(type == typeof(ushort)) return Convert.ToUInt16(o);
			
			return o;
		}
	}
}