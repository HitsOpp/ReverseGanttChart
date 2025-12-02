using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReverseGanttChart.Migrations
{
    /// <inheritdoc />
    public partial class AddProj2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StageTeamAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StageId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TeamId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AssignedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageTeamAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StageTeamAssignments_TaskStages_StageId",
                        column: x => x.StageId,
                        principalTable: "TaskStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StageTeamAssignments_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TaskTeamAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TaskId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TeamId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AssignedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTeamAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskTeamAssignments_ProjectTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "ProjectTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskTeamAssignments_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_StageTeamAssignments_StageId_TeamId",
                table: "StageTeamAssignments",
                columns: new[] { "StageId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StageTeamAssignments_TeamId",
                table: "StageTeamAssignments",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTeamAssignments_TaskId_TeamId",
                table: "TaskTeamAssignments",
                columns: new[] { "TaskId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskTeamAssignments_TeamId",
                table: "TaskTeamAssignments",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StageTeamAssignments");

            migrationBuilder.DropTable(
                name: "TaskTeamAssignments");
        }
    }
}
