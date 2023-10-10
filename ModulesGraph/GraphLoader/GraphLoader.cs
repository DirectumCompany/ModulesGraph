using Newtonsoft.Json.Linq;

namespace ModulesGraphDesktopApp
{
  /// <summary>
  /// Загрузчик графа.
  /// </summary>
  public class GraphLoader
  {
    /// <summary>
    /// Загрузить элементы модуля.
    /// </summary>
    /// <param name="moduleFolder">Родительская папка модуля.</param>
    /// <returns>Список элементов модуля.</returns>
    private static List<ModuleItemMetadata> LoadItems(string moduleFolder)
    {
      var result = new List<ModuleItemMetadata>();
      foreach (var directory in Directory.EnumerateDirectories(moduleFolder, "*", new EnumerationOptions { RecurseSubdirectories = true }))
      {
        foreach (var fileName in Directory.EnumerateFiles(directory, "*.mtd"))
        {
          var jobject = JObject.Parse(File.ReadAllText(fileName));
          var item = new ModuleItemMetadata() { Metadata = jobject };
          result.Add(item);
        }
      }
      return result;
    }

    /// <summary>
    /// Загрузить модули.
    /// </summary>
    /// <param name="parentFolder">Родительский каталог.</param>
    /// <returns>Список модулей.</returns>
    private static List<ModuleMetadata> LoadModules(string parentFolder)
    {
      var result = new List<ModuleMetadata>();
      foreach (var directory in Directory.EnumerateDirectories(parentFolder, "*", new EnumerationOptions { RecurseSubdirectories = true }))
      {
        if (!directory.Contains("VersionData"))
        {
          foreach (var fileName in Directory.EnumerateFiles(directory, "Module.mtd"))
          {
            var jobject = JObject.Parse(File.ReadAllText(fileName));
            var items = LoadItems(directory);
            var module = new ModuleMetadata() { Metadata = jobject, Items = items };
            result.Add(module);
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Добавить узлы модулей.
    /// </summary>
    /// <param name="graph">Граф.</param>
    /// <param name="modules">Список модулей.</param>
    private static void AddNodes(Microsoft.Msagl.Drawing.Graph graph, List<ModuleMetadata> modules)
    {
      foreach (var module in modules)
      {
        var moduleName = module.Metadata?["Name"]?.ToString() ?? "";
        if (moduleName != "DirectumRX")
        {
          var node = new Microsoft.Msagl.Drawing.Node(moduleName);
          node.UserData = module;
          graph.AddNode(node);
        }
      }
    }

    /// <summary>
    /// Найти метаданные модуля.
    /// </summary>
    /// <param name="nameGuid">NameGuid типа сущности.</param>
    /// <param name="modules">Список модулей.</param>
    /// <returns>Метаданные найденного модуля.</returns>
    private static ModuleMetadata? FindModuleMetadata(string nameGuid, List<ModuleMetadata> modules)
    {
      foreach (var module in modules)
      {
        foreach (var item in module.Items ?? new List<ModuleItemMetadata>())
        {
          if (item.Metadata?["NameGuid"]?.ToString() == nameGuid)
            return module;
        }
      }
      return null;
    }

    /// <summary>
    /// Добавить список явных зависимостей.
    /// </summary>
    /// <param name="graph">Граф.</param>
    /// <param name="modules">Список модулей.</param>
    private static void AddExplicitDependencies(Microsoft.Msagl.Drawing.Graph graph, List<ModuleMetadata> modules)
    {
      foreach (var module in modules)
      {
        var source = module.Metadata?["Name"]?.ToString() ?? "";
        var sourceNode = graph.Nodes.FirstOrDefault(node => node.Id == source);
        var dependencies = module.Metadata?["Dependencies"];
        if (dependencies != null)
        {
          foreach (var dependency in dependencies)
          {
            var targetId = dependency["Id"]?.ToString() ?? "";
            var targetNode = graph.Nodes.FirstOrDefault(node => ((ModuleMetadata)node.UserData).Metadata?["NameGuid"]?.ToString() == targetId);
            if (!string.IsNullOrEmpty(source) && sourceNode != null && targetNode != null)
            {
              Microsoft.Msagl.Drawing.Edge? edge = graph.Edges.FirstOrDefault(edge => edge.SourceNode.Id == sourceNode.Id && edge.TargetNode.Id == targetNode.Id);

              if (edge == null)
                edge = graph.AddEdge(sourceNode.Id, targetNode.Id);

              GraphEdgeData graphEdgeData = (GraphEdgeData)edge.UserData;
              if (graphEdgeData == null)
                graphEdgeData = new GraphEdgeData();

              graphEdgeData.DependencyInfos.Add(new ExplicitDependencyInfo());
              edge.UserData = graphEdgeData;
            }
          }
        }
      }
    }

    /// <summary>
    /// Добавить зависимость между модулями.
    /// </summary>
    /// <param name="graph">Граф.</param>
    /// <param name="sourceModule">Исходный модуль.</param>
    /// <param name="targetModule">Целевой модуль.</param>
    /// <param name="dependencyInfo">Информация о зависимости.</param>
    private static void AddEdgeBetweenModules(Microsoft.Msagl.Drawing.Graph graph, ModuleMetadata sourceModule, ModuleMetadata targetModule, DependencyInfo dependencyInfo)
    {
      var sourceId = sourceModule.Metadata?["Name"]?.ToString();
      var targetId = targetModule.Metadata?["Name"]?.ToString();
      if (sourceId != targetId)
      {
        Microsoft.Msagl.Drawing.Edge? edge = graph.Edges.FirstOrDefault(edge => edge.SourceNode.Id == sourceId && edge.TargetNode.Id == targetId);

        if (edge == null)
          edge = graph.AddEdge(sourceId, targetId);

        GraphEdgeData edgeData = (GraphEdgeData)edge.UserData;
        if (edgeData == null)
          edgeData = new GraphEdgeData();

        edgeData.DependencyInfos.Add(dependencyInfo);
        edge.UserData = edgeData;
      }
    }

    /// <summary>
    /// Добавить зависимости по наследованию.
    /// </summary>
    /// <param name="graph">Граф.</param>
    /// <param name="modules">Список модулей.</param>
    private static void AddInheritanceDependencies(Microsoft.Msagl.Drawing.Graph graph, List<ModuleMetadata> modules)
    {
      foreach (var module in modules)
      {
        foreach (var item in module.Items ?? new List<ModuleItemMetadata>())
        {
          var nameGuid = item.Metadata?["NameGuid"]?.ToString();
          var baseGuid = item.Metadata?["BaseGuid"]?.ToString() ?? Guid.Empty.ToString();
          var parentModule = FindModuleMetadata(baseGuid, modules);
          if (parentModule != null)
          {
            var dependencyInfo = new InheritanceDependencyInfo() { ItemNameGuid = nameGuid, ItemBaseGuid = baseGuid };
            AddEdgeBetweenModules(graph, module, parentModule, dependencyInfo);
          }
        }
      }
    }

    /// <summary>
    /// Добавить зависимости через действие на обложке модуля.
    /// </summary>
    /// <param name="graph">Граф.</param>
    /// <param name="modules">Список модулей.</param>
    private static void AddDependenciesByCoverActions(Microsoft.Msagl.Drawing.Graph graph, List<ModuleMetadata> modules)
    {
      foreach (var module in modules)
      {
        var actions = module.Metadata?["Cover"]?["Actions"];
        if (actions != null)
        {
          foreach (var action in actions)
          {
            var actionName = action["Name"]?.ToString() ?? String.Empty;
            var entityTypeId = action["EntityTypeId"]?.ToString() ?? String.Empty;
            if (!string.IsNullOrEmpty(entityTypeId))
            {
              var linkedModule = FindModuleMetadata(entityTypeId, modules);
              if (linkedModule != null)
              {
                var dependencyInfo = new CoverActionDependencyInfo() { ActionName = actionName, ActionEntityTypeId = entityTypeId };
                AddEdgeBetweenModules(graph, module, linkedModule, dependencyInfo);
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Добавить зависимости через свойства-ссылки.
    /// </summary>
    /// <param name="graph">Граф.</param>
    /// <param name="modules">Список модулей.</param>
    private static void AddDependenciesByNavigationProperties(Microsoft.Msagl.Drawing.Graph graph, List<ModuleMetadata> modules)
    {
      foreach (var module in modules)
      {
        foreach (var item in module.Items ?? new List<ModuleItemMetadata>())
        {
          var properties = item.Metadata?["Properties"] ?? new JObject();
          foreach (var property in properties)
          {
            var propertyName = property?["Name"]?.ToString();
            var propertyNameGuid = property?["NameGuid"]?.ToString();
            if (property?["$type"]?.ToString() == "Sungero.Metadata.NavigationProperty, Sungero.Metadata" &&
                property?["IsAncestorMetadata"]?.ToString() != "true")
            {
              var linkedEntityType = property?["EntityGuid"]?.ToString() ?? Guid.Empty.ToString();
              var linkedModule = FindModuleMetadata(linkedEntityType, modules);
              if (linkedModule != null)
              {
                var dependencyInfo =
                  new NavigationPropertyDependencyInfo() { PropertyName = propertyName, PropertyNameGuid = propertyNameGuid, PropertyEntityGuid = linkedEntityType };
                AddEdgeBetweenModules(graph, module, linkedModule, dependencyInfo);
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Добавить зависимости через папки модуля.
    /// </summary>
    /// <param name="graph">Граф.</param>
    /// <param name="modules">Список модулей.</param>
    private static void AddDependenciesBySpecialFolders(Microsoft.Msagl.Drawing.Graph graph, List<ModuleMetadata> modules)
    {
      foreach (var module in modules)
      {
        var specialFolders = module.Metadata?["SpecialFolders"] ?? new JObject();
        foreach (var specialFolder in specialFolders)
        {
          var folderName = specialFolder?["Name"]?.ToString() ?? String.Empty.ToString();
          var folderNameGuid = specialFolder?["NameGuid"]?.ToString() ?? Guid.Empty.ToString();
          var defaultContentType = specialFolder?["DefaultContentType"]?.ToString() ?? Guid.Empty.ToString();
          var linkedModule = FindModuleMetadata(defaultContentType, modules);
          if (linkedModule != null)
          {
            var dependencyInfo =
              new SpecialFolderDependencyInfo() { FolderName = folderName, FolderNameGuid = folderNameGuid, ContentType = defaultContentType };
            AddEdgeBetweenModules(graph, module, linkedModule, dependencyInfo);
          }
        }
      }
    }

    /// <summary>
    /// Добавить зависимости через виджеты.
    /// </summary>
    /// <param name="graph">Граф.</param>
    /// <param name="modules">Список модулей.</param>
    private static void AddDependenciesByWidgets(Microsoft.Msagl.Drawing.Graph graph, List<ModuleMetadata> modules)
    {
      foreach (var module in modules)
      {
        var widgets = module.Metadata?["Widgets"] ?? new JObject();
        foreach (var widget in widgets)
        {
          var items = widget?["WidgetItems"] ?? new JObject();
          foreach (var item in items)
          {
            var name = item?["Name"]?.ToString() ?? String.Empty;
            var nameGuid = item?["NameGuid"]?.ToString() ?? Guid.Empty.ToString();
            var entityGuid = item?["EntityGuid"]?.ToString() ?? "";
            var linkedModule = FindModuleMetadata(entityGuid, modules);
            if (linkedModule != null)
            {
              var dependencyInfo =
                new WidgetDpendencyInfo() { Name = name, NameGuid = nameGuid, EntityGuid = entityGuid };
              AddEdgeBetweenModules(graph, module, linkedModule, dependencyInfo);
            }
          }
        }
      }
    }

    /// <summary>
    /// Загрузить граф.
    /// </summary>
    /// <param name="parentFolder">Родительская папка.</param>
    /// <returns>Загруженный граф.</returns>
    public static Microsoft.Msagl.Drawing.Graph Load(string parentFolder)
    {
      Microsoft.Msagl.Drawing.Graph result = new Microsoft.Msagl.Drawing.Graph("graph");
      List<ModuleMetadata> modules = LoadModules(parentFolder);

      AddNodes(result, modules);
      AddExplicitDependencies(result, modules);
      AddInheritanceDependencies(result, modules);
      AddDependenciesByCoverActions(result, modules);
      AddDependenciesByNavigationProperties(result, modules);
      AddDependenciesBySpecialFolders(result, modules);
      AddDependenciesByWidgets(result, modules);

      return result;
    }
  }
}
