using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;

namespace TatBlog.WebApp.Validations;

public static class FluentValidatioDependencyInjection {
    public static WebApplicationBuilder ConfigureFluentValidation(
        this WebApplicationBuilder builder) {

        // Enable client-side integration
        builder.Services.AddFluentValidationClientsideAdapters();

        builder.Services.AddValidatorsFromAssembly(
            Assembly.GetExecutingAssembly());

        return builder;
    }
}


