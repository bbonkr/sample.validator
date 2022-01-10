using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using FluentValidation;
using FluentValidation.AspNetCore;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Sample.Validator.App.Features;
using Sample.Validator.App.Models;

namespace Sample.Validator.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IValidatorInterceptor, ValidatorInterceptor>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

services.AddControllers(options =>
{
options.Filters.Add<ApiExceptionHandlerFilter>();
})
.AddJsonOptions(options => 
{
    options.JsonSerializerOptions.Converters.Add(new GuidJsonConverter());
})
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = actionContext =>
                    {
                        var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => new ErrorModel
                        {
                            Code = e.Key,
                            Message = e.Value.Errors.First().ErrorMessage,
                        });

                        throw new ApiException(StatusCodes.Status400BadRequest, new ErrorModel
                        {
                            Message = errors.FirstOrDefault().Message,
                            InnerErrors = errors,
                        });
                    };
                })
                  .AddFluentValidation(options =>
                  {
                      options.RegisterValidatorsFromAssemblies(new List<Assembly> {
                          GetType().Assembly,
                      });
                      //options.DisableDataAnnotationsValidation = true;
                      //options.ImplicitlyValidateChildProperties = true;
                  });

            services.AddValidatorsFromAssembly(this.GetType().Assembly);
            services.AddMediatR(GetType().Assembly);

            services.AddSwaggerGen(options => {
                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

public class GuidJsonConverter : System.Text.Json.Serialization.JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        var guidValue = Guid.Empty;
        if(!Guid.TryParse(stringValue, out guidValue))
        {
            throw new Exception("Invaild format value detected.");
        }

        return guidValue;
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
}
