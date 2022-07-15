using ClientConsole;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static ClientConsole.ConsoleUI;

var builder = new ConfigurationBuilder()
		.SetBasePath(Directory.GetCurrentDirectory())
		.AddJsonFile("appsettings.Development.json");

var configuration = builder.Build();

HttpClient client = new();
client.BaseAddress = new Uri(configuration["BaseUrl"]);
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

while (true)
{
	var command = AnsiConsole.Prompt(
	   new SelectionPrompt<string>()
		   .Title("What would you like to do?")
		   .AddChoices(new[]
		   {
			   "List all Recipes",
		   })
		   .AddChoiceGroup("Recipes", new[]
		   {
			   "Add a Recipe",
			   "Delete a Recipe",
			   "Edit a Recipe"
		   })
		   .AddChoiceGroup("Categories", new[]
		   {
			   "Add a Category",
			   "Delete a Category",
			   "Edit a Category"
		   }));
	AnsiConsole.Clear();
	switch (command)
	{
		case "List all Recipes":
			{
				DisplayRecipes(await ListRecipesAsync());
				break;
			}
		case "Add a Recipe":
			{
				Recipe recipe = CreateRecipe(await ListCategoriesAsync());
				await PostRecipeAsync(recipe);
				break;
			}
		case "Delete a Recipe":
			{
				var selectedRecipes = ChooseRecipes(await ListRecipesAsync());
				await DeleteRecipesAsync(selectedRecipes);
				break;
			}
		case "Edit a Recipe":
			{
				Recipe? recipe = EditRecipe(await ListRecipesAsync(), await ListCategoriesAsync());
				if (recipe != null)
					await PutRecipeAsync(recipe);
				break;
			}
		case "Add a Category":
			string category = CreateCategory();
			await PostCategoryAsync(category);
			break;
		case "Delete a Category":
			var selectedCategories = ChooseCategories(await ListCategoriesAsync());
			await DeleteCategoriesAsync(selectedCategories);
			break;
		case "Edit a Category":
			var oldCategory = ChooseCategory(await ListCategoriesAsync());
			var newCategory = CreateCategory();
			await PutCategoryAsync(oldCategory, newCategory);
			break;
	}
}

async Task<List<Recipe>> ListRecipesAsync()
{
	var recipeList = await client.GetFromJsonAsync<List<Recipe>>("recipes");
	if (recipeList != null)
		return recipeList;
	return new List<Recipe>();
}

async Task<List<string>> ListCategoriesAsync()
{
	var categoriesList = await client.GetFromJsonAsync<List<string>>("categories");
	if (categoriesList != null)
		return categoriesList;
	return new List<string>();
}

async Task PostRecipeAsync(Recipe recipe)
{
	await client.PostAsJsonAsync("recipes", recipe);
}

async Task DeleteRecipesAsync(List<Recipe> recipesList)
{
	var deleteTasks = new List<Task>();
	foreach (var recipe in recipesList)
		deleteTasks.Add(client.DeleteAsync("recipes?id=" + recipe.Id));
	await Task.WhenAll(deleteTasks);
}

async Task PutRecipeAsync(Recipe recipe)
{
	await client.PutAsJsonAsync("recipes", recipe);
}

async Task PostCategoryAsync(string category)
{
	await client.PostAsJsonAsync("categories", category);
}

async Task DeleteCategoriesAsync(List<string> categoriesList)
{
	var deleteTasks = new List<Task>();
	foreach (var category in categoriesList)
		deleteTasks.Add(client.DeleteAsync("categories?category=" + category));
	await Task.WhenAll(deleteTasks);
}

async Task PutCategoryAsync(string oldCategory, String editedCategory)
{
	await client.PutAsync($"categories?oldcategory={oldCategory}&editedcategory={editedCategory}", null);
}