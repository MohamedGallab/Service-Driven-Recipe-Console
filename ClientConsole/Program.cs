using ClientConsole;
using Spectre.Console;
using System.Net.Http.Headers;
using System.Net.Http.Json;

HttpClient client = new();
client.BaseAddress = new Uri("https://localhost:7131/");
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

async Task AddRecipe(Recipe recipe)
{
	HttpResponseMessage response = await client.PostAsJsonAsync("add-recipe", recipe);
	Console.WriteLine(
		$"{(response.IsSuccessStatusCode ? "Success" : "Error")} - {response.StatusCode}");
}

async Task ListRecipe()
{
	var recipeList = await client.GetFromJsonAsync<List<Recipe>>("list-recipes");

	foreach (Recipe r in recipeList)
	{
		Console.WriteLine(r.Id);
	}
}