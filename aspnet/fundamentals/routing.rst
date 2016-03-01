Routing
=======
By `Steve Smith`_

Routing middleware is used to map requests to route handlers. Routes are configured when the application starts up, and can define values that will be passed as parameters to route handlers. Routing functionality is also responsible generating links that correspond to routes in ASP.NET apps.

Sections:
	- `Routing Middleware`_
	- `Configuring Routing`_
	- `Template Routes`_
	- `Link Generation`_

`View sample files <https://github.com/aspnet/Docs/tree/1.0.0-rc1/aspnet/fundamentals/routing/sample>`_

Routing Middleware
------------------
The routing :doc:`middleware` uses *routes* to map requests to an ``IRouteHandler``. The ``IRouteHandler`` returns a ``RequestDelegate``, or ``null``. If it returns a delegate, that delegate is invoked; otherwise, routing continues. If no route handler is found for a request, then the middleware calls *next* (and the next middleware in the request pipeline is invoked).

.. TODO in RC2 Middleware later in the request pipeline can access route data using extensions methods on ``HttpContext``.

To get started using routing in your app, add it to the **dependencies** in ``project.json``:

.. literalinclude:: routing/sample/RoutingSample/project.json
  :linenos:
  :dedent: 2
  :language: javascript
  :lines: 6-12
  :emphasize-lines: 4
  
Next, add it to ``ConfigureServices`` in ``Startup.cs``:

.. literalinclude:: routing/sample/RoutingSample/Startup.cs
  :linenos:
  :dedent: 8
  :language: c#
  :lines: 16-19
  :emphasize-lines: 3

With this in place, you will have access to routing in your app. The next step is to configure it.

Configuring Routing
-------------------
Routing is configured in the ``Configure`` method in your ``Startup`` class. Create an instance of `RouteBuilder <https://github.com/aspnet/Routing/blob/1.0.0-rc1/src/Microsoft.AspNet.Routing/RouteBuilder.cs>`_. You can optionally set the ``ServiceProvider`` and/or ``DefaultHandler`` properties, in order to make them available as you build routes.

.. literalinclude:: routing/sample/RoutingSample/Startup.cs
  :linenos:
  :dedent: 8
  :language: c#
  :lines: 21-34
  :emphasize-lines: 7-8,10-13
  
You can see on line 13 how ``ApplicationServices`` is used to access the ``IInlineConstraingResolver`` dependency; if this were done from a ``RouteBuilder`` extension method, access to the services would be done through the ``ServiceProvider`` property.

Once you've finished adding routes to the ``RouteBuilder`` instance, call ``UseRouter`` and pass it the result of the ``RouteBuilder.Build`` method.

.. tip:: If you are only configuring a single route, you can simply call ``app.UseRouter`` and pass in the ``IRouter`` instance you wish to use, bypassing the need to use a ``RouteBuilder``.

The sample shown above uses the default empty web template, which includes the "Hello World" request delegate shown on lines 16-19. The route added on line 8 will only match requests of the form "hello/{name}" where `name` is constrained to be alphabetical. Requests that match this will be handled by a custom ``IRouter`` implementation, ``HelloRouter``.

.. literalinclude:: routing/sample/RoutingSample/HelloRouter.cs
  :linenos:
  :language: c#

``HelloRouter`` checks to see if ``RouteData`` includes a value for the key ``name``. If not, it immediately returns without handling the request. Otherwise, the request is handled (by writing out "Hi {name}!") and the ``RouteContext`` is updated to note that the request was handled. This prevents additional routes from handling the request.

This route was configured to use an inline constraint (see below), signified by the ":alpha" in the route template for the name route value. This constraint limits which requests this route will handle, in this case to alphabetical values for ``name``. Thus, a request for "/hello/steve" will be handled, but a request to "/hello/123" will not (instead, the request will pass through to the "Hello World!" request delegate).

Template Routes
---------------
The most common way to define routes is using ``TemplateRoute`` and route template strings. In a typical MVC app, you might use a default template route with a string like this one: 

.. image:: /fundamentals/routing/_static/default-mvc-routetemplate.png

Tokens within curly braces (``{ }``) define `route value` placeholders which will be bound if the route is matched. You can define more than one route value placeholder in a route segment, but they must be separated by a literal value. For example "/{controller}{action" would not be a valid route, since there is no literal value between ``{controller}`` and ``{action}``. These route value placeholders must have a name, and may have additional attributes specified.

The following table demonstrates the available route value placeholder options and `route value constraints <https://github.com/aspnet/Routing/blob/1.0.0-rc1/src/Microsoft.AspNet.Routing/RouteOptions.cs>`_.

.. list-table:: Route Template Values
	:header-rows: 1

	* - Route Template
	  - Example Matching URL
	  - Notes
	* - hello
	  - /hello
	  - Will literally match a single path
	* - {Page=Home}
	  - /
	  - Will match and set ``Page`` to ``Home``.
	* - {Page=Home}
	  - /Contact
	  - Will match and set ``Page`` to ``Contact``
	* - {controller}/{action}/{id?}
	  - /Products/List
	  - Will map to ``Products`` controller and ``List`` method; ``id`` is ignored.
	* - {controller}/{action}/{id?}
	  - /Products/Details/123
	  - Will map to ``Products`` controller and ``List`` method, with ``id`` set to ``123``.
	* - {controller=Home}/{action=Index}/{id?}
	  - /
	  - Will map to ``Home`` controller and ``Index`` method; ``id`` is ignored.

Adding a colon ``:`` after the name allows additional contraints to be set on a route value placeholder.

.. list-table:: Route Contraints
	:header-rows: 1

	* - Contraint
	  - Example
	  - Example Match
	  - Notes
	* - ``int``
	  - {id:int}
	  - 123
	  - Matches any integer
	* - ``bool``
	  - {active:bool}
	  - true
	  - Matches ``true`` or ``false``
	* - ``datetime``
	  - {dob:datetime}
	  - 2016-01-01
	  - Matches a valid ``DateTime`` value
	* - ``decimal``
	  - {price:decimal}
	  - 49.99
	  - Matches a valid ``decimal`` value
	* - ``decimal``
	  - {price:double}
	  - 4.234
	  - Matches a valid ``double`` value
	* - ``float``
	  - {price:float}
	  - 3.14
	  - Matches a valid ``float`` value
	* - ``guid``
	  - {id:guid}
	  - 7342570B-44E7-471C-A267-947DD2A35BF9
	  - Matches a valid ``Guid`` value
	* - ``long``
	  - {ticks:long}
	  - 123456789
	  - Matches a valid ``long`` value
	* - ``minlength(value)``
	  - {username:minlength(5)}
	  - steve
	  - String must be at least 5 characters long.
	* - ``maxlength(value)``
	  - {filename:maxlength(8)}
	  - somefile
	  - String must be no more than 8 characters long.
	* - ``length(min,max)``
	  - {filename:length(4,16)}
	  - Somefile.txt
	  - String must be at least 8 and no more than 16 characters long.
	* - ``min(value)``
	  - {age:min(18)}
	  - 19
	  - Value must be at least 18.
	* - ``max(value)``
	  - {age:max(120)}
	  - 91
	  - Value must be no more than 120.
	  - Value must be at least 18.
	* - ``range(min,max)``
	  - {age:max(18,120)}
	  - 91
	  - Value must be at least 18 but no more than 120.
	* - ``alpha``
	  - {name:alpha}
	  - Steve
	  - String must consist of alphabetical characters.
	* - ``regex(expression)``
	  - {ssn:regex(\d{3}-\d{2}-\d{4})}
	  - 123-45-6789
	  - String must match the provided regular expression.
	  * - ``required``
	  - {name:required}
	  - Steve
	  - Denotes a parameter is required; you need not specify this since it is the default.
	* - ``any|regex|value``
	  - {foo:a|b|c}
	  - a
	  - If the contraint isn't known, it is treated as a regex (if provided in the constraints dictionary - not within a template)

.. tip:: To constrain a parameter to a known set of possible values, you can use a regex: ``{action:regex(list|get|create)}``. This would only match the ``action`` route value to ``list``, ``get``, or ``create``. If passed into the contraints dictionary, the string "list|get|create" would be equivalent.

Contraints can be *chained*, so you can specify that a route value is of a certain type and also must fall within a certain range, like so: ``{age:int:range(1,120)}``. You can use the ``*`` character as a prefix to a route value name to bind to the rest of the URI. For example, ``blog/{*slug}`` would match any URI that started with ``/blog/`` and had any value following it (which would be assigned to the ``slug`` route value.

Route templates must be unambiguous, or they will be ignored. For example, consider these two routes:

1. ``{blog}/{year}/{month=1}``
2. ``{blog}/{year}``

The second route will never be used, since anything it would match will be matched by the first route (because it has a default value for month). If you wanted to construct a separate route that would for instance include all months or ask the user to select a month, you could change the route to include constant values, like this:

1. ``monthly/{blog}/{year}/{month=1}``
2. ``annual/{blog}/{year}``

.. note:: There is a special case route for filenames, such that you can define a route value like ``files/{filename}.{ext?}``. When both ``filename`` and ``ext`` exist, both values will be populated. However, if only ``filename`` exists in the URL, the trailing period ``.`` is also optional. Thus, these would both match: ``/files/foo.txt`` and ``/files/foo``.

.. tip:: Enable :doc:`logging` to see how the built in routing implementations, such as ``TemplateRoute``, match requests.

Route Builder Extensions
------------------------
Several `extension methods on RouteBuilder <https://github.com/aspnet/Routing/blob/1.0.0-rc1/src/Microsoft.AspNet.Routing/RouteBuilderExtensions.cs>`_ are available for convenience. The most common of these is ``MapRoute``, which allows the specification of a route given a name and template, and optionally default values, constraints, and/or data tokens. When using these extensions, you must have specified the ``DefaultHandler`` and ``ServiceProvider`` properties of the ``RouteBuilder`` instance to which you're adding the route. These extensions all make use of the ``TemplateRoute`` type to add new route templates to whichever ``IRouter`` is configured to as the ``DefaultHandler``.

.. note:: ``MapRoute`` doesn't take an ``IRouter`` parameter - it only adds routes to whatever the ``DefaultHandler`` is. This means that if the default is itself a route with named route value placeholders, only these placeholders can be used by routes defined using ``MapRoute``.

If you want ``MapRoute`` to work for any route template, you'll need to ensure the ``DefaultHandler`` is set to use an ``IRouter`` instance that can accept any value. The following ``DelegateRouter`` accepts any values and displays them:

.. literalinclude:: routing/sample/RoutingSample/DelegateRouter.cs
  :linenos:
  :language: c#

You configure this ``IRouter`` implementation as the ``DefaultHandler`` for a ``RouteBuilder`` in the ``Configure`` method in ``Startup``:

.. literalinclude:: routing/sample/RoutingSample/Startup.cs
  :linenos:
  :language: c#
  :lines: 27-49
  :dedent: 12
  :emphasize-lines: 9,16


Link Generation
---------------

Routing is also used to generate URLs based on route definitions. This is used by helpers to generate links to known actions on controllers in ASP.NET MVC 6, but can also be used independent of MVC. Given a set of route values, and optionally a route name, you can produce a ``VirtualPathContext`` object. Using the ``VirtualPathContext`` object along with a ``RouteCollection``, you can generate a ``VirtualPath``.

The example below shows how to generate a link to a route given a dictionary of route values and a ``RouteCollection``.

.. literalinclude:: routing/sample/RoutingSample/Startup.cs
  :linenos:
  :language: c#
  :lines: 45-69
  :dedent: 8
  :emphasize-lines: 9,14,17-18,23

The ``VirtualPath`` generated on line XX is ``/package/create/123``.

The second parameter to the ``VirtualPathContext`` constructor is a collection of ambient values. The current route values of the current request are considered ambient values for link generation. For example, in an MVC application if you are in the About action of the HomeController, you don't need to specify the controller route value to link to the Index action (the ambient value of HomeController will be used). Ambient values that don't match a parameter are ignored, and ambient values are also ignored when an explicitly-provided value overrides it, going from left to right in the URL.

(add ambient values example table)

Values that are explicitly provided but which don't match anything are added to the query string. (show example)

If a route has default values that don't match a parameter and that value is explicitly provided, it must match the default value.

