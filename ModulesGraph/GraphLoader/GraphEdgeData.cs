using System.Text;

namespace ModulesGraphDesktopApp
{
  /// <summary>
  /// Данные, ассоциированные со связью графа.
  /// </summary>
  public class GraphEdgeData
  {
    /// <summary>
    /// Информация о зависимостях.
    /// </summary>
    public List<DependencyInfo> DependencyInfos { get; set; } = new List<DependencyInfo>();

    /// <summary>
    /// Преобразовать информацию о зависимостях в html.
    /// </summary>
    /// <returns></returns>
    public string ToHtml()
    {
      var header = "<html><head><meta charset=\"utf-8\"></head><body style=\"overflow-x: hidden; overflow-y: hidden;\"><ul>";
      var content = new StringBuilder();
      foreach (var dependencyInfo in this.DependencyInfos)
        content.Append(dependencyInfo.ToHtmlFragment());
      var footer = "</ul></body></html>";
      return header + content.ToString() + footer;
    }
  }

  /// <summary>
  /// Информация о зависимости.
  /// </summary>
  public abstract class DependencyInfo
  {
    /// <summary>
    /// Преобразовать информацию о зависимости в html.
    /// </summary>
    /// <returns>Html-фрагмент.</returns>
    public abstract string ToHtmlFragment();
  }

  /// <summary>
  /// Информация о явной зависимости.
  /// </summary>
  public class ExplicitDependencyInfo : DependencyInfo
  {
    public override string ToHtmlFragment()
    {
      return "<li>явная зависимость</li>";
    }
  }

  /// <summary>
  /// Информация о зависимости через блок.
  /// </summary>
  public class BlockDependencyInfo : DependencyInfo
  {
    /// <summary>
    /// NameGuid блока.
    /// </summary>
    public string? BlockNameGuid { get; set; }

    /// <summary>
    /// NameGuid базового блока.
    /// </summary>
    public string? BlockBaseGuid { get; set; }

    public override string ToHtmlFragment()
    {
      return $"<li>Зависимость блока ({this.BlockNameGuid}) от базового блока ({this.BlockBaseGuid}).</li>";
    }
  }


  /// <summary>
  /// Информация о зависимости через перекрытый модуль.
  /// </summary>
  public class LayeredModuleDependencyInfo : DependencyInfo
  { 
    /// <summary>
    /// NameGuid модуля.
    /// </summary>
    public string? LayeredModuleNameGuid { get; set; }

    /// <summary>
    /// NameGuid базового модуля.
    /// </summary>
    public string? LayeredModuleBaseGuid { get; set; }

    public override string ToHtmlFragment()
    {
      return $"<li>Зависимость перекрытого модуля {this.LayeredModuleNameGuid} от базового модуля {this.LayeredModuleBaseGuid}.</li>";
    }
  }

  /// <summary>
  /// Информация о зависимости по наследованию.
  /// </summary>
  public class InheritanceDependencyInfo : DependencyInfo
  {
    /// <summary>
    /// NameGuid элемента, который унаследовался от другого модуля.
    /// </summary>
    public string? ItemNameGuid { get; set; }

    /// <summary>
    /// BaseGuid элемента, который унаследовался от другого модуля.
    /// </summary>
    public string? ItemBaseGuid { get; set; }

    public override string ToHtmlFragment()
    {
      return $"<li>Зависимость по наследованию типа сущности {this.ItemNameGuid} от типа сущности {this.ItemBaseGuid}</li>";
    }
  }

  /// <summary>
  /// Информация о зависимости через действие обложки.
  /// </summary>
  public class CoverActionDependencyInfo : DependencyInfo
  {
    /// <summary>
    /// Название действия.
    /// </summary>
    public string? ActionName { get; set; }

    /// <summary>
    /// Тип сущности действия.
    /// </summary>
    public string? ActionEntityTypeId { get; set; }

    public override string ToHtmlFragment()
    {
      return $"<li>Зависимость через действие обложки { this.ActionName }, ссылающееся на тип сущности { this.ActionEntityTypeId }</li>";
    }
  }

  /// <summary>
  /// Информация о зависимости через свойство-ссылку.
  /// </summary>
  public class NavigationPropertyDependencyInfo : DependencyInfo
  {
    /// <summary>
    /// Название свойства.
    /// </summary>
    public string? PropertyName { get; set; } 

    /// <summary>
    /// NameGuid свойства.
    /// </summary>
    public string? PropertyNameGuid { get; set; } 

    /// <summary>
    /// Тип сущности свойства-ссылки.
    /// </summary>
    public string? PropertyEntityGuid { get; set; }

    public override string ToHtmlFragment()
    {
      return $"<li>Зависимость через свойство-ссылку { this.PropertyName } ({ this.PropertyNameGuid }), ссылающееся на тип сущности { this.PropertyEntityGuid }</li>";
    }
  }

  /// <summary>
  /// Информация о зависимости через специальную папку.
  /// </summary>
  public class SpecialFolderDependencyInfo : DependencyInfo
  {
    /// <summary>
    /// Название папки.
    /// </summary>
    public string? FolderName { get; set; } 

    /// <summary>
    /// NameGuid папки.
    /// </summary>
    public string? FolderNameGuid { get; set; }

    /// <summary>
    /// Тип содержимого папки.
    /// </summary>
    public string? ContentType { get; set; }

    public override string ToHtmlFragment()
    {
      return $"<li>Зависимость через папку {this.FolderName} ({this.FolderNameGuid}) с типом содержимого {this.ContentType}</li>";
    }
  }

  /// <summary>
  /// Информация о зависимости через виджет.
  /// </summary>
  public class WidgetDpendencyInfo : DependencyInfo
  {
    /// <summary>
    /// Название элемента виджета.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// NameGuid элемента виджета.
    /// </summary>
    public string? NameGuid { get; set; }

    /// <summary>
    /// Тип сущности элемента виджета.
    /// </summary>
    public string? EntityGuid { get; set; }

    public override string ToHtmlFragment()
    {
      return $"<li>Зависимость через элемент виджета {this.Name} ({this.NameGuid}) с типом содержимого {this.EntityGuid}</li>";
    }
  }
}
