using Newtonsoft.Json.Linq;

namespace ModulesGraphDesktopApp
{
  /// <summary>
  /// Метаданные элемента модуля.
  /// </summary>
  public class ModuleItemMetadata
  {
    /// <summary>
    /// Метаданные в виде JSON-объекта.
    /// </summary>
    public JObject? Metadata { get; set; }
  }
}
