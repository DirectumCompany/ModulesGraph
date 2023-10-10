using Newtonsoft.Json.Linq;

namespace ModulesGraphDesktopApp
{
  /// <summary>
  /// Метаданные модуля.
  /// </summary>
  public class ModuleMetadata
  {
    /// <summary>
    /// Метаданные модуля в виде JSON-объекта.
    /// </summary>
    public JObject? Metadata { get; set; }

    /// <summary>
    /// Список элементов модуля.
    /// </summary>
    public List<ModuleItemMetadata>? Items { get; set; }
  }
}
