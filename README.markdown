Griffin.MvcContrib
==================

A contribution project for ASP.NET MVC3

Current features
----------------

Read the wiki for a more detailed introduction.

* A more SOLID membership provider (uses the DependencyResolver to fetch each part)
* Easy model, view and validation localization without ugly attributes.
* HtmlHelpers that allows you to extend them or modify the generated HTML before it's written to the view.
* Base structure for JSON responses to allow integration between different plugins.
* Administration area for localization administration

Installation (nuget)
--------------------

	// base package
    install-package griffin.mvccontrib
	
	// administration area
	install-package griffin.mvccontrib.admin

	// sql server membership provider (and localization storage)
	install-package griffin.mvccontrib.sqlserver
	
	// ravendb membership provider (and localization storage)
	install-package griffin.mvccontrib.ravendb

Documentation
--------------

* [Core](http://griffinframework.net/docs/mvccontrib/)
* [Admin](http://griffinframework.net/docs/mvccontrib/admin/)