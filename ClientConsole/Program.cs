using ClientConsole.Models;
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
				try
				{
					DisplayRecipes(await ListRecipesAsync());
				}
				catch (Exception)
				{
					DisplayErrorMessage();
				}
				break;
			}
		case "Add a Recipe":
			{
				try
				{
					Recipe recipe = CreateRecipe(await ListCategoriesAsync());
					await PostRecipeAsync(recipe);
				}
				catch (Exception)
				{
					DisplayErrorMessage();
				}
				break;
			}
		case "Delete a Recipe":
			{
				try
				{
					var selectedRecipes = ChooseRecipes(await ListRecipesAsync());
					await DeleteRecipesAsync(selectedRecipes);
				}
				catch (Exception)
				{
					DisplayErrorMessage();
				}
				break;
			}
		case "Edit a Recipe":
			{
				try
				{
					Recipe? recipe = EditRecipe(await ListRecipesAsync(), await ListCategoriesAsync());
					if (recipe != null)
						await PutRecipeAsync(recipe);
				}
				catch (Exception)
				{
					DisplayErrorMessage();
					
				}
				break;
			}
		case "Add a Category":
			string category = CreateCategory();
			try
			{
				await PostCategoryAsync(category);
			}
			catch (Exception)
			{
				DisplayErrorMessage();
			}
			break;
		case "Delete a Category":
			try
			{
				var selectedCategories = ChooseCategories(await ListCategoriesAsync());
				await DeleteCategoriesAsync(selectedCategories);
			}
			catch (Exception)
			{
				DisplayErrorMessage();
			}
			break;
		case "Edit a Category":
			try
			{
				var oldCategory = ChooseCategory(await ListCategoriesAsync());
				var newCategory = CreateCategory();
				await PutCategoryAsync(oldCategory, newCategory);
			}
			catch (Exception)
			{
				DisplayErrorMessage();
			}
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
	var response = await client.PostAsJsonAsync("recipes", recipe);
	response.EnsureSuccessStatusCode();
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
	var response = await client.PutAsJsonAsync("recipes", recipe);
	response.EnsureSuccessStatusCode();
}

async Task PostCategoryAsync(string category)
{
	var response = await client.PostAsJsonAsync("categories?category="+category, category);
	response.EnsureSuccessStatusCode();
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
	var response = await client.PutAsync($"categories?oldcategory={oldCategory}&editedcategory={editedCategory}", null);
	response.EnsureSuccessStatusCode();
}