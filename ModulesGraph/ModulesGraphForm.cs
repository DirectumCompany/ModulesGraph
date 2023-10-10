namespace ModulesGraphDesktopApp
{
  /// <summary>
  /// Форма для отображения графа зависимостей между модулями.
  /// </summary>
  public partial class ModulesGraphForm : Form
  {
    /// <summary>
    /// Папка с разработкой.
    /// </summary>
    private string folder;

    /// <summary>
    /// Просмотрщик графа.
    /// </summary>
    private Microsoft.Msagl.GraphViewerGdi.GViewer? viewer;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public ModulesGraphForm()
    {
      InitializeComponent();

      using (var folderBrowserDialog = new FolderBrowserDialog())
      {
        folderBrowserDialog.Description = "Выберите папку с разработкой";
        folderBrowserDialog.UseDescriptionForTitle = true;
        DialogResult result = folderBrowserDialog.ShowDialog();
        if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
          this.folder = folderBrowserDialog.SelectedPath ?? "";
        else
          this.folder = "";
      }

      this.prepareGraph();
      this.Show();
      this.Activate();
    }

    /// <summary>
    /// Подготовить граф зависимостей.
    /// </summary>
    private void prepareGraph()
    {
      this.viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
      Microsoft.Msagl.Drawing.Graph graph = GraphLoader.Load(this.folder);
      this.viewer.Graph = graph;
      this.SuspendLayout();
      this.viewer.Dock = DockStyle.Fill;
      this.splitContainer.Panel1.Controls.Add(this.viewer);
      this.ResumeLayout();
      this.viewer.MouseClick += Viewer_MouseClick;
    }

    /// <summary>
    /// Обработать щелчок кнопкой мыши в редакторе графа.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Viewer_MouseClick(object? sender, MouseEventArgs e)
    {
      var selectedObject = this.viewer?.SelectedObject;
      if (selectedObject is Microsoft.Msagl.Drawing.Edge selectedEdge)
      {
        var graphEdgeData = (GraphEdgeData)selectedEdge.UserData;
        if (graphEdgeData != null)
        {
          var html = graphEdgeData.ToHtml();
          var tempFileName = Path.GetTempFileName() ?? "";
          File.WriteAllText(tempFileName, html);
          this.webBrowser.Navigate("file:///" + tempFileName);
        }
      }
    }
  }
}