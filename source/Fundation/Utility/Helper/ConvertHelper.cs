using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utility.Helper
{
    public static class ConvertHelper
    {
        public static List<T> CopyAToB<T>(this IEnumerable<object> R) where T : new()
        {
            if (R == null || !R.Any())
                return new List<T> { };

            // copy fields
            var typeOfA = R.First().GetType();
            var targetBList = new List<T>();
            foreach (var item in R)
            {
                var targetB = new T();
                foreach (var fieldOfA in typeOfA.GetProperties())
                {
                    var fieldOfB = targetB.GetType().GetProperty(fieldOfA.Name);
                    if (fieldOfB == null)
                        continue;
                    fieldOfB.SetValue(targetB, typeOfA.GetProperty(fieldOfA.Name).GetValue(item));
                }
                targetBList.Add(targetB);
            }

            return targetBList;
        }


        public static T CopyAToB<T>(this object InObj) where T : new()
        {
            if (InObj == null)
                return new T();

            // copy fields
            var typeOfFrom = InObj.GetType();
            var targetOut = new T();
            foreach (var fieldOfFrom in typeOfFrom.GetProperties())
            {
                var fieldOfB = targetOut.GetType().GetProperty(fieldOfFrom.Name);
                if (fieldOfB == null)
                    continue;
                fieldOfB.SetValue(targetOut, typeOfFrom.GetProperty(fieldOfFrom.Name).GetValue(InObj));
            }

            return targetOut;
        }

        /*
         * B b = new B();
            // copy fields
            var typeOfA = a.GetType();
            var typeOfB = b.GetType();
            foreach (var fieldOfA in typeOfA.GetFields())
            {
                var fieldOfB = typeOfB.GetField(fieldOfA.Name);
                fieldOfB.SetValue(b, fieldOfA.GetValue(a));
            }
            // copy properties
            foreach (var propertyOfA in typeOfA.GetProperties())
            {
                var propertyOfB = typeOfB.GetProperty(propertyOfA.Name);
                propertyOfB.SetValue(b, propertyOfA.GetValue(a));
            }

         */
    }
}
