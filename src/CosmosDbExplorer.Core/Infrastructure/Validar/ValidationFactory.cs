﻿using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using FluentValidation;

namespace CosmosDbExplorer.Infrastructure.Validar
{

    public static class ValidationFactory
    {
        static ConcurrentDictionary<RuntimeTypeHandle, IValidator> _validators = new ConcurrentDictionary<RuntimeTypeHandle, IValidator>();

        public static IValidator<T> GetValidator<T>()
            where T : INotifyPropertyChanged
        {
            var modelType = typeof(T);
            var modelTypeHandle = modelType.TypeHandle;
            if (!_validators.TryGetValue(modelTypeHandle, out var validator))
            {
                var typeName = $"{modelType.Namespace}.{modelType.Name}Validator";
                var type = modelType.Assembly.GetType(typeName, true);
                _validators[modelTypeHandle] = validator = (IValidator)Activator.CreateInstance(type);
            }

            return (IValidator<T>)validator;
        }
    }
}
