﻿using Spectre.Console;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole.Models;

[TypeConverter(typeof(RecipeTypeConverter))]
internal class Recipe
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Title { get; set; } = string.Empty;
	public List<string> Ingredients { get; set; } = new();
	public List<string> Instructions { get; set; } = new();
	public List<string> Categories { get; set; } = new();
}
