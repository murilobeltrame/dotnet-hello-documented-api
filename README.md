# Hello Documented API

This repos shows Open API Documentation practices aplied to a ASP.Net WebApi project.<br />
It uses `Swashbuckle.AspNetCore` (avaliable on [nuget](https://www.nuget.org/packages/swashbuckle.aspnetcore.swagger/#)) to achieve that.
It also uses `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer` (also avaliable on [nuget](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer/)) to enable API versioning and support documentation.

## Remarks

Configuring documentation and versioning support (@`Startup.cs`):
```cs
public class Startup
{
    // Gets XML Documentation path generated by the build process
    static readonly string XmlCommentsPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, $"{typeof(Startup).GetTypeInfo().Assembly.GetName().Name}.xml");

    // ... 

    public void ConfigureServices(IServiceCollection services)
    {
        // ...
        //Enabling API Versioning
        services.AddApiVersioning(o =>
        {
            o.ReportApiVersions = true;
            o.Conventions.Add(new VersionByNamespaceConvention());
            o.AssumeDefaultVersionWhenUnspecified = true;
        });
        services.AddVersionedApiExplorer(o => {
            o.GroupNameFormat = "'v'VVV";
            o.SubstituteApiVersionInUrl = true;
        });
        // Configure Swagger Document generation based on reported versions
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        // Include XML Documentation (based on <summary/>) to the Swagger
        services.AddSwaggerGen(o =>
        {
            o.IncludeXmlComments(XmlCommentsPath);
        });
    }

    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        // ...

        // Configuring Swagger and Swagger UI for reported API versions
        app.UseSwagger();
        app.UseSwaggerUI(o =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                o.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }
            o.RoutePrefix = string.Empty;
        });

        // ...
    }
}

// Defines the way the documentation header are built for the reported API versions
internal class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        this.provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = $"The API Title",
                Version = description.ApiVersion.ToString()
            });
        }
    }
}
```

Documenting _Models_:
```cs
public class Todo
{
    /// <summary>
    /// Identificação unica da tarefa
    /// </summary>
    /// <example>18647c67-be2b-46b9-9be2-49de8b9a3b88</example>
    public Guid Id { get; set; }

    /// <summary>
    /// Descrição da tarefa
    /// </summary>
    /// <example>Fazer exemplo de documentação de APIs</example>
    public string Description { get; set; }

    /// <summary>
    /// Prazo para a conclusão da tarefa
    /// </summary>
    /// <example>2021-04-17T14:22:39.3973797-03:00</example>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Estado atual da tarefa
    /// </summary>
    public Status CurrentStatus { get; set; }

    /// <summary>
    /// Define se a tarefa foi concluida
    /// </summary>
    public bool Finished => CurrentStatus == Status.CANCELLED || CurrentStatus == Status.DONE;
}
```
> `<example>` defines sample values for the Swagger UI and allows fast testing to the client.<br />
> The value type are infered by de C# types<br />
> The _reference types_ are considered optional unless they're marked as `[Required]` and _value types_ are considered required unless they're marked as `Nullable`.

Documenting _Controllers_:
```cs
[ApiController]
[Route("[controller]")]
public class TodosController : ControllerBase
{
    // ...

    /// <summary>
    /// Lista as tarefas
    /// </summary>
    /// <remarks>
    /// Lista todas as tarefas conforme os parâmetros especificados.<br />
    /// Caso `_offset` não seja definido, o valor **0** usado como padrão.<br />
    /// Caso `_limit` não seja definido, o valor **10** é usado como padrão. O valor máximo para esse parâmetro é *255*.<br />
    /// A ordenação padrão usada é por `DueDate`.<br/>
    /// O número total de registros da pesquisa é retornado no cabeçalho `X-Total-Count`.
    /// </remarks>
    /// <param name="request">Condigurações da pesquisa</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetail), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> List([FromQuery] ListRequest request)
    {
        // ...
    }

    // ...
}
```

> The `<summary>` defines a short description while `remarks` can have a longer and detailed instructions (and also support _markdown_ syntax)<br />
>`ProducesResponseType` has a constructor that receives a `Type` wich and be used to describe the returning model even when using `IActionResult` or returning an error.<br />
> It's a good practice do document all expected returning Status Code to the client.
> It's also a good practice to handle error in a custom error structure to avoid to return unnecessary technical details on implementation.