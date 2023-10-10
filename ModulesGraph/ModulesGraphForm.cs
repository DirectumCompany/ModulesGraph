namespace ModulesGraphDesktopApp
{
  /// <summary>
  /// ����� ��� ����������� ����� ������������ ����� ��������.
  /// </summary>
  public partial class ModulesGraphForm : Form
  {
    /// <summary>
    /// ����� � �����������.
    /// </summary>
    private string folder;

    /// <summary>
    /// ����������� �����.
    /// </summary>
    private Microsoft.Msagl.GraphViewerGdi.GViewer? viewer;

    /// <summary>
    /// �����������.
    /// </summary>
    public ModulesGraphForm()
    {
      InitializeComponent();

      using (var folderBrowserDialog = new FolderBrowserDialog())
      {
        folderBrowserDialog.Description = "�������� ����� � �����������";
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
    /// ����������� ���� ������������.
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
    /// ���������� ������ ������� ���� � ��������� �����.
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