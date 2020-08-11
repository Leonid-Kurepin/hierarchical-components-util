using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComponentsUtil.Persistence.Migrations
{
    public partial class InitDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Details",
                columns: table => new
                {
                    DetailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Details", x => x.DetailId);
                });

            migrationBuilder.CreateTable(
                name: "DetailRelations",
                columns: table => new
                {
                    DetailRelationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DetailId = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: true),
                    HierarchyId = table.Column<HierarchyId>(type: "hierarchyId", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailRelations", x => x.DetailRelationId);
                    table.ForeignKey(
                        name: "FK_DetailRelations_Details_DetailId",
                        column: x => x.DetailId,
                        principalTable: "Details",
                        principalColumn: "DetailId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Details",
                columns: new[] { "DetailId", "Name" },
                values: new object[,]
                {
                    { 1, "Engine #234" },
                    { 2, "Engine #456" },
                    { 3, "Crankshaft" },
                    { 4, "Camshaft" },
                    { 5, "Piston assembly" },
                    { 6, "Piston" },
                    { 7, "Compression ring" },
                    { 8, "Oil scrapper ring" },
                    { 9, "Connecting rod" },
                    { 10, "Cylinder block" }
                });

            migrationBuilder.InsertData(
                table: "DetailRelations",
                columns: new[] { "DetailRelationId", "Count", "DetailId", "HierarchyId" },
                values: new object[,]
                {
                    { 1, null, 1, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/1/") },
                    { 2, null, 2, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/2/") },
                    { 4, 1, 3, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/1/3/") },
                    { 5, 2, 4, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/1/4/") },
                    { 6, 4, 5, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/1/5/") },
                    { 7, 1, 6, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/1/5/6/") },
                    { 8, 2, 7, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/1/5/7/") },
                    { 9, 1, 8, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/1/5/8/") },
                    { 10, 4, 9, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/1/9/") },
                    { 3, 1, 10, Microsoft.EntityFrameworkCore.HierarchyId.Parse("/1/10/") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetailRelations_DetailId",
                table: "DetailRelations",
                column: "DetailId");

            migrationBuilder.CreateIndex(
                name: "RelationHierarchyUnique",
                table: "DetailRelations",
                column: "HierarchyId",
                unique: true)
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailRelations");

            migrationBuilder.DropTable(
                name: "Details");
        }
    }
}
