using ASKON_TestTask.Common.Exceptions;
using ASKON_TestTask.Domain;
using ASKON_TestTask.Services;
using GemBox.Document;
using GemBox.Document.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static ASKON_TestTask.FormsHelper;
using DetailInTreeView = ASKON_TestTask.Domain.DetailInTreeView;
using Task = System.Threading.Tasks.Task;

namespace ASKON_TestTask.Forms
{
    public partial class MainForm : Form
    {
        private readonly FormsHelper _formsHelper;
        private readonly IDetailService _detailService;

        private readonly List<ToolStripMenuItem> _contextMenuItemsToHide = new List<ToolStripMenuItem>();
        private TreeNode _selectedNode = new TreeNode();

        private DetailInTreeView SelectedNodeTag => _selectedNode.Tag as DetailInTreeView;

        static ImageList _imageList;
        private static ImageList ImageList
        {
            get
            {
                if (_imageList == null)
                {
                    _imageList = new ImageList();
                    _imageList.Images.Add("NodeIcon", Properties.Resources.gear_icon_64x64);
                }
                return _imageList;
            }
        }

        public MainForm(
            FormsHelper formsHelper,
            IDetailService detailService)
        {
            _formsHelper = formsHelper;
            _detailService = detailService;

            InitializeComponent();

            InitContextMenu();

            treeView.NodeMouseClick += (sender, args) => treeView.SelectedNode = args.Node;

            treeView.ImageList = ImageList;
        }

        private async void MainForm_LoadAsync(object sender, EventArgs e)
        {
            await LoadParentDetailsWithFirstChildrenAsync();
        }

        private async void treeView_BeforeExpandAsync(object sender, TreeViewCancelEventArgs e)
        {
            // Loading third generation children nodes and adding them to the tree
            if (e.Node.Tag != null)
            {
                _selectedNode = e.Node;

                var selectedDetailTag = SelectedNodeTag;

                if (selectedDetailTag.IsThirdGenLoaded)
                {
                    return;
                }

                await LoadThirdGenChildrenAsync(_selectedNode);

                selectedDetailTag.IsThirdGenLoaded = true;
            }
        }


        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _selectedNode = e.Node;

                foreach (var contextMenuItem in _contextMenuItemsToHide)
                {
                    contextMenuItem.Enabled = true;
                }
            }

        }

        private void contextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            foreach (var contextMenuItem in _contextMenuItemsToHide)
            {
                contextMenuItem.Enabled = false;
            }
        }


        private async void contextMenu_ItemClickedAsync(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.AccessibleName)
            {
                case "contextMenuItemRename":
                    {
                        await RenameDetailAsync(SelectedNodeTag);

                        break;
                    }

                case "contextMenuItemDelete":
                    {
                        await DeleteDetailAsync(SelectedNodeTag);

                        break;
                    }

                case "contextMenuItemExport":
                    {
                        await CreateReportForDetailAsync(SelectedNodeTag);

                        break;

                    }
            }
        }

        private async void contextMenu_DropDownItemClickedAsync(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.AccessibleName)
            {
                case "contextMenuItemAddParent":

                    await AddParentDetailAsync();

                    break;

                case "contextMenuItemAddChild":
                    {
                        await AddChildDetailAsync(_selectedNode);

                        break;
                    }

            }

        }


        #region TreeViewOperations

        private async Task LoadParentDetailsWithFirstChildrenAsync()
        {
            var firstGenDetails = await _detailService.GetParentDetailsAsync();

            var firstGenDetailsHierarchyIds = firstGenDetails
                .Select(x => x.HierarchyLevel)
                .ToList();


            var secondGenDetails =
                await _detailService.GetDetailsFirstDescendantsAsync(firstGenDetailsHierarchyIds);

            foreach (var firstGenDetail in firstGenDetails)
            {
                var treeNode = new TreeNode()
                {
                    Text = DetailNameInTree(firstGenDetail.Name),
                    Tag = new DetailInTreeView()
                    {
                        DetailId = firstGenDetail.DetailId,
                        Name = firstGenDetail.Name,
                        HierarchyLevel = firstGenDetail.HierarchyLevel
                    }
                };

                var childDetails = secondGenDetails
                    .Where(x => x.HierarchyLevel.IsDescendantOf(firstGenDetail.HierarchyLevel))
                    .ToList();

                foreach (var childDetail in childDetails)
                {
                    treeNode.Nodes.Add(new TreeNode()
                    {
                        Text = DetailNameInTree(childDetail.Name, childDetail.Count),
                        Tag = new DetailInTreeView()
                        {
                            DetailId = childDetail.DetailId,
                            Name = childDetail.Name,
                            Count = childDetail.Count,
                            HierarchyLevel = childDetail.HierarchyLevel
                        }
                    });
                }

                treeView.Nodes.Add(treeNode);
            }
        }

        private async Task LoadThirdGenChildrenAsync(TreeNode selectedNode)
        {
            var childNodes = new List<TreeNode>();

            foreach (TreeNode childNode in selectedNode.Nodes)
            {
                childNodes.Add(childNode);
            }

            // Getting HierarchyLevels of the child nodes
            var childNodesHierarchyLevels = childNodes
                .Where(x => x.Tag is DetailInTreeView)
                .Select(x =>
                {
                    var tag = (DetailInTreeView)x.Tag;
                    return tag.HierarchyLevel;
                })
                .ToList();

            var thirdGenDetailsWithRelations = await _detailService
                .GetDetailsFirstDescendantsAsync(childNodesHierarchyLevels);

            foreach (var childNodesHierarchyLevel in childNodesHierarchyLevels)
            {
                // Across all retrieved third-gen details
                // looking for descendants of the currently iterated child of the selected node
                var thirdGenDetails = thirdGenDetailsWithRelations
                    .Where(x => x.HierarchyLevel.GetAncestor(1)
                        .Equals(childNodesHierarchyLevel));

                if (thirdGenDetails.Any())
                {
                    // If we find ones then retrieving its ancestor's TreeNode object
                    // in order to add objects to the tree control
                    var childNodeToAddNodes =
                        childNodes.SingleOrDefault(x =>
                        {
                            var tag = (DetailInTreeView)x.Tag;
                            return tag.HierarchyLevel.Equals(childNodesHierarchyLevel);
                        });

                    if (childNodeToAddNodes != null)
                    {
                        foreach (var thirdGenDetail in thirdGenDetails)
                        {
                            childNodeToAddNodes.Nodes.Add(new TreeNode()
                            {
                                Text = DetailNameInTree(thirdGenDetail.Name, thirdGenDetail.Count),
                                Tag = new DetailInTreeView()
                                {
                                    DetailId = thirdGenDetail.DetailId,
                                    Name = thirdGenDetail.Name,
                                    Count = thirdGenDetail.Count,
                                    HierarchyLevel = thirdGenDetail.HierarchyLevel
                                }
                            });
                        }
                    }
                }
            }
        }

        private async Task RenameDetailAsync(DetailInTreeView selectedNodeTag)
        {
            // Renaming detail
            using (var renameDetailForm = new RenameDetailForm(_formsHelper))
            {
                if (renameDetailForm.ShowDialog() == DialogResult.OK)
                {
                    var newDetailName = renameDetailForm.EnteredName;

                    var selectedDetailId = selectedNodeTag.DetailId;

                    var updatedDetail = await _detailService
                        .UpdateDetailNameAsync(selectedDetailId, newDetailName);

                    UpdateTreeViewNodesName(
                        treeView.Nodes,
                        updatedDetail.DetailId,
                        newDetailName);

                    ShowMessageDialog("Success renamed!");
                }
            }
        }

        private async Task DeleteDetailAsync(DetailInTreeView selectedNodeTag)
        {
            await _detailService.DeleteDetailWithDescendantsAsync(selectedNodeTag.HierarchyLevel);

            treeView.Nodes.Remove(_selectedNode);

            ShowMessageDialog("Success deleted!");
        }

        private async Task CreateReportForDetailAsync(DetailInTreeView selectedNodeTag)
        {
            var rootDetailId = selectedNodeTag.DetailId;
            var rootHierarchyId = selectedNodeTag.HierarchyLevel;

            var detailsForReport =
                await _detailService.GetDetailsForReportAsync(rootDetailId, rootHierarchyId);

            CreateAndOpenWordDocument(detailsForReport);

            ShowMessageDialog("Success exported!");
        }

        private async Task AddParentDetailAsync()
        {
            using (var addParentForm = new AddParentForm(_formsHelper))
            {
                if (addParentForm.ShowDialog() == DialogResult.OK)
                {
                    // Adding parent component
                    var newDetailName = addParentForm.EnteredName;
                    DetailInTreeView detailInTree = await _detailService.AddParentDetailAsync(newDetailName);

                    if (detailInTree != null)
                    {
                        treeView.Nodes.Add(new TreeNode()
                        {
                            Text = DetailNameInTree(newDetailName),
                            Tag = detailInTree
                        });
                    }

                    ShowMessageDialog("Success added!");
                }
            }
        }

        private async Task AddChildDetailAsync(TreeNode selectedNode)
        {
            using (var addChildDetailForm = new AddChildForm(_formsHelper))
            {
                if (addChildDetailForm.ShowDialog() == DialogResult.OK)
                {
                    List<HierarchyId> childHierarchyIds = null;

                    var childDetailNodes = selectedNode.Nodes;

                    if (childDetailNodes.Count > 0)
                    {
                        childHierarchyIds = new List<HierarchyId>();

                        foreach (TreeNode childNode in selectedNode.Nodes)
                        {
                            var childNodeTag = childNode.Tag as DetailInTreeView;

                            childHierarchyIds.Add(childNodeTag.HierarchyLevel);
                        }
                    }

                    try
                    {
                        var addedChildDetails = await _detailService.AddChildDetailAsync(
                            SelectedNodeTag.DetailId,
                            SelectedNodeTag.HierarchyLevel,
                            childHierarchyIds,
                            addChildDetailForm.EnteredName,
                            addChildDetailForm.CountToAdd);

                        DetailInTreeView addedDetail =
                            addedChildDetails
                                .SingleOrDefault(x => x.Name == addChildDetailForm.EnteredName);

                        if (childHierarchyIds != null)
                        {
                            // Handle case when detail is already present in the selected scope
                            foreach (TreeNode childNode in selectedNode.Nodes)
                            {
                                var childNodeTag = childNode.Tag as DetailInTreeView;

                                if (childNodeTag.HierarchyLevel == addedDetail.HierarchyLevel)
                                {
                                    childNode.Text = DetailNameInTree(addedDetail.Name,
                                        addedDetail.Count);
                                    childNode.Tag = addedDetail;

                                    ShowMessageDialog("Success added" +
                                                      "\n\rto the presented child");

                                    return;
                                }
                            }
                        }

                        var addedNodeIndex = selectedNode.Nodes.Add(new TreeNode
                        {
                            Text = DetailNameInTree(addedDetail.Name, addedDetail.Count),
                            Tag = addedDetail
                        });

                        var addedNode = selectedNode.Nodes[addedNodeIndex];

                        addedChildDetails.ForEach(x =>
                        {
                            if (addedDetail.HierarchyLevel == x.HierarchyLevel.GetAncestor(1))
                            {
                                addedNode.Nodes.Add(
                                    new TreeNode
                                    {
                                        Text = DetailNameInTree(x.Name, x.Count),
                                        Tag = x
                                    });
                            }
                        });

                        ShowMessageDialog("Success added!");
                    }
                    catch (BusinessLogicException e)
                    {
                        ShowMessageDialog(e.Message);
                    }
                }
            }
        }

        private void UpdateTreeViewNodesName(
            TreeNodeCollection nodeCollection,
            int detailId,
            string newDetailName)
        {
            if (nodeCollection.Count > 0)
            {
                foreach (TreeNode treeNode in nodeCollection)
                {
                    UpdateTreeViewNodesName(treeNode.Nodes, detailId, newDetailName);

                    var nodeTag = treeNode.Tag as DetailInTreeView;

                    if (nodeTag.DetailId == detailId)
                    {
                        treeNode.Text = DetailNameInTree(newDetailName, nodeTag?.Count);
                        nodeTag.Name = newDetailName;
                        treeNode.Tag = nodeTag;
                    }
                }
            }
        }

        #endregion


        private void CreateAndOpenWordDocument(List<DetailForReport> detailsForReport)
        {
            var reportsDirectoryPath = Directory.GetCurrentDirectory() + "\\reports";

            if (!Directory.Exists(reportsDirectoryPath))
            {
                Directory.CreateDirectory(reportsDirectoryPath);
            }

            int rowCount = detailsForReport.Count;

            ComponentInfo.SetLicense("FREE-LIMITED-KEY");

            var document = new DocumentModel();

            var section = new Section(document);
            document.Sections.Add(section);

            // Create a table with 100% width.
            var table = new Table(document);
            table.TableFormat.PreferredWidth = new TableWidth(100, TableWidthUnit.Percentage);
            section.Blocks.Add(table);

            for (int r = 0; r < rowCount; r++)
            {
                // Create a row and add it to table.
                var row = new TableRow(document);
                table.Rows.Add(row);

                // Add detail name 
                var cell = new TableCell(document);
                cell.CellFormat.PreferredWidth = new TableWidth(50, TableWidthUnit.Percentage);
                var paragraph = new Paragraph(document, $"{detailsForReport[r].DetailName}");

                cell.Blocks.Add(paragraph);
                row.Cells.Add(cell);

                // Add detail count 
                cell = new TableCell(document);
                cell.CellFormat.PreferredWidth = new TableWidth(50, TableWidthUnit.Percentage);
                paragraph = new Paragraph(document, $"{detailsForReport[r].Count} шт");

                cell.Blocks.Add(paragraph);
                row.Cells.Add(cell);
            }

            var exportTime = DateTime.Now.ToString("dd-MMM-yyyy HH_mm_ss", CultureInfo.InvariantCulture);
            var detailName = SelectedNodeTag.Name;
            var fileName = $"{detailName} {exportTime}.docx";

            var filePath = $"{reportsDirectoryPath}\\{fileName}";

            document.Save(filePath);

            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }


        private string DetailNameInTree(string detailName, int? detailCount = null)
            => detailCount == null ?
                $"{detailName}" :
                $"{detailName} ({detailCount})";

        private void InitContextMenu()
        {
            ToolStripMenuItem item, submenu;

            submenu = new ToolStripMenuItem();
            submenu.Text = "Add";
            submenu.DropDownItemClicked += contextMenu_DropDownItemClickedAsync;

            item = new ToolStripMenuItem()
            {
                Text = "Add parent",
                AccessibleName = "contextMenuItemAddParent",
            };

            submenu.DropDownItems.Add(item);

            item = new ToolStripMenuItem();
            item.Text = "Add child";
            item.AccessibleName = "contextMenuItemAddChild";
            item.Enabled = false;
            _contextMenuItemsToHide.Add(item);
            submenu.DropDownItems.Add(item);

            contextMenu.Items.Add(submenu);

            item = new ToolStripMenuItem();
            item.Text = "Rename";
            item.AccessibleName = "contextMenuItemRename";
            item.Enabled = false;
            _contextMenuItemsToHide.Add(item);
            submenu.DropDownItems.Add(item);

            contextMenu.Items.Add(item);

            item = new ToolStripMenuItem();
            item.Text = "Delete";
            item.AccessibleName = "contextMenuItemDelete";
            item.Enabled = false;
            _contextMenuItemsToHide.Add(item);
            submenu.DropDownItems.Add(item);

            contextMenu.Items.Add(item);

            item = new ToolStripMenuItem();
            item.Text = "Export Report";
            item.AccessibleName = "contextMenuItemExport";
            item.Enabled = false;
            _contextMenuItemsToHide.Add(item);
            submenu.DropDownItems.Add(item);

            contextMenu.Items.Add(item);

            contextMenu.Closed += contextMenu_Closed;
        }
    }
}
