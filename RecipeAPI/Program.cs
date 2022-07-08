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

app.MapGet("/list-recipes", () => 
{
	Results.Ok(recipesList);
});

app.MapPost("/add-recipe", (Recipe recipe) =>
{
	recipesList.Add(recipe);
	Save();
	return Results.Created($"/todoitems/{recipe.Id}", recipe);
});

app.MapDelete("/delete-recipe", (Guid id) =>
{
	if(recipesList.Find(recipe => recipe.Id == id) is Recipe recipe)
	{
		recipesList.Remove(recipe);
		Save();
		return Results.Ok(recipe);
	}
	return Results.NotFound();
});

app.MapPut("/edit-recipe", (Recipe editedRecipe) =>
{
	if (recipesList.Find(recipe => recipe.Id == editedRecipe.Id) is Recipe recipe)
	{
		recipesList.Remove(recipe);
		recipesList.Add(editedRecipe);
		Save();
		return Results.Ok(editedRecipe);
	}
	return Results.NotFound();
});

app.MapGet("/list-categories", () =>
{
	Results.Ok(categoriesList);
});

app.MapPost("/add-category", (string category) =>
{
	categoriesList.Add(category);
	Save();
	return Results.Created($"/todoitems/{category}", category);
});

app.MapDelete("/delete-category", (string category) =>
{
	if (categoriesList.Contains(category))
	{
		categoriesList.Remove(category);
		Save();
		return Results.Ok(category);
	}
	return Results.NotFound();
});

app.MapPut("/edit-category", (string oldCategory, string editedcategory) =>
{
	if (categoriesList.Contains(oldCategory))
	{
		categoriesList.Remove(oldCategory);
		categoriesList.Add(editedcategory);
		Save();
		return Results.Ok(editedcategory);
	}
	return Results.NotFound();
});

void Save()
{
	File.WriteAllText(recipesFile, JsonSerializer.Serialize(recipesList));
	File.WriteAllText(categoriesFile, JsonSerializer.Serialize(categoriesList));
}

app.Run();