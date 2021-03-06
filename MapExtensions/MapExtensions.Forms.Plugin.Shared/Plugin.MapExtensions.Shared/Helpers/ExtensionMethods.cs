﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MapExtensions.Forms.Plugin
{
	internal static class ExtensionMethods
	{
		internal static void ExecuteIfPossible(this ICommand command, object parameter = null)
		{
			if (command.CanExecute(parameter))
			{
				command.Execute(parameter);
			}
		}
	}
}
