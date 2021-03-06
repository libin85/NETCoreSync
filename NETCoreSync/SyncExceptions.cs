﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace NETCoreSync.Exceptions
{
    public class SyncConfigurationDuplicateTypeException : Exception
    {
        public SyncConfigurationDuplicateTypeException(Type type) : base($"Duplicate Type found: {type.FullName}")
        {
        }
    }

    public class SyncConfigurationMismatchPropertyTypeException : Exception
    {
        public SyncConfigurationMismatchPropertyTypeException(PropertyInfo propertyInfo, Type expectedType, Type type) : base($"Mismatch Property Type for Property: {propertyInfo.Name} ({propertyInfo.PropertyType.Name}) for Type: {type.FullName}. Expected Property Type: {expectedType.ToString()}.")
        {
        }
    }

    public class SyncConfigurationMissingSyncPropertyAttributeException : Exception
    {
        public SyncConfigurationMissingSyncPropertyAttributeException(SyncPropertyAttribute.PropertyIndicatorEnum propertyIndicator, Type type) : base($"Missing Property with {nameof(SyncPropertyAttribute)} ({nameof(SyncPropertyAttribute.PropertyIndicator)}: {propertyIndicator.ToString()}) defined for Type: {type.FullName}")
        {
        }
    }

    public class SyncConfigurationMissingSyncSchemaAttributeException : Exception
    {
        public SyncConfigurationMissingSyncSchemaAttributeException(Type type) : base($"Missing {nameof(SyncSchemaAttribute)} for Type: {type.FullName}")
        {
        }
    }

    public class SyncEngineConstraintException : Exception
    {
        public SyncEngineConstraintException(string errorMessage) : base(errorMessage)
        {
        }
    }

    public class SyncEngineMissingTypeInSyncConfigurationException : Exception
    {
        public SyncEngineMissingTypeInSyncConfigurationException(Type missingType) : base($"Missing Type: {missingType.FullName} in {nameof(SyncConfiguration)}")
        {
        }
    }
}
