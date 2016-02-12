Model Binding
=============

By `Rachel Appel <http://github.com/rachelappel>`_

In this article

- `Introduction to model binding`_
- `How model binding works`_
- `Customize model binding behavior with attributes`_
- `Binding formatted data from the request body`_ 
- `Resources`_

Introduction to model binding
-----------------------------
Model binding in MVC maps data from HTTP requests to action method parameters. The parameters may be simple types such as strings, integers, or floats, or they may be complex classes. This is a great feature of MVC because mapping incoming data to a counterpart is an often repeated scenario, regardless of size or complexity of the data. MVC solves this problem by abstracting binding away so developers don't have to keep rewriting a slightly different version of that same code in every app. Code like this quickly becomes repetitive, cumbersome, and error prone to write, even when it's just a few fields. 

How model binding works
-----------------------
When ASP.NET MVC receives an incoming HTTP request, it routes it to a specific action method of a controller. It determines which action method to run based on what is in the route data, then it binds values from the HTTP request to that action method's parameters. For example, consider the following URL:

`http://contoso.com/movies/edit/2`

Since the  routing pattern looks like this, ``{controller=Home}/{action=Index}/{id?}``, the route ``movies/edit/2`` maps to the ``Movies`` controller, and its ``Edit`` action method. It also accepts an optional parameter called ``id``. This is due to MVC's use of conventions. So the code for the action method should look something like this: 

.. literalinclude:: model-binding/sample/src/MVCMovie/Controllers/MoviesController.cs
   :language: c#
   :lines: 61-76  
   
.. note:: The strings in the URL route are not case sensitive.

MVC checks for model binding attributes applied to the action method's arguments. If it finds a model binding attribute it will use the data source specified by the attribute. Otherwise, for each property in the model it queries the value provider for a value with a matching name. In the above example the only matching item is an integer named `id`, so then the value provider associates its values with action method parameters. Below is a list of the data sources in the order that model binding looks through them:
 
#. ``Form values``: These are form values that go in the HTTP request using the POST method.
#. ``Route values``: Contains information about each parameter of the route. 
#. ``Query strings``: The query string part of the URI.

.. note:: Form values, route data, and query strings are all stored as name-value pairs.

Since model binding asked the form value provider for a key named `id` and didn't find anything, it moved on to the route values looking for a key with a name of `id`. In our example, it's a match. Binding happens, and the value of the id parameter becomes 2. So far the example use primitive types. If the action method's parameter were a complex type such as the ``Movie`` type, which contains both primitive and complex types as properties, MVC's model binding will still handle it nicely. It uses reflection and recursion to traverse the properties of complex types. While doing so, it compares primitive members' names against the names of keys in data sources and recurses through the properties that are complex types. When a value provider delivers the first item with a matching name from the data source, it stops looking for more items with that name. If binding can't occur, the error is recorded in the ``ModelState.Errors`` collection, and model binding attempts to bind to the next parameter or property. You can query for model state errors by checking the ``ModelState.IsValid`` method. 

.. Note:: The model state errors are contained in a ``ModelStateDictionary`` object that contains name-value pairs of items. It's rarely necessary to query this collection yourself; if there are items in this collection, then ``ModelState.IsValid`` returns ``false``. 

Additionally, there are some special data types that ASP.NET MVC must consider when performing model binding:

- ``IFormFile``: Uploaded files that are part of the HTTP request.
- ``IFormCollection``: The collection of key-value pairs that represent the form data.
- ``CancelationToken``: Used to cancel activity in asynchronous controllers.

Once model binding is complete, `validation <https://docs.asp.net/projects/mvc/en/latest/models/validation.html>`_ occurs. Default Model binding works great for the vast majority of development scenarios. It is also extensible so if you have unique needs you can customize the built-in behavior.  

Customize model binding behavior with attributes 
--------------------------------------------------------
ASP.NET MVC contains several attributes that you can use to direct its default model binding behavior to a different source. For example, you can specify whether binding is required for a field, or if it should never happen at all by using the ``[BindRequired]`` or ``[BindNever]`` attributes. Alternatively, you can override the default value, and specify the model binder's data source, which is what the ``[From*]`` attributes do, such as the ``[FromForm]`` or ``[FromQuery]``. While the topic of model binding extensibility in depth is outside of the scope of this doc, here are a few customizations that you should be aware of:

- ``[BindRequired]``: This attribute forces a model state error if binding cannot occur.
- ``[BindNever]``: Tells the model binder to never bind to this parameter.
- ``[FromHeader]``, ``[FromQuery]``, ``[FromRoute]``, ``[FromForm]``: Use these to specify the exact binding source you want to apply.
- ``[FromServices]``: This attribute uses `dependency injection <https://docs.asp.net/en/latest/fundamentals/dependency-injection.html>`_ to bind parameters from services.
- ``[FromBody]``: Use the configured formatters to bind data from the request body. The formatter is selected based on content type of the request.
- ``[ModelBinder]``: Used to override the default model binder, binding source and name.

Binding attributes are very helpful tools when you need to override the default behavior of model binding.

Binding formatted data from the request body
--------------------------------------------
Request data can come in a variety of formats including JSON, XML and many others. When you use the [FrommBody] attribute to indicate that you want to bind a parameter to data in the request body, MVC uses a configured set of formatters to handle the request data based on its content type. By default MVC includes a ``JsonInputFormatter`` class for handling JSON data, but you can add additional formatters for handling XML and other custom formats. 

.. Note:: The ``JsonInputFormatter`` is based off of Json.NET.

ASP.NET selects input formatters based on the Content-Type header and the type of the parameter, unless it has a model binding attribute specifying otherwise. If you'd like to use XML or another format you must configure it in the Startup.cs file. You may have to install a NuGet package for the formatter you want. Your startup code should look something like this:

.. code:: c#

       public void ConfigureServices(IServiceCollection services)
       {
         services.AddMvc(options =>
         {
            options.Filters.Add(new FormatFilterAttribute());
         })
		 .AddXmlSerializerFormatters()
         .AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder);
       {

Code in the Startup.cs file contains a ``ConfigureServices`` method with a ``services`` argument you can use to build up services for your ASP.NET app. In the sample, we are adding an XML formatter to the list of services MVC will provide for this app.

Resources
---------
Below are links to related routing and formatting docs:

- `Routing to controller actions <https://docs.asp.net/projects/mvc/en/latest/controllers/routing.html>`_
- `Formatting <https://docs.asp.net/projects/mvc/en/latest/models/formatting.html>`_