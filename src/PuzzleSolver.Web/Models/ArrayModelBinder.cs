using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections;

namespace PuzzleSolver.Web.Models;

public class ArrayModelBinder<T> : IModelBinder where T : class
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (!bindingContext.ModelMetadata.IsEnumerableType)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        var value = new List<T>();

        for (int i = 0; ; i++)
        {
            var hasMore = false;
            var obj = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var key = $"{bindingContext.ModelName}[{i}].{prop.Name}";
                var val = bindingContext.ValueProvider.GetValue(key);

                if (val == ValueProviderResult.None)
                    continue;

                hasMore = true;
                var propType = prop.PropertyType;
                var convertedValue = Convert.ChangeType(val.FirstValue, propType);
                prop.SetValue(obj, convertedValue);
            }

            if (!hasMore)
                break;

            value.Add(obj);
        }

        bindingContext.Result = ModelBindingResult.Success(value);
        return Task.CompletedTask;
    }
} 