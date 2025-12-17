using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReverseGanttChart.Migrations
{
    /// <inheritdoc />
    public partial class AddProj3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_Projects_ProjectId1",
                table: "ProjectTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskStages_Users_CompletedById",
                table: "TaskStages");

            migrationBuilder.DropTable(
                name: "StageTeamAssignments");

            migrationBuilder.DropTable(
                name: "TaskTeamAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TaskStages_CompletedById",
                table: "TaskStages");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTasks_ProjectId1",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "TaskStages");

            migrationBuilder.DropColumn(
                name: "CompletedById",
                table: "TaskStages");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "TaskStages");

            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "ProjectId1",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProjectTasks");

            migrationBuilder.CreateTable(
                name: "TeamStageProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StageId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TeamId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsCompleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CompletedById = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamStageProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamStageProgress_TaskStages_StageId",
                        column: x => x.StageId,
                        principalTable: "TaskStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamStageProgress_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamStageProgress_Users_CompletedById",
                        column: x => x.CompletedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TeamTaskProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TaskId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TeamId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CompletedById = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamTaskProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamTaskProgress_ProjectTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "ProjectTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamTaskProgress_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamTaskProgress_Users_CompletedById",
                        column: x => x.CompletedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TeamStageProgress_CompletedById",
                table: "TeamStageProgress",
                column: "CompletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TeamStageProgress_StageId_TeamId",
                table: "TeamStageProgress",
                columns: new[] { "StageId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamStageProgress_TeamId",
                table: "TeamStageProgress",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamTaskProgress_CompletedById",
                table: "TeamTaskProgress",
                column: "CompletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TeamTaskProgress_TaskId_TeamId",
                table: "TeamTaskProgress",
                columns: new[] { "TaskId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamTaskProgress_TeamId",
                table: "TeamTaskProgress",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamStageProgress");

            migrationBuilder.DropTable(
                name: "TeamTaskProgress");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "TaskStages",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompletedById",
                table: "TaskStages",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "TaskStages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "ProjectTasks",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId1",
                table: "ProjectTasks",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ProjectTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                name: "IX_TaskStages_CompletedById",
                table: "TaskStages",
                column: "CompletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_ProjectId1",
                table: "ProjectTasks",
                column: "ProjectId1");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_Projects_ProjectId1",
                table: "ProjectTasks",
                column: "ProjectId1",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStages_Users_CompletedById",
                table: "TaskStages",
                column: "CompletedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
