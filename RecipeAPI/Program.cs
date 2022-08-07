using RecipeAPI.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
	options.AddPolicy("Cors Policy",
		policy =>
		{
			policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
		});
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
	options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
	options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors("Cors Policy");

// load previous categories if exists
string categoriesFile = "categories.json";
string jsonCategoriesString;
var categoriesList = new List<string>();

if (File.Exists(categoriesFile))
{
	if (new FileInfo(categoriesFile).Length > 0)
	{
		jsonCategoriesString = await File.ReadAllTextAsync(categoriesFile);
		categoriesList = JsonSerializer.Deserialize<List<string>>(jsonCategoriesString)!;
	}
}

// load previous recipes if exists
string recipesFile = "Recipes.json";
string jsonRecipesString;
var recipesList = new List<Recipe>();

if (File.Exists(recipesFile))
{
	if (new FileInfo(recipesFile).Length > 0)
	{
		jsonRecipesString = await File.ReadAllTextAsync(recipesFile);
		recipesList = JsonSerializer.Deserialize<List<Recipe>>(jsonRecipesString)!;
	}
}

// endpoints
app.MapGet("/recipes", () =>
{
	return Results.Ok(recipesList);
});

app.MapGet("/recipes/{id}", (Guid id) =>
{
	if (recipesList.Find(recipe => recipe.Id == id) is Recipe recipe)
	{
		return Results.Ok(recipe);
	}
	return Results.NotFound();
});

app.MapPost("/recipes", async (Recipe recipe) =>
{
	if (recipe.Title == String.Empty)
	{
		return Results.BadRequest();
	}
	recipe.Id = Guid.NewGuid();
	recipesList.Add(recipe);
	await SaveAsync();
	return Results.Created($"/recipes/{recipe.Id}", recipe);
});

app.MapDelete("/recipes/{id}", async (Guid id) =>
{
	if (recipesList.Find(recipe => recipe.Id == id) is Recipe recipe)
	{
		recipesList.Remove(recipe);
		await SaveAsync();
		return Results.Ok(recipe);
	}
	return Results.NotFound();
});

app.MapPut("/recipes/{id}", async (Recipe editedRecipe) =>
{
	int oldRecipeIndex = recipesList.FindIndex(recipe => recipe.Id == editedRecipe.Id);

	if (oldRecipeIndex == -1)
	{
		return Results.NotFound();
	}

	recipesList[oldRecipeIndex] = editedRecipe;
	await SaveAsync();
	return Results.NoContent();
});

app.MapGet("/categories", () =>
{
	return Results.Ok(categoriesList);
});

app.MapPost("/categories", async (string category) =>
{
	if (category == String.Empty)
	{
		return Results.BadRequest();
	}

	categoriesList.Add(category);
	await SaveAsync();
	return Results.Created($"/categories/{category}", category);
});

app.MapDelete("/categories/{category}", async (string category) =>
{
	if (category == String.Empty)
	{
		return Results.BadRequest();
	}

	if (!categoriesList.Contains(category))
	{
		return Results.NotFound();
	}

	foreach (Recipe recipe in recipesList)
	{
		recipe.Categories.Remove(category);
	}
	categoriesList.Remove(category);
	await SaveAsync();
	return Results.Ok(category);
});

app.MapPut("/categories/{category}", async (string category, string editedCategory) =>
{
	if (editedCategory == String.Empty)
	{
		return Results.BadRequest();
	}

	int oldCategoryIndex = categoriesList.IndexOf(category);

	if (oldCategoryIndex == -1)
	{
		return Results.NotFound();
	}
	categoriesList[oldCategoryIndex] = editedCategory;

	foreach (var recipe in recipesList)
	{
		if (recipe.Categories.Contains(category))
		{
			recipe.Categories.Remove(category);
			recipe.Categories.Add(editedCategory);
		}
	}

	await SaveAsync();
	return Results.NoContent();

});

async Task SaveAsync()
{
	await Task.WhenAll(
		File.WriteAllTextAsync(recipesFile, JsonSerializer.Serialize(recipesList)),
		File.WriteAllTextAsync(categoriesFile, JsonSerializer.Serialize(categoriesList))
		);
}

app.Run();