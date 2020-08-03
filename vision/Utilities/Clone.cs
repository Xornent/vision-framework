using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Vision.Utilities {

    public static class Clone {

        public static T CloneReflection<T>(this T t) where T : class {
            T model = System.Activator.CreateInstance<T>();
            PropertyInfo[] propertyInfos = model.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos) {

                // 判断值是否为空

                if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {

                    // 如果convertsionType为nullable类，声明一个 NullableConverter 类，
                    // 该类提供从Nullable类到基础基元类型的转换

                    NullableConverter nullableConverter = new NullableConverter(propertyInfo.PropertyType);

                    // 将 convertsionType 转换为 nullable 对的基础基元类型

                    propertyInfo.SetValue(model, Convert.ChangeType(propertyInfo.GetValue(t), nullableConverter.UnderlyingType), null);
                } else { propertyInfo.SetValue(model, Convert.ChangeType(propertyInfo.GetValue(t), propertyInfo.PropertyType), null);}
            }
            return model;
        }

    }
}
