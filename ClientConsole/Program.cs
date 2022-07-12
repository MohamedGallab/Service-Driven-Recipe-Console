using ClientConsole;
using Spectre.Console;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using static ClientConsole.ConsoleUI;

HttpClient client = new();
client.BaseAddress = new Uri("https://localhost:7131/");
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
				DisplayRecipes(await ListRecipes());
				break;
			}
		case "Add a Recipe":
			{
				Recipe recipe = CreateRecipe(await ListCategories());
				await PostRecipe(recipe);
				break;
			}
		case "Delete a Recipe":
			{
				var selectedRecipes = ChooseRecipes(await ListRecipes());
				await DeleteRecipes(selectedRecipes);
				break;
			}
		case "Edit a Recipe":
			{
				Recipe? recipe = EditRecipe(await ListRecipes(), await ListCategories());
				if (recipe != null)
					await PutRecipe(recipe);
				break;
			}
		case "Add a Category":
			string category = CreateCategory();
			await PostCategory(category);
			break;
		case "Delete a Category":
			var selectedCategories = ChooseCategories(await ListCategories());
			await DeleteCategories(selectedCategories);
			break;
		case "Edit a Category":
			var oldCategory = ChooseCategory(await ListCategories());
			var newCategory = CreateCategory();
			await PutCategory(oldCategory, newCategory);
			break;
	}
}

async Task<List<Recipe>> ListRecipes()
{
	var recipeList = await client.GetFromJsonAsync<List<Recipe>>("recipes");
	if (recipeList != null)
		return recipeList;
	return new List<Recipe>();
}

async Task<List<string>> ListCategories()
{
	var categoriesList = await client.GetFromJsonAsync<List<string>>("categories");
	if (categoriesList != null)
		return categoriesList;
	return new List<string>();
}

async Task PostRecipe(Recipe recipe)
{
	await client.PostAsJsonAsync("recipes", recipe);
}

async Task DeleteRecipes(List<Recipe> recipesList)
{
	var deleteTasks = new List<Task>();
	foreach (var recipe in recipesList)
		deleteTasks.Add(client.DeleteAsync("recipes?id=" + recipe.Id));
	await Task.WhenAll(deleteTasks);
}

async Task PutRecipe(Recipe recipe)
{
	await client.PutAsJsonAsync("recipes", recipe);
}

async Task PostCategory(string category)
{
	await client.PostAsJsonAsync("categories", category);
}

async Task DeleteCategories(List<string> categoriesList)
{
	var deleteTasks = new List<Task>();
	foreach (var category in categoriesList)
		deleteTasks.Add(client.DeleteAsync("categories?category=" + category));
	await Task.WhenAll(deleteTasks);
}

async Task PutCategory(string oldCategory, String editedCategory)
{
	await client.PutAsync($"categories?oldcategory={oldCategory}&editedcategory={editedCategory}", null);
}