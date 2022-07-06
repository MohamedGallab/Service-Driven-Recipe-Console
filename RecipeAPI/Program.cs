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

// routes
app.MapGet("/", () => "Hello World!");

app.MapGet("/list-recipes", () => recipesList);

app.MapPost("/add-recipe", (Recipe recipe) =>
{
	recipesList.Add(recipe);
	return Results.Ok();
});

app.MapDelete("/delete-recipe", (Guid guid) =>
{
	var recipe = recipesList.Find(recipe => recipe.Id == guid);
	if(recipe == null)
	{
		return Results.NotFound();
	}
	recipesList.Remove(recipe);
	return Results.Ok();
});

void Save()
{
	File.WriteAllText(recipesFile, JsonSerializer.Serialize(recipesList));
	File.WriteAllText(categoriesFile, JsonSerializer.Serialize(categoriesList));
}

app.Run();