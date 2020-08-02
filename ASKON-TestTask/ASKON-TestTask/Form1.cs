using ASKON_TestTask.Domain;
using ASKON_TestTask.Persistence;
using ASKON_TestTask.Persistence.Entities;
using GemBox.Document;
using GemBox.Document.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DetailInTreeView = ASKON_TestTask.Domain.DetailInTreeView;
using Task = System.Threading.Tasks.Task;

namespace ASKON_TestTask
{
    public partial class Form1 : Form
    {
        private List<ToolStripMenuItem> contextMenuItemsToHide = new List<ToolStripMenuItem>();
        private TreeNode selectedNode = new TreeNode();

        private DetailInTreeView SelectedNodeTag
        {
            get => selectedNode.Tag as DetailInTreeView;
            set => selectedNode.Tag = value;
        }

        public Form1()
        {
            InitializeComponent();

            #region InitContextMenu


            ToolStripMenuItem item, submenu;

            submenu = new ToolStripMenuItem();
            submenu.Text = "Add";
            submenu.DropDownItemClicked += contextMenu_DropDownItemClicked;

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
            contextMenuItemsToHide.Add(item);
            submenu.DropDownItems.Add(item);

            contextMenu.Items.Add(submenu);

            item = new ToolStripMenuItem();
            item.Text = "Rename";
            item.AccessibleName = "contextMenuItemRename";
            item.Enabled = false;
            contextMenuItemsToHide.Add(item);
            submenu.DropDownItems.Add(item);

            contextMenu.Items.Add(item);

            item = new ToolStripMenuItem();
            item.Text = "Delete";
            item.AccessibleName = "contextMenuItemDelete";
            item.Enabled = false;
            contextMenuItemsToHide.Add(item);
            submenu.DropDownItems.Add(item);

            contextMenu.Items.Add(item);

            item = new ToolStripMenuItem();
            item.Text = "Export Report";
            item.AccessibleName = "contextMenuItemExport";
            item.Enabled = false;
            contextMenuItemsToHide.Add(item);
            submenu.DropDownItems.Add(item);

            contextMenu.Items.Add(item);

            contextMenu.Closed += contextMenu_Closed;

            #endregion

            treeView.NodeMouseClick += (sender, args) => treeView.SelectedNode = args.Node;
        }


        private async Task DoSomethingAsync()
        {
            using (var context = new TestTaskContext())
            {

                var firstGenRelationsQuery = context.DetailRelations
                    .Where(x => x.HierarchyLevel.GetLevel() == 1);


                var secondGenRelationsQuery = context.DetailRelations
                    .Where(x => x.HierarchyLevel.GetLevel() == 2);

                var firstGenDetailIdsQuery = context.Details
                    .Join(firstGenRelationsQuery,
                        detail => detail.DetailId,
                        firstGenRelation => firstGenRelation.DetailId,
                        (detail, firstGenRelation) => new DetailInTreeView()
                        {
                            DetailId = detail.DetailId,
                            Name = detail.Name,
                            HierarchyLevel = firstGenRelation.HierarchyLevel
                        });

                var firstGenDetails = firstGenDetailIdsQuery.ToList();

                var secondGenRelations = secondGenRelationsQuery
                    .Include(x => x.Detail)
                    .ToList();

                /*
                var secondGenRelations = secondGenRelationsQuery
                    .Include(x=> x.Detail)
                    .ToList();

                var secondGenDetails = context.Details
                    .Where(x => secondGenRelations
                        .Select(r => r.DetailId).Contains(x.DetailId));
                */

                /*
                var secondGenDetailsWithRelations =
                    from detail in context.Details
                    join secondGenRelation in secondGenRelationsQuery
                        on detail.DetailId equals secondGenRelation.DetailId
                    select new
                    {
                        DetailId = detail.DetailId,
                        DetailName = detail.Name,
                        Count = secondGenRelation.Count,
                        HierarchyLevel = secondGenRelation.HierarchyLevel
                    };
                */


                var secondGenDetailsWithRelations = context.Details
                    .Join(secondGenRelationsQuery,
                        detail => detail.DetailId, secondGenRelation => secondGenRelation.DetailId,
                        (detail, secondGenRelation) => new DetailInTreeView()
                        {
                            DetailId = detail.DetailId,
                            Name = detail.Name,
                            Count = secondGenRelation.Count,
                            HierarchyLevel = secondGenRelation.HierarchyLevel
                        })
                    .ToList();

                foreach (var firstGenDetail in firstGenDetails)
                {
                    var treeNode = new TreeNode()
                    {
                        Text = firstGenDetail.Name,
                        Tag = new DetailInTreeView()
                        {
                            DetailId = firstGenDetail.DetailId,
                            Name = firstGenDetail.Name,
                            HierarchyLevel = firstGenDetail.HierarchyLevel
                        }
                    };

                    /*
                        This part doesn't work for some mysterious reason

                        var childDetails = secondGenDetailsWithRelations
                        .Where(x => x.HierarchyLevel.IsDescendantOf(firstGenDetail.HierarchyLevel));
                    */

                    var childDetails = new List<DetailInTreeView>();

                    foreach (var item in secondGenDetailsWithRelations)
                    {
                        if (item.HierarchyLevel.IsDescendantOf(firstGenDetail.HierarchyLevel))
                        {
                            childDetails.Add(item);
                        }
                    }


                    foreach (var childDetail in childDetails)
                    {
                        treeNode.Nodes.Add(new TreeNode()
                        {
                            Text = $"{childDetail.Name} ({childDetail.Count})",
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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DoSomethingAsync();
        }

        private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // Loading third generation children nodes and adding them to the tree
            if (e.Node.Tag != null)
            {
                var selectedDetailTag = SelectedNodeTag;

                if (selectedDetailTag.IsThirdGenLoaded)
                {
                    return;
                }

                using (var context = new TestTaskContext())
                {
                    //var childNodes = e.Node.Nodes;
                    var childNodes = new List<TreeNode>();

                    foreach (TreeNode childNode in e.Node.Nodes)
                    {
                        childNodes.Add(childNode);
                    }

                    // Getting HierarchyLevels of the child nodes
                    var childNodesHierarchyLevels = childNodes
                        .Where(x => x.Tag is DetailInTreeView)
                        .Select(x =>
                        {
                            var tag = (DetailInTreeView) x.Tag;
                            return tag.HierarchyLevel;
                        })
                        .ToList();

                    var targetRelationsQuery = context.DetailRelations
                        .Where(x => childNodesHierarchyLevels.Contains(x.HierarchyLevel.GetAncestor(1)));

                    var thirdGenDetailsWithRelations = context.Details
                        .Join(targetRelationsQuery,
                            detail => detail.DetailId, targetRelation => targetRelation.DetailId,
                            (detail, targetRelation) => new DetailInTreeView()
                            {
                                DetailId = detail.DetailId,
                                Name = detail.Name,
                                Count = targetRelation.Count,
                                HierarchyLevel = targetRelation.HierarchyLevel
                            })
                        .ToList();

                    foreach (var childNodesHierarchyLevel in childNodesHierarchyLevels)
                    {
                        // Across all retrieved third-gen details
                        // looking for the descendants of the children of the selected node
                        var thirdGenDetails = thirdGenDetailsWithRelations
                            .Where(x => x.HierarchyLevel.GetAncestor(1)
                                .Equals(childNodesHierarchyLevel));

                        if (thirdGenDetails.Any())
                        {
                            // If we find ones then retrieving its TreeNode object
                            // in order to add objects to the tree control
                            var childNodeToAddNodes =
                                childNodes.SingleOrDefault(x =>
                                {
                                    var tag = (DetailInTreeView) x.Tag;
                                    return tag.HierarchyLevel.Equals(childNodesHierarchyLevel);
                                });

                            if (childNodeToAddNodes != null)
                            {
                                foreach (var thirdGenDetail in thirdGenDetails)
                                {
                                    childNodeToAddNodes.Nodes.Add(new TreeNode()
                                    {
                                        Text = $"{thirdGenDetail.Name} ({thirdGenDetail.Count})",
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

                selectedDetailTag.IsThirdGenLoaded = true;
            }
        }


        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                selectedNode = e.Node;

                foreach (var contextMenuItem in contextMenuItemsToHide)
                {
                    contextMenuItem.Enabled = true;
                }
            }

        }

        private void contextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            foreach (var contextMenuItem in contextMenuItemsToHide)
            {
                contextMenuItem.Enabled = false;
            }
        }

        private void contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.AccessibleName)
            {
                case "contextMenuItemRename":
                {
                    // Renaming detail
                    var selectedNodeTag = selectedNode.Tag as DetailInTreeView;

                    var newDetailName = "Шатун3";
                    DetailInTreeView detailInTree = null;

                    using (var context = new TestTaskContext())
                    {
                        var isUniqueName = !context.Details.Any(x => x.Name == newDetailName);

                        if (!isUniqueName)
                        {
                            return;
                        }

                        var selectedDetailId = selectedNodeTag.DetailId;

                        var retrievedDetailToUpdate = context.Details
                            .SingleOrDefault(x => x.DetailId == selectedDetailId);

                        if (retrievedDetailToUpdate == null)
                        {
                            return;
                        }

                        retrievedDetailToUpdate.Name = newDetailName;

                        context.SaveChanges();


                        UpdateTreeViewNodesName(
                            treeView.Nodes,
                            retrievedDetailToUpdate.DetailId,
                            newDetailName);
                    }

                    break;
                }

                case "contextMenuItemDelete":
                {
                    // Deleting detail    
                    var selectedNodeTag = selectedNode.Tag as DetailInTreeView;


                    using (var context = new TestTaskContext())
                    {
                        var descendantsRelations = context.DetailRelations
                            .Where(x => x.HierarchyLevel.IsDescendantOf(selectedNodeTag.HierarchyLevel));

                        var descendantsDetailsIds = descendantsRelations
                            .AsNoTracking()
                            .Select(x => x.DetailId)
                            .Distinct()
                            .ToList();

                        context.DetailRelations.RemoveRange(descendantsRelations);

                        context.SaveChanges();

                        var detailIdsWithRelations = context.DetailRelations
                            .Select(x => x.DetailId).Distinct();

                        var detailsToRemove = context.Details
                            .Where(x => descendantsDetailsIds.Contains(x.DetailId) &&
                                        !detailIdsWithRelations.Contains(x.DetailId));

                        context.Details.RemoveRange(detailsToRemove);

                        context.SaveChanges();
                    }

                    treeView.Nodes.Remove(selectedNode);

                    break;
                }

                case "contextMenuItemExport":
                {
                    using (var context = new TestTaskContext())
                    {
                        var rootDetailId = SelectedNodeTag.DetailId;
                        var rootHierarchyId = SelectedNodeTag.HierarchyLevel;

                        /*
                        var details = context.DetailsForReport
                            .FromSqlRaw($"EXECUTE dbo.SelectDetailsForReport {rootDetailId}, N'{rootHierarchyId}'")
                            .ToList();
                        */

                        var childRelations = context.DetailRelations
                            .Include(x => x.Detail)
                            .Where(x => x.HierarchyLevel.IsDescendantOf(rootHierarchyId))
                            .AsNoTracking()
                            .ToList();

                        childRelations
                            .ForEach(x => childRelations
                                .ForEach(y =>
                                    {
                                        if (x.DetailId != rootDetailId &&
                                            y.DetailId != x.DetailId &&
                                            y.HierarchyLevel.IsDescendantOf(x.HierarchyLevel))
                                        {
                                            y.Count = x.Count != null ? y.Count * x.Count : y.Count;
                                        }
                                    }
                                ));

                        var ancestorNodesIds = childRelations
                            .Select(x => x.HierarchyLevel.GetAncestor(1));

                        childRelations
                            .RemoveAll(x => x.Count == null ||
                                            ancestorNodesIds.Contains(x.HierarchyLevel));

                        var detailsForReport = childRelations
                            .GroupBy(x => x.Detail.Name)
                            .Select(
                                g => new DetailForReport
                                {
                                    DetailName = g.First().Detail.Name,
                                    Count = g.Sum(dr => dr.Count)
                                })
                            .ToList();

                            #region CreateWordDocument

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

                            var exportTime = DateTime.Now.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                            var detailName = "Detail Name";
                            var fileName = $"{detailName} {exportTime}.docx";

                            var filePath = $"{reportsDirectoryPath}\\{fileName}";

                            document.Save(filePath);

                            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });

                            #endregion

                            var rs = 1;
                    }

                    break;

                }
            }
        }

        private void contextMenu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.AccessibleName)
            {
                case "contextMenuItemAddParent":

                    // Adding parent component
                    var newDetailName = "Двигатель 336";
                    DetailInTreeView detailInTree = null;

                    using (var context = new TestTaskContext())
                    {
                        var isUniqueName = !context.Details.Any(x => x.Name == newDetailName);

                        if (!isUniqueName)
                        {
                            return;
                        }

                        var addedDetail = context.Details.Add(new Detail()
                        {
                            Name = newDetailName
                        });

                        context.SaveChanges();

                        var addedRelation = context.DetailRelations.Add(new DetailRelation()
                        {
                            DetailId = addedDetail.Entity.DetailId,
                            HierarchyLevel = HierarchyId.Parse($"/{addedDetail.Entity.DetailId}/")
                        });

                        context.SaveChanges();

                        detailInTree = new DetailInTreeView()
                        {
                            DetailId = addedDetail.Entity.DetailId,
                            Name = addedDetail.Entity.Name,
                            HierarchyLevel = addedRelation.Entity.HierarchyLevel
                        };
                    }

                    if (detailInTree != null)
                    {
                        treeView.Nodes.Add(new TreeNode()
                        {
                            Text = newDetailName,
                            Tag = detailInTree
                        });
                    }


                    break;

                case "contextMenuItemAddChild":
                {
                    // Adding child detail
                    var selectedNodeTag = selectedNode.Tag as DetailInTreeView;

                    var detailsIdsForbiddenToAdd = new List<int>()
                    {
                        selectedNodeTag.DetailId
                    };

                    var newChildDetailName = "Шатун";
                    //var newChildDetailName = "Шатун";
                    var countToAdd = 3;

                    DetailInTreeView childDetailInTree = null;

                    using (var context = new TestTaskContext())
                    {
                        var retrievedDetail = context.Details.SingleOrDefault(x => x.Name == newChildDetailName);

                        HierarchyId hierarchyIdToAdd;
                        //EntityEntry<Detail> addedDetail;
                        //EntityEntry<Detail> addedDetailRelation;

                        // Handle case when there is already a detail with such name in db
                        if (retrievedDetail != null)
                        {
                            var selectedNodeLevel = selectedNodeTag.HierarchyLevel.GetLevel();

                            var ancestorsLevels = new List<HierarchyId>();

                            for (int i = 1; i < selectedNodeLevel; i++)
                            {
                                ancestorsLevels.Add(selectedNodeTag.HierarchyLevel.GetAncestor(i));
                            }

                            var retrievedForbiddenIds = context.DetailRelations
                                .Where(x => ancestorsLevels.Contains(x.HierarchyLevel))
                                .Select(x => x.DetailId)
                                .ToList();

                            detailsIdsForbiddenToAdd.AddRange(retrievedForbiddenIds);

                            if (!detailsIdsForbiddenToAdd.Contains(retrievedDetail.DetailId))
                            {
                                // Handle case when detail is already present in the selected scope
                                hierarchyIdToAdd = HierarchyId.Parse(
                                    selectedNodeTag.HierarchyLevel.ToString() + $"{retrievedDetail.DetailId}/");

                                var isDetailAdded = false;

                                foreach (TreeNode childTreeNode in selectedNode.Nodes)
                                {
                                    var tag = childTreeNode.Tag as DetailInTreeView;

                                    if (tag.HierarchyLevel.Equals(hierarchyIdToAdd))
                                    {
                                        var relationToUpdate = context.DetailRelations
                                            .SingleOrDefault(x => x.HierarchyLevel.Equals(hierarchyIdToAdd));

                                        relationToUpdate.Count += countToAdd;

                                        context.SaveChanges();

                                        tag.Count = relationToUpdate.Count;
                                        childTreeNode.Text = $"{tag.Name} ({tag.Count})";
                                        isDetailAdded = true;
                                    }
                                }

                                if (isDetailAdded)
                                {
                                    return;
                                }

                                var addedDetailRelation = context.DetailRelations.Add(new DetailRelation()
                                {
                                    DetailId = retrievedDetail.DetailId,
                                    Count = countToAdd,
                                    HierarchyLevel = HierarchyId.Parse(
                                        selectedNodeTag.HierarchyLevel.ToString() + $"{retrievedDetail.DetailId}/")
                                });

                                context.SaveChanges();

                                childDetailInTree = new DetailInTreeView()
                                {
                                    DetailId = retrievedDetail.DetailId,
                                    Name = retrievedDetail.Name,
                                    Count = addedDetailRelation.Entity.Count,
                                    HierarchyLevel = addedDetailRelation.Entity.HierarchyLevel
                                };

                                selectedNode.Nodes.Add(new TreeNode()
                                {
                                    Text = $"{childDetailInTree.Name} ({childDetailInTree.Count})",
                                    Tag = childDetailInTree
                                });
                            }

                            return;
                        }

                        if (retrievedDetail == null)
                        {
                            // Handle case when there is no detail with such name in db
                            var addedDetail = context.Details.Add(new Detail()
                            {
                                Name = newChildDetailName
                            });

                            context.SaveChanges();

                            hierarchyIdToAdd = HierarchyId.Parse(
                                selectedNodeTag.HierarchyLevel.ToString() + $"{addedDetail.Entity.DetailId}/");

                            var addedRelation = context.DetailRelations.Add(new DetailRelation()
                            {
                                DetailId = addedDetail.Entity.DetailId,
                                Count = countToAdd,
                                HierarchyLevel = hierarchyIdToAdd
                            });

                            context.SaveChanges();

                            childDetailInTree = new DetailInTreeView()
                            {
                                DetailId = addedDetail.Entity.DetailId,
                                Name = addedDetail.Entity.Name,
                                Count = addedRelation.Entity.Count,
                                HierarchyLevel = addedRelation.Entity.HierarchyLevel
                            };

                            selectedNode.Nodes.Add(new TreeNode()
                            {
                                Text = $"{childDetailInTree.Name} ({childDetailInTree.Count})",
                                Tag = childDetailInTree
                            });
                        }
                    }


                    break;
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
                        var detailNameCountSuffix = nodeTag?.Count == null ? "" : $" ({nodeTag.Count})";
                        treeNode.Text = newDetailName + detailNameCountSuffix;
                        nodeTag.Name = newDetailName;
                        treeNode.Tag = nodeTag;
                    }
                }
            }
        }
    }
}
