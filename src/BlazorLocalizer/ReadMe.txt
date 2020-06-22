## BlazorLocalization Usage Guide

1: Implement the interface BlazorLocalizer.IResourceProvider

2: Register your IResourceProvider implementation with your dependency injection 

	-- Example from Program.cs: 
		builder.Services.AddTransient<IResourceProvider, YourResourceProviderImplementation>();

3: Call AddBlazorLocalization() from Program.cs 

	-- Examples from Program.cs:
		builder.Services.AddBlazorLocalization();
		builder.Services.AddBlazorLocalization(config => { 
			config.FallbackCategory = "My.Fallback.Category";
			config.LocalStorageOptions.CacheInvalidation = TimeSpan.FromMinutes(10);
		});

4: Add "@using BlazorLocalizer" to your _Imports.razor

5: Add "@Inject IBlazorLocalizer Loc" to any components where you need localization.
	Key:		The key for the text you're translating.
	Category:	The category for the text you're translating. This category could be the same for all localized keys on the same page. If left null or empty, BlazorLocalizer will use FallbackCategory from options.
	Culture:	OPTIONAL - The culture you want translated to. Fallback to CurrentUICulture.Name if no value.

	-- Examples for usage within HTML:
		@Loc["Key goes here", this]
		@Loc["Key goes here", "category", "en-US"]
		In the above examples the Key will be rendered as a placeholder value untill the localized values has been fetched.

	-- Examples for usage within codebloc:
		string localizedKey = Loc.L("Key goes here", this);
		string localizedKey = Loc.L("Key goes here", "category", "en-US");

Project URL: https://github.com/JonTvermose/BlazorLocalizer