using RecipeAPI;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// load previous categories if exists
string categoriesFile = "categories.json";
string jsonCategoriesString;
var categoriesList = new List<string>();

if (File.Exists(categoriesFile))
{
	if (new FileInfo(categoriesFile).Length > 0)
	{
		jsonCategoriesString = File.ReadAllText(categoriesFile);
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
		jsonRecipesString = File.ReadAllText(recipesFile);
		recipesList = JsonSerializer.Deserialize<List<Recipe>>(jsonRecipesString)!;
	}
}

// endpoints
app.MapGet("/recipes", () =>
{
	return Results.Ok(recipesList);
});

app.MapPost("/recipes", (Recipe recipe) =>
{
	recipesList.Add(recipe);
	Save();
	return Results.Created($"/recipes/{recipe.Id}", recipe);
});

app.MapDelete("/recipes", (Guid id) =>
{
	if (recipesList.Find(recipe => recipe.Id == id) is Recipe recipe)
	{
		recipesList.Remove(recipe);
		Save();
		return Results.Ok(recipe);
	}
	return Results.NotFound();
});

app.MapPut("/recipes", (Recipe editedRecipe) =>
{
	if (recipesList.Find(recipe => recipe.Id == editedRecipe.Id) is Recipe recipe)
	{
		recipesList.Remove(recipe);
		recipesList.Add(editedRecipe);
		Save();
		return Results.NoContent();
	}
	return Results.NotFound();
});

app.MapGet("/categories", () =>
{
	return Results.Ok(categoriesList);
});

app.MapPost("/categories", (string category) =>
{
	categoriesList.Add(category);
	Save();
	return Results.Created($"/categories/{category}", category);
});

app.MapDelete("/categories", (string category) =>
{
	if (categoriesList.Contains(category))
	{
		foreach (Recipe recipe in recipesList)
		{
			recipe.Categories.Remove(category);
		}
		categoriesList.Remove(category);
		Save();
		return Results.Ok(category);
	}
	return Results.NotFound();
});

app.MapPut("/categories", (string oldCategory, string editedcategory) =>
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

		Save();
		return Results.NoContent();
	}
	return Results.NotFound();
});

void Save()
{
	File.WriteAllText(recipesFile, JsonSerializer.Serialize(recipesList));
	File.WriteAllText(categoriesFile, JsonSerializer.Serialize(categoriesList));
}

app.Run();