using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole;

internal class ConsoleUI
{
	public static Recipe CreateRecipe(List<string> categoriesList)
	{
		var title = AnsiConsole.Ask<string>("What is the [green]recipe[/] called?");
		var ingredients = new List<string>();
		var instructions = new List<string>();
		var categories = new List<string>();

		AnsiConsole.MarkupLine("Enter all the [green]ingredients[/]. Once done, leave the ingredient field [red]empty[/] and press enter");
		var ingredient = AnsiConsole.Ask<string>("Enter ingredient: ");
		while (ingredient != "")
		{
			ingredients.Add(ingredient);
			ingredient = AnsiConsole.Prompt(new TextPrompt<string>("Enter ingredient: ").AllowEmpty());
		};

		AnsiConsole.MarkupLine("Enter all the [green]instructions[/]. Once done, leave the instruction field [red]empty[/] and press enter");
		var instruction = AnsiConsole.Ask<string>("Enter instruction: ");
		while (instruction != "")
		{
			instructions.Add(instruction);
			instruction = AnsiConsole.Prompt(new TextPrompt<string>("Enter instruction: ").AllowEmpty());
		};

		var recipe = new Recipe()
		{
			Title = title,
			Ingredients = ingredients,
			Instructions = instructions,
			Categories = categories
		};

		if (categoriesList.Count == 0)
		{
			return recipe;
		}

		var selectedcategories = AnsiConsole.Prompt(
		new MultiSelectionPrompt<string>()
		.PageSize(10)
		.Title("Which [green]categories[/] does this recipe belong to?")
		.MoreChoicesText("[grey](Move up and down to reveal more categories)[/]")
		.InstructionsText("[grey](Press [blue]Space[/] to toggle a category, [green]Enter[/] to accept)[/]")
		.AddChoices(categoriesList));

		recipe.Categories = selectedcategories;

		return recipe;
	}

	public static void DisplayRecipes(List<Recipe> recipesList)
	{
		var table = new Table();
		table.AddColumn("Recipe Name");
		table.AddColumn("Ingredients");
		table.AddColumn("Instructions");
		table.AddColumn("Categories");

		foreach (var recipe in recipesList)
		{
			var ingredients = new StringBuilder();
			foreach (string ingredient in recipe.Ingredients)
				ingredients.Append("- " + ingredient + "\n");
			var instructions = new StringBuilder();
			foreach (string instruction in recipe.Instructions)
				instructions.Append("- " + instruction + "\n");
			var categories = new StringBuilder();
			foreach (string category in recipe.Categories)
				categories.Append("- " + category + "\n");
			table.AddRow(recipe.Title, ingredients.ToString(), instructions.ToString(), categories.ToString());
		}
		AnsiConsole.Write(table);
	}

	public static List<Recipe> ChooseRecipes(List<Recipe> recipesList)
	{
		if (recipesList.Count == 0)
		{
			AnsiConsole.MarkupLine("There are no Recipes");
			return new();
		}
		var selectedRecipes = AnsiConsole.Prompt(
		new MultiSelectionPrompt<Recipe>()
		.PageSize(10)
		.Title("Which [green]recipes[/] does this recipe belong to?")
		.MoreChoicesText("[grey](Move up and down to reveal more recipes)[/]")
		.InstructionsText("[grey](Press [blue]Space[/] to toggle a recipe, [green]Enter[/] to accept)[/]")
		.AddChoices(recipesList));

		return selectedRecipes;
	}

	public static Recipe? EditRecipe(List<Recipe> recipesList, List<string> categoriesList)
	{
		if (recipesList.Count == 0)
		{
			AnsiConsole.MarkupLine("There are no Recipes");
			return null;
		}

		var chosenRecipe = AnsiConsole.Prompt(
		   new SelectionPrompt<Recipe>()
			   .Title("Which Recipe would you like to edit?")
			   .AddChoices(recipesList));

		var command = AnsiConsole.Prompt(
		   new SelectionPrompt<string>()
			   .Title("What would you like to do?")
			   .AddChoices(new[]
			   {
			   "Edit title",
			   "Edit Ingredients",
			   "Edit Instructions",
			   "Edit Categories"
			   }));

		AnsiConsole.Clear();
		switch (command)
		{
			case "Edit title":
				chosenRecipe.Title = AnsiConsole.Ask<string>("What is the [green]recipe[/] called?");
				break;
			case "Edit Ingredients":
				chosenRecipe.Ingredients.Clear();
				AnsiConsole.MarkupLine("Enter all the [green]ingredients[/]. Once done, leave the ingredient field [red]empty[/] and press enter");
				var ingredient = AnsiConsole.Ask<string>("Enter ingredient: ");
				while (ingredient != "")
				{
					chosenRecipe.Ingredients.Add(ingredient);
					ingredient = AnsiConsole.Prompt(new TextPrompt<string>("Enter ingredient: ").AllowEmpty());
				};
				break;
			case "Edit Instructions":
				chosenRecipe.Instructions.Clear();
				AnsiConsole.MarkupLine("Enter all the [green]instructions[/]. Once done, leave the instruction field [red]empty[/] and press enter");
				var instruction = AnsiConsole.Ask<string>("Enter instruction: ");
				while (instruction != "")
				{
					chosenRecipe.Instructions.Add(instruction);
					instruction = AnsiConsole.Prompt(new TextPrompt<string>("Enter instruction: ").AllowEmpty());
				};
				break;
			case "Edit Categories":
				var selectedcategories = AnsiConsole.Prompt(
				new MultiSelectionPrompt<string>()
				.PageSize(10)
				.Title("Which [green]categories[/] does this recipe belong to?")
				.MoreChoicesText("[grey](Move up and down to reveal more categories)[/]")
				.InstructionsText("[grey](Press [blue]Space[/] to toggle a category, [green]Enter[/] to accept)[/]")
				.AddChoices(categoriesList));

				chosenRecipe.Categories = selectedcategories;
				break;
		}
		return chosenRecipe;
	}
	public static string CreateCategory()
	{
		return AnsiConsole.Ask<string>("What is the [green]category[/] called?");
	}

	public static List<string> ChooseCategories(List<string> categoriesList)
	{
		if (categoriesList.Count == 0)
		{
			AnsiConsole.MarkupLine("There are no Categories");
			return new();
		}
		var selectedcategories = AnsiConsole.Prompt(
		new MultiSelectionPrompt<string>()
		.PageSize(10)
		.Title("Which [green]categories[/] does this recipe belong to?")
		.MoreChoicesText("[grey](Move up and down to reveal more categories)[/]")
		.InstructionsText("[grey](Press [blue]Space[/] to toggle a category, [green]Enter[/] to accept)[/]")
		.AddChoices(categoriesList));

		return selectedcategories;
	}
	public static string ChooseCategory(List<string> categoriesList)
	{
		if (categoriesList.Count == 0)
		{
			throw new InvalidOperationException("There are no categories!");
		}
		var chosenCategory = AnsiConsole.Prompt(
		   new SelectionPrompt<string>()
			   .Title("Which Category would you like to edit?")
			   .AddChoices(categoriesList));

		return chosenCategory;
	}
	public static void DisplayErrorMessage(string s = "An error has occured try again later please")
	{
		AnsiConsole.MarkupLine(s);
		Console.ReadKey();
		AnsiConsole.Clear();
		return;
	}
}
