﻿#region

using System;
using System.Collections.Generic;
using Dev2.Studio.Core.Interfaces;

#endregion

namespace Dev2.Studio.Core.AppResources.DependencyInjection.EqualityComparers
{
    public class ContexttualResourceModelEqualityComparer : IEqualityComparer<IContextualResourceModel>
    {
        private static readonly Lazy<ContexttualResourceModelEqualityComparer> _current
            = new Lazy<ContexttualResourceModelEqualityComparer>(() => new ContexttualResourceModelEqualityComparer());

        private ContexttualResourceModelEqualityComparer()
        {
        }

        public static ContexttualResourceModelEqualityComparer Current
        {
            get { return _current.Value; }
        }

        public bool Equals(IContextualResourceModel x, IContextualResourceModel y)
        {
            //Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return EnvironmentModelEqualityComparer.Current.Equals(x.Environment, y.Environment) && x.ResourceName == y.ResourceName;
        }

        public int GetHashCode(IContextualResourceModel obj)
        {
            //Check whether the object is null
            if (ReferenceEquals(obj, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashProductName = obj.ResourceName == null ? 0 : obj.ResourceName.GetHashCode();

            //Get hash code for the Code field.
            return hashProductName;
        }
    }
}