using RecipeAPI;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

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

app.MapPost("/recipes", async (Recipe recipe) =>
{
	recipesList.Add(recipe);
	await SaveAsync();
	return Results.Created($"/recipes/{recipe.Id}", recipe);
});

app.MapDelete("/recipes", async (Guid id) =>
{
	if (recipesList.Find(recipe => recipe.Id == id) is Recipe recipe)
	{
		recipesList.Remove(recipe);
		await SaveAsync();
		return Results.Ok(recipe);
	}
	return Results.NotFound();
});

app.MapPut("/recipes", async (Recipe editedRecipe) =>
{
	if (recipesList.Find(recipe => recipe.Id == editedRecipe.Id) is Recipe recipe)
	{
		recipesList.Remove(recipe);
		recipesList.Add(editedRecipe);
		await SaveAsync();
		return Results.NoContent();
	}
	return Results.NotFound();
});

app.MapGet("/categories", () =>
{
	return Results.Ok(categoriesList);
});

app.MapPost("/categories", async (string category) =>
{
	categoriesList.Add(category);
	await SaveAsync();
	return Results.Created($"/categories/{category}", category);
});

app.MapDelete("/categories", async (string category) =>
{
	if (categoriesList.Contains(category))
	{
		foreach (Recipe recipe in recipesList)
		{
			recipe.Categories.Remove(category);
		}
		categoriesList.Remove(category);
		await SaveAsync();
		return Results.Ok(category);
	}
	return Results.NotFound();
});

app.MapPut("/categories", async (string oldCategory, string editedcategory) =>
{
	if (categoriesList.Contains(oldCategory))
	{
		categoriesList.Remove(oldCategory);
		categoriesList.Add(editedcategory);

		foreach (var recipe in recipesList)
		{
			recipe.Categories.Remove(oldCategory);
			recipe.Categories.Add(editedcategory);
		}

		await SaveAsync();
		return Results.NoContent();
	}
	return Results.NotFound();
});

async Task SaveAsync()
{
	await Task.WhenAll(
		File.WriteAllTextAsync(recipesFile, JsonSerializer.Serialize(recipesList)),
		File.WriteAllTextAsync(categoriesFile, JsonSerializer.Serialize(categoriesList))
		);
}

app.Run();