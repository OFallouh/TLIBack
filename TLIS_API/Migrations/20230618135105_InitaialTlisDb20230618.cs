using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Oracle.EntityFrameworkCore.Metadata;

namespace TLIS_API.Migrations
{
    public partial class InitaialTlisDb20230618 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TLIaction",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Proposal = table.Column<int>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIaction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIactor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIactor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIarea",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    AreaName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIarea", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIasType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Disable = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIasType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIattributeActivated",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(nullable: true),
                    Label = table.Column<string>(nullable: true),
                    Tabel = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Required = table.Column<bool>(nullable: false, defaultValue: false),
                    Manage = table.Column<bool>(nullable: false, defaultValue: false),
                    enable = table.Column<bool>(nullable: false, defaultValue: true),
                    AutoFill = table.Column<bool>(nullable: false, defaultValue: false),
                    DataType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIattributeActivated", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIbaseBU",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    Disable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIbaseBU", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIbaseCivilWithLegsType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIbaseCivilWithLegsType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIbaseGeneratorType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIbaseGeneratorType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIbaseType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    Disable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIbaseType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIboardType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIboardType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIcabinetPowerType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Disable = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcabinetPowerType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIcapacity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Disable = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcapacity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIcity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIcivilNonSteelType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Disable = table.Column<bool>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcivilNonSteelType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIcivilSteelSupportCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcivilSteelSupportCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIcivilWithoutLegCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcivilWithoutLegCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIcondition",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcondition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIconditionType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIconditionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIdataType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIdataType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIdiversityType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIdiversityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIdocumentType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIdocumentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIenforcmentCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIenforcmentCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIguyLineType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIguyLineType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIhistoryType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIhistoryType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIimportSheets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    UniqueName = table.Column<string>(nullable: true),
                    SheetName = table.Column<string>(nullable: true),
                    RefTable = table.Column<string>(nullable: true),
                    IsLib = table.Column<bool>(nullable: false),
                    ErrMsg = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIimportSheets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIInstCivilwithoutLegsType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIInstCivilwithoutLegsType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIinstallationPlace",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Disable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIinstallationPlace", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIinstallationType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Disable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIinstallationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIintegration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIintegration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIitemConnectTo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIitemConnectTo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIitemStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    DeleteDate = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIitemStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIloadOtherLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Length = table.Column<float>(nullable: false),
                    Width = table.Column<float>(nullable: false),
                    Height = table.Column<float>(nullable: false),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIloadOtherLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIlocationType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    Disable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIlocationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIlog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(nullable: true),
                    ActionType = table.Column<string>(nullable: true),
                    AffectedTable = table.Column<string>(nullable: true),
                    RecordId = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIlog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIlogicalOperation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIlogicalOperation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIlogisticalType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIlogisticalType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIlogUsersActions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    ControllerName = table.Column<string>(nullable: true),
                    FunctionName = table.Column<string>(nullable: true),
                    BodyParameters = table.Column<string>(nullable: true),
                    HeaderParameters = table.Column<string>(nullable: true),
                    ResponseStatus = table.Column<string>(nullable: true),
                    Result = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIlogUsersActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLImailTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Subject = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLImailTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLImwOtherLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Length = table.Column<float>(nullable: false),
                    Width = table.Column<float>(nullable: false),
                    Height = table.Column<float>(nullable: false),
                    L_W_H = table.Column<string>(nullable: true),
                    frequency_band = table.Column<string>(nullable: true),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLImwOtherLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIoduInstallationType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIoduInstallationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIoperation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIoperation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIorderStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    IsFinish = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIorderStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIowner",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    OwnerName = table.Column<string>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    Disable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIowner", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIparity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Disable = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIparity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIpart",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIpart", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIpermission",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Module = table.Column<string>(nullable: true),
                    PermissionType = table.Column<string>(nullable: true),
                    ControllerName = table.Column<string>(nullable: false),
                    ActionName = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIpermission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIpolarityOnLocation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIpolarityOnLocation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIpolarityType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Disable = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIpolarityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIpowerLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    FrequencyRange = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Size = table.Column<float>(nullable: false),
                    L_W_H = table.Column<string>(nullable: true),
                    Weight = table.Column<float>(nullable: false),
                    width = table.Column<float>(nullable: false),
                    Length = table.Column<float>(nullable: false),
                    Depth = table.Column<float>(nullable: false),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIpowerLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIpowerType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false),
                    Delete = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIpowerType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIradioAntennaLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    FrequencyBand = table.Column<string>(nullable: true),
                    Weight = table.Column<float>(nullable: true),
                    Width = table.Column<float>(nullable: false),
                    Depth = table.Column<float>(nullable: false),
                    Length = table.Column<float>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIradioAntennaLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIradioOtherLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Weight = table.Column<float>(nullable: true),
                    Width = table.Column<float>(nullable: false),
                    Length = table.Column<float>(nullable: false),
                    Height = table.Column<float>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIradioOtherLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIradioRRULibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Band = table.Column<string>(nullable: true),
                    ChannelBandwidth = table.Column<float>(nullable: true),
                    Weight = table.Column<float>(nullable: true),
                    L_W_H_cm3 = table.Column<string>(nullable: true),
                    Length = table.Column<float>(nullable: false),
                    Width = table.Column<float>(nullable: false),
                    Height = table.Column<float>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIradioRRULibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIregion",
                columns: table => new
                {
                    RegionCode = table.Column<string>(nullable: false),
                    RegionName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIregion", x => x.RegionCode);
                });

            migrationBuilder.CreateTable(
                name: "TLIrenewableCabinetType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Disable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIrenewableCabinetType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIrepeaterType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIrepeaterType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIrole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIrole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIrow",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIrow", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIsectionsLegType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIsectionsLegType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIsideArmInstallationPlace",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false),
                    CivilInstallationPlaceType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIsideArmInstallationPlace", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIsideArmLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Width = table.Column<float>(nullable: false),
                    Weight = table.Column<float>(nullable: true),
                    Length = table.Column<float>(nullable: false),
                    Height = table.Column<float>(nullable: false),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIsideArmLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIsideArmType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIsideArmType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIsiteStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIsiteStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIstructureType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIstructureType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIsubType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Disable = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIsubType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIsupportTypeDesigned",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIsupportTypeDesigned", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIsupportTypeImplemented",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIsupportTypeImplemented", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLItablePartName",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    PartName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLItablePartName", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLItaskStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false),
                    DisableDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLItaskStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLItelecomType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLItelecomType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIuser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    MobileNumber = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    Domain = table.Column<string>(nullable: true),
                    AdGUID = table.Column<string>(nullable: true),
                    UserType = table.Column<int>(nullable: false),
                    ChangedPasswordDate = table.Column<DateTime>(nullable: true),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    ConfirmationCode = table.Column<string>(nullable: true),
                    ValidateAccount = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIuser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TLIactionItemOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    ActionId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    DeleteDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIactionItemOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIactionItemOption_TLIaction_ActionId",
                        column: x => x.ActionId,
                        principalTable: "TLIaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIgroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    UpperId = table.Column<int>(nullable: true),
                    GroupType = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    ActorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIgroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIgroup_TLIactor_ActorId",
                        column: x => x.ActorId,
                        principalTable: "TLIactor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIgroup_TLIgroup_UpperId",
                        column: x => x.UpperId,
                        principalTable: "TLIgroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIcabinetPowerLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Weight = table.Column<float>(nullable: true),
                    NumberOfBatteries = table.Column<int>(nullable: true),
                    LayoutCode = table.Column<string>(nullable: true),
                    Dimension_W_D_H = table.Column<string>(nullable: true),
                    BatteryWeight = table.Column<float>(nullable: true),
                    BatteryType = table.Column<string>(nullable: true),
                    BatteryDimension_W_D_H = table.Column<string>(nullable: true),
                    Depth = table.Column<float>(nullable: false),
                    Width = table.Column<float>(nullable: false),
                    Height = table.Column<float>(nullable: false),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    CabinetPowerTypeId = table.Column<int>(nullable: true),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcabinetPowerLibrary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIcabinetPowerLibrary_TLIcabinetPowerType_CabinetPowerTypeId",
                        column: x => x.CabinetPowerTypeId,
                        principalTable: "TLIcabinetPowerType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIgeneratorLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Width = table.Column<float>(nullable: false),
                    Weight = table.Column<float>(nullable: true),
                    Length = table.Column<float>(nullable: false),
                    LayoutCode = table.Column<string>(nullable: true),
                    Height = table.Column<float>(nullable: false),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    CapacityId = table.Column<int>(nullable: true),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIgeneratorLibrary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIgeneratorLibrary_TLIcapacity_CapacityId",
                        column: x => x.CapacityId,
                        principalTable: "TLIcapacity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIsolarLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Weight = table.Column<float>(nullable: true),
                    TotaPanelsDimensions = table.Column<string>(nullable: true),
                    StructureDesign = table.Column<string>(nullable: true),
                    LayoutCode = table.Column<string>(nullable: true),
                    HeightFromFront = table.Column<float>(nullable: true),
                    HeightFromBack = table.Column<float>(nullable: true),
                    BasePlateDimension = table.Column<string>(nullable: true),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    CapacityId = table.Column<int>(nullable: true),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIsolarLibrary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIsolarLibrary_TLIcapacity_CapacityId",
                        column: x => x.CapacityId,
                        principalTable: "TLIcapacity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIcivilNonSteelLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Hight = table.Column<float>(nullable: false),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    VerticalMeasured = table.Column<int>(nullable: false),
                    civilNonSteelTypeId = table.Column<int>(nullable: false),
                    NumberofBoltHoles = table.Column<float>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Manufactured_Max_Load = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcivilNonSteelLibrary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIcivilNonSteelLibrary_TLIcivilNonSteelType_civilNonSteelTypeId",
                        column: x => x.civilNonSteelTypeId,
                        principalTable: "TLIcivilNonSteelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIattActivatedCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    attributeActivatedId = table.Column<int>(nullable: true),
                    civilWithoutLegCategoryId = table.Column<int>(nullable: true),
                    enable = table.Column<bool>(nullable: false, defaultValue: true),
                    Required = table.Column<bool>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsLibrary = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIattActivatedCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIattActivatedCategory_TLIattributeActivated_attributeActivatedId",
                        column: x => x.attributeActivatedId,
                        principalTable: "TLIattributeActivated",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIattActivatedCategory_TLIcivilWithoutLegCategory_civilWithoutLegCategoryId",
                        column: x => x.civilWithoutLegCategoryId,
                        principalTable: "TLIcivilWithoutLegCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIoption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    ConditionId = table.Column<int>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIoption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIoption_TLIcondition_ConditionId",
                        column: x => x.ConditionId,
                        principalTable: "TLIcondition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLImwBULibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    L_W_H = table.Column<string>(nullable: true),
                    Length = table.Column<float>(nullable: false),
                    Width = table.Column<float>(nullable: false),
                    Height = table.Column<float>(nullable: false),
                    Weight = table.Column<float>(nullable: false),
                    BUSize = table.Column<string>(nullable: true),
                    NumOfRFU = table.Column<int>(nullable: false),
                    frequency_band = table.Column<string>(nullable: true),
                    channel_bandwidth = table.Column<float>(nullable: true),
                    FreqChannel = table.Column<string>(nullable: true),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    diversityTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLImwBULibrary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLImwBULibrary_TLIdiversityType_diversityTypeId",
                        column: x => x.diversityTypeId,
                        principalTable: "TLIdiversityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLImwRFULibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Weight = table.Column<float>(nullable: true),
                    L_W_H = table.Column<string>(nullable: true),
                    Length = table.Column<float>(nullable: false),
                    Width = table.Column<float>(nullable: false),
                    Height = table.Column<float>(nullable: false),
                    size = table.Column<string>(nullable: true),
                    tx_parity = table.Column<string>(nullable: true),
                    frequency_band = table.Column<string>(nullable: true),
                    FrequencyRange = table.Column<string>(nullable: true),
                    RFUType = table.Column<string>(nullable: true),
                    VenferBoardName = table.Column<string>(nullable: true),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    diversityTypeId = table.Column<int>(nullable: true),
                    boardTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLImwRFULibrary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLImwRFULibrary_TLIboardType_boardTypeId",
                        column: x => x.boardTypeId,
                        principalTable: "TLIboardType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLImwRFULibrary_TLIdiversityType_diversityTypeId",
                        column: x => x.diversityTypeId,
                        principalTable: "TLIdiversityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIactionOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    ActionId = table.Column<int>(nullable: true),
                    ParentId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true),
                    ItemStatusId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIactionOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIactionOption_TLIaction_ActionId",
                        column: x => x.ActionId,
                        principalTable: "TLIaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIactionOption_TLIitemStatus_ItemStatusId",
                        column: x => x.ItemStatusId,
                        principalTable: "TLIitemStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIactionOption_TLIactionOption_ParentId",
                        column: x => x.ParentId,
                        principalTable: "TLIactionOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIloadOther",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    HeightBase = table.Column<float>(nullable: false),
                    HeightLand = table.Column<float>(nullable: false),
                    SpaceInstallation = table.Column<float>(nullable: false),
                    loadOtherLibraryId = table.Column<int>(nullable: false),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false),
                    InstallationPlaceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIloadOther", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIloadOther_TLIinstallationPlace_InstallationPlaceId",
                        column: x => x.InstallationPlaceId,
                        principalTable: "TLIinstallationPlace",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIloadOther_TLIloadOtherLibrary_loadOtherLibraryId",
                        column: x => x.loadOtherLibraryId,
                        principalTable: "TLIloadOtherLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLImwOther",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    HeightBase = table.Column<float>(nullable: false),
                    HeightLand = table.Column<float>(nullable: false),
                    VisibleStatus = table.Column<string>(nullable: true),
                    mwOtherLibraryId = table.Column<int>(nullable: false),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false),
                    Spaceinstallation = table.Column<float>(nullable: false),
                    InstallationPlaceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLImwOther", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLImwOther_TLIinstallationPlace_InstallationPlaceId",
                        column: x => x.InstallationPlaceId,
                        principalTable: "TLIinstallationPlace",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLImwOther_TLImwOtherLibrary_mwOtherLibraryId",
                        column: x => x.mwOtherLibraryId,
                        principalTable: "TLImwOtherLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLImwODULibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Weight = table.Column<float>(nullable: true),
                    H_W_D = table.Column<string>(nullable: true),
                    Depth = table.Column<float>(nullable: false),
                    Width = table.Column<float>(nullable: false),
                    Height = table.Column<float>(nullable: false),
                    frequency_range = table.Column<string>(nullable: true),
                    frequency_band = table.Column<string>(nullable: true),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    parityId = table.Column<int>(nullable: true),
                    Diameter = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLImwODULibrary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLImwODULibrary_TLIparity_parityId",
                        column: x => x.parityId,
                        principalTable: "TLIparity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIequipment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Table = table.Column<string>(nullable: true),
                    TableId = table.Column<int>(nullable: false),
                    PartId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIequipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIequipment_TLIpart_PartId",
                        column: x => x.PartId,
                        principalTable: "TLIpart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLImwDishLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Weight = table.Column<float>(nullable: false),
                    dimensions = table.Column<string>(nullable: true),
                    Length = table.Column<float>(nullable: false),
                    Width = table.Column<float>(nullable: false),
                    Height = table.Column<float>(nullable: false),
                    diameter = table.Column<float>(nullable: false),
                    frequency_band = table.Column<string>(nullable: true),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    polarityTypeId = table.Column<int>(nullable: true),
                    asTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLImwDishLibrary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLImwDishLibrary_TLIasType_asTypeId",
                        column: x => x.asTypeId,
                        principalTable: "TLIasType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLImwDishLibrary_TLIpolarityType_polarityTypeId",
                        column: x => x.polarityTypeId,
                        principalTable: "TLIpolarityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIpower",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    SerialNumber = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    HeightBase = table.Column<float>(nullable: false),
                    HeightLand = table.Column<float>(nullable: false),
                    SpaceInstallation = table.Column<float>(nullable: false),
                    Azimuth = table.Column<float>(nullable: false),
                    Height = table.Column<float>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    ownerId = table.Column<int>(nullable: true),
                    installationPlaceId = table.Column<int>(nullable: true),
                    powerLibraryId = table.Column<int>(nullable: false),
                    powerTypeId = table.Column<int>(nullable: true),
                    VisibleStatus = table.Column<string>(nullable: true),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIpower", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIpower_TLIinstallationPlace_installationPlaceId",
                        column: x => x.installationPlaceId,
                        principalTable: "TLIinstallationPlace",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIpower_TLIowner_ownerId",
                        column: x => x.ownerId,
                        principalTable: "TLIowner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIpower_TLIpowerLibrary_powerLibraryId",
                        column: x => x.powerLibraryId,
                        principalTable: "TLIpowerLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIpower_TLIpowerType_powerTypeId",
                        column: x => x.powerTypeId,
                        principalTable: "TLIpowerType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIradioAntenna",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Azimuth = table.Column<float>(nullable: false),
                    MechanicalTilt = table.Column<float>(nullable: true),
                    ElectricalTilt = table.Column<float>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    HBASurface = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    HeightBase = table.Column<float>(nullable: false),
                    HeightLand = table.Column<float>(nullable: false),
                    SpaceInstallation = table.Column<float>(nullable: false),
                    VisibleStatus = table.Column<string>(nullable: true),
                    ownerId = table.Column<int>(nullable: true),
                    installationPlaceId = table.Column<int>(nullable: true),
                    radioAntennaLibraryId = table.Column<int>(nullable: false),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIradioAntenna", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIradioAntenna_TLIinstallationPlace_installationPlaceId",
                        column: x => x.installationPlaceId,
                        principalTable: "TLIinstallationPlace",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIradioAntenna_TLIowner_ownerId",
                        column: x => x.ownerId,
                        principalTable: "TLIowner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIradioAntenna_TLIradioAntennaLibrary_radioAntennaLibraryId",
                        column: x => x.radioAntennaLibraryId,
                        principalTable: "TLIradioAntennaLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIradioOther",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    HeightBase = table.Column<float>(nullable: false),
                    HeightLand = table.Column<float>(nullable: false),
                    ownerId = table.Column<int>(nullable: false),
                    installationPlaceId = table.Column<int>(nullable: false),
                    radioOtherLibraryId = table.Column<int>(nullable: false),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false),
                    Spaceinstallation = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIradioOther", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIradioOther_TLIinstallationPlace_installationPlaceId",
                        column: x => x.installationPlaceId,
                        principalTable: "TLIinstallationPlace",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIradioOther_TLIowner_ownerId",
                        column: x => x.ownerId,
                        principalTable: "TLIowner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIradioOther_TLIradioOtherLibrary_radioOtherLibraryId",
                        column: x => x.radioOtherLibraryId,
                        principalTable: "TLIradioOtherLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIrolePermission",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    roleId = table.Column<int>(nullable: false),
                    permissionId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIrolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIrolePermission_TLIpermission_permissionId",
                        column: x => x.permissionId,
                        principalTable: "TLIpermission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIrolePermission_TLIrole_roleId",
                        column: x => x.roleId,
                        principalTable: "TLIrole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIsite",
                columns: table => new
                {
                    SiteCode = table.Column<string>(nullable: false),
                    SiteName = table.Column<string>(nullable: true),
                    LocationType = table.Column<string>(nullable: true),
                    LocationHieght = table.Column<float>(nullable: false),
                    Latitude = table.Column<float>(nullable: false),
                    Longitude = table.Column<float>(nullable: false),
                    siteStatusId = table.Column<int>(nullable: false),
                    RentedSpace = table.Column<float>(nullable: false),
                    ReservedSpace = table.Column<float>(nullable: false),
                    SiteVisiteDate = table.Column<DateTime>(nullable: false),
                    Zone = table.Column<string>(nullable: true),
                    SubArea = table.Column<string>(nullable: true),
                    RegionCode = table.Column<string>(nullable: true),
                    STATUS_DATE = table.Column<string>(nullable: true),
                    CREATE_DATE = table.Column<string>(nullable: true),
                    AreaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIsite", x => x.SiteCode);
                    table.ForeignKey(
                        name: "FK_TLIsite_TLIarea_AreaId",
                        column: x => x.AreaId,
                        principalTable: "TLIarea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIsite_TLIregion_RegionCode",
                        column: x => x.RegionCode,
                        principalTable: "TLIregion",
                        principalColumn: "RegionCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIsite_TLIsiteStatus_siteStatusId",
                        column: x => x.siteStatusId,
                        principalTable: "TLIsiteStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIworkFlow",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    DateDeleted = table.Column<DateTime>(nullable: true),
                    SiteStatusId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIworkFlow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIworkFlow_TLIsiteStatus_SiteStatusId",
                        column: x => x.SiteStatusId,
                        principalTable: "TLIsiteStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIcivilWithoutLegLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Height_Designed = table.Column<float>(nullable: false),
                    Max_Load = table.Column<float>(nullable: true),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    HeightBase = table.Column<float>(nullable: false),
                    Prefix = table.Column<string>(nullable: true),
                    structureTypeId = table.Column<int>(nullable: false),
                    CivilSteelSupportCategoryId = table.Column<int>(nullable: false),
                    InstCivilwithoutLegsTypeId = table.Column<int>(nullable: true),
                    CivilWithoutLegCategoryId = table.Column<int>(nullable: true),
                    Manufactured_Max_Load = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcivilWithoutLegLibrary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithoutLegLibrary_TLIcivilSteelSupportCategory_CivilSteelSupportCategoryId",
                        column: x => x.CivilSteelSupportCategoryId,
                        principalTable: "TLIcivilSteelSupportCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithoutLegLibrary_TLIcivilWithoutLegCategory_CivilWithoutLegCategoryId",
                        column: x => x.CivilWithoutLegCategoryId,
                        principalTable: "TLIcivilWithoutLegCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithoutLegLibrary_TLIInstCivilwithoutLegsType_InstCivilwithoutLegsTypeId",
                        column: x => x.InstCivilwithoutLegsTypeId,
                        principalTable: "TLIInstCivilwithoutLegsType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithoutLegLibrary_TLIstructureType_structureTypeId",
                        column: x => x.structureTypeId,
                        principalTable: "TLIstructureType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIcivilWithLegLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Prefix = table.Column<string>(nullable: true),
                    Height_Designed = table.Column<float>(nullable: false),
                    Max_load_M2 = table.Column<float>(nullable: true),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    supportTypeDesignedId = table.Column<int>(nullable: false),
                    sectionsLegTypeId = table.Column<int>(nullable: false),
                    structureTypeId = table.Column<int>(nullable: true),
                    civilSteelSupportCategoryId = table.Column<int>(nullable: false),
                    Manufactured_Max_Load = table.Column<float>(nullable: false),
                    NumberOfLegs = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcivilWithLegLibrary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithLegLibrary_TLIcivilSteelSupportCategory_civilSteelSupportCategoryId",
                        column: x => x.civilSteelSupportCategoryId,
                        principalTable: "TLIcivilSteelSupportCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithLegLibrary_TLIsectionsLegType_sectionsLegTypeId",
                        column: x => x.sectionsLegTypeId,
                        principalTable: "TLIsectionsLegType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithLegLibrary_TLIstructureType_structureTypeId",
                        column: x => x.structureTypeId,
                        principalTable: "TLIstructureType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithLegLibrary_TLIsupportTypeDesigned_supportTypeDesignedId",
                        column: x => x.supportTypeDesignedId,
                        principalTable: "TLIsupportTypeDesigned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIlogistical",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    tablePartNameId = table.Column<int>(nullable: false),
                    logisticalTypeId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIlogistical", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIlogistical_TLIlogisticalType_logisticalTypeId",
                        column: x => x.logisticalTypeId,
                        principalTable: "TLIlogisticalType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIlogistical_TLItablePartName_tablePartNameId",
                        column: x => x.tablePartNameId,
                        principalTable: "TLItablePartName",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLItablesNames",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    TableName = table.Column<string>(nullable: false),
                    tablePartNameId = table.Column<int>(nullable: true),
                    IsEquip = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLItablesNames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLItablesNames_TLItablePartName_tablePartNameId",
                        column: x => x.tablePartNameId,
                        principalTable: "TLItablePartName",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIcabinetTelecomLibrary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Model = table.Column<string>(nullable: true),
                    MaxWeight = table.Column<float>(nullable: true),
                    LayoutCode = table.Column<string>(nullable: true),
                    Dimension_W_D_H = table.Column<string>(nullable: true),
                    Width = table.Column<float>(nullable: false),
                    Depth = table.Column<float>(nullable: false),
                    Height = table.Column<float>(nullable: false),
                    SpaceLibrary = table.Column<float>(nullable: false),
                    TelecomTypeId = table.Column<int>(nullable: true),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcabinetTelecomLibrary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIcabinetTelecomLibrary_TLItelecomType_TelecomTypeId",
                        column: x => x.TelecomTypeId,
                        principalTable: "TLItelecomType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIuserPermission",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    permissionId = table.Column<int>(nullable: false),
                    userId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIuserPermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIuserPermission_TLIpermission_permissionId",
                        column: x => x.permissionId,
                        principalTable: "TLIpermission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIuserPermission_TLIuser_userId",
                        column: x => x.userId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIgroupRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    groupId = table.Column<int>(nullable: false),
                    roleId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIgroupRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIgroupRole_TLIgroup_groupId",
                        column: x => x.groupId,
                        principalTable: "TLIgroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIgroupRole_TLIrole_roleId",
                        column: x => x.roleId,
                        principalTable: "TLIrole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIgroupUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    groupId = table.Column<int>(nullable: false),
                    userId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIgroupUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIgroupUser_TLIgroup_groupId",
                        column: x => x.groupId,
                        principalTable: "TLIgroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIgroupUser_TLIuser_userId",
                        column: x => x.userId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIstepActionMail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    GroupId = table.Column<int>(nullable: true),
                    ActorId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    TLIintegrationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIstepActionMail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIstepActionMail_TLIactor_ActorId",
                        column: x => x.ActorId,
                        principalTable: "TLIactor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionMail_TLIgroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "TLIgroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionMail_TLIintegration_TLIintegrationId",
                        column: x => x.TLIintegrationId,
                        principalTable: "TLIintegration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionMail_TLIuser_UserId",
                        column: x => x.UserId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIgenerator",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    NumberOfFuelTanks = table.Column<int>(nullable: true),
                    LocationDescription = table.Column<string>(nullable: true),
                    BaseExisting = table.Column<bool>(nullable: true),
                    SpaceInstallation = table.Column<float>(nullable: false),
                    VisibleStatus = table.Column<string>(nullable: true),
                    BaseGeneratorTypeId = table.Column<int>(nullable: true),
                    GeneratorLibraryId = table.Column<int>(nullable: false),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIgenerator", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIgenerator_TLIbaseGeneratorType_BaseGeneratorTypeId",
                        column: x => x.BaseGeneratorTypeId,
                        principalTable: "TLIbaseGeneratorType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIgenerator_TLIgeneratorLibrary_GeneratorLibraryId",
                        column: x => x.GeneratorLibraryId,
                        principalTable: "TLIgeneratorLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIcivilNonSteel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CurrentLoads = table.Column<double>(nullable: false),
                    SpaceInstallation = table.Column<float>(nullable: false),
                    CivilNonSteelLibraryId = table.Column<int>(nullable: false),
                    ownerId = table.Column<int>(nullable: true),
                    supportTypeImplementedId = table.Column<int>(nullable: true),
                    locationTypeId = table.Column<int>(nullable: true),
                    locationHeight = table.Column<float>(nullable: false),
                    BuildingMaxLoad = table.Column<float>(nullable: false),
                    CivilSupportCurrentLoad = table.Column<float>(nullable: false),
                    H2Height = table.Column<float>(nullable: false),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false),
                    Support_Limited_Load = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcivilNonSteel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIcivilNonSteel_TLIcivilNonSteelLibrary_CivilNonSteelLibraryId",
                        column: x => x.CivilNonSteelLibraryId,
                        principalTable: "TLIcivilNonSteelLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIcivilNonSteel_TLIlocationType_locationTypeId",
                        column: x => x.locationTypeId,
                        principalTable: "TLIlocationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIcivilNonSteel_TLIowner_ownerId",
                        column: x => x.ownerId,
                        principalTable: "TLIowner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIcivilNonSteel_TLIsupportTypeImplemented_supportTypeImplementedId",
                        column: x => x.supportTypeImplementedId,
                        principalTable: "TLIsupportTypeImplemented",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIsuboption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    OptionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIsuboption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIsuboption_TLIoption_OptionId",
                        column: x => x.OptionId,
                        principalTable: "TLIoption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLImwDish",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Azimuth = table.Column<float>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    Far_End_Site_Code = table.Column<string>(nullable: true),
                    HBA_Surface = table.Column<string>(nullable: true),
                    Is_Repeator = table.Column<bool>(nullable: false),
                    Serial_Number = table.Column<string>(nullable: true),
                    DishName = table.Column<string>(nullable: true),
                    MW_LinkId = table.Column<string>(nullable: true),
                    Visiable_Status = table.Column<string>(nullable: true),
                    SpaceInstallation = table.Column<float>(nullable: false),
                    HeightBase = table.Column<float>(nullable: false),
                    HeightLand = table.Column<float>(nullable: false),
                    Temp = table.Column<string>(nullable: true),
                    ownerId = table.Column<int>(nullable: true),
                    RepeaterTypeId = table.Column<int>(nullable: true),
                    PolarityOnLocationId = table.Column<int>(nullable: true),
                    ItemConnectToId = table.Column<int>(nullable: true),
                    MwDishLibraryId = table.Column<int>(nullable: false),
                    InstallationPlaceId = table.Column<int>(nullable: true),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLImwDish", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLImwDish_TLIinstallationPlace_InstallationPlaceId",
                        column: x => x.InstallationPlaceId,
                        principalTable: "TLIinstallationPlace",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLImwDish_TLIitemConnectTo_ItemConnectToId",
                        column: x => x.ItemConnectToId,
                        principalTable: "TLIitemConnectTo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLImwDish_TLImwDishLibrary_MwDishLibraryId",
                        column: x => x.MwDishLibraryId,
                        principalTable: "TLImwDishLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLImwDish_TLIpolarityOnLocation_PolarityOnLocationId",
                        column: x => x.PolarityOnLocationId,
                        principalTable: "TLIpolarityOnLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLImwDish_TLIrepeaterType_RepeaterTypeId",
                        column: x => x.RepeaterTypeId,
                        principalTable: "TLIrepeaterType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLImwDish_TLIowner_ownerId",
                        column: x => x.ownerId,
                        principalTable: "TLIowner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIRadioRRU",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    SerialNumber = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    HeightBase = table.Column<float>(nullable: false),
                    HeightLand = table.Column<float>(nullable: false),
                    SpaceInstallation = table.Column<float>(nullable: false),
                    VisibleStatus = table.Column<string>(nullable: true),
                    radioRRULibraryId = table.Column<int>(nullable: false),
                    ownerId = table.Column<int>(nullable: true),
                    radioAntennaId = table.Column<int>(nullable: true),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false),
                    installationPlaceId = table.Column<int>(nullable: true),
                    Azimuth = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIRadioRRU", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIRadioRRU_TLIinstallationPlace_installationPlaceId",
                        column: x => x.installationPlaceId,
                        principalTable: "TLIinstallationPlace",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIRadioRRU_TLIowner_ownerId",
                        column: x => x.ownerId,
                        principalTable: "TLIowner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIRadioRRU_TLIradioAntenna_radioAntennaId",
                        column: x => x.radioAntennaId,
                        principalTable: "TLIradioAntenna",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIRadioRRU_TLIradioRRULibrary_radioRRULibraryId",
                        column: x => x.radioRRULibraryId,
                        principalTable: "TLIradioRRULibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIstep",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    WorkFlowId = table.Column<int>(nullable: true),
                    ParentStepId = table.Column<int>(nullable: true),
                    type = table.Column<int>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIstep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIstep_TLIstep_ParentStepId",
                        column: x => x.ParentStepId,
                        principalTable: "TLIstep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstep_TLIworkFlow_WorkFlowId",
                        column: x => x.WorkFlowId,
                        principalTable: "TLIworkFlow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIworkFlowGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    WorkFlowId = table.Column<int>(nullable: false),
                    ActorId = table.Column<int>(nullable: true),
                    IntegrationId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    GroupId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIworkFlowGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIworkFlowGroup_TLIactor_ActorId",
                        column: x => x.ActorId,
                        principalTable: "TLIactor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIworkFlowGroup_TLIgroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "TLIgroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIworkFlowGroup_TLIintegration_IntegrationId",
                        column: x => x.IntegrationId,
                        principalTable: "TLIintegration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIworkFlowGroup_TLIuser_UserId",
                        column: x => x.UserId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIworkFlowGroup_TLIworkFlow_WorkFlowId",
                        column: x => x.WorkFlowId,
                        principalTable: "TLIworkFlow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIcivilWithoutLeg",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    HeightBase = table.Column<float>(nullable: false),
                    UpperPartLengthm = table.Column<float>(nullable: true),
                    UpperPartDiameterm = table.Column<float>(nullable: true),
                    SpindlesBasePlateLengthcm = table.Column<float>(nullable: true),
                    SpindlesBasePlateWidthcm = table.Column<float>(nullable: true),
                    SpinBasePlateAnchorDiametercm = table.Column<float>(nullable: true),
                    NumberOfCivilParts = table.Column<int>(nullable: true),
                    NumberOfLongitudinalSpindles = table.Column<int>(nullable: true),
                    NumberOfhorizontalSpindle = table.Column<int>(nullable: true),
                    CivilLengthAboveEndOfSpindles = table.Column<float>(nullable: true),
                    CivilBaseLevelFromGround = table.Column<float>(nullable: true),
                    LongitudinalSpinDiameterrmm = table.Column<float>(nullable: true),
                    HorizontalSpindlesHBAm = table.Column<float>(nullable: true),
                    HorizontalSpindleDiametermm = table.Column<float>(nullable: true),
                    FlangeThicknesscm = table.Column<float>(nullable: true),
                    FlangeDiametercm = table.Column<float>(nullable: true),
                    FlangeBoltsDiametermm = table.Column<float>(nullable: true),
                    ConcreteBaseThicknessm = table.Column<float>(nullable: true),
                    ConcreteBaseLengthm = table.Column<float>(nullable: true),
                    Civil_Remarks = table.Column<string>(nullable: true),
                    BottomPartLengthm = table.Column<float>(nullable: true),
                    BottomPartDiametermm = table.Column<float>(nullable: true),
                    BasePlateWidthcm = table.Column<float>(nullable: true),
                    BasePlateThicknesscm = table.Column<float>(nullable: true),
                    BasePlateLengthcm = table.Column<float>(nullable: true),
                    BPlateBoltsAnchorDiametermm = table.Column<float>(nullable: true),
                    BaseBeamSectionmm = table.Column<float>(nullable: true),
                    WindMaxLoadm2 = table.Column<float>(nullable: true),
                    Location_Height = table.Column<float>(nullable: true),
                    PoType = table.Column<string>(nullable: true),
                    PoNo = table.Column<string>(nullable: true),
                    PoDate = table.Column<DateTime>(nullable: false),
                    reinforced = table.Column<int>(nullable: true),
                    ladderSteps = table.Column<int>(nullable: true),
                    availabilityOfWorkPlatforms = table.Column<int>(nullable: true),
                    equipmentsLocation = table.Column<int>(nullable: true),
                    HeightImplemented = table.Column<float>(nullable: true),
                    BuildingMaxLoad = table.Column<float>(nullable: true),
                    SupportMaxLoadAfterInforcement = table.Column<float>(nullable: true),
                    CurrentLoads = table.Column<float>(nullable: true),
                    BuildingHeightH3 = table.Column<float>(nullable: true),
                    WarningPercentageLoads = table.Column<int>(nullable: true),
                    Visiable_Status = table.Column<string>(nullable: true),
                    SpaceInstallation = table.Column<float>(nullable: false),
                    CivilWithoutlegsLibId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: true),
                    subTypeId = table.Column<int>(nullable: true),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false),
                    Support_Limited_Load = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcivilWithoutLeg", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithoutLeg_TLIcivilWithoutLegLibrary_CivilWithoutlegsLibId",
                        column: x => x.CivilWithoutlegsLibId,
                        principalTable: "TLIcivilWithoutLegLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithoutLeg_TLIowner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "TLIowner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithoutLeg_TLIsubType_subTypeId",
                        column: x => x.subTypeId,
                        principalTable: "TLIsubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIcivilWithLegs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    WindMaxLoadm2 = table.Column<float>(nullable: true),
                    LocationHeight = table.Column<double>(nullable: true),
                    PoType = table.Column<string>(nullable: true),
                    PoNo = table.Column<string>(nullable: true),
                    PoDate = table.Column<DateTime>(nullable: true),
                    HeightImplemented = table.Column<double>(nullable: true),
                    BuildingMaxLoad = table.Column<float>(nullable: true),
                    SupportMaxLoadAfterInforcement = table.Column<float>(nullable: true),
                    CurrentLoads = table.Column<double>(nullable: true),
                    warningpercentageloads = table.Column<double>(nullable: true),
                    VisiableStatus = table.Column<string>(nullable: true),
                    VerticalMeasured = table.Column<string>(nullable: true),
                    OtherBaseType = table.Column<string>(nullable: true),
                    IsEnforeced = table.Column<bool>(nullable: true),
                    H2height = table.Column<double>(nullable: false),
                    HeightBase = table.Column<float>(nullable: false),
                    DimensionsLeg = table.Column<string>(nullable: true),
                    DiagonalMemberSection = table.Column<string>(nullable: true),
                    DiagonalMemberDimensions = table.Column<string>(nullable: true),
                    BoltHoles = table.Column<int>(nullable: true),
                    BasePlatethickness = table.Column<string>(nullable: true),
                    BasePlateShape = table.Column<string>(nullable: true),
                    BasePlateDimensions = table.Column<string>(nullable: true),
                    BaseNote = table.Column<string>(nullable: true),
                    locationTypeId = table.Column<int>(nullable: true),
                    baseTypeId = table.Column<int>(nullable: false),
                    VerticalMeasurement = table.Column<string>(nullable: true),
                    SteelCrossSection = table.Column<string>(nullable: true),
                    DiagonalMemberPrefix = table.Column<string>(nullable: true),
                    EnforcementHeightBase = table.Column<float>(nullable: false),
                    Enforcementlevel = table.Column<float>(nullable: false),
                    StructureType = table.Column<int>(nullable: false),
                    SectionsLegType = table.Column<int>(nullable: false),
                    TotalHeight = table.Column<float>(nullable: false),
                    SpaceInstallation = table.Column<float>(nullable: false),
                    OwnerId = table.Column<int>(nullable: true),
                    CivilWithLegsLibId = table.Column<int>(nullable: false),
                    BaseCivilWithLegTypeId = table.Column<int>(nullable: false),
                    GuylineTypeId = table.Column<int>(nullable: true),
                    SupportTypeImplementedId = table.Column<int>(nullable: true),
                    enforcmentCategoryId = table.Column<int>(nullable: true),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false),
                    Support_Limited_Load = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcivilWithLegs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithLegs_TLIbaseCivilWithLegsType_BaseCivilWithLegTypeId",
                        column: x => x.BaseCivilWithLegTypeId,
                        principalTable: "TLIbaseCivilWithLegsType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithLegs_TLIcivilWithLegLibrary_CivilWithLegsLibId",
                        column: x => x.CivilWithLegsLibId,
                        principalTable: "TLIcivilWithLegLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithLegs_TLIguyLineType_GuylineTypeId",
                        column: x => x.GuylineTypeId,
                        principalTable: "TLIguyLineType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithLegs_TLIowner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "TLIowner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithLegs_TLIsupportTypeImplemented_SupportTypeImplementedId",
                        column: x => x.SupportTypeImplementedId,
                        principalTable: "TLIsupportTypeImplemented",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithLegs_TLIbaseType_baseTypeId",
                        column: x => x.baseTypeId,
                        principalTable: "TLIbaseType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithLegs_TLIenforcmentCategory_enforcmentCategoryId",
                        column: x => x.enforcmentCategoryId,
                        principalTable: "TLIenforcmentCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIcivilWithLegs_TLIlocationType_locationTypeId",
                        column: x => x.locationTypeId,
                        principalTable: "TLIlocationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIattachedFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SiteCode = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Description2 = table.Column<string>(nullable: true),
                    fileSize = table.Column<float>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    RecordId = table.Column<int>(nullable: false),
                    documenttypeId = table.Column<int>(nullable: false),
                    tablesNamesId = table.Column<int>(nullable: true),
                    IsImg = table.Column<bool>(nullable: false),
                    UnAttached = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIattachedFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIattachedFiles_TLIdocumentType_documenttypeId",
                        column: x => x.documenttypeId,
                        principalTable: "TLIdocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIattachedFiles_TLItablesNames_tablesNamesId",
                        column: x => x.tablesNamesId,
                        principalTable: "TLItablesNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIdynamicAtt",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(nullable: true),
                    LibraryAtt = table.Column<bool>(nullable: false),
                    DataTypeId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CivilWithoutLegCategoryId = table.Column<int>(nullable: true),
                    tablesNamesId = table.Column<int>(nullable: false),
                    Required = table.Column<bool>(nullable: false, defaultValue: false),
                    disable = table.Column<bool>(nullable: false, defaultValue: false),
                    DefaultValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIdynamicAtt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIdynamicAtt_TLIcivilWithoutLegCategory_CivilWithoutLegCategoryId",
                        column: x => x.CivilWithoutLegCategoryId,
                        principalTable: "TLIcivilWithoutLegCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIdynamicAtt_TLIdataType_DataTypeId",
                        column: x => x.DataTypeId,
                        principalTable: "TLIdataType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIdynamicAtt_TLItablesNames_tablesNamesId",
                        column: x => x.tablesNamesId,
                        principalTable: "TLItablesNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIeditableManagmentView",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    View = table.Column<string>(nullable: false),
                    TLItablesNames1Id = table.Column<int>(nullable: false),
                    TLItablesNames2Id = table.Column<int>(nullable: true),
                    TLItablesNames3Id = table.Column<int>(nullable: true),
                    CivilWithoutLegCategoryId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIeditableManagmentView", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIeditableManagmentView_TLIcivilWithoutLegCategory_CivilWithoutLegCategoryId",
                        column: x => x.CivilWithoutLegCategoryId,
                        principalTable: "TLIcivilWithoutLegCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIeditableManagmentView_TLItablesNames_TLItablesNames1Id",
                        column: x => x.TLItablesNames1Id,
                        principalTable: "TLItablesNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIeditableManagmentView_TLItablesNames_TLItablesNames2Id",
                        column: x => x.TLItablesNames2Id,
                        principalTable: "TLItablesNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIeditableManagmentView_TLItablesNames_TLItablesNames3Id",
                        column: x => x.TLItablesNames3Id,
                        principalTable: "TLItablesNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIlogisticalitem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    RecordId = table.Column<int>(nullable: false),
                    tablesNamesId = table.Column<int>(nullable: false),
                    IsLib = table.Column<bool>(nullable: false),
                    logisticalId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIlogisticalitem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIlogisticalitem_TLIlogistical_logisticalId",
                        column: x => x.logisticalId,
                        principalTable: "TLIlogistical",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIlogisticalitem_TLItablesNames_tablesNamesId",
                        column: x => x.tablesNamesId,
                        principalTable: "TLItablesNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLItablesHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    HistoryTypeId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    RecordId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    TablesNameId = table.Column<int>(nullable: false),
                    PreviousHistoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLItablesHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLItablesHistory_TLIhistoryType_HistoryTypeId",
                        column: x => x.HistoryTypeId,
                        principalTable: "TLIhistoryType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLItablesHistory_TLItablesHistory_PreviousHistoryId",
                        column: x => x.PreviousHistoryId,
                        principalTable: "TLItablesHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLItablesHistory_TLItablesNames_TablesNameId",
                        column: x => x.TablesNameId,
                        principalTable: "TLItablesNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLItablesHistory_TLIuser_UserId",
                        column: x => x.UserId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIcabinet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    TPVersion = table.Column<string>(nullable: true),
                    RenewableCabinetNumberOfBatteries = table.Column<int>(nullable: true),
                    NUmberOfPSU = table.Column<int>(nullable: true),
                    SpaceInstallation = table.Column<float>(nullable: false),
                    VisibleStatus = table.Column<string>(nullable: true),
                    CabinetPowerLibraryId = table.Column<int>(nullable: true),
                    CabinetTelecomLibraryId = table.Column<int>(nullable: true),
                    RenewableCabinetTypeId = table.Column<int>(nullable: true),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcabinet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIcabinet_TLIcabinetPowerLibrary_CabinetPowerLibraryId",
                        column: x => x.CabinetPowerLibraryId,
                        principalTable: "TLIcabinetPowerLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIcabinet_TLIcabinetTelecomLibrary_CabinetTelecomLibraryId",
                        column: x => x.CabinetTelecomLibraryId,
                        principalTable: "TLIcabinetTelecomLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIcabinet_TLIrenewableCabinetType_RenewableCabinetTypeId",
                        column: x => x.RenewableCabinetTypeId,
                        principalTable: "TLIrenewableCabinetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIstepAction",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    WorkflowId = table.Column<int>(nullable: false),
                    ActionId = table.Column<int>(nullable: false),
                    type = table.Column<int>(nullable: false),
                    label = table.Column<string>(nullable: true),
                    sequence = table.Column<int>(nullable: false),
                    Period = table.Column<int>(nullable: false),
                    OutputMode = table.Column<int>(nullable: false),
                    InputMode = table.Column<int>(nullable: false),
                    OrderStatusId = table.Column<int>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true),
                    Operation = table.Column<int>(nullable: true),
                    AllowNote = table.Column<bool>(nullable: false),
                    NoteIsMandatory = table.Column<bool>(nullable: false),
                    AllowUploadFile = table.Column<bool>(nullable: false),
                    UploadFileIsMandatory = table.Column<bool>(nullable: false),
                    CalculateLandSpace = table.Column<bool>(nullable: false),
                    CalculateLoadSpace = table.Column<bool>(nullable: false),
                    IsStepActionMail = table.Column<bool>(nullable: false),
                    MailSubject = table.Column<string>(nullable: true),
                    MailBody = table.Column<string>(nullable: true),
                    StepActionMailFromId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIstepAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIstepAction_TLIaction_ActionId",
                        column: x => x.ActionId,
                        principalTable: "TLIaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIstepAction_TLIorderStatus_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "TLIorderStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepAction_TLIstepActionMail_StepActionMailFromId",
                        column: x => x.StepActionMailFromId,
                        principalTable: "TLIstepActionMail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepAction_TLIworkFlow_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "TLIworkFlow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLImwBU",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Notes = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Serial_Number = table.Column<string>(nullable: true),
                    Height = table.Column<float>(nullable: true),
                    Azimuth = table.Column<float>(nullable: false),
                    BUNumber = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Visiable_Status = table.Column<string>(nullable: true),
                    SpaceInstallation = table.Column<float>(nullable: false),
                    BaseBUId = table.Column<int>(nullable: false),
                    InstallationPlaceId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: true),
                    MwBULibraryId = table.Column<int>(nullable: false),
                    MainDishId = table.Column<int>(nullable: true),
                    SdDishId = table.Column<int>(nullable: true),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false),
                    PortCascadeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLImwBU", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLImwBU_TLIbaseBU_BaseBUId",
                        column: x => x.BaseBUId,
                        principalTable: "TLIbaseBU",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLImwBU_TLIinstallationPlace_InstallationPlaceId",
                        column: x => x.InstallationPlaceId,
                        principalTable: "TLIinstallationPlace",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLImwBU_TLImwDish_MainDishId",
                        column: x => x.MainDishId,
                        principalTable: "TLImwDish",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLImwBU_TLImwBULibrary_MwBULibraryId",
                        column: x => x.MwBULibraryId,
                        principalTable: "TLImwBULibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLImwBU_TLIowner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "TLIowner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLImwODU",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Serial_Number = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    Height = table.Column<float>(nullable: true),
                    ODUConnections = table.Column<int>(nullable: false),
                    Visiable_Status = table.Column<string>(nullable: true),
                    SpaceInstallation = table.Column<float>(nullable: false),
                    OwnerId = table.Column<int>(nullable: true),
                    Mw_DishId = table.Column<int>(nullable: true),
                    OduInstallationTypeId = table.Column<int>(nullable: true),
                    MwODULibraryId = table.Column<int>(nullable: false),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLImwODU", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLImwODU_TLImwODULibrary_MwODULibraryId",
                        column: x => x.MwODULibraryId,
                        principalTable: "TLImwODULibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLImwODU_TLImwDish_Mw_DishId",
                        column: x => x.Mw_DishId,
                        principalTable: "TLImwDish",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLImwODU_TLIoduInstallationType_OduInstallationTypeId",
                        column: x => x.OduInstallationTypeId,
                        principalTable: "TLIoduInstallationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLImwODU_TLIowner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "TLIowner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIantennaRRUInst",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    radioAntennaId = table.Column<int>(nullable: false),
                    radioRRUId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIantennaRRUInst", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIantennaRRUInst_TLIradioAntenna_radioAntennaId",
                        column: x => x.radioAntennaId,
                        principalTable: "TLIradioAntenna",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIantennaRRUInst_TLIRadioRRU_radioRRUId",
                        column: x => x.radioRRUId,
                        principalTable: "TLIRadioRRU",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIleg",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    CiviLegName = table.Column<string>(nullable: true),
                    LegLetter = table.Column<string>(nullable: true),
                    LegAzimuth = table.Column<float>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    CivilWithLegInstId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIleg", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIleg_TLIcivilWithLegs_CivilWithLegInstId",
                        column: x => x.CivilWithLegInstId,
                        principalTable: "TLIcivilWithLegs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIdependency",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    DynamicAttId = table.Column<int>(nullable: true),
                    OperationId = table.Column<int>(nullable: true),
                    ValueString = table.Column<string>(nullable: true),
                    ValueDouble = table.Column<double>(nullable: true),
                    ValueDateTime = table.Column<DateTime>(nullable: true),
                    ValueBoolean = table.Column<bool>(nullable: true),
                    IsResult = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIdependency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIdependency_TLIdynamicAtt_DynamicAttId",
                        column: x => x.DynamicAttId,
                        principalTable: "TLIdynamicAtt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIdependency_TLIoperation_OperationId",
                        column: x => x.OperationId,
                        principalTable: "TLIoperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIdynamicListValues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(nullable: true),
                    dynamicAttId = table.Column<int>(nullable: false),
                    Delete = table.Column<bool>(nullable: false, defaultValue: false),
                    Disable = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIdynamicListValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIdynamicListValues_TLIdynamicAtt_dynamicAttId",
                        column: x => x.dynamicAttId,
                        principalTable: "TLIdynamicAtt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIrule",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    attributeActivatedId = table.Column<int>(nullable: true),
                    dynamicAttId = table.Column<int>(nullable: true),
                    OperationId = table.Column<int>(nullable: true),
                    OperationValueString = table.Column<string>(nullable: true),
                    OperationValueDouble = table.Column<double>(nullable: true),
                    OperationValueDateTime = table.Column<DateTime>(nullable: true),
                    OperationValueBoolean = table.Column<bool>(nullable: true),
                    tablesNamesId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIrule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIrule_TLIoperation_OperationId",
                        column: x => x.OperationId,
                        principalTable: "TLIoperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIrule_TLIattributeActivated_attributeActivatedId",
                        column: x => x.attributeActivatedId,
                        principalTable: "TLIattributeActivated",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIrule_TLIdynamicAtt_dynamicAttId",
                        column: x => x.dynamicAttId,
                        principalTable: "TLIdynamicAtt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIrule_TLItablesNames_tablesNamesId",
                        column: x => x.tablesNamesId,
                        principalTable: "TLItablesNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIvalidation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    DynamicAttId = table.Column<int>(nullable: false),
                    OperationId = table.Column<int>(nullable: false),
                    ValueString = table.Column<string>(nullable: true),
                    ValueDouble = table.Column<double>(nullable: true),
                    ValueDateTime = table.Column<DateTime>(nullable: true),
                    ValueBoolean = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIvalidation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIvalidation_TLIdynamicAtt_DynamicAttId",
                        column: x => x.DynamicAttId,
                        principalTable: "TLIdynamicAtt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIvalidation_TLIoperation_OperationId",
                        column: x => x.OperationId,
                        principalTable: "TLIoperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIattributeViewManagment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Enable = table.Column<bool>(nullable: false),
                    EditableManagmentViewId = table.Column<int>(nullable: false),
                    AttributeActivatedId = table.Column<int>(nullable: true),
                    DynamicAttId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIattributeViewManagment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIattributeViewManagment_TLIattributeActivated_AttributeActivatedId",
                        column: x => x.AttributeActivatedId,
                        principalTable: "TLIattributeActivated",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIattributeViewManagment_TLIdynamicAtt_DynamicAttId",
                        column: x => x.DynamicAttId,
                        principalTable: "TLIdynamicAtt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIattributeViewManagment_TLIeditableManagmentView_EditableManagmentViewId",
                        column: x => x.EditableManagmentViewId,
                        principalTable: "TLIeditableManagmentView",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIsolar",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    PVPanelBrandAndWattage = table.Column<string>(nullable: true),
                    PVArrayAzimuth = table.Column<string>(nullable: true),
                    PVArrayAngel = table.Column<string>(nullable: true),
                    Prefix = table.Column<string>(nullable: true),
                    PowerLossRatio = table.Column<string>(nullable: true),
                    NumberOfSSU = table.Column<int>(nullable: false),
                    NumberOfLightingRod = table.Column<int>(nullable: false),
                    NumberOfInstallPVs = table.Column<int>(nullable: false),
                    LocationDescription = table.Column<string>(nullable: true),
                    ExtenstionDimension = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    SpaceInstallation = table.Column<float>(nullable: false),
                    VisibleStatus = table.Column<string>(nullable: true),
                    SolarLibraryId = table.Column<int>(nullable: false),
                    CabinetId = table.Column<int>(nullable: true),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIsolar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIsolar_TLIcabinet_CabinetId",
                        column: x => x.CabinetId,
                        principalTable: "TLIcabinet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIsolar_TLIsolarLibrary_SolarLibraryId",
                        column: x => x.SolarLibraryId,
                        principalTable: "TLIsolarLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIstepActionFileGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    StepActionId = table.Column<int>(nullable: true),
                    ActorId = table.Column<int>(nullable: true),
                    IntegrationId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    GroupId = table.Column<int>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIstepActionFileGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIstepActionFileGroup_TLIactor_ActorId",
                        column: x => x.ActorId,
                        principalTable: "TLIactor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionFileGroup_TLIgroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "TLIgroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionFileGroup_TLIintegration_IntegrationId",
                        column: x => x.IntegrationId,
                        principalTable: "TLIintegration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionFileGroup_TLIstepAction_StepActionId",
                        column: x => x.StepActionId,
                        principalTable: "TLIstepAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionFileGroup_TLIuser_UserId",
                        column: x => x.UserId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIstepActionGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    StepActionId = table.Column<int>(nullable: true),
                    ActorId = table.Column<int>(nullable: true),
                    IntegrationId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    GroupId = table.Column<int>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIstepActionGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIstepActionGroup_TLIactor_ActorId",
                        column: x => x.ActorId,
                        principalTable: "TLIactor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionGroup_TLIgroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "TLIgroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionGroup_TLIintegration_IntegrationId",
                        column: x => x.IntegrationId,
                        principalTable: "TLIintegration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionGroup_TLIstepAction_StepActionId",
                        column: x => x.StepActionId,
                        principalTable: "TLIstepAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionGroup_TLIuser_UserId",
                        column: x => x.UserId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIstepActionIncomeItemStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    StepActionId = table.Column<int>(nullable: false),
                    ItemStatusId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIstepActionIncomeItemStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIstepActionIncomeItemStatus_TLIitemStatus_ItemStatusId",
                        column: x => x.ItemStatusId,
                        principalTable: "TLIitemStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIstepActionIncomeItemStatus_TLIstepAction_StepActionId",
                        column: x => x.StepActionId,
                        principalTable: "TLIstepAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIstepActionItemOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    StepActionId = table.Column<int>(nullable: false),
                    ActionItemOptionId = table.Column<int>(nullable: false),
                    OrderStatusId = table.Column<int>(nullable: true),
                    AllowNote = table.Column<bool>(nullable: true),
                    NoteIsMandatory = table.Column<bool>(nullable: true),
                    TLIitemStatusId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIstepActionItemOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIstepActionItemOption_TLIactionItemOption_ActionItemOptionId",
                        column: x => x.ActionItemOptionId,
                        principalTable: "TLIactionItemOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIstepActionItemOption_TLIorderStatus_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "TLIorderStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionItemOption_TLIstepAction_StepActionId",
                        column: x => x.StepActionId,
                        principalTable: "TLIstepAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIstepActionItemOption_TLIitemStatus_TLIitemStatusId",
                        column: x => x.TLIitemStatusId,
                        principalTable: "TLIitemStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIstepActionMailTo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    StepActionId = table.Column<int>(nullable: false),
                    GroupId = table.Column<int>(nullable: true),
                    ActorId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    TLIintegrationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIstepActionMailTo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIstepActionMailTo_TLIactor_ActorId",
                        column: x => x.ActorId,
                        principalTable: "TLIactor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionMailTo_TLIgroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "TLIgroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionMailTo_TLIstepAction_StepActionId",
                        column: x => x.StepActionId,
                        principalTable: "TLIstepAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIstepActionMailTo_TLIintegration_TLIintegrationId",
                        column: x => x.TLIintegrationId,
                        principalTable: "TLIintegration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionMailTo_TLIuser_UserId",
                        column: x => x.UserId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIstepActionOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    StepActionId = table.Column<int>(nullable: false),
                    ActionOptionId = table.Column<int>(nullable: false),
                    ItemStatusId = table.Column<int>(nullable: true),
                    OrderStatusId = table.Column<int>(nullable: true),
                    AllowNote = table.Column<bool>(nullable: false),
                    NoteIsMandatory = table.Column<bool>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIstepActionOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIstepActionOption_TLIactionOption_ActionOptionId",
                        column: x => x.ActionOptionId,
                        principalTable: "TLIactionOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIstepActionOption_TLIitemStatus_ItemStatusId",
                        column: x => x.ItemStatusId,
                        principalTable: "TLIitemStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionOption_TLIorderStatus_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "TLIorderStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionOption_TLIstepAction_StepActionId",
                        column: x => x.StepActionId,
                        principalTable: "TLIstepAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIstepActionPart",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    StepActionId = table.Column<int>(nullable: false),
                    PartId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    AllowUploadFile = table.Column<bool>(nullable: false),
                    UploadFileIsMandatory = table.Column<bool>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIstepActionPart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIstepActionPart_TLIpart_PartId",
                        column: x => x.PartId,
                        principalTable: "TLIpart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIstepActionPart_TLIstepAction_StepActionId",
                        column: x => x.StepActionId,
                        principalTable: "TLIstepAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIworkFlowType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    WorkFlowId = table.Column<int>(nullable: false),
                    nextStepActionId = table.Column<int>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIworkFlowType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIworkFlowType_TLIworkFlow_WorkFlowId",
                        column: x => x.WorkFlowId,
                        principalTable: "TLIworkFlow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIworkFlowType_TLIstepAction_nextStepActionId",
                        column: x => x.nextStepActionId,
                        principalTable: "TLIstepAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLImwPort",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Port_Name = table.Column<string>(nullable: true),
                    TX_Frequency = table.Column<string>(nullable: true),
                    MwBULibraryId = table.Column<int>(nullable: false),
                    MwBUId = table.Column<int>(nullable: false),
                    Port_Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLImwPort", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLImwPort_TLImwBU_MwBUId",
                        column: x => x.MwBUId,
                        principalTable: "TLImwBU",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLImwPort_TLImwBULibrary_MwBULibraryId",
                        column: x => x.MwBULibraryId,
                        principalTable: "TLImwBULibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIdependencyRow",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    DependencyId = table.Column<int>(nullable: true),
                    RowId = table.Column<int>(nullable: true),
                    LogicalOperationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIdependencyRow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIdependencyRow_TLIdependency_DependencyId",
                        column: x => x.DependencyId,
                        principalTable: "TLIdependency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIdependencyRow_TLIlogicalOperation_LogicalOperationId",
                        column: x => x.LogicalOperationId,
                        principalTable: "TLIlogicalOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIdependencyRow_TLIrow_RowId",
                        column: x => x.RowId,
                        principalTable: "TLIrow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIdynamicAttLibValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(nullable: true),
                    ValueString = table.Column<string>(nullable: true),
                    ValueDouble = table.Column<double>(nullable: true),
                    ValueDateTime = table.Column<DateTime>(nullable: true),
                    ValueBoolean = table.Column<bool>(nullable: true),
                    DynamicAttId = table.Column<int>(nullable: false),
                    disable = table.Column<bool>(nullable: false, defaultValue: false),
                    tablesNamesId = table.Column<int>(nullable: false),
                    InventoryId = table.Column<int>(nullable: false),
                    dynamicListValuesId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIdynamicAttLibValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIdynamicAttLibValue_TLIdynamicAtt_DynamicAttId",
                        column: x => x.DynamicAttId,
                        principalTable: "TLIdynamicAtt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIdynamicAttLibValue_TLIdynamicListValues_dynamicListValuesId",
                        column: x => x.dynamicListValuesId,
                        principalTable: "TLIdynamicListValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIdynamicAttLibValue_TLItablesNames_tablesNamesId",
                        column: x => x.tablesNamesId,
                        principalTable: "TLItablesNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIrowRule",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    RuleId = table.Column<int>(nullable: true),
                    RowId = table.Column<int>(nullable: true),
                    LogicalOperationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIrowRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIrowRule_TLIlogicalOperation_LogicalOperationId",
                        column: x => x.LogicalOperationId,
                        principalTable: "TLIlogicalOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIrowRule_TLIrow_RowId",
                        column: x => x.RowId,
                        principalTable: "TLIrow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIrowRule_TLIrule_RuleId",
                        column: x => x.RuleId,
                        principalTable: "TLIrule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIstepActionItemStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    StepActionItemOptionId = table.Column<int>(nullable: false),
                    IncomingItemStatusId = table.Column<int>(nullable: false),
                    OutgoingItemStatusId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIstepActionItemStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIstepActionItemStatus_TLIitemStatus_IncomingItemStatusId",
                        column: x => x.IncomingItemStatusId,
                        principalTable: "TLIitemStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIstepActionItemStatus_TLIitemStatus_OutgoingItemStatusId",
                        column: x => x.OutgoingItemStatusId,
                        principalTable: "TLIitemStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIstepActionItemStatus_TLIstepActionItemOption_StepActionItemOptionId",
                        column: x => x.StepActionItemOptionId,
                        principalTable: "TLIstepActionItemOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLInextStepAction",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    StepActionItemOptionId = table.Column<int>(nullable: true),
                    StepActionOptionId = table.Column<int>(nullable: true),
                    StepActionId = table.Column<int>(nullable: true),
                    NextStepActionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLInextStepAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLInextStepAction_TLIstepAction_StepActionId",
                        column: x => x.StepActionId,
                        principalTable: "TLIstepAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLInextStepAction_TLIstepActionItemOption_StepActionItemOptionId",
                        column: x => x.StepActionItemOptionId,
                        principalTable: "TLIstepActionItemOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLInextStepAction_TLIstepActionOption_StepActionOptionId",
                        column: x => x.StepActionOptionId,
                        principalTable: "TLIstepActionOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIstepActionPartGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    StepActionPartId = table.Column<int>(nullable: false),
                    ActorId = table.Column<int>(nullable: true),
                    IntegrationId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    GroupId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIstepActionPartGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIstepActionPartGroup_TLIactor_ActorId",
                        column: x => x.ActorId,
                        principalTable: "TLIactor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionPartGroup_TLIgroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "TLIgroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionPartGroup_TLIintegration_IntegrationId",
                        column: x => x.IntegrationId,
                        principalTable: "TLIintegration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIstepActionPartGroup_TLIstepActionPart_StepActionPartId",
                        column: x => x.StepActionPartId,
                        principalTable: "TLIstepActionPart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIstepActionPartGroup_TLIuser_UserId",
                        column: x => x.UserId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIticket",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    WorkFlowId = table.Column<int>(nullable: false),
                    SiteCode = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<int>(nullable: true),
                    IntegrationId = table.Column<int>(nullable: true),
                    StatusId = table.Column<int>(nullable: true),
                    TypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIticket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIticket_TLIuser_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIticket_TLIintegration_IntegrationId",
                        column: x => x.IntegrationId,
                        principalTable: "TLIintegration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIticket_TLIsite_SiteCode",
                        column: x => x.SiteCode,
                        principalTable: "TLIsite",
                        principalColumn: "SiteCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIticket_TLIorderStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "TLIorderStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIticket_TLIworkFlowType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "TLIworkFlowType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIticket_TLIworkFlow_WorkFlowId",
                        column: x => x.WorkFlowId,
                        principalTable: "TLIworkFlow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLImwRFU",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Azimuth = table.Column<float>(nullable: false),
                    heightBase = table.Column<float>(nullable: false),
                    SerialNumber = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    OwnerId = table.Column<int>(nullable: true),
                    MwRFULibraryId = table.Column<int>(nullable: false),
                    MwPortId = table.Column<int>(nullable: true),
                    SpaceInstallation = table.Column<float>(nullable: false),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLImwRFU", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLImwRFU_TLImwPort_MwPortId",
                        column: x => x.MwPortId,
                        principalTable: "TLImwPort",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLImwRFU_TLImwRFULibrary_MwRFULibraryId",
                        column: x => x.MwRFULibraryId,
                        principalTable: "TLImwRFULibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLImwRFU_TLIowner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "TLIowner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIallCivilInst",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    civilWithLegsId = table.Column<int>(nullable: true),
                    civilWithoutLegId = table.Column<int>(nullable: true),
                    civilNonSteelId = table.Column<int>(nullable: true),
                    ItemStatusId = table.Column<int>(nullable: true),
                    TicketId = table.Column<int>(nullable: true),
                    Draft = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIallCivilInst", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIallCivilInst_TLIitemStatus_ItemStatusId",
                        column: x => x.ItemStatusId,
                        principalTable: "TLIitemStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallCivilInst_TLIticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "TLIticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallCivilInst_TLIcivilNonSteel_civilNonSteelId",
                        column: x => x.civilNonSteelId,
                        principalTable: "TLIcivilNonSteel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallCivilInst_TLIcivilWithLegs_civilWithLegsId",
                        column: x => x.civilWithLegsId,
                        principalTable: "TLIcivilWithLegs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallCivilInst_TLIcivilWithoutLeg_civilWithoutLegId",
                        column: x => x.civilWithoutLegId,
                        principalTable: "TLIcivilWithoutLeg",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIallOtherInventoryInst",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    cabinetId = table.Column<int>(nullable: true),
                    solarId = table.Column<int>(nullable: true),
                    generatorId = table.Column<int>(nullable: true),
                    ItemStatusId = table.Column<int>(nullable: true),
                    TicketId = table.Column<int>(nullable: true),
                    Draft = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIallOtherInventoryInst", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIallOtherInventoryInst_TLIitemStatus_ItemStatusId",
                        column: x => x.ItemStatusId,
                        principalTable: "TLIitemStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallOtherInventoryInst_TLIticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "TLIticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallOtherInventoryInst_TLIcabinet_cabinetId",
                        column: x => x.cabinetId,
                        principalTable: "TLIcabinet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallOtherInventoryInst_TLIgenerator_generatorId",
                        column: x => x.generatorId,
                        principalTable: "TLIgenerator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallOtherInventoryInst_TLIsolar_solarId",
                        column: x => x.solarId,
                        principalTable: "TLIsolar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIsideArm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    HeightBase = table.Column<float>(nullable: true),
                    Azimuth = table.Column<float>(nullable: true),
                    ReservedSpace = table.Column<float>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    VisibleStatus = table.Column<string>(nullable: true),
                    SpaceInstallation = table.Column<float>(nullable: false),
                    sideArmLibraryId = table.Column<int>(nullable: false),
                    sideArmInstallationPlaceId = table.Column<int>(nullable: true),
                    ownerId = table.Column<int>(nullable: true),
                    sideArmTypeId = table.Column<int>(nullable: false),
                    ItemStatusId = table.Column<int>(nullable: true),
                    TicketId = table.Column<int>(nullable: true),
                    Draft = table.Column<bool>(nullable: false),
                    CenterHigh = table.Column<float>(nullable: false),
                    HBA = table.Column<float>(nullable: false),
                    HieghFromLand = table.Column<float>(nullable: false),
                    EquivalentSpace = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIsideArm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIsideArm_TLIitemStatus_ItemStatusId",
                        column: x => x.ItemStatusId,
                        principalTable: "TLIitemStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIsideArm_TLIticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "TLIticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIsideArm_TLIowner_ownerId",
                        column: x => x.ownerId,
                        principalTable: "TLIowner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIsideArm_TLIsideArmInstallationPlace_sideArmInstallationPlaceId",
                        column: x => x.sideArmInstallationPlaceId,
                        principalTable: "TLIsideArmInstallationPlace",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIsideArm_TLIsideArmLibrary_sideArmLibraryId",
                        column: x => x.sideArmLibraryId,
                        principalTable: "TLIsideArmLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIsideArm_TLIsideArmType_sideArmTypeId",
                        column: x => x.sideArmTypeId,
                        principalTable: "TLIsideArmType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIticketAction",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    TicketId = table.Column<int>(nullable: false),
                    StepActionId = table.Column<int>(nullable: false),
                    ExecuterId = table.Column<int>(nullable: true),
                    ExecutionDate = table.Column<DateTime>(nullable: true),
                    AssignedToId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIticketAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIticketAction_TLIuser_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIticketAction_TLIuser_ExecuterId",
                        column: x => x.ExecuterId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIticketAction_TLIstepAction_StepActionId",
                        column: x => x.StepActionId,
                        principalTable: "TLIstepAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIticketAction_TLIticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "TLIticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIticketStep",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    TicketId = table.Column<int>(nullable: true),
                    StepId = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIticketStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIticketStep_TLIstep_StepId",
                        column: x => x.StepId,
                        principalTable: "TLIstep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIticketStep_TLIticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "TLIticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIticketTarget",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    TicketId = table.Column<int>(nullable: false),
                    TargetTable = table.Column<string>(nullable: true),
                    TableId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIticketTarget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIticketTarget_TLIticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "TLIticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIallLoadInst",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    mwBUId = table.Column<int>(nullable: true),
                    mwDishId = table.Column<int>(nullable: true),
                    mwODUId = table.Column<int>(nullable: true),
                    mwRFUId = table.Column<int>(nullable: true),
                    mwOtherId = table.Column<int>(nullable: true),
                    radioAntennaId = table.Column<int>(nullable: true),
                    radioRRUId = table.Column<int>(nullable: true),
                    radioOtherId = table.Column<int>(nullable: true),
                    powerId = table.Column<int>(nullable: true),
                    loadOtherId = table.Column<int>(nullable: true),
                    ItemStatusId = table.Column<int>(nullable: true),
                    TicketId = table.Column<int>(nullable: true),
                    Draft = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIallLoadInst", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIallLoadInst_TLIitemStatus_ItemStatusId",
                        column: x => x.ItemStatusId,
                        principalTable: "TLIitemStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallLoadInst_TLIticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "TLIticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallLoadInst_TLIloadOther_loadOtherId",
                        column: x => x.loadOtherId,
                        principalTable: "TLIloadOther",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallLoadInst_TLImwBU_mwBUId",
                        column: x => x.mwBUId,
                        principalTable: "TLImwBU",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallLoadInst_TLImwDish_mwDishId",
                        column: x => x.mwDishId,
                        principalTable: "TLImwDish",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallLoadInst_TLImwODU_mwODUId",
                        column: x => x.mwODUId,
                        principalTable: "TLImwODU",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallLoadInst_TLImwOther_mwOtherId",
                        column: x => x.mwOtherId,
                        principalTable: "TLImwOther",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallLoadInst_TLImwRFU_mwRFUId",
                        column: x => x.mwRFUId,
                        principalTable: "TLImwRFU",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallLoadInst_TLIpower_powerId",
                        column: x => x.powerId,
                        principalTable: "TLIpower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallLoadInst_TLIradioAntenna_radioAntennaId",
                        column: x => x.radioAntennaId,
                        principalTable: "TLIradioAntenna",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallLoadInst_TLIradioOther_radioOtherId",
                        column: x => x.radioOtherId,
                        principalTable: "TLIradioOther",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIallLoadInst_TLIRadioRRU_radioRRUId",
                        column: x => x.radioRRUId,
                        principalTable: "TLIRadioRRU",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIcivilSiteDate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    LongitudinalSpindleLengthm = table.Column<float>(nullable: true),
                    HorizontalSpindleLengthm = table.Column<float>(nullable: true),
                    InstallationDate = table.Column<DateTime>(nullable: false),
                    allCivilInstId = table.Column<int>(nullable: false),
                    SiteCode = table.Column<string>(nullable: false),
                    ReservedSpace = table.Column<bool>(nullable: false),
                    Dismantle = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcivilSiteDate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIcivilSiteDate_TLIsite_SiteCode",
                        column: x => x.SiteCode,
                        principalTable: "TLIsite",
                        principalColumn: "SiteCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIcivilSiteDate_TLIallCivilInst_allCivilInstId",
                        column: x => x.allCivilInstId,
                        principalTable: "TLIallCivilInst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIcivilSupportDistance",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Distance = table.Column<float>(nullable: true),
                    Azimuth = table.Column<float>(nullable: true),
                    CivilInstId = table.Column<int>(nullable: false),
                    ReferenceCivilId = table.Column<int>(nullable: false),
                    SiteCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcivilSupportDistance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIcivilSupportDistance_TLIallCivilInst_CivilInstId",
                        column: x => x.CivilInstId,
                        principalTable: "TLIallCivilInst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIcivilSupportDistance_TLIsite_SiteCode",
                        column: x => x.SiteCode,
                        principalTable: "TLIsite",
                        principalColumn: "SiteCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIotherInSite",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    OtherInSiteStatus = table.Column<string>(nullable: true),
                    OtherInventoryStatus = table.Column<string>(nullable: true),
                    InstallationDate = table.Column<DateTime>(nullable: false),
                    SiteCode = table.Column<string>(nullable: true),
                    allOtherInventoryInstId = table.Column<int>(nullable: false),
                    ReservedSpace = table.Column<bool>(nullable: false),
                    Dismantle = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIotherInSite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIotherInSite_TLIsite_SiteCode",
                        column: x => x.SiteCode,
                        principalTable: "TLIsite",
                        principalColumn: "SiteCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIotherInSite_TLIallOtherInventoryInst_allOtherInventoryInstId",
                        column: x => x.allOtherInventoryInstId,
                        principalTable: "TLIallOtherInventoryInst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIotherInventoryDistance",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Distance = table.Column<float>(nullable: false),
                    Azimuth = table.Column<float>(nullable: false),
                    allOtherInventoryInstId = table.Column<int>(nullable: false),
                    ReferenceOtherInventoryId = table.Column<int>(nullable: false),
                    SiteCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIotherInventoryDistance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIotherInventoryDistance_TLIsite_SiteCode",
                        column: x => x.SiteCode,
                        principalTable: "TLIsite",
                        principalColumn: "SiteCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIotherInventoryDistance_TLIallOtherInventoryInst_allOtherInventoryInstId",
                        column: x => x.allOtherInventoryInstId,
                        principalTable: "TLIallOtherInventoryInst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIdynamicAttInstValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(nullable: true),
                    ValueString = table.Column<string>(nullable: true),
                    ValueDouble = table.Column<double>(nullable: true),
                    ValueDateTime = table.Column<DateTime>(nullable: true),
                    ValueBoolean = table.Column<bool>(nullable: true),
                    DynamicAttId = table.Column<int>(nullable: false),
                    disable = table.Column<bool>(nullable: false, defaultValue: false),
                    sideArmId = table.Column<int>(nullable: true),
                    tablesNamesId = table.Column<int>(nullable: false),
                    InventoryId = table.Column<int>(nullable: false),
                    dynamicListValuesId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIdynamicAttInstValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIdynamicAttInstValue_TLIdynamicAtt_DynamicAttId",
                        column: x => x.DynamicAttId,
                        principalTable: "TLIdynamicAtt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIdynamicAttInstValue_TLIdynamicListValues_dynamicListValuesId",
                        column: x => x.dynamicListValuesId,
                        principalTable: "TLIdynamicListValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIdynamicAttInstValue_TLIsideArm_sideArmId",
                        column: x => x.sideArmId,
                        principalTable: "TLIsideArm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIdynamicAttInstValue_TLItablesNames_tablesNamesId",
                        column: x => x.tablesNamesId,
                        principalTable: "TLItablesNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIagenda",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    TicketActionId = table.Column<int>(nullable: false),
                    period = table.Column<int>(nullable: true),
                    ExecuterId = table.Column<int>(nullable: true),
                    ExecutionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIagenda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIagenda_TLIuser_ExecuterId",
                        column: x => x.ExecuterId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIagenda_TLIticketAction_TicketActionId",
                        column: x => x.TicketActionId,
                        principalTable: "TLIticketAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIticketActionFile",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    TicketActionId = table.Column<int>(nullable: false),
                    filename = table.Column<string>(nullable: true),
                    mimetype = table.Column<string>(nullable: true),
                    filebody = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIticketActionFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIticketActionFile_TLIticketAction_TicketActionId",
                        column: x => x.TicketActionId,
                        principalTable: "TLIticketAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIworkflowTableHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    RecordId = table.Column<int>(nullable: false),
                    TablesNameId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    TicketId = table.Column<int>(nullable: false),
                    TicketActionId = table.Column<int>(nullable: false),
                    ItemStatusId = table.Column<int>(nullable: false),
                    PartId = table.Column<int>(nullable: false),
                    PreviousHistoryId = table.Column<int>(nullable: true),
                    HistoryTypeId = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Operation = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIworkflowTableHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIworkflowTableHistory_TLIhistoryType_HistoryTypeId",
                        column: x => x.HistoryTypeId,
                        principalTable: "TLIhistoryType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIworkflowTableHistory_TLIitemStatus_ItemStatusId",
                        column: x => x.ItemStatusId,
                        principalTable: "TLIitemStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIworkflowTableHistory_TLIpart_PartId",
                        column: x => x.PartId,
                        principalTable: "TLIpart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIworkflowTableHistory_TLIworkflowTableHistory_PreviousHistoryId",
                        column: x => x.PreviousHistoryId,
                        principalTable: "TLIworkflowTableHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIworkflowTableHistory_TLItablesNames_TablesNameId",
                        column: x => x.TablesNameId,
                        principalTable: "TLItablesNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIworkflowTableHistory_TLIticketAction_TicketActionId",
                        column: x => x.TicketActionId,
                        principalTable: "TLIticketAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIworkflowTableHistory_TLIticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "TLIticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIworkflowTableHistory_TLIuser_UserId",
                        column: x => x.UserId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIticketEquipment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    TicketId = table.Column<int>(nullable: false),
                    EquipemntId = table.Column<int>(nullable: false),
                    ItemStatusId = table.Column<int>(nullable: true),
                    TicketTargetId = table.Column<int>(nullable: true),
                    Operation = table.Column<int>(nullable: false),
                    Proposal = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIticketEquipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIticketEquipment_TLIequipment_EquipemntId",
                        column: x => x.EquipemntId,
                        principalTable: "TLIequipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIticketEquipment_TLIitemStatus_ItemStatusId",
                        column: x => x.ItemStatusId,
                        principalTable: "TLIitemStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIticketEquipment_TLIticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "TLIticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIticketEquipment_TLIticketTarget_TicketTargetId",
                        column: x => x.TicketTargetId,
                        principalTable: "TLIticketTarget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIcivilLoads",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    InstallationDate = table.Column<DateTime>(nullable: false),
                    ItemOnCivilStatus = table.Column<string>(nullable: true),
                    ItemStatus = table.Column<string>(nullable: true),
                    Dismantle = table.Column<bool>(nullable: false),
                    legId = table.Column<int>(nullable: true),
                    Leg2Id = table.Column<int>(nullable: true),
                    ReservedSpace = table.Column<bool>(nullable: false),
                    sideArmId = table.Column<int>(nullable: true),
                    allCivilInstId = table.Column<int>(nullable: false),
                    allLoadInstId = table.Column<int>(nullable: true),
                    civilSteelSupportCategoryId = table.Column<int>(nullable: false),
                    SiteCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcivilLoads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIcivilLoads_TLIsite_SiteCode",
                        column: x => x.SiteCode,
                        principalTable: "TLIsite",
                        principalColumn: "SiteCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIcivilLoads_TLIallCivilInst_allCivilInstId",
                        column: x => x.allCivilInstId,
                        principalTable: "TLIallCivilInst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIcivilLoads_TLIallLoadInst_allLoadInstId",
                        column: x => x.allLoadInstId,
                        principalTable: "TLIallLoadInst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIcivilLoads_TLIcivilSteelSupportCategory_civilSteelSupportCategoryId",
                        column: x => x.civilSteelSupportCategoryId,
                        principalTable: "TLIcivilSteelSupportCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIcivilLoads_TLIleg_legId",
                        column: x => x.legId,
                        principalTable: "TLIleg",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIcivilLoads_TLIsideArm_sideArmId",
                        column: x => x.sideArmId,
                        principalTable: "TLIsideArm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIagendaGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    AgendaId = table.Column<int>(nullable: false),
                    ActorId = table.Column<int>(nullable: true),
                    IntegrationId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    GroupId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIagendaGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIagendaGroup_TLIactor_ActorId",
                        column: x => x.ActorId,
                        principalTable: "TLIactor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIagendaGroup_TLIagenda_AgendaId",
                        column: x => x.AgendaId,
                        principalTable: "TLIagenda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIagendaGroup_TLIgroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "TLIgroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIagendaGroup_TLIintegration_IntegrationId",
                        column: x => x.IntegrationId,
                        principalTable: "TLIintegration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIagendaGroup_TLIuser_UserId",
                        column: x => x.UserId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIhistoryDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    TablesHistoryId = table.Column<int>(nullable: true),
                    WorkflowTableHistoryId = table.Column<int>(nullable: true),
                    AttName = table.Column<string>(nullable: false),
                    OldValue = table.Column<string>(nullable: true),
                    NewValue = table.Column<string>(nullable: false),
                    AttributeType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIhistoryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIhistoryDetails_TLItablesHistory_TablesHistoryId",
                        column: x => x.TablesHistoryId,
                        principalTable: "TLItablesHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIhistoryDetails_TLIworkflowTableHistory_WorkflowTableHistoryId",
                        column: x => x.WorkflowTableHistoryId,
                        principalTable: "TLIworkflowTableHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TLIticketEquipmentAction",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    TicketId = table.Column<int>(nullable: false),
                    TicketActionId = table.Column<int>(nullable: true),
                    TicketEquipmentId = table.Column<int>(nullable: false),
                    ExecuterId = table.Column<int>(nullable: false),
                    ExecutionDate = table.Column<DateTime>(nullable: true),
                    ItemStatusId = table.Column<int>(nullable: false),
                    ProposalType = table.Column<int>(nullable: false),
                    Proposal = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIticketEquipmentAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIticketEquipmentAction_TLIuser_ExecuterId",
                        column: x => x.ExecuterId,
                        principalTable: "TLIuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIticketEquipmentAction_TLIitemStatus_ItemStatusId",
                        column: x => x.ItemStatusId,
                        principalTable: "TLIitemStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIticketEquipmentAction_TLIticketAction_TicketActionId",
                        column: x => x.TicketActionId,
                        principalTable: "TLIticketAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIticketEquipmentAction_TLIticketEquipment_TicketEquipmentId",
                        column: x => x.TicketEquipmentId,
                        principalTable: "TLIticketEquipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIcivilLoadLegs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    civilLoadsId = table.Column<int>(nullable: false),
                    legId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIcivilLoadLegs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIcivilLoadLegs_TLIcivilLoads_civilLoadsId",
                        column: x => x.civilLoadsId,
                        principalTable: "TLIcivilLoads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIcivilLoadLegs_TLIleg_legId",
                        column: x => x.legId,
                        principalTable: "TLIleg",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIticketEquipmentActionFile",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    TicketEquipmentActionId = table.Column<int>(nullable: false),
                    filename = table.Column<string>(nullable: true),
                    mimetype = table.Column<string>(nullable: true),
                    filebody = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIticketEquipmentActionFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIticketEquipmentActionFile_TLIticketEquipmentAction_TicketEquipmentActionId",
                        column: x => x.TicketEquipmentActionId,
                        principalTable: "TLIticketEquipmentAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLIticketOptionNote",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    TicketId = table.Column<int>(nullable: false),
                    TicketActionId = table.Column<int>(nullable: false),
                    TicketEquipmentActionId = table.Column<int>(nullable: true),
                    StepActionOptionId = table.Column<int>(nullable: true),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLIticketOptionNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TLIticketOptionNote_TLIstepActionOption_StepActionOptionId",
                        column: x => x.StepActionOptionId,
                        principalTable: "TLIstepActionOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIticketOptionNote_TLIticketAction_TicketActionId",
                        column: x => x.TicketActionId,
                        principalTable: "TLIticketAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLIticketOptionNote_TLIticketEquipmentAction_TicketEquipmentActionId",
                        column: x => x.TicketEquipmentActionId,
                        principalTable: "TLIticketEquipmentAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TLIticketOptionNote_TLIticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "TLIticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TLIlogisticalType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Vendor" });

            migrationBuilder.InsertData(
                table: "TLIlogisticalType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Designer" });

            migrationBuilder.InsertData(
                table: "TLIlogisticalType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Supplier" });

            migrationBuilder.InsertData(
                table: "TLIlogisticalType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "Manufacturer" });

            migrationBuilder.InsertData(
                table: "TLIlogisticalType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 5, "Contractor" });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 319, "GetForAdd", true, "RadioLibrary", "RadioRRULibrary", "Get Radios For Add", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 320, "DeleteRadioOtherLibrary", true, "RadioLibrary", "RadioOtherLibrary", "Delete Radio Other By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 321, "DeleteRadioAntennaLibrary", true, "RadioLibrary", "RadioAntennaLibrary", "Delete Radio Antenna By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 322, "DeleteRadioRRULibrary", true, "RadioLibrary", "RadioRRULibrary", "Delete Radio RRU By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 323, "getAll", true, "Role", "Role", "Get All Roles", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 326, "EditRole", true, "Role", "Role", "Edit Role", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 325, "AddRolePermissionList", true, "Role", "Role", "Add Role Permission List", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 318, "DisableRadioRRULibrary", true, "RadioLibrary", "RadioRRULibrary", "Disable Radio RRU Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 327, "DeleteRole", true, "Role", "Role", "Delete Role", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 328, "DeleteRoleGroups", true, "Role", "Role", "Delete Role Groups", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 329, "GetAllPermissionsByRoleId", true, "RolePermission", "RolePermissions", "Get All Permissions By Role Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 324, "CheckRoleGroups", true, "Role", "Role", "Check Role Groups", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 317, "DisableRadioAntennaLibrary", true, "RadioLibrary", "RadioAntennaLibrary", "Disable Radio Antenna Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 315, "UpdateRadioRRULibrary", true, "RadioLibrary", "RadioRRULibrary", "Update Radio RRU Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 330, "GetSideArm", true, "SideArm", "SideArm", "Get Side Arms", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 314, "UpdateRadioAntennaLibrary", true, "RadioLibrary", "RadioAntennaLibrary", "Update Radio Antenna Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 313, "UpdateRadioOtherLibrary", true, "RadioLibrary", "RadioOtherLibrary", "Update Radio Other Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 312, "AddRadioRRULibrary", true, "RadioLibrary", "RadioRRULibrary", "Add Radio RRU Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 311, "AddRadioAntennaLibrary", true, "RadioLibrary", "RadioAntennaLibrary", "Add Radio Antenna Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 310, "AddRadioOtherLibrary", true, "RadioLibrary", "RadioOtherLibrary", "Add Radio Other Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 309, "GetRadioAntennaLibraryById", true, "RadioLibrary", "RadioAntennaLibrary", "Get Radio Antenna Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 307, "GetRadioRRULibraryById", true, "RadioLibrary", "RadioRRULibrary", "Get Radio RRU Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 306, "GetOtherRadioLibraryById", true, "RadioLibrary", "OtherRadioLibrary", "Get Other Radio Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 305, "GetRadioRRULibraries", true, "RadioLibrary", "RadioRRULibrary", "Get Radio RRU Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 304, "GetRadioAntennaLibraries", true, "RadioLibrary", "RadioAntennaLibrary", "Get Radio Antenna Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 303, "GetOtherRadioLibraries", true, "RadioLibrary", "OtherRadioLibrary", "Get Other Radio Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 316, "DisableRadioOtherLibrary", true, "RadioLibrary", "RadioOtherLibrary", "Disable Radio Other Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 331, "getSideArmsWithEnabledAtt", true, "SideArm", "SideArm", "Get Side Arms's Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 482, "getById", true, "SideArm", "SideArm", "Get Side Arms By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 302, "GetRadioOtherList", true, "RadioInst", "RadioOther", "Get Radio Other List", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 359, "GetAllSiteStatus", true, "Site", "SiteStatus", "Get All Site Status", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 358, "GetSMIS_Site", true, "Site", "Site", "Get SMIS Site", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 357, "GetSiteStatusById", true, "Site", "SiteStatus", "Get Site Status By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 356, "AddSiteStatus", true, "Site", "SiteStatus", "Add Site Status", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 355, "UpdateSiteStatus", true, "Site", "SiteStatus", "Update Site Status", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 354, "GetSpaceDetails", true, "Site", "SiteSpace", "Get Space Details", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 353, "GetLoadsBySiteCode", true, "Site", "Loads", "Get Other Loads By Site Code", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 352, "GetOtherInventoriesBySiteCode", true, "Site", "OtherInventory", "Get Other Inventories By Site Code", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 351, "GetSideArmsBySiteCode", true, "Site", "SideArm", "Get SideArms By Site Code", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 350, "GetCivilsBySiteCode", true, "Site", "Civil", "Get Civils By Site Code", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 349, "getAllSites", true, "Site", "Site", "Get All Sites", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 348, "GetSideArmLibs", true, "SideArmLibrary", "SideArmLibrary", "Get Side Arm Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 332, "getSideArmsForAdd", true, "SideArm", "SideArm", "Get Side Arms For Add", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 347, "DeleteSideArmLibrary", true, "SideArmLibrary", "SideArmLibrary", "Delete Side Arm Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 345, "EditSideArmLibrary", true, "SideArmLibrary", "SideArmLibrary", "Edit Side Arm Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 344, "AddSideArmLibrary", true, "SideArmLibrary", "SideArmLibrary", "Add Side Arm Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 343, "GetSideArmLibraryById", true, "SideArmLibrary", "SideArmLibrary", "Get Side Arms Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 342, "GetSideArmLibrariesWithEnabledAttributes", true, "SideArmLibrary", "SideArmLibrary", "Get Side Arms Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 340, "getAll", true, "SideArmLibrary", "SideArmLibrary", "Get Side Arms Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 339, "GetSideArmType", true, "SideArm", "SideArm", "Get Side Arm's Type", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 338, "GetAttForAdd", true, "SideArm", "SideArm", "Get Side Arm's Attributes For Add", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 337, "GetSideArmInstallationPlaceByType", true, "SideArm", "SideArm", "Get Side Arm Installation Place By Type", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 336, "UpdateSideArmInstallationPlace", true, "SideArm", "SideArm", "Update Side Arm Installation Place", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 335, "AddSideArmInstallationPlace", true, "SideArm", "SideArm", "Add Side Arm Installation Place", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 334, "UpdateSideArm", true, "SideArm", "SideArm", "Update Side Arm", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 333, "AddSideArm", true, "SideArm", "SideArm", "Add Side Arm", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 346, "DisableSideArmLibrary", true, "SideArmLibrary", "SideArmLibrary", "Disable Side Arm Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 301, "GetRadioAntennasList", true, "RadioInst", "RadioAntenna", "Get Radio Antennas List", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 298, "EditRadioAntennaInstallation", true, "RadioInst", "RadioAntennaInstallation", "Edit Radio Antenna Installation", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 360, "DeleteSiteStatus", true, "Site", "SiteStatus", "Delete Site Status", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 267, "AddSolar", true, "OtherInventoryInst", "Solar", "Add Solar", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 266, "AddCabinet", true, "OtherInventoryInst", "Cabinet", "Add Cabinet", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 265, "GetAttForAddGenerator", true, "OtherInventoryInst", "Generator", "Get Attributes For Add Generator", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 264, "GetAttForAddSolar", true, "OtherInventoryInst", "Solar", "Get Attributes For Add Solar", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 263, "GetAttForAddCabinet", true, "OtherInventoryInst", "Cabinet", "Get Attributes For Add Cabinet", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 262, "getMW_RFU", true, "MWInst", "MW_RFU", "Get MW_RFU", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 261, "getMW_Dish", true, "MWInst", "MW_Dish", "Get MW_Dish", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 260, "getMW_ODU", true, "MWInst", "MW_ODU", "Get MW_ODU", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 259, "getMW_BU", true, "MWInst", "MW_BU", "Get MW_BU", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 258, "GetMW_OtherById", true, "MWInst", "MW_Other", "Get MW_Other By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 257, "GetMW_RFUById", true, "MWInst", "MW_RFU", "Get MW_RFU By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 256, "GetMW_DishById", true, "MWInst", "MW_Dish", "Get MW_Dish By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 268, "AddGenerator", true, "OtherInventoryInst", "Generator", "Add Generator", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 255, "GetMW_ODUById", true, "MWInst", "MW_ODU", "Get MW_ODU By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 253, "EditMw_Other", true, "MWInst", "Mw_Other", "Edit Mw_Other", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 252, "EditMW_RFU", true, "MWInst", "MW_RFU", "Edit MW_RFU", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 251, "EditMW_ODU", true, "MWInst", "MW_ODU", "Edit MW_ODU", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 250, "EditMW_Dish", true, "MWInst", "MW_Dish", "Edit MW_Dish", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 249, "EditMW_BU", true, "MWInst", "MW_BU", "Edit MW_BU", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 248, "AddMW_RFU", true, "MWInst", "MW_RFU", "Add MW_RFU", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 247, "AddMW_Dish", true, "MWInst", "MW_Dish", "Add MW_Dish", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 246, "AddMW_ODU", true, "MWInst", "MW_ODU", "Add MW_ODU", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 245, "AddMW_BU", true, "MWInst", "MW_BU", "Add MW_BU", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 244, "GetAttForAddMW_Other", true, "MWInst", "MW_Other", "Get Attributes For Add MW_Other", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 243, "GetAttForAddMW_RFU", true, "MWInst", "MW_RFU", "Get Attributes For Add MW_RFU", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 242, "GetAttForAddMW_Dish", true, "MWInst", "MW_Dish", "Get Attributes For Add MW_Dish", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 254, "GetMW_BUById", true, "MWInst", "MW_BU", "Get MW_BU By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 300, "GetRadioRRUsList", true, "RadioInst", "RadioRRU", "Get Radio RRUs List", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 269, "GetCabinetById", true, "OtherInventoryInst", "Cabinet", "Get Cabinet By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 271, "GetGeneratorById", true, "OtherInventoryInst", "Generator", "Get Generator By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 297, "GetById", true, "RadioInst", "Radio", "Get Radio By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 296, "AddRadioOtherInstallation", true, "RadioInst", "RadioOther", "Add Radio Other Installation", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 295, "AddRadioRRUInstallation", true, "RadioInst", "RadioRRU", "Add Radio RRU Installation", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 294, "AddRadioAntenna", true, "RadioInst", "RadioAntenna", "Add Radio Antenna Installation", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 293, "GetAttForAdd", true, "RadioInst", "Radio", "Get Radio Attributes For Add", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 292, "GetList", true, "Power", "Power", "Get Power List", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 291, "GetById", true, "Power", "Power", "Get Power By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 290, "EditPower", true, "Power", "Power", "Edit Power", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 289, "AddPower", true, "Power", "Power", "Add Power", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 288, "GetAttForAdd", true, "Power", "Power", "Get Power Attributes For Add", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 287, "AddPermission", true, "Permission", "Permission", "Add Permission", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 286, "getAll", true, "Permission", "Permission", "Get All Permissions", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 270, "GetSolarById", true, "OtherInventoryInst", "Solar", "Get Solar By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 285, "EditOwner", true, "Owner", "Owner", "Edit Owner", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 283, "getById", true, "Owner", "Owner", "Get Owner By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 282, "getAll", true, "Owner", "Owner", "Get All Owners", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 281, "GetAllOtherInventory", true, "OtherInventoryInst", "OtherInventory", "Get All Other Inventory", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 280, "GetGeneratorBySiteWithEnabledAtt", true, "OtherInventoryInst", "Generator", "Get Generator By Site With Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 279, "GetSolarBySiteWithEnabledAtt", true, "OtherInventoryInst", "Solar", "Get Solar By Site With Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 278, "GetCabinetBySiteWithEnabledAtt", true, "OtherInventoryInst", "Cabinet", "Get Cabinet By Site With Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 277, "GetGeneratorBySite", true, "OtherInventoryInst", "Generator", "Get Generator By Site", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 276, "GetSolarBySite", true, "OtherInventoryInst", "Solar", "Get Solar By Site", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 275, "GetCabinetBySite", true, "OtherInventoryInst", "Cabinet", "Get Cabinet By Site", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 274, "EditGenerator", true, "OtherInventoryInst", "Generator", "Edit Generator", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 273, "EditSolar", true, "OtherInventoryInst", "Solar", "Edit Solar", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 272, "EditCabinet", true, "OtherInventoryInst", "Cabinet", "Edit Cabinet", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 284, "AddOwner", true, "Owner", "Owner", "Add Owner", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 361, "GetSteelCivil", true, "Site", "CivilSteel", "Get Steel Civil", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 363, "GetLoadsOnSite", true, "Site", "Loads", "Get Loads On Site", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 241, "GetAttForAddMW_ODU", true, "MWInst", "MW_ODU", "Get Attributes For Add MW_ODU", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 450, "SetStepActionPermission", true, "Workflow", "Workflow", "Set Step Action Permission", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 449, "AddTicketStatusStepAction", true, "Workflow", "Workflow", "Add Ticket Status Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 448, "AddCivilDecisionStepAction", true, "Workflow", "Workflow", "Add Civil Decision Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 447, "AddCivilValidationStepAction", true, "Workflow", "Workflow", "Add Civil Validation Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 446, "AddTelecomValidationStepAction", true, "Workflow", "Workflow", "Add Telecom Validation Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 445, "AddProposalApprovedStepAction", true, "Workflow", "Workflow", "Add Proposal Approved Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 444, "AddStudyResultStepAction", true, "Workflow", "Workflow", "Add Study Result Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 443, "AddCheckAvailableSpaceStepAction", true, "Workflow", "Workflow", "Add Check Available Space Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 442, "AddSelectTargetSupportStepAction", true, "Workflow", "Workflow", "Add Select Target Support Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 441, "AddConditionStepAction", true, "Workflow", "Workflow", "Add Condition Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 440, "AddStepActionApplyCalculation", true, "Workflow", "Workflow", "Add Step Action Apply Calculation", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 439, "AddCorrectDataStepAction", true, "Workflow", "Workflow", "Add Correct Data Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 451, "UpdateStepAction", true, "Workflow", "Workflow", "Update Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 438, "AddUpdateDataStepAction", true, "Workflow", "Workflow", "Add Update Data Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 436, "AddUploadFileStepAction", true, "Workflow", "Workflow", "Add Upload File Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 435, "AddMailStepAction", true, "Workflow", "Workflow", "Add Mail Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 434, "GetStepActionById", true, "Workflow", "Workflow", "Get Step Action By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 433, "GetWorkFlowStepActions", true, "Workflow", "Workflow", "Get WorkFlow Step Actions By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 432, "GetAllStepActions", true, "Workflow", "Workflow", "Get All Step Actions", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 431, "GetActionById", true, "Workflow", "Workflow", "Get Action By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 430, "GetAvailableSpaceOptions", true, "Workflow", "Workflow", "Get Available Space Options", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 429, "GetCivilDecisionAction", true, "Workflow", "Workflow", "Get Civil Decision Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 428, "GetCivilDecisionActions", true, "Workflow", "Workflow", "Get Civil Decision Actions", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 427, "GetCivilValidationActions", true, "Workflow", "Workflow", "Get Civil Validation Actions", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 426, "GetTelecomValidationActions", true, "Workflow", "Workflow", "Get Telecom Validation Actions", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 425, "GetConditionActions", true, "Workflow", "Workflow", "Get Condition Actions", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 437, "AddInsertDataStepAction", true, "Workflow", "Workflow", "Add Insert Data Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 424, "GetAllActions", true, "Workflow", "Workflow", "Get All Actions", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 452, "MoveUpStepAction", true, "Workflow", "Workflow", "Move Up Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 455, "DelStepAction", true, "Workflow", "Workflow", "Delete Step Action By StepId", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 454, "UpdateItemStatus", true, "WorkFlowSetting", "WorkflowSetting", "Update Item Status", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 375, "AddItemStatus", true, "WorkFlowSetting", "WorkflowSetting", "Add Item Status", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 341, "GetItemStatusById", true, "WorkFlowSetting", "WorkflowSetting", "Get Item Status By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 308, "GetAllItemStatus", true, "WorkFlowSetting", "WorkflowSetting", "Get All Item Status", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 168, "GetSubOptionsByOptionId", true, "WorkFlowSetting", "WorkflowSetting", "Get SubOptions By Option Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 476, "UpdateSubOption", true, "WorkFlowSetting", "WorkflowSetting", "Update Sub Option", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 475, "AddSubOption", true, "WorkFlowSetting", "WorkflowSetting", "Add SubOption", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 474, "GetAllSubOptions", true, "WorkFlowSetting", "WorkflowSetting", "Get All SubOptions", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 473, "UpdateOption", true, "WorkFlowSetting", "WorkflowSetting", "Update Option", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 472, "AddOption", true, "WorkFlowSetting", "WorkflowSetting", "Add Option", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 471, "GetAllOptions", true, "WorkFlowSetting", "WorkflowSetting", "Get All Options", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 470, "UpdateCondition", true, "WorkFlowSetting", "WorkflowSetting", "Update Condition", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 453, "MoveDownStepAction", true, "Workflow", "Workflow", "Move Down Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 469, "AddCondition", true, "WorkFlowSetting", "WorkflowSetting", "Add Condition", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 467, "GetOptionsByConditionId", true, "WorkFlowSetting", "WorkflowSetting", "Get Options By Condition Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 466, "EditTicketStatus", true, "WorkFlowSetting", "WorkflowSetting", "Edit Ticket Status", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 465, "AddTicketStatus", true, "WorkFlowSetting", "WorkflowSetting", "Add Ticket Status", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 464, "getTicketStatusById", true, "WorkFlowSetting", "WorkflowSetting", "Get Ticket Status By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 463, "getAllTicketStatus", true, "WorkFlowSetting", "WorkflowSetting", "Get All Ticket Status", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 462, "UpdateMailTemplate", true, "WorkFlowSetting", "WorkflowSetting", "Update Mail Template", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 461, "AddMailTemplate", true, "WorkFlowSetting", "WorkflowSetting", "Add Mail Template", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 460, "GetAllMailTemplates", true, "WorkFlowSetting", "WorkflowSetting", "Get All Mail Templates", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 459, "UpdateActionItemOption", true, "WorkFlowSetting", "WorkflowSetting", "Update Action Item Option", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 458, "AddActionItemOption", true, "WorkFlowSetting", "WorkflowSetting", "Add Action Item Option", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 457, "GetAllActionItemOptions", true, "WorkFlowSetting", "WorkflowSetting", "Get All Action Item Options", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 456, "GetAllParts", true, "WorkFlowSetting", "WorkflowSetting", "Get All Parts", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 468, "GetAllConditions", true, "WorkFlowSetting", "WorkflowSetting", "Get All Conditions", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 362, "GetNonSteel", true, "Site", "CivilNonSteel", "Get Non Steel Civil", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 423, "DeleteWorkflowType", true, "Workflow", "Workflow", "Delete Workflow Type By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 421, "AddWorkflowType", true, "Workflow", "Workflow", "Add Workflow Type", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 390, "AddExternalUser", true, "User", "User", "Add External User", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 389, "ValidateEmail", true, "User", "User", "Validate Email", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 388, "SendConfirmationCode", true, "User", "User", "Send Confirmation Code", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 387, "DeactivateUser", true, "User", "User", "Deactivate User", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 386, "ForgetPassword", true, "User", "User", "Forget Password", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 385, "ChangePassword", true, "User", "User", "Change Password", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 384, "Updateuser", true, "User", "User", "Update User", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 383, "AddInternalUser", true, "User", "User", "Add Internal User", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 382, "CreateToken", true, "Token", "Token", "Create Token", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 381, "ExecuteTicktRequeste", true, "Ticket", "Ticket", "Execute Ticket Requeste", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 380, "AddTicket", true, "Ticket", "Ticket", "Add Ticket", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 379, "GetTicketActionById", true, "Ticket", "Ticket", "Get Ticket Action By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 391, "getAll", true, "User", "User", "Get Users By Group Name", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 378, "GetPendingRequestes", true, "Ticket", "Requestes", "Get Pending Requestes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 376, "EditSupportTypeImplemented", true, "SupportTypeImplemented", "SupportTypeImplemented", "Edit Support Type Implemented", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 374, "AddSupportTypeImplemented", true, "SupportTypeImplemented", "SupportTypeImplemented", "Add Support Type Implemented", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 373, "getById", true, "SupportTypeImplemented", "SupportTypeImplemented", "Get Support Type Implemented By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 372, "getAll", true, "SupportTypeImplemented", "SupportTypeImplemented", "Get All Support Type Implemented", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 371, "GetPowerOnSiteWithEnableAtt", true, "Site", "Power", "Get Power On Site Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 370, "GetRadioRRUOnSiteWithEnableAtt", true, "Site", "RadioRRU", "Get Radio RRU On Site Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 369, "GetRadioAntennaOnSiteWithEnableAtt", true, "Site", "RadioAntenna", "Get Radio Antenna On Site Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 368, "GetMW_RFUOnSiteWithEnableAtt", true, "Site", "MW_RFU", "Get MW_RFU On Site Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 367, "GetMW_ODUOnSiteWithEnableAtt", true, "Site", "MW_ODU", "Get MW_ODU On Site Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 366, "GetMW_BUOnSiteWithEnableAtt", true, "Site", "MW_BU", "Get MW_BU On Site Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 365, "GetMW_DishOnSiteWithEnableAtt", true, "Site", "MW_Dish", "Get MW_Dish On Site Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 364, "GetLoadsOnSiteWithEnableAtt", true, "Site", "Loads", "Get Loads On Site Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 377, "GetAllTickets", true, "Ticket", "Ticket", "Get All Tickets", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 422, "UpdateWorkflowType", true, "Workflow", "Workflow", "Update Workflow Type", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 392, "GetAll", true, "User", "User", "Get All Users", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 394, "GetAllInternalUsers", true, "User", "User", "Get All Internal Users", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 420, "GetWorkflowTypeById", true, "Workflow", "Workflow", "Get Workflow Type By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 419, "GetAllWorkflowTyps", true, "Workflow", "Workflow", "Get All Workflow Typs", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 418, "ChangeWorkflowStatus", true, "Workflow", "Workflow", "Change Workflow Status By WorkflowId", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 417, "DeleteWorkflow", true, "Workflow", "Workflow", "Delete Workflow By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 416, "WorkflowPermissions", true, "Workflow", "Workflow", "Workflow Permissions", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 415, "EditTicketStatusStepAction", true, "Workflow", "Workflow", "Edit Ticket Status Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 414, "EditCivilValidationStepAction", true, "Workflow", "Workflow", "Edit Civil Validation Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 413, "EditTelecomValidationStepAction", true, "Workflow", "Workflow", "Edit Telecom Validation Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 412, "EditSelectTargetSupportStepAction", true, "Workflow", "Workflow", "Edit Select Target Support Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 411, "EditProposalApprovedStepAction", true, "Workflow", "Workflow", "Edit Proposal Approved Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 410, "EditStudyResultStepAction", true, "Workflow", "Workflow", "Edit Study Result Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 409, "EditCorrectDataStepAction", true, "Workflow", "Workflow", "Edit Correct Data Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 393, "GetAllExternalUsers", true, "User", "User", "Get All External Users", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 408, "EditUpdateDataStepAction", true, "Workflow", "Workflow", "Edit Update Data Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 406, "EditConditionStepAction", true, "Workflow", "Workflow", "Edit Condition Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 405, "EditCivilDecisionStepAction", true, "Workflow", "Workflow", "Edit Civil Decision Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 404, "EditCheckAvailableSpaceStepAction", true, "Workflow", "Workflow", "Edit Check Available Space Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 403, "EditStepActionApplyCalculation", true, "Workflow", "Workflow", "Edit Step Action Apply Calculation", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 402, "EditMailStepAction", true, "Workflow", "Workflow", "Edit Mail Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 401, "UpdateWorkflow", true, "Workflow", "Workflow", "Update Workflow", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 400, "AddWorkflow", true, "Workflow", "Workflow", "Add Workflow", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 399, "GetWorkflowById", true, "Workflow", "Workflow", "Get Workflow By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 398, "GetAllWorkflows", true, "Workflow", "Workflow", "Get All Workflows", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 397, "getById", true, "UserPermission", "User", "Get User Permissions", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 396, "CheckPasswordExpiryDate", true, "User", "User", "Check Password Expiry Date", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 395, "GetById", true, "User", "User", "Get User By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 407, "EditInsertDataStepAction", true, "Workflow", "Workflow", "Edit Insert Data Step Action", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 240, "GetAttForAddMW_BU", true, "MWInst", "MW_BU", "Get Attributes For Add MW_BU", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 299, "EditRadioRRUInstallation", true, "RadioInst", "RadioRRUInstallation", "Edit Radio RRU Installation", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 238, "AddPort", true, "MW_Port", "MW_Ports", "Add Port", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 86, "GetRadioAntennaLibraries", true, "RadioAntennaLibrary", "RadioAntennaLibrary", "Get Radio Antenna Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 85, "DeletePowerLibrary", true, "PowerLibrary", "PowerLibrary", "Delete Power Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 84, "DisablePowerLibrary", true, "PowerLibrary", "PowerLibrary", "Disable Power Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 83, "EditPowerLibrary", true, "PowerLibrary", "PowerLibrary", "Edit Power Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 82, "AddPowerLibrary", true, "PowerLibrary", "PowerLibrary", "Add Power Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 81, "getById", true, "PowerLibrary", "PowerLibrary", "Get Power Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 80, "GetForAdd", true, "PowerLibrary", "PowerLibrary", "Get Power Libraries For Add", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 79, "GetPowerLibrariesWithEnableAttributes", true, "PowerLibrary", "PowerLibrary", "Get Power Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 78, "getAll", true, "PowerLibrary", "PowerLibrary", "Get Power Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 77, "DeleteMW_RFULibrary", true, "MW_RFULibrary", "MW_RFULibrary", "Delete MW_RFU Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 76, "GetForAdd", true, "MW_RFULibrary", "MW_RFULibrary", "Get MW_RFU Libraries For Add", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 75, "DisableMW_RFULibrary", true, "MW_RFULibrary", "MW_RFULibrary", "Disable MW_RFU Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 87, "GetRadioAntennaLibrariesWithEnabledAttribute", true, "RadioAntennaLibrary", "RadioAntennaLibrary", "Get Radio Antenna Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 74, "EditMW_RFULibrary", true, "MW_RFULibrary", "MW_RFULibrary", "Edit MW_RFU Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 72, "getById", true, "MW_RFULibrary", "MW_RFULibrary", "Get MW_RFU Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 71, "GetMW_RFULibraries", true, "MW_RFULibrary", "MW_RFULibrary", "Get MW_RFU Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 70, "getAll", true, "MW_RFULibrary", "MW_RFULibrary", "Get MW_RFU Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 69, "DeleteMW_OtherLibrary", true, "MW_OtherLibrary", "MW_OtherLibrary", "Delete MW_Other Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 68, "GetForAdd", true, "MW_OtherLibrary", "MW_OtherLibrary", "Get MW_Other Libraries For Add", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 67, "DisableMW_OtherLibrary", true, "MW_OtherLibrary", "MW_OtherLibrary", "Disable MW_Other Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 66, "EditMW_OtherLibrary", true, "MW_OtherLibrary", "MW_OtherLibrary", "Edit MW_Other Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 65, "AddMW_OtherLibrary", true, "MW_OtherLibrary", "MW_OtherLibrary", "Add MW_Other Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 64, "getById", true, "MW_OtherLibrary", "MW_OtherLibrary", "Get MW_Other Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 63, "getAll", true, "MW_OtherLibrary", "MW_OtherLibrary", "Get MW_Other Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 62, "DeleteMW_ODULibrary", true, "MW_ODULibrary", "MW_ODULibrary", "Delete MW_ODU Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 61, "GetForAdd", true, "MW_ODULibrary", "MW_ODULibrary", "Get MW_ODU Libraries For Add", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 73, "AddMW_RFULibrary", true, "MW_RFULibrary", "MW_RFULibrary", "Add MW_RFU Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 60, "DisableMW_ODULibrary", true, "MW_ODULibrary", "MW_ODULibrary", "Disable MW_ODU Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 88, "GetRadioAntennaLibraryById", true, "RadioAntennaLibrary", "RadioAntennaLibrary", "Get Radio Antenna Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 90, "UpdateRadioAntennaLibrary", true, "RadioAntennaLibrary", "RadioAntennaLibrary", "Update Radio Antenna Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 116, "AddCabinetTelecomLibrary", true, "CabinetTelecomLibrary", "CabinetTelecomLibrary", "Add Cabinet Telecom Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 115, "GetCabinetTelecomLibraryById", true, "CabinetTelecomLibrary", "CabinetTelecomLibrary", "Get Cabinet Telecom Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 114, "GetCabinetTelecomLibraryEnabledAtt", true, "CabinetTelecomLibrary", "CabinetTelecomLibrary", "Get Cabinet Telecom Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 113, "GetCabinetTelecomLibraries", true, "CabinetTelecomLibrary", "CabinetTelecomLibrary", "Get Cabinet Telecom Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 112, "DeleteCabinetPowerLibrary", true, "CabinetPowerLibrary", "CabinetPowerLibrary", "Delete Cabinet Power Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 111, "DisableCabinetPowerLibrary", true, "CabinetPowerLibrary", "CabinetPowerLibrary", "Disable Cabinet Power Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 110, "UpdateCabinetPowerLibrary", true, "CabinetPowerLibrary", "CabinetPowerLibrary", "Update Cabinet Power Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 109, "AddCabinetPowerLibrary", true, "CabinetPowerLibrary", "CabinetPowerLibrary", "Add Cabinet Power Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 108, "GetCabinetPowerLibraryById", true, "CabinetPowerLibrary", "CabinetPowerLibrary", "Get Cabinet Power Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 107, "GetCabinetPowerLibraryEnabledAtt", true, "CabinetPowerLibrary", "CabinetPowerLibrary", "Get Cabinet Power Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 106, "GetCabinetPowerLibraries", true, "CabinetPowerLibrary", "CabinetPowerLibrary", "Get Cabinet Power Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 105, "DeleteRadioRRULibrary", true, "RadioRRULibrary", "RadioRRULibrary", "Delete Radio RRU Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 89, "AddRadioAntennaLibrary", true, "RadioAntennaLibrary", "RadioAntennaLibrary", "Add Radio Antenna Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 104, "DisableRadioRRULibrary", true, "RadioRRULibrary", "RadioRRULibrary", "Disable Radio RRU Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 102, "AddRadioRRULibrary", true, "RadioRRULibrary", "RadioRRULibrary", "Add Radio RRU Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 101, "GetRadioRRULibraryById", true, "RadioRRULibrary", "RadioRRULibrary", "Get Radio RRU Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 100, "GetRadioRRULibrariesWithEnabledAttribute", true, "RadioRRULibrary", "RadioRRULibrary", "Get Radio RRU Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 99, "GetRadioRRULibraries", true, "RadioRRULibrary", "RadioRRULibrary", "Get Radio RRU Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 98, "DeleteRadioOtherLibrary", true, "RadioOtherLibrary", "RadioOtherLibrary", "Delete Radio Other Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 97, "DisableRadioOtherLibrary", true, "RadioOtherLibrary", "RadioOtherLibrary", "Disable Radio Other Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 96, "UpdateRadioOtherLibrary", true, "RadioOtherLibrary", "RadioOtherLibrary", "Update Radio Other Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 95, "AddRadioOtherLibrary", true, "RadioOtherLibrary", "RadioOtherLibrary", "Add Radio Other Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 94, "GetOtherRadioLibraryById", true, "RadioOtherLibrary", "RadioOtherLibrary", "Get Radio Other Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 93, "GetOtherRadioLibraries", true, "RadioOtherLibrary", "RadioOtherLibrary", "Get Radio Other Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 92, "DeleteRadioAntennaLibrary", true, "RadioAntennaLibrary", "RadioAntennaLibrary", "Delete Radio Antenna Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 91, "DisableRadioAntennaLibrary", true, "RadioAntennaLibrary", "RadioAntennaLibrary", "Disable Radio Antenna Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 103, "UpdateRadioRRULibrary", true, "RadioRRULibrary", "RadioRRULibrary", "Update Radio RRU Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 117, "UpdateCabinetTelecomLibrary", true, "CabinetTelecomLibrary", "CabinetTelecomLibrary", "Update Cabinet Telecom Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 59, "EditMW_ODULibrary", true, "MW_ODULibrary", "MW_ODULibrary", "Edit MW_ODU Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 57, "getById", true, "MW_ODULibrary", "MW_ODULibrary", "Get MW_ODU Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 26, "GetDependencyInst", true, "DynamicAtt", "DynamicAttInst", "Get Dynamic Attribute Installation's Dependency Property", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 25, "GetDependencyPropertyLib", true, "DynamicAtt", "DynamicAttLibValue", "Get Dynamic Attribute Library's Dependency Property", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 24, "AddDynamicAttInst", true, "DynamicAtt", "DynamicAttInst", "Add Dynamic Attribute Installation Value", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 23, "AddDynamicAttLibValue", true, "DynamicAtt", "DynamicAttLibValue", "Add Dynamic Attribute Library Value", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 22, "AddDynamicAtts", true, "DynamicAtt", "DynamicAtt", "Add Dynamic Attribute", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 21, "DeleteCivilWithoutLegLibrary", true, "CivilWithoutLegLibrary", "CivilWithoutLegLibrary", "Delete Civil Without Leg Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 20, "DisableCivilWithoutLegLibrary", true, "CivilWithoutLegLibrary", "CivilWithoutLegLibrary", "Disable Civil Without Leg Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 19, "EditCivilWithoutLegLibrary", true, "CivilWithoutLegLibrary", "CivilWithoutLegLibrary", "Edit Civil Without Leg Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 18, "AddCivilWithoutLegLibrary", true, "CivilWithoutLegLibrary", "CivilWithoutLegLibrary", "Add Civil Without Leg Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 17, "getById", true, "CivilWithoutLegLibrary", "CivilWithoutLegLibrary", "Get Civil Without Leg Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 16, "GetCivilWithoutLegLibrariesEnabledAtt", true, "CivilWithoutLegLibrary", "CivilWithoutLegLibrary", "Get Civil Without Leg Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 15, "getAll", true, "CivilWithoutLegLibrary", "CivilWithoutLegLibrary", "Get Civil Without Leg Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 27, "AddDependencyLib", true, "DynamicAtt", "DynamicAttLibValue", "Add Dynamic Attribute Library's Dependency Property", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 14, "DeleteCivilWithLegLibrary", true, "CivilWithLegLibrary", "CivilWithLegLibrary", "Delete Civil With Leg Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 12, "EditCivilWithLegLibrary", true, "CivilWithLegLibrary", "CivilWithLegLibrary", "Edit Civil With Leg Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 11, "AddCivilWithLegLibrary", true, "CivilWithLegLibrary", "CivilWithLegLibrary", "Add Civil With Leg Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 10, "getById", true, "CivilWithLegLibrary", "CivilWithLegLibrary", "Get Civil With Leg Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 9, "GetCivilWithLegLibrariesEnabledAtt", true, "CivilWithLegLibrary", "CivilWithLegLibrary", "Get Civil With Leg Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 8, "getAll", true, "CivilWithLegLibrary", "CivilWithLegLibrary", "Get Civil With Leg Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 7, "DeleteCivilNonSteelLibrary", true, "CivilNonSteelLibrary", "CivilNonSteelLibrary", "Delete Civil Non Steel Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 6, "DisableCivilNonSteelLibrary", true, "CivilNonSteelLibrary", "CivilNonSteelLibrary", "Disable Civil Non Steel Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 5, "EditCivilNonSteelLibrary", true, "CivilNonSteelLibrary", "CivilNonSteelLibrary", "Edit Civil Non Steel Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 4, "AddCivilNonSteelLibrary", true, "CivilNonSteelLibrary", "CivilNonSteelLibrary", "Add Civil Non Steel Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 3, "getById", true, "CivilNonSteelLibrary", "CivilNonSteelLibrary", "Get Civil Non Steel Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 2, "GetCivilNonSteelLibrary", true, "CivilNonSteelLibrary", "CivilNonSteelLibrary", "Get Civil Non Steel Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 1, "getAll", true, "CivilNonSteelLibrary", "CivilNonSteelLibrary", "Get Civil Non Steel Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 13, "DisableCivilWithLegLibrary", true, "CivilWithLegLibrary", "CivilWithLegLibrary", "Disable Civil With Leg Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 239, "UpdatePort", true, "MW_Port", "MW_Ports", "Update Port", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 28, "GetForAdd", true, "DynamicAtt", "DynamicAtt", "Get Dynamic Attribute For Add", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 30, "GetById", true, "DynamicAtt", "DynamicAtt", "Get Dynamic Attribute By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 56, "GetMW_ODULibraries", true, "MW_ODULibrary", "MW_ODULibrary", "Get MW_ODU Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 55, "getAll", true, "MW_ODULibrary", "MW_ODULibrary", "Get MW_ODU Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 54, "DeleteMW_DishLibrary", true, "MW_DishLibrary", "MW_DishLibrary", "Delete Mw_Dish Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 53, "GetForAdd", true, "MW_DishLibrary", "MW_DishLibrary", "Get Mw_Dish Libraries For Add", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 52, "DisableMW_DishLibrary", true, "MW_DishLibrary", "MW_DishLibrary", "Disable Mw_Dish Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 51, "EditMW_DishLibrary", true, "MW_DishLibrary", "MW_DishLibrary", "Edit Mw_Dish Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 50, "AddMW_DishLibrary", true, "MW_DishLibrary", "MW_DishLibrary", "Add Mw_Dish Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 49, "getById", true, "MW_DishLibrary", "MW_DishLibrary", "Get Mw_Dish Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 48, "GetMW_DishLibraries", true, "MW_DishLibrary", "MW_DishLibrary", "Get Mw_Dish Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 47, "getAll", true, "MW_DishLibrary", "MW_DishLibrary", "Get Mw_Dish Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 46, "DeleteMW_BULibrary", true, "MW_BULibrary", "MW_BULibrary", "Delete MW_BU Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 45, "DisableMW_BULibrary", true, "MW_BULibrary", "MW_BULibrary", "Disable MW_BU Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 29, "GetDynamicAtts", true, "DynamicAtt", "DynamicAtt", "Get Dynamic Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 44, "EditMW_BULibrary", true, "MW_BULibrary", "MW_BULibrary", "Edit MW_BU Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 42, "GetForAdd", true, "MW_BULibrary", "MW_BULibrary", "Get MW_BU Libraries For Add", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 41, "getById", true, "MW_BULibrary", "MW_BULibrary", "Get MW_BU Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 40, "GetMW_BULibraries", true, "MW_BULibrary", "MW_BULibrary", "Get MW_BU Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 39, "getAll", true, "MW_BULibrary", "MW_BULibrary", "Get MW_BU Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 38, "DeleteLoadOtherLibrary", true, "LoadOtherLibrary", "LoadOtherLibrary", "Delete Load Other Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 37, "DisableLoadOtherLibrary", true, "LoadOtherLibrary", "LoadOtherLibrary", "Diable Load Other Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 36, "UpdateLoadOtherLibrary", true, "LoadOtherLibrary", "LoadOtherLibrary", "Update Load Other Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 35, "AddLoadOtherLibrary", true, "LoadOtherLibrary", "LoadOtherLibrary", "Add Load Other Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 34, "GetLoadOtherLibraryById", true, "LoadOtherLibrary", "LoadOtherLibrary", "Get Load Other Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 33, "GetLoadOtherLibrariesWithEnableAtt", true, "LoadOtherLibrary", "LoadOtherLibrary", "Get Load Other Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 32, "GetLoadOtherLibraries", true, "LoadOtherLibrary", "LoadOtherLibrary", "Get Load Other Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 31, "EditDynamicAtt", true, "DynamicAtt", "DynamicAtt", "Edit Dynamic Attribute", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 43, "AddMW_BULibrary", true, "MW_BULibrary", "MW_BULibrary", "Add MW_BU Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 118, "DisableCabinetTelecomLibrary", true, "CabinetTelecomLibrary", "CabinetTelecomLibrary", "Disable Cabinet Telecom Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 58, "AddMW_ODULibrary", true, "MW_ODULibrary", "MW_ODULibrary", "Add MW_ODU Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 120, "GetGeneratorLibraries", true, "GeneratorLibrary", "GeneratorLibrary", "Get Generator Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 207, "AddGroupRole", true, "GroupRole", "GroupRole", "Add Group Role", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 206, "GetRolesByGroupId", true, "GroupRole", "GroupRole", "Get Roles By Group Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 205, "GetGroupsByRoleId", true, "GroupRole", "GroupRole", "Get Group Role By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 204, "getAll", true, "GroupRole", "GroupRole", "Get Group Roles", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 203, "GetGroupsTest", true, "Group", "Groups", "Get Groups Test", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 202, "DeleteActorToGroup", true, "Group", "Groups", "Delete Actor To Group", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 201, "UpdateActorToGroup", true, "Group", "Groups", "Update Actor To Group", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 200, "AddActorToGroup", true, "Group", "Groups", "Add Actor To Group", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 199, "DeleteGroup", true, "Group", "Groups", "Delete Group By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 198, "ValidateGroupNameFromDatabase", true, "Group", "Groups", "Validate Group Name From Database", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 197, "ValidateGroupNameFromAD", true, "Group", "Groups", "Validate Group Name From AD", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 196, "AddGroup", true, "Group", "Groups", "Add Group", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 195, "EditGroup", true, "Group", "Groups", "Edit Group", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 194, "GetGroupByName", true, "Group", "Groups", "Get Group By Name", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 193, "CheckGroupChildrens", true, "Group", "Groups", "Check Group Childrens", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 192, "GetById", true, "Group", "Groups", "Get Group By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 191, "getAll", true, "Group", "Groups", "Get Groups", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 190, "GetAttachecdFilesBySite", true, "FileManagment", "Excel File", "Get Attachecd Files By Site", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 189, "GetAttachecdFiles", true, "FileManagment", "Excel File", "Get Attachecd Files", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 188, "GetFilesByRecordIdAndTableName", true, "FileManagment", "Excel File", "Get Files By Record Id And Table Name", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 187, "DeleteFile", true, "FileManagment", "Excel File", "Delete Excel File", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 186, "AttachFile", true, "FileManagment", "Excel File", "Attach Excel File", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 185, "ImportFile", true, "FileManagment", "Excel File", "Import Excel File", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 184, "GenerateExcelTemplacteByTableName", true, "FileManagment", "Excel File", "Generate Excel Templacte By Table Name", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 183, "Delete", true, "ConfigurationAtts", "Configuration", "Delete Configration Table By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 208, "getAll", true, "GroupUser", "GroupUser", "Get Group Users", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 209, "GetGroupsByUserId", true, "GroupUser", "GroupUser", "Get Group User By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 210, "GetUsersByGroupId", true, "GroupUser", "GroupUser", "Get Users By Group Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 211, "getAll", true, "GuyLineType", "GuyLineType", "Get Guy Line Types", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 237, "GetById", true, "MW_Port", "MW_Ports", "Get Port By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 236, "GetPorts", true, "MW_Port", "MW_Ports", "Get Ports", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 235, "RequiredNOTRequired", true, "ManageAtt", "ManageAttributes", "Required NOT Required", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 234, "Disable", true, "ManageAtt", "ManageAttributes", "Disable Static Attribute By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 233, "getById", true, "ManageAtt", "ManageAttributes", "Get Static Attribute By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 232, "GetStaticAtts", true, "ManageAtt", "ManageAttributes", "Get Static Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 231, "EditStaticAtt", true, "ManageAtt", "ManageAttributes", "Edit Static Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 230, "GetLoadOtherOnSiteWithEnableAtt", true, "LoadOther", "LoadOther", "Get Load Others's Enabled Attributes By Site", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 229, "GetLoadsOtherBySite", true, "LoadOther", "LoadOther", "Get Load Others By Site", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 228, "GetLoadOtherList", true, "LoadOther", "LoadOther", "Get Load Others Lists", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 227, "GetById", true, "LoadOther", "LoadOther", "Get Load Other By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 226, "EditLoadOther", true, "LoadOther", "LoadOther", "Edit Load Other", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 182, "Disable", true, "ConfigurationAtts", "Configuration", "Disable Configration Table By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 225, "AddLoadOther", true, "LoadOther", "LoadOther", "Add Load Other", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 223, "GetLegsByCivilId", true, "Leg", "Leg", "Get Legs By Civil Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 222, "EditLeg", true, "Leg", "Leg", "Edit Leg", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 221, "AddLeg", true, "Leg", "Leg", "Add Leg", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 220, "getById", true, "Leg", "Leg", "Get Leg By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 219, "getAll", true, "Leg", "Leg", "Get All Legs", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 218, "GetForAdd", true, "Inventory", "Inventory", "Get Inventories For Add", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 217, "ImportInstallationFileData", true, "ImportDataFromSharePoint", "InstallationFile", "Import Installation File Data", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 216, "ImportLibraryFileData", true, "ImportDataFromSharePoint", "LibraryFile", "Import Library File Data", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 215, "ImportSiteFileData", true, "ImportDataFromSharePoint", "SiteFile", "Import Site File Data", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 214, "EditBaseCivilWithLegsType", true, "GuyLineType", "GuyLineType", "Edit Guy Line Type", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 213, "AddGuyLineType", true, "GuyLineType", "GuyLineType", "Add Guy Line Type", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 212, "getById", true, "GuyLineType", "GuyLineType", "Get Guy Line Type By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 224, "GetAttForAdd", true, "LoadOther", "LoadOther", "Get Load Other's Attributes For Add", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 181, "Update", true, "ConfigurationAtts", "Configuration", "Update Configration Table", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 180, "Add", true, "ConfigurationAtts", "Configuration", "Add Configration Table", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 149, "WhereCity", true, "City", "BaseCivilWithLegs", "Get Cities", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 147, "getAllRep", true, "City", "BaseCivilWithLegs", "Get All Cities", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 146, "getAll", true, "City", "BaseCivilWithLegs", "Get Cities", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 145, "EditBaseCivilWithLegsType", true, "BaseCivilWithLegsType", "BaseCivilWithLegs", "Edit Base Civil With Legs Type", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 144, "AddBaseCivilWithLegsType", true, "BaseCivilWithLegsType", "BaseCivilWithLegs", "Add Base Civil With Legs Type", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 143, "getById", true, "BaseCivilWithLegsType", "BaseCivilWithLegs", "Get Base Civil With Legs Type By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 142, "getAll", true, "BaseCivilWithLegsType", "BaseCivilWithLegs", "Get All Base Civil With Legs Types", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 141, "UpdateAttributeStatus", true, "AttributeViewManagment", "Actor", "Update Attribute View Managment Statues", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 140, "GetAllAttributes", true, "AttributeViewManagment", "Actor", "Get Attribute View Managments", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 139, "AddTablesActivatedAttributes", true, "AttributeActivated", "Actor", "Add To Attribute Activated", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 138, "DeleteActorFromGroups", true, "Actor", "Actor", "Delete Actor From Group", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 137, "DeleteActor", true, "Actor", "Actor", "Delete Actor", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 136, "UpdateActor", true, "Actor", "Actor", "Update Actor", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 135, "AddActor", true, "Actor", "Actor", "Add Actor", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 134, "GetActors", true, "Actor", "Actor", "Get Actors", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 133, "DeleteSolarLibrary", true, "SolarLibrary", "SolarLibrary", "Delete Solar Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 132, "DisableSolarLibrary", true, "SolarLibrary", "SolarLibrary", "Disable Solar Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 131, "UpdateSolarLibrary", true, "SolarLibrary", "SolarLibrary", "Update Solar Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 130, "AddSolarLibrary", true, "SolarLibrary", "SolarLibrary", "Add Solar Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 129, "GetSolarLibraryById", true, "SolarLibrary", "SolarLibrary", "Get Solar Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 128, "GetSolarLibraryEnabledAtt", true, "SolarLibrary", "SolarLibrary", "Get Solar Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 127, "GetSolarLibraries", true, "SolarLibrary", "SolarLibrary", "Get Solar Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 126, "DeleteGeneratorLibrary", true, "GeneratorLibrary", "GeneratorLibrary", "Delete Generator Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 125, "DisableGeneratorLibrary", true, "GeneratorLibrary", "GeneratorLibrary", "Disable Generator Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 124, "UpdateGeneratorLibrary", true, "GeneratorLibrary", "GeneratorLibrary", "Update Generator Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 123, "AddGeneratorLibrary", true, "GeneratorLibrary", "GeneratorLibrary", "Add Generator Library", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 122, "GetGeneratorLibraryById", true, "GeneratorLibrary", "GeneratorLibrary", "Get Generator Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 121, "GetGeneratorLibraryEnabledAtt", true, "GeneratorLibrary", "GeneratorLibrary", "Get Generator Libraries Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 148, "AddCity", true, "City", "BaseCivilWithLegs", "Add City", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 179, "GetById", true, "ConfigurationAtts", "Configuration", "Get Configration Table By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 119, "DeleteCabinetTelecomLibrary", true, "CabinetTelecomLibrary", "CabinetTelecomLibrary", "Delete Cabinet Telecom Library By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 151, "GetCivilWithLegsBySite", true, "CivilInst", "CivilWithLegs", "Get Civil With Legs By Site Code", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 178, "GetAll", true, "ConfigurationAtts", "Configuration", "Get All Configration Tables", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 177, "GetConfigrationTables", true, "ConfigurationAtts", "Configuration", "Get Configration Tables", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 176, "DisableCivilWithoutLegCategory", true, "CivilWithoutLegCategory", "CivilWithoutLegsCategory", "Disable Civil Without Legs Category By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 175, "getById", true, "CivilWithoutLegCategory", "CivilWithoutLegsCategory", "Get Civil Without Legs Category By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 174, "EditCivilWithoutLegCategory", true, "CivilWithoutLegCategory", "CivilWithoutLegsCategory", "Edit Civil Without Legs Category", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 173, "AddCivilWithoutLegCategory", true, "CivilWithoutLegCategory", "CivilWithoutLegsCategory", "Add Civil Without Legs Category", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 172, "getAll", true, "CivilWithoutLegCategory", "CivilWithoutLegsCategory", "Get Civil Without Legs Categories", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 171, "GetForAdd", true, "CivilLibrary", "CivilWithLegsLibrary", "Get Civil With Legs Libraries For Add", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 170, "GetCivilLibraryByType", true, "CivilLibrary", "CivilWithLegsLibrary", "Get Civil With Legs Libraries", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 169, "EditCivilNonSteel", true, "CivilInst", "CivilNonSteel", "Edit Civil Non Steel", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 167, "EditCivilWithoutLegs", true, "CivilInst", "CivilWithoutLegs", "Edit Civil Without Legs", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 166, "EditCivilWithLegs", true, "CivilInst", "CivilWithLegs", "Edit Civil With Legs", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 150, "GetAttForAddCivilWithLegs", true, "CivilInst", "CivilWithLegs", "Get Attributes For Civil With Legs", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 165, "GetCivilNonSteelById", true, "CivilInst", "CivilNonSteel", "Get Civil Non Steel By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 163, "GetCivilWithLegsById", true, "CivilInst", "CivilWithLegs", "Get Civil With Legs By Id", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 162, "AddCivilNonSteel", true, "CivilInst", "CivilNonSteel", "Add Civil Non Steel On Site", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 161, "AddCivilWithoutLegs", true, "CivilInst", "CivilWithoutLegs", "Add Civil Without Legs On Site", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 160, "AddCivilWithLegs", true, "CivilInst", "CivilWithLegs", "Add Civil With Legs On Site", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 159, "GetAttForAddCivilNonSteel", true, "CivilInst", "CivilNonSteel", "Get Attributes For Add Civil Non Steel", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 158, "GetAttForAddCivilWithoutLegs", true, "CivilInst", "CivilWithoutLegs", "Get Attributes For Add Civil Without Legs", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 157, "GetAllCivils", true, "CivilInst", "Civil", "Get All Civils", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 156, "GetCivilNonSteelWithEnableAtt", true, "CivilInst", "CivilNonSteel", "Get Civil Non Steel Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 155, "GetCivilWithoutLegWithEnableAtt", true, "CivilInst", "CivilWithoutLegs", "Get Civil Without Legs Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 154, "GetCivilWithLegsWithEnableAtt", true, "CivilInst", "CivilWithLegs", "Get Civil With Legs Enabled Attributes", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 153, "GetCivilNonSteelBySite", true, "CivilInst", "CivilNonSteel", "Get Civil Non Steel By Site Code", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 152, "GetCivilWithoutLegsBySite", true, "CivilInst", "CivilWithoutLegs", "Get Civil Without Legs By Site Code", null });

            migrationBuilder.InsertData(
                table: "TLIpermission",
                columns: new[] { "Id", "ActionName", "Active", "ControllerName", "Module", "Name", "PermissionType" },
                values: new object[] { 164, "GetCivilWithoutLegsById", true, "CivilInst", "CivilWithoutLegs", "Get Civil Without Legs By Id", null });

            migrationBuilder.InsertData(
                table: "TLItablePartName",
                columns: new[] { "Id", "PartName" },
                values: new object[,]
                {
                    { 5, "OtherInventory" },
                    { 4, "Power" },
                    { 3, "Radio" },
                    { 1, "CivilSupport" },
                    { 2, "MW" }
                });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 117, "TLImwOther", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 84, "TLIsideArmInstallationPlace", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 83, "TLIsideArm", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 82, "TLIsectionsLegType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 81, "TLIrule", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 80, "TLIrowRule", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 79, "TLIrow", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 78, "TLIrolePermission", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 77, "TLIrole", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 76, "TLIrepeaterType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 75, "TLIrenewableCabinetType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 74, "TLIregion", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 73, "TLIradioRRULibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 72, "TLIradioRRU", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 71, "TLIradioOtherLibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 70, "TLIradioAntennaLibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 69, "TLIradioAntenna", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 68, "TLIpowerLibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 67, "TLIpolarityType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 66, "TLIpolarityOnLocation", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 65, "TLIpermission", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 64, "TLIparity", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 63, "TLIowner", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 62, "TLIotherInventoryDistance", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 61, "TLIotherInSite", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 60, "TLIoption", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 85, "TLIsideArmLibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 86, "TLIsite", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 87, "TLIsiteStatus", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 59, "TLIoperation", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 116, "TLIloadOtherLibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 115, "TLIloadOther", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 114, "TLIpower", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 113, "TLImwRFU", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 112, "TLImwODU", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 111, "TLImwDish", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 110, "TLImwBU", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 109, "TLIworkFlowType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 108, "TLIworkFlow", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 107, "TLIvalidation", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 106, "TLIuserPermission", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 105, "TLIuser", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 118, "TLImwOtherLibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 104, "TLItelecomType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 102, "TLItablesNames", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 101, "TLItablesHistory", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 99, "TLIsupportTypeImplemented", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 98, "TLIsupportTypeDesigned", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 97, "TLIsuboption", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 96, "TLIstructureType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 95, "TLIstepActionOption", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 94, "TLIstepActionItemOption", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 93, "TLIstepActionIncomeItemStatus", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 92, "TLIstepAction", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 91, "TLIstep", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 90, "TLIsolarLibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 103, "TLItaskStatus", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 88, "TLIsolar", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 58, "TLIoduInstallationType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 56, "TLImwODULibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 25, "TLIcivilWithoutLegCategory", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 24, "TLIcivilWithoutLeg", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 23, "TLIcivilWithLegs", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 22, "TLIcivilWithLegLibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 21, "TLIcivilSteelSupportCategory", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 20, "TLIcivilSiteDate", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 19, "TLIcivilNonSteelLibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 18, "TLIcivilNonSteel", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 17, "TLIcity", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 16, "TLIcapacity", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 15, "TLIcabinetTelecomLibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 14, "TLIcabinetPowerType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 26, "TLIcivilWithoutLegLibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 13, "TLIcabinetPowerLibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 11, "TLIboardType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 10, "TLIbaseGeneratorType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 9, "TLIbaseCivilWithLegsType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 8, "TLIattributeActivated", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 7, "TLIattActivatedCategory", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 6, "TLIasType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 5, "TLIarea", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 4, "TLIactor", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 3, "TLIactionOption", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 2, "TLIactionItemOption", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 1, "TLIaction", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 119, "TLIradioOther", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 12, "TLIcabinet", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 57, "TLImwRFULibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 27, "TLIcondition", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 29, "TLIdataType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 55, "TLImwDishLibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 54, "TLImwBULibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 53, "TLIlogisticalType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 52, "TLIlogisticalitem", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 51, "TLIlogistical", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 50, "TLIlogicalOperation", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 49, "TLIlog", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 48, "TLIleg", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 47, "TLIitemStatus", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 46, "TLIitemConnectTo", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 45, "TLIinstallationPlace", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 44, "TLIInstCivilwithoutLegsType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 28, "TLIconditionType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 43, "TLIhistoryType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 41, "TLIguyLineType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 40, "TLIgroupUser", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 39, "TLIgroupRole", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 38, "TLIgroup", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 37, "TLIgeneratorLibrary", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 36, "TLIgenerator", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 35, "TLIdynamicAttLibValue", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 34, "TLIdynamicAttInstValue", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 33, "TLIdynamicAtt", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 32, "TLIdiversityType", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 31, "TLIdependencyRow", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 30, "TLIdependency", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 42, "TLIhistoryDetails", null });

            migrationBuilder.InsertData(
                table: "TLItablesNames",
                columns: new[] { "Id", "TableName", "tablePartNameId" },
                values: new object[] { 120, "TLImwPort", null });

            migrationBuilder.InsertData(
                table: "TLIeditableManagmentView",
                columns: new[] { "Id", "CivilWithoutLegCategoryId", "Description", "TLItablesNames1Id", "TLItablesNames2Id", "TLItablesNames3Id", "View" },
                values: new object[,]
                {
                    { 32, null, null, 119, null, null, "OtherRadioInstallation" },
                    { 20, null, null, 54, null, null, "MW_BULibrary" },
                    { 13, null, null, 37, null, null, "GeneratorLibrary" },
                    { 12, null, null, 36, null, null, "GeneratorInstallation" },
                    { 38, 3, null, 26, null, null, "CivilWithoutLegsLibraryMonopole" },
                    { 37, 2, null, 26, null, null, "CivilWithoutLegsLibraryCapsule" },
                    { 36, 1, null, 26, null, null, "CivilWithoutLegsLibraryMast" },
                    { 41, 2, null, 24, null, null, "CivilWithoutLegInstallationMonopole" },
                    { 19, null, null, 55, null, null, "MW_DishLibrary" },
                    { 40, 2, null, 24, null, null, "CivilWithoutLegInstallationCapsule" },
                    { 3, null, null, 24, null, null, "CivilWithoutLegInstallation" },
                    { 1, null, null, 23, null, null, "CivilWithLegInstallation" },
                    { 2, null, null, 22, null, null, "CivilWithLegLibrary" },
                    { 6, null, null, 19, null, null, "CivilNonSteelLibrary" },
                    { 8, null, null, 15, null, null, "CabinetTelecomLibrary" },
                    { 7, null, null, 13, null, null, "CabinetPowerLibrary" },
                    { 9, null, null, 12, null, null, "CabinetInstallation" },
                    { 39, 1, null, 24, null, null, "CivilWithoutLegInstallationMast" },
                    { 15, null, null, 56, null, null, "MW_ODULibrary" },
                    { 31, null, null, 118, null, null, "OtherMWLibrary" },
                    { 24, null, null, 68, null, null, "PowerLibrary" },
                    { 17, null, null, 57, null, null, "MW_RFULibrary" },
                    { 30, null, null, 117, null, null, "OtherMWInstallation" },
                    { 29, null, null, 116, null, null, "OtherLoadLibrary" },
                    { 28, null, null, 115, null, null, "OtherLoadInstallation" },
                    { 16, null, null, 113, null, null, "MW_RFUInstallation" },
                    { 14, null, null, 112, null, null, "MW_ODUInstallation" },
                    { 18, null, null, 111, null, null, "MW_DishInstallation" },
                    { 21, null, null, 110, null, null, "MW_BUInstallation" },
                    { 25, null, null, 114, null, null, "PowerInstallation" },
                    { 10, null, null, 88, null, null, "SolarInstallation" },
                    { 27, null, null, 85, null, null, "SideArmLibrary" },
                    { 26, null, null, 83, null, null, "SideArmInstallation" },
                    { 34, null, null, 73, null, null, "RadioRRULibrary" },
                    { 35, null, null, 72, null, null, "RadioRRUInstallation" },
                    { 33, null, null, 71, null, null, "OtherRadioLibrary" },
                    { 23, null, null, 70, null, null, "RadioAntennaLibrary" },
                    { 11, null, null, 90, null, null, "SolarLibrary" },
                    { 22, null, null, 69, null, null, "RadioAntennaInstallation" }
                });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 789, true, 321, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 321, true, 321, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 788, true, 320, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 320, true, 320, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 318, true, 318, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 319, true, 319, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 786, true, 318, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 785, true, 317, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 322, true, 322, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 787, true, 319, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 790, true, 322, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 327, true, 327, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 791, true, 323, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 324, true, 324, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 792, true, 324, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 325, true, 325, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 793, true, 325, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 326, true, 326, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 794, true, 326, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 317, true, 317, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 795, true, 327, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 328, true, 328, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 796, true, 328, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 323, true, 323, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 784, true, 316, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 779, true, 311, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 783, true, 315, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 770, true, 302, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 303, true, 303, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 771, true, 303, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 304, true, 304, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 772, true, 304, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 305, true, 305, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 773, true, 305, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 306, true, 306, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 774, true, 306, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 307, true, 307, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 775, true, 307, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 316, true, 316, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 309, true, 309, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 310, true, 310, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 778, true, 310, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 311, true, 311, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 329, true, 329, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 312, true, 312, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 780, true, 312, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 313, true, 313, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 781, true, 313, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 314, true, 314, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 782, true, 314, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 315, true, 315, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 777, true, 309, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 797, true, 329, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 802, true, 334, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 798, true, 330, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 346, true, 346, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 814, true, 346, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 347, true, 347, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 815, true, 347, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 348, true, 348, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 816, true, 348, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 349, true, 349, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 817, true, 349, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 350, true, 350, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 818, true, 350, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 351, true, 351, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 813, true, 345, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 819, true, 351, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 820, true, 352, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 353, true, 353, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 821, true, 353, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 354, true, 354, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 822, true, 354, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 355, true, 355, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 823, true, 355, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 356, true, 356, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 824, true, 356, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 357, true, 357, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 825, true, 357, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 352, true, 352, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 330, true, 330, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 345, true, 345, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 344, true, 344, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 331, true, 331, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 799, true, 331, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 332, true, 332, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 800, true, 332, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 333, true, 333, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 801, true, 333, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 334, true, 334, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 302, true, 302, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 335, true, 335, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 803, true, 335, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 336, true, 336, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 812, true, 344, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 804, true, 336, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 805, true, 337, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 338, true, 338, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 806, true, 338, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 339, true, 339, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 807, true, 339, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 340, true, 340, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 808, true, 340, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 342, true, 342, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 810, true, 342, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 343, true, 343, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 811, true, 343, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 337, true, 337, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 769, true, 301, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 297, true, 297, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 768, true, 300, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 260, true, 260, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 728, true, 260, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 261, true, 261, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 729, true, 261, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 262, true, 262, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 730, true, 262, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 263, true, 263, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 731, true, 263, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 264, true, 264, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 732, true, 264, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 265, true, 265, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 727, true, 259, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 733, true, 265, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 734, true, 266, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 267, true, 267, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 735, true, 267, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 268, true, 268, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 736, true, 268, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 269, true, 269, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 737, true, 269, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 270, true, 270, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 738, true, 270, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 271, true, 271, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 739, true, 271, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 266, true, 266, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 272, true, 272, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 259, true, 259, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 258, true, 258, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 246, true, 246, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 714, true, 246, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 247, true, 247, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 715, true, 247, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 248, true, 248, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 716, true, 248, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 249, true, 249, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 717, true, 249, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 250, true, 250, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 718, true, 250, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 251, true, 251, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 726, true, 258, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 719, true, 251, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 720, true, 252, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 253, true, 253, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 721, true, 253, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 254, true, 254, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 722, true, 254, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 255, true, 255, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 723, true, 255, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 256, true, 256, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 724, true, 256, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 257, true, 257, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 725, true, 257, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 252, true, 252, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 740, true, 272, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 273, true, 273, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 741, true, 273, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 756, true, 288, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 289, true, 289, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 757, true, 289, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 290, true, 290, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 758, true, 290, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 291, true, 291, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 759, true, 291, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 292, true, 292, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 760, true, 292, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 293, true, 293, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 761, true, 293, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 288, true, 288, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 294, true, 294, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 295, true, 295, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 763, true, 295, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 296, true, 296, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 764, true, 296, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 358, true, 358, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 765, true, 297, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 298, true, 298, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 766, true, 298, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 299, true, 299, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 767, true, 299, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 300, true, 300, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 762, true, 294, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 755, true, 287, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 287, true, 287, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 754, true, 286, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 274, true, 274, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 742, true, 274, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 275, true, 275, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 743, true, 275, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 276, true, 276, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 744, true, 276, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 277, true, 277, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 745, true, 277, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 278, true, 278, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 746, true, 278, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 279, true, 279, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 747, true, 279, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 280, true, 280, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 748, true, 280, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 281, true, 281, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 749, true, 281, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 282, true, 282, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 750, true, 282, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 283, true, 283, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 751, true, 283, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 284, true, 284, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 752, true, 284, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 285, true, 285, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 753, true, 285, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 286, true, 286, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 301, true, 301, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 826, true, 358, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 831, true, 363, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 827, true, 359, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 432, true, 432, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 900, true, 432, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 433, true, 433, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 901, true, 433, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 434, true, 434, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 902, true, 434, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 435, true, 435, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 903, true, 435, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 436, true, 436, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 904, true, 436, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 437, true, 437, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 899, true, 431, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 905, true, 437, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 906, true, 438, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 439, true, 439, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 907, true, 439, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 440, true, 440, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 908, true, 440, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 441, true, 441, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 909, true, 441, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 442, true, 442, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 910, true, 442, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 443, true, 443, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 911, true, 443, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 438, true, 438, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 444, true, 444, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 431, true, 431, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 430, true, 430, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 418, true, 418, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 886, true, 418, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 419, true, 419, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 887, true, 419, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 420, true, 420, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 888, true, 420, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 421, true, 421, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 889, true, 421, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 422, true, 422, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 890, true, 422, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 423, true, 423, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 898, true, 430, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 891, true, 423, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 892, true, 424, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 425, true, 425, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 893, true, 425, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 426, true, 426, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 894, true, 426, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 427, true, 427, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 895, true, 427, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 428, true, 428, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 896, true, 428, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 429, true, 429, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 897, true, 429, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 424, true, 424, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 885, true, 417, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 912, true, 444, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 913, true, 445, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 461, true, 461, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 929, true, 461, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 462, true, 462, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 930, true, 462, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 463, true, 463, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 931, true, 463, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 464, true, 464, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 932, true, 464, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 465, true, 465, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 933, true, 465, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 466, true, 466, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 928, true, 460, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 934, true, 466, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 935, true, 467, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 168, true, 168, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 636, true, 168, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 308, true, 308, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 776, true, 308, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 341, true, 341, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 809, true, 341, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 375, true, 375, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 843, true, 375, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 454, true, 454, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 922, true, 454, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 467, true, 467, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 445, true, 445, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 460, true, 460, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 459, true, 459, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 446, true, 446, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 914, true, 446, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 447, true, 447, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 915, true, 447, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 448, true, 448, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 916, true, 448, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 449, true, 449, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 917, true, 449, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 450, true, 450, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 918, true, 450, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 451, true, 451, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 927, true, 459, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 919, true, 451, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 920, true, 452, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 453, true, 453, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 921, true, 453, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 455, true, 455, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 923, true, 455, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 456, true, 456, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 924, true, 456, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 457, true, 457, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 925, true, 457, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 458, true, 458, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 926, true, 458, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 452, true, 452, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 417, true, 417, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 884, true, 416, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 416, true, 416, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 842, true, 374, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 376, true, 376, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 844, true, 376, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 377, true, 377, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 845, true, 377, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 378, true, 378, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 846, true, 378, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 379, true, 379, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 847, true, 379, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 380, true, 380, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 848, true, 380, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 374, true, 374, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 381, true, 381, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 382, true, 382, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 850, true, 382, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 383, true, 383, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 851, true, 383, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 384, true, 384, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 852, true, 384, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 385, true, 385, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 853, true, 385, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 386, true, 386, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 854, true, 386, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 387, true, 387, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 849, true, 381, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 855, true, 387, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 841, true, 373, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 840, true, 372, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 360, true, 360, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 828, true, 360, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 361, true, 361, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 829, true, 361, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 362, true, 362, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 830, true, 362, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 363, true, 363, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 364, true, 364, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 832, true, 364, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 365, true, 365, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 833, true, 365, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 373, true, 373, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 366, true, 366, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 367, true, 367, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 835, true, 367, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 368, true, 368, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 713, true, 245, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 369, true, 369, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 837, true, 369, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 370, true, 370, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 838, true, 370, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 371, true, 371, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 839, true, 371, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 372, true, 372, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 834, true, 366, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 388, true, 388, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 856, true, 388, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 389, true, 389, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 404, true, 404, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 872, true, 404, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 405, true, 405, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 873, true, 405, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 406, true, 406, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 874, true, 406, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 407, true, 407, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 875, true, 407, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 408, true, 408, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 876, true, 408, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 409, true, 409, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 871, true, 403, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 877, true, 409, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 878, true, 410, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 411, true, 411, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 879, true, 411, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 412, true, 412, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 880, true, 412, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 413, true, 413, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 881, true, 413, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 414, true, 414, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 882, true, 414, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 415, true, 415, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 883, true, 415, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 410, true, 410, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 403, true, 403, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 870, true, 402, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 402, true, 402, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 857, true, 389, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 390, true, 390, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 858, true, 390, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 391, true, 391, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 859, true, 391, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 392, true, 392, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 860, true, 392, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 393, true, 393, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 861, true, 393, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 394, true, 394, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 862, true, 394, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 395, true, 395, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 863, true, 395, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 396, true, 396, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 864, true, 396, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 397, true, 397, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 865, true, 397, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 398, true, 398, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 866, true, 398, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 399, true, 399, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 867, true, 399, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 400, true, 400, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 868, true, 400, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 401, true, 401, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 869, true, 401, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 359, true, 359, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 836, true, 368, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 245, true, 245, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 244, true, 244, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 545, true, 77, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 78, true, 78, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 546, true, 78, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 79, true, 79, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 547, true, 79, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 80, true, 80, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 548, true, 80, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 81, true, 81, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 549, true, 81, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 82, true, 82, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 550, true, 82, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 83, true, 83, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 77, true, 77, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 551, true, 83, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 552, true, 84, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 85, true, 85, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 553, true, 85, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 86, true, 86, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 554, true, 86, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 87, true, 87, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 555, true, 87, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 88, true, 88, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 556, true, 88, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 89, true, 89, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 557, true, 89, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 90, true, 90, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 84, true, 84, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 558, true, 90, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 544, true, 76, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 543, true, 75, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 530, true, 62, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 63, true, 63, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 531, true, 63, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 64, true, 64, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 532, true, 64, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 65, true, 65, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 533, true, 65, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 66, true, 66, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 534, true, 66, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 67, true, 67, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 535, true, 67, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 68, true, 68, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 76, true, 76, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 536, true, 68, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 537, true, 69, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 70, true, 70, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 538, true, 70, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 71, true, 71, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 539, true, 71, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 72, true, 72, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 540, true, 72, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 73, true, 73, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 541, true, 73, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 74, true, 74, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 542, true, 74, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 75, true, 75, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 69, true, 69, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 91, true, 91, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 559, true, 91, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 92, true, 92, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 108, true, 108, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 576, true, 108, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 109, true, 109, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 577, true, 109, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 110, true, 110, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 578, true, 110, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 111, true, 111, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 579, true, 111, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 112, true, 112, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 580, true, 112, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 113, true, 113, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 581, true, 113, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 575, true, 107, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 114, true, 114, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 115, true, 115, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 583, true, 115, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 116, true, 116, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 584, true, 116, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 117, true, 117, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 585, true, 117, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 118, true, 118, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 586, true, 118, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 119, true, 119, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 587, true, 119, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 120, true, 120, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 588, true, 120, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 582, true, 114, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 107, true, 107, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 574, true, 106, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 106, true, 106, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 560, true, 92, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 93, true, 93, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 561, true, 93, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 94, true, 94, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 562, true, 94, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 95, true, 95, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 563, true, 95, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 96, true, 96, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 564, true, 96, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 97, true, 97, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 565, true, 97, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 98, true, 98, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 566, true, 98, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 99, true, 99, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 567, true, 99, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 100, true, 100, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 568, true, 100, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 101, true, 101, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 569, true, 101, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 102, true, 102, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 570, true, 102, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 103, true, 103, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 571, true, 103, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 104, true, 104, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 572, true, 104, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 105, true, 105, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 573, true, 105, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 62, true, 62, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 529, true, 61, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 61, true, 61, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 528, true, 60, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 17, true, 17, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 485, true, 17, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 18, true, 18, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 486, true, 18, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 19, true, 19, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 487, true, 19, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 20, true, 20, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 488, true, 20, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 21, true, 21, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 489, true, 21, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 22, true, 22, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 490, true, 22, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 484, true, 16, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 23, true, 23, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 24, true, 24, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 492, true, 24, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 25, true, 25, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 493, true, 25, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 26, true, 26, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 494, true, 26, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 27, true, 27, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 495, true, 27, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 28, true, 28, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 496, true, 28, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 29, true, 29, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 497, true, 29, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 491, true, 23, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 16, true, 16, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 483, true, 15, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 15, true, 15, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 468, true, 1, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 2, true, 2, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 469, true, 2, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 3, true, 3, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 470, true, 3, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 4, true, 4, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 471, true, 4, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 5, true, 5, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 472, true, 5, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 6, true, 6, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 473, true, 6, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 7, true, 7, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 474, true, 7, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 8, true, 8, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 475, true, 8, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 9, true, 9, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 476, true, 9, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 10, true, 10, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 478, true, 10, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 11, true, 11, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 479, true, 11, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 12, true, 12, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 480, true, 12, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 13, true, 13, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 481, true, 13, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 14, true, 14, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 482, true, 14, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 30, true, 30, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 121, true, 121, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 498, true, 30, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 499, true, 31, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 515, true, 47, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 48, true, 48, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 516, true, 48, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 49, true, 49, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 517, true, 49, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 50, true, 50, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 518, true, 50, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 51, true, 51, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 519, true, 51, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 52, true, 52, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 520, true, 52, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 53, true, 53, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 47, true, 47, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 521, true, 53, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 522, true, 54, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 55, true, 55, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 523, true, 55, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 56, true, 56, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 524, true, 56, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 57, true, 57, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 525, true, 57, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 58, true, 58, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 526, true, 58, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 59, true, 59, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 527, true, 59, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 60, true, 60, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 54, true, 54, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 514, true, 46, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 46, true, 46, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 513, true, 45, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 32, true, 32, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 500, true, 32, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 33, true, 33, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 501, true, 33, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 34, true, 34, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 502, true, 34, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 35, true, 35, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 503, true, 35, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 36, true, 36, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 504, true, 36, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 37, true, 37, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 505, true, 37, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 38, true, 38, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 506, true, 38, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 39, true, 39, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 507, true, 39, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 40, true, 40, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 508, true, 40, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 41, true, 41, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 509, true, 41, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 42, true, 42, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 510, true, 42, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 43, true, 43, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 511, true, 43, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 44, true, 44, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 512, true, 44, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 45, true, 45, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 31, true, 31, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 589, true, 121, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 122, true, 122, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 590, true, 122, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 668, true, 200, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 201, true, 201, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 669, true, 201, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 202, true, 202, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 670, true, 202, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 203, true, 203, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 671, true, 203, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 204, true, 204, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 672, true, 204, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 205, true, 205, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 673, true, 205, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 206, true, 206, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 200, true, 200, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 674, true, 206, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 675, true, 207, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 208, true, 208, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 676, true, 208, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 209, true, 209, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 677, true, 209, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 210, true, 210, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 678, true, 210, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 211, true, 211, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 679, true, 211, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 212, true, 212, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 680, true, 212, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 213, true, 213, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 207, true, 207, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 667, true, 199, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 199, true, 199, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 666, true, 198, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 185, true, 185, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 653, true, 185, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 186, true, 186, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 654, true, 186, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 187, true, 187, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 655, true, 187, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 188, true, 188, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 656, true, 188, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 189, true, 189, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 657, true, 189, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 190, true, 190, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 658, true, 190, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 191, true, 191, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 659, true, 191, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 192, true, 192, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 660, true, 192, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 193, true, 193, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 661, true, 193, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 194, true, 194, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 662, true, 194, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 195, true, 195, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 663, true, 195, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 196, true, 196, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 664, true, 196, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 197, true, 197, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 665, true, 197, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 198, true, 198, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 681, true, 213, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 652, true, 184, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 214, true, 214, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 215, true, 215, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 231, true, 231, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 699, true, 231, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 232, true, 232, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 700, true, 232, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 233, true, 233, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 701, true, 233, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 234, true, 234, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 702, true, 234, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 235, true, 235, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 703, true, 235, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 236, true, 236, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 704, true, 236, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 698, true, 230, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 237, true, 237, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 238, true, 238, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 706, true, 238, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 239, true, 239, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 707, true, 239, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 240, true, 240, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 708, true, 240, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 241, true, 241, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 709, true, 241, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 242, true, 242, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 710, true, 242, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 243, true, 243, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 711, true, 243, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 705, true, 237, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 230, true, 230, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 697, true, 229, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 229, true, 229, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 683, true, 215, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 216, true, 216, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 684, true, 216, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 217, true, 217, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 685, true, 217, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 218, true, 218, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 686, true, 218, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 219, true, 219, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 687, true, 219, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 220, true, 220, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 688, true, 220, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 221, true, 221, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 689, true, 221, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 222, true, 222, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 690, true, 222, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 223, true, 223, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 691, true, 223, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 224, true, 224, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 692, true, 224, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 225, true, 225, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 693, true, 225, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 226, true, 226, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 694, true, 226, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 227, true, 227, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 695, true, 227, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 228, true, 228, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 696, true, 228, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 682, true, 214, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 712, true, 244, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 184, true, 184, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 183, true, 183, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 606, true, 138, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 139, true, 139, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 607, true, 139, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 140, true, 140, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 608, true, 140, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 141, true, 141, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 609, true, 141, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 142, true, 142, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 610, true, 142, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 143, true, 143, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 611, true, 143, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 144, true, 144, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 138, true, 138, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 612, true, 144, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 613, true, 145, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 146, true, 146, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 614, true, 146, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 147, true, 147, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 615, true, 147, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 148, true, 148, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 616, true, 148, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 149, true, 149, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 617, true, 149, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 150, true, 150, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 618, true, 150, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 151, true, 151, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 145, true, 145, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 605, true, 137, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 137, true, 137, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 604, true, 136, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 123, true, 123, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 591, true, 123, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 124, true, 124, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 592, true, 124, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 125, true, 125, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 593, true, 125, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 126, true, 126, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 594, true, 126, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 127, true, 127, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 595, true, 127, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 128, true, 128, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 596, true, 128, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 129, true, 129, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 597, true, 129, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 130, true, 130, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 598, true, 130, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 131, true, 131, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 599, true, 131, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 132, true, 132, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 600, true, 132, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 133, true, 133, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 601, true, 133, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 134, true, 134, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 602, true, 134, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 135, true, 135, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 603, true, 135, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 136, true, 136, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 619, true, 151, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 651, true, 183, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 152, true, 152, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 153, true, 153, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 170, true, 170, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 638, true, 170, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 171, true, 171, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 639, true, 171, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 172, true, 172, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 640, true, 172, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 173, true, 173, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 641, true, 173, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 174, true, 174, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 642, true, 174, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 175, true, 175, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 643, true, 175, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 637, true, 169, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 176, true, 176, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 177, true, 177, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 645, true, 177, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 178, true, 178, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 646, true, 178, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 179, true, 179, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 647, true, 179, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 180, true, 180, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 648, true, 180, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 181, true, 181, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 649, true, 181, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 182, true, 182, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 650, true, 182, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 644, true, 176, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 169, true, 169, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 635, true, 167, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 167, true, 167, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 621, true, 153, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 154, true, 154, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 622, true, 154, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 155, true, 155, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 623, true, 155, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 156, true, 156, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 624, true, 156, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 157, true, 157, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 625, true, 157, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 158, true, 158, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 626, true, 158, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 159, true, 159, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 627, true, 159, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 160, true, 160, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 628, true, 160, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 161, true, 161, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 629, true, 161, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 162, true, 162, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 630, true, 162, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 163, true, 163, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 631, true, 163, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 164, true, 164, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 632, true, 164, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 165, true, 165, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 633, true, 165, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 166, true, 166, 434 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 634, true, 166, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 620, true, 152, 435 });

            migrationBuilder.InsertData(
                table: "TLIuserPermission",
                columns: new[] { "Id", "Active", "permissionId", "userId" },
                values: new object[] { 1, true, 1, 434 });

            migrationBuilder.CreateIndex(
                name: "IX_TLIactionItemOption_ActionId",
                table: "TLIactionItemOption",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIactionOption_ActionId",
                table: "TLIactionOption",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIactionOption_ItemStatusId",
                table: "TLIactionOption",
                column: "ItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIactionOption_ParentId",
                table: "TLIactionOption",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIactor_Name",
                table: "TLIactor",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIagenda_ExecuterId",
                table: "TLIagenda",
                column: "ExecuterId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIagenda_TicketActionId",
                table: "TLIagenda",
                column: "TicketActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIagendaGroup_ActorId",
                table: "TLIagendaGroup",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIagendaGroup_AgendaId",
                table: "TLIagendaGroup",
                column: "AgendaId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIagendaGroup_GroupId",
                table: "TLIagendaGroup",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIagendaGroup_IntegrationId",
                table: "TLIagendaGroup",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIagendaGroup_UserId",
                table: "TLIagendaGroup",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallCivilInst_ItemStatusId",
                table: "TLIallCivilInst",
                column: "ItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallCivilInst_TicketId",
                table: "TLIallCivilInst",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallCivilInst_civilNonSteelId",
                table: "TLIallCivilInst",
                column: "civilNonSteelId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallCivilInst_civilWithLegsId",
                table: "TLIallCivilInst",
                column: "civilWithLegsId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallCivilInst_civilWithoutLegId",
                table: "TLIallCivilInst",
                column: "civilWithoutLegId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallLoadInst_ItemStatusId",
                table: "TLIallLoadInst",
                column: "ItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallLoadInst_TicketId",
                table: "TLIallLoadInst",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallLoadInst_loadOtherId",
                table: "TLIallLoadInst",
                column: "loadOtherId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallLoadInst_mwBUId",
                table: "TLIallLoadInst",
                column: "mwBUId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallLoadInst_mwDishId",
                table: "TLIallLoadInst",
                column: "mwDishId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallLoadInst_mwODUId",
                table: "TLIallLoadInst",
                column: "mwODUId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallLoadInst_mwOtherId",
                table: "TLIallLoadInst",
                column: "mwOtherId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallLoadInst_mwRFUId",
                table: "TLIallLoadInst",
                column: "mwRFUId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallLoadInst_powerId",
                table: "TLIallLoadInst",
                column: "powerId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallLoadInst_radioAntennaId",
                table: "TLIallLoadInst",
                column: "radioAntennaId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallLoadInst_radioOtherId",
                table: "TLIallLoadInst",
                column: "radioOtherId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallLoadInst_radioRRUId",
                table: "TLIallLoadInst",
                column: "radioRRUId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallOtherInventoryInst_ItemStatusId",
                table: "TLIallOtherInventoryInst",
                column: "ItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallOtherInventoryInst_TicketId",
                table: "TLIallOtherInventoryInst",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallOtherInventoryInst_cabinetId",
                table: "TLIallOtherInventoryInst",
                column: "cabinetId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallOtherInventoryInst_generatorId",
                table: "TLIallOtherInventoryInst",
                column: "generatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIallOtherInventoryInst_solarId",
                table: "TLIallOtherInventoryInst",
                column: "solarId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIantennaRRUInst_radioAntennaId",
                table: "TLIantennaRRUInst",
                column: "radioAntennaId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIantennaRRUInst_radioRRUId",
                table: "TLIantennaRRUInst",
                column: "radioRRUId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIattachedFiles_documenttypeId",
                table: "TLIattachedFiles",
                column: "documenttypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIattachedFiles_tablesNamesId",
                table: "TLIattachedFiles",
                column: "tablesNamesId");

            migrationBuilder.CreateIndex(
                name: "attributeActivatedIdAndcivilWithoutLegCategoryIdAndSameInstallationOrLibrarys",
                table: "TLIattActivatedCategory",
                column: "attributeActivatedId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "attributeActivatedIdAndcivilWithoutLegCategoryIdAndSameInstallationOrLibrary",
                table: "TLIattActivatedCategory",
                columns: new[] { "civilWithoutLegCategoryId", "IsLibrary" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIattributeViewManagment_AttributeActivatedId",
                table: "TLIattributeViewManagment",
                column: "AttributeActivatedId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIattributeViewManagment_DynamicAttId",
                table: "TLIattributeViewManagment",
                column: "DynamicAttId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIattributeViewManagment_EditableManagmentViewId",
                table: "TLIattributeViewManagment",
                column: "EditableManagmentViewId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIbaseCivilWithLegsType_Name",
                table: "TLIbaseCivilWithLegsType",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIcabinet_CabinetPowerLibraryId",
                table: "TLIcabinet",
                column: "CabinetPowerLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcabinet_CabinetTelecomLibraryId",
                table: "TLIcabinet",
                column: "CabinetTelecomLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcabinet_RenewableCabinetTypeId",
                table: "TLIcabinet",
                column: "RenewableCabinetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcabinetPowerLibrary_CabinetPowerTypeId",
                table: "TLIcabinetPowerLibrary",
                column: "CabinetPowerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcabinetTelecomLibrary_TelecomTypeId",
                table: "TLIcabinetTelecomLibrary",
                column: "TelecomTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilLoadLegs_civilLoadsId",
                table: "TLIcivilLoadLegs",
                column: "civilLoadsId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilLoadLegs_legId",
                table: "TLIcivilLoadLegs",
                column: "legId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilLoads_SiteCode",
                table: "TLIcivilLoads",
                column: "SiteCode");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilLoads_allCivilInstId",
                table: "TLIcivilLoads",
                column: "allCivilInstId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilLoads_allLoadInstId",
                table: "TLIcivilLoads",
                column: "allLoadInstId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilLoads_civilSteelSupportCategoryId",
                table: "TLIcivilLoads",
                column: "civilSteelSupportCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilLoads_legId",
                table: "TLIcivilLoads",
                column: "legId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilLoads_sideArmId",
                table: "TLIcivilLoads",
                column: "sideArmId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilNonSteel_CivilNonSteelLibraryId",
                table: "TLIcivilNonSteel",
                column: "CivilNonSteelLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilNonSteel_locationTypeId",
                table: "TLIcivilNonSteel",
                column: "locationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilNonSteel_ownerId",
                table: "TLIcivilNonSteel",
                column: "ownerId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilNonSteel_supportTypeImplementedId",
                table: "TLIcivilNonSteel",
                column: "supportTypeImplementedId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilNonSteelLibrary_civilNonSteelTypeId",
                table: "TLIcivilNonSteelLibrary",
                column: "civilNonSteelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilSiteDate_SiteCode",
                table: "TLIcivilSiteDate",
                column: "SiteCode");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilSiteDate_allCivilInstId",
                table: "TLIcivilSiteDate",
                column: "allCivilInstId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilSupportDistance_CivilInstId",
                table: "TLIcivilSupportDistance",
                column: "CivilInstId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilSupportDistance_SiteCode",
                table: "TLIcivilSupportDistance",
                column: "SiteCode");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithLegLibrary_civilSteelSupportCategoryId",
                table: "TLIcivilWithLegLibrary",
                column: "civilSteelSupportCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithLegLibrary_sectionsLegTypeId",
                table: "TLIcivilWithLegLibrary",
                column: "sectionsLegTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithLegLibrary_structureTypeId",
                table: "TLIcivilWithLegLibrary",
                column: "structureTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithLegLibrary_supportTypeDesignedId",
                table: "TLIcivilWithLegLibrary",
                column: "supportTypeDesignedId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithLegs_BaseCivilWithLegTypeId",
                table: "TLIcivilWithLegs",
                column: "BaseCivilWithLegTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithLegs_CivilWithLegsLibId",
                table: "TLIcivilWithLegs",
                column: "CivilWithLegsLibId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithLegs_GuylineTypeId",
                table: "TLIcivilWithLegs",
                column: "GuylineTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithLegs_OwnerId",
                table: "TLIcivilWithLegs",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithLegs_SupportTypeImplementedId",
                table: "TLIcivilWithLegs",
                column: "SupportTypeImplementedId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithLegs_baseTypeId",
                table: "TLIcivilWithLegs",
                column: "baseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithLegs_enforcmentCategoryId",
                table: "TLIcivilWithLegs",
                column: "enforcmentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithLegs_locationTypeId",
                table: "TLIcivilWithLegs",
                column: "locationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithoutLeg_CivilWithoutlegsLibId",
                table: "TLIcivilWithoutLeg",
                column: "CivilWithoutlegsLibId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithoutLeg_OwnerId",
                table: "TLIcivilWithoutLeg",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithoutLeg_subTypeId",
                table: "TLIcivilWithoutLeg",
                column: "subTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithoutLegCategory_Name",
                table: "TLIcivilWithoutLegCategory",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithoutLegLibrary_CivilSteelSupportCategoryId",
                table: "TLIcivilWithoutLegLibrary",
                column: "CivilSteelSupportCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithoutLegLibrary_CivilWithoutLegCategoryId",
                table: "TLIcivilWithoutLegLibrary",
                column: "CivilWithoutLegCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithoutLegLibrary_InstCivilwithoutLegsTypeId",
                table: "TLIcivilWithoutLegLibrary",
                column: "InstCivilwithoutLegsTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIcivilWithoutLegLibrary_structureTypeId",
                table: "TLIcivilWithoutLegLibrary",
                column: "structureTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdependency_DynamicAttId",
                table: "TLIdependency",
                column: "DynamicAttId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdependency_OperationId",
                table: "TLIdependency",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdependencyRow_DependencyId",
                table: "TLIdependencyRow",
                column: "DependencyId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdependencyRow_LogicalOperationId",
                table: "TLIdependencyRow",
                column: "LogicalOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdependencyRow_RowId",
                table: "TLIdependencyRow",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdynamicAtt_CivilWithoutLegCategoryId",
                table: "TLIdynamicAtt",
                column: "CivilWithoutLegCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdynamicAtt_DataTypeId",
                table: "TLIdynamicAtt",
                column: "DataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdynamicAtt_tablesNamesId",
                table: "TLIdynamicAtt",
                column: "tablesNamesId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdynamicAttInstValue_DynamicAttId",
                table: "TLIdynamicAttInstValue",
                column: "DynamicAttId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdynamicAttInstValue_dynamicListValuesId",
                table: "TLIdynamicAttInstValue",
                column: "dynamicListValuesId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdynamicAttInstValue_sideArmId",
                table: "TLIdynamicAttInstValue",
                column: "sideArmId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdynamicAttInstValue_tablesNamesId",
                table: "TLIdynamicAttInstValue",
                column: "tablesNamesId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdynamicAttLibValue_DynamicAttId",
                table: "TLIdynamicAttLibValue",
                column: "DynamicAttId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdynamicAttLibValue_dynamicListValuesId",
                table: "TLIdynamicAttLibValue",
                column: "dynamicListValuesId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdynamicAttLibValue_tablesNamesId",
                table: "TLIdynamicAttLibValue",
                column: "tablesNamesId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIdynamicListValues_dynamicAttId",
                table: "TLIdynamicListValues",
                column: "dynamicAttId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIeditableManagmentView_CivilWithoutLegCategoryId",
                table: "TLIeditableManagmentView",
                column: "CivilWithoutLegCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIeditableManagmentView_TLItablesNames1Id",
                table: "TLIeditableManagmentView",
                column: "TLItablesNames1Id");

            migrationBuilder.CreateIndex(
                name: "IX_TLIeditableManagmentView_TLItablesNames2Id",
                table: "TLIeditableManagmentView",
                column: "TLItablesNames2Id");

            migrationBuilder.CreateIndex(
                name: "IX_TLIeditableManagmentView_TLItablesNames3Id",
                table: "TLIeditableManagmentView",
                column: "TLItablesNames3Id");

            migrationBuilder.CreateIndex(
                name: "IX_TLIeditableManagmentView_View_CivilWithoutLegCategoryId",
                table: "TLIeditableManagmentView",
                columns: new[] { "View", "CivilWithoutLegCategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIequipment_PartId",
                table: "TLIequipment",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIgenerator_BaseGeneratorTypeId",
                table: "TLIgenerator",
                column: "BaseGeneratorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIgenerator_GeneratorLibraryId",
                table: "TLIgenerator",
                column: "GeneratorLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIgeneratorLibrary_CapacityId",
                table: "TLIgeneratorLibrary",
                column: "CapacityId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIgroup_ActorId",
                table: "TLIgroup",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIgroup_Name",
                table: "TLIgroup",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIgroup_UpperId",
                table: "TLIgroup",
                column: "UpperId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIgroupRole_roleId",
                table: "TLIgroupRole",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "GroupIdAndRoleId",
                table: "TLIgroupRole",
                columns: new[] { "groupId", "roleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIgroupUser_userId",
                table: "TLIgroupUser",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "GroupIdAndUserId",
                table: "TLIgroupUser",
                columns: new[] { "groupId", "userId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIhistoryDetails_TablesHistoryId",
                table: "TLIhistoryDetails",
                column: "TablesHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIhistoryDetails_WorkflowTableHistoryId",
                table: "TLIhistoryDetails",
                column: "WorkflowTableHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIintegration_Name",
                table: "TLIintegration",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIleg_CivilWithLegInstId",
                table: "TLIleg",
                column: "CivilWithLegInstId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIloadOther_InstallationPlaceId",
                table: "TLIloadOther",
                column: "InstallationPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIloadOther_loadOtherLibraryId",
                table: "TLIloadOther",
                column: "loadOtherLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIlogistical_logisticalTypeId",
                table: "TLIlogistical",
                column: "logisticalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIlogistical_tablePartNameId",
                table: "TLIlogistical",
                column: "tablePartNameId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIlogisticalitem_logisticalId",
                table: "TLIlogisticalitem",
                column: "logisticalId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIlogisticalitem_tablesNamesId",
                table: "TLIlogisticalitem",
                column: "tablesNamesId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImailTemplate_Name",
                table: "TLImailTemplate",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLImwBU_BaseBUId",
                table: "TLImwBU",
                column: "BaseBUId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwBU_InstallationPlaceId",
                table: "TLImwBU",
                column: "InstallationPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwBU_MainDishId",
                table: "TLImwBU",
                column: "MainDishId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwBU_MwBULibraryId",
                table: "TLImwBU",
                column: "MwBULibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwBU_OwnerId",
                table: "TLImwBU",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwBULibrary_diversityTypeId",
                table: "TLImwBULibrary",
                column: "diversityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwDish_InstallationPlaceId",
                table: "TLImwDish",
                column: "InstallationPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwDish_ItemConnectToId",
                table: "TLImwDish",
                column: "ItemConnectToId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwDish_MwDishLibraryId",
                table: "TLImwDish",
                column: "MwDishLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwDish_PolarityOnLocationId",
                table: "TLImwDish",
                column: "PolarityOnLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwDish_RepeaterTypeId",
                table: "TLImwDish",
                column: "RepeaterTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwDish_ownerId",
                table: "TLImwDish",
                column: "ownerId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwDishLibrary_asTypeId",
                table: "TLImwDishLibrary",
                column: "asTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwDishLibrary_polarityTypeId",
                table: "TLImwDishLibrary",
                column: "polarityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwODU_MwODULibraryId",
                table: "TLImwODU",
                column: "MwODULibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwODU_Mw_DishId",
                table: "TLImwODU",
                column: "Mw_DishId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwODU_OduInstallationTypeId",
                table: "TLImwODU",
                column: "OduInstallationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwODU_OwnerId",
                table: "TLImwODU",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwODULibrary_parityId",
                table: "TLImwODULibrary",
                column: "parityId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwOther_InstallationPlaceId",
                table: "TLImwOther",
                column: "InstallationPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwOther_mwOtherLibraryId",
                table: "TLImwOther",
                column: "mwOtherLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwPort_MwBUId",
                table: "TLImwPort",
                column: "MwBUId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwPort_MwBULibraryId",
                table: "TLImwPort",
                column: "MwBULibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwRFU_MwPortId",
                table: "TLImwRFU",
                column: "MwPortId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwRFU_MwRFULibraryId",
                table: "TLImwRFU",
                column: "MwRFULibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwRFU_OwnerId",
                table: "TLImwRFU",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwRFULibrary_boardTypeId",
                table: "TLImwRFULibrary",
                column: "boardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLImwRFULibrary_diversityTypeId",
                table: "TLImwRFULibrary",
                column: "diversityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLInextStepAction_StepActionId",
                table: "TLInextStepAction",
                column: "StepActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLInextStepAction_StepActionItemOptionId",
                table: "TLInextStepAction",
                column: "StepActionItemOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLInextStepAction_StepActionOptionId",
                table: "TLInextStepAction",
                column: "StepActionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIoption_ConditionId",
                table: "TLIoption",
                column: "ConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIotherInSite_SiteCode",
                table: "TLIotherInSite",
                column: "SiteCode");

            migrationBuilder.CreateIndex(
                name: "IX_TLIotherInSite_allOtherInventoryInstId",
                table: "TLIotherInSite",
                column: "allOtherInventoryInstId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIotherInventoryDistance_SiteCode",
                table: "TLIotherInventoryDistance",
                column: "SiteCode");

            migrationBuilder.CreateIndex(
                name: "IX_TLIotherInventoryDistance_allOtherInventoryInstId",
                table: "TLIotherInventoryDistance",
                column: "allOtherInventoryInstId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIpart_Name",
                table: "TLIpart",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ControllerNameAndActionName",
                table: "TLIpermission",
                columns: new[] { "ControllerName", "ActionName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIpower_Name",
                table: "TLIpower",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIpower_SerialNumber",
                table: "TLIpower",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIpower_installationPlaceId",
                table: "TLIpower",
                column: "installationPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIpower_ownerId",
                table: "TLIpower",
                column: "ownerId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIpower_powerLibraryId",
                table: "TLIpower",
                column: "powerLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIpower_powerTypeId",
                table: "TLIpower",
                column: "powerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIradioAntenna_installationPlaceId",
                table: "TLIradioAntenna",
                column: "installationPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIradioAntenna_ownerId",
                table: "TLIradioAntenna",
                column: "ownerId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIradioAntenna_radioAntennaLibraryId",
                table: "TLIradioAntenna",
                column: "radioAntennaLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIradioOther_installationPlaceId",
                table: "TLIradioOther",
                column: "installationPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIradioOther_ownerId",
                table: "TLIradioOther",
                column: "ownerId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIradioOther_radioOtherLibraryId",
                table: "TLIradioOther",
                column: "radioOtherLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIRadioRRU_installationPlaceId",
                table: "TLIRadioRRU",
                column: "installationPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIRadioRRU_ownerId",
                table: "TLIRadioRRU",
                column: "ownerId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIRadioRRU_radioAntennaId",
                table: "TLIRadioRRU",
                column: "radioAntennaId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIRadioRRU_radioRRULibraryId",
                table: "TLIRadioRRU",
                column: "radioRRULibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIrole_Name",
                table: "TLIrole",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIrolePermission_permissionId",
                table: "TLIrolePermission",
                column: "permissionId");

            migrationBuilder.CreateIndex(
                name: "RoleIdAndPermissionId",
                table: "TLIrolePermission",
                columns: new[] { "roleId", "permissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIrowRule_LogicalOperationId",
                table: "TLIrowRule",
                column: "LogicalOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIrowRule_RowId",
                table: "TLIrowRule",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIrowRule_RuleId",
                table: "TLIrowRule",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIrule_OperationId",
                table: "TLIrule",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIrule_attributeActivatedId",
                table: "TLIrule",
                column: "attributeActivatedId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIrule_dynamicAttId",
                table: "TLIrule",
                column: "dynamicAttId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIrule_tablesNamesId",
                table: "TLIrule",
                column: "tablesNamesId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIsideArm_ItemStatusId",
                table: "TLIsideArm",
                column: "ItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIsideArm_TicketId",
                table: "TLIsideArm",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIsideArm_ownerId",
                table: "TLIsideArm",
                column: "ownerId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIsideArm_sideArmInstallationPlaceId",
                table: "TLIsideArm",
                column: "sideArmInstallationPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIsideArm_sideArmLibraryId",
                table: "TLIsideArm",
                column: "sideArmLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIsideArm_sideArmTypeId",
                table: "TLIsideArm",
                column: "sideArmTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIsite_AreaId",
                table: "TLIsite",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIsite_RegionCode",
                table: "TLIsite",
                column: "RegionCode");

            migrationBuilder.CreateIndex(
                name: "IX_TLIsite_SiteName",
                table: "TLIsite",
                column: "SiteName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIsite_siteStatusId",
                table: "TLIsite",
                column: "siteStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIsolar_CabinetId",
                table: "TLIsolar",
                column: "CabinetId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIsolar_SolarLibraryId",
                table: "TLIsolar",
                column: "SolarLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIsolarLibrary_CapacityId",
                table: "TLIsolarLibrary",
                column: "CapacityId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstep_ParentStepId",
                table: "TLIstep",
                column: "ParentStepId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstep_WorkFlowId",
                table: "TLIstep",
                column: "WorkFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepAction_ActionId",
                table: "TLIstepAction",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepAction_OrderStatusId",
                table: "TLIstepAction",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepAction_StepActionMailFromId",
                table: "TLIstepAction",
                column: "StepActionMailFromId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepAction_WorkflowId",
                table: "TLIstepAction",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionFileGroup_ActorId",
                table: "TLIstepActionFileGroup",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionFileGroup_GroupId",
                table: "TLIstepActionFileGroup",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionFileGroup_IntegrationId",
                table: "TLIstepActionFileGroup",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionFileGroup_StepActionId",
                table: "TLIstepActionFileGroup",
                column: "StepActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionFileGroup_UserId",
                table: "TLIstepActionFileGroup",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionGroup_ActorId",
                table: "TLIstepActionGroup",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionGroup_GroupId",
                table: "TLIstepActionGroup",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionGroup_IntegrationId",
                table: "TLIstepActionGroup",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionGroup_StepActionId",
                table: "TLIstepActionGroup",
                column: "StepActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionGroup_UserId",
                table: "TLIstepActionGroup",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionIncomeItemStatus_ItemStatusId",
                table: "TLIstepActionIncomeItemStatus",
                column: "ItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionIncomeItemStatus_StepActionId",
                table: "TLIstepActionIncomeItemStatus",
                column: "StepActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionItemOption_ActionItemOptionId",
                table: "TLIstepActionItemOption",
                column: "ActionItemOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionItemOption_OrderStatusId",
                table: "TLIstepActionItemOption",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionItemOption_StepActionId",
                table: "TLIstepActionItemOption",
                column: "StepActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionItemOption_TLIitemStatusId",
                table: "TLIstepActionItemOption",
                column: "TLIitemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionItemStatus_IncomingItemStatusId",
                table: "TLIstepActionItemStatus",
                column: "IncomingItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionItemStatus_OutgoingItemStatusId",
                table: "TLIstepActionItemStatus",
                column: "OutgoingItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionItemStatus_StepActionItemOptionId",
                table: "TLIstepActionItemStatus",
                column: "StepActionItemOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionMail_ActorId",
                table: "TLIstepActionMail",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionMail_GroupId",
                table: "TLIstepActionMail",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionMail_TLIintegrationId",
                table: "TLIstepActionMail",
                column: "TLIintegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionMail_UserId",
                table: "TLIstepActionMail",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionMailTo_ActorId",
                table: "TLIstepActionMailTo",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionMailTo_GroupId",
                table: "TLIstepActionMailTo",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionMailTo_StepActionId",
                table: "TLIstepActionMailTo",
                column: "StepActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionMailTo_TLIintegrationId",
                table: "TLIstepActionMailTo",
                column: "TLIintegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionMailTo_UserId",
                table: "TLIstepActionMailTo",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionOption_ActionOptionId",
                table: "TLIstepActionOption",
                column: "ActionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionOption_ItemStatusId",
                table: "TLIstepActionOption",
                column: "ItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionOption_OrderStatusId",
                table: "TLIstepActionOption",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionOption_StepActionId",
                table: "TLIstepActionOption",
                column: "StepActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionPart_PartId",
                table: "TLIstepActionPart",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionPart_StepActionId",
                table: "TLIstepActionPart",
                column: "StepActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionPartGroup_ActorId",
                table: "TLIstepActionPartGroup",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionPartGroup_GroupId",
                table: "TLIstepActionPartGroup",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionPartGroup_IntegrationId",
                table: "TLIstepActionPartGroup",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionPartGroup_StepActionPartId",
                table: "TLIstepActionPartGroup",
                column: "StepActionPartId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIstepActionPartGroup_UserId",
                table: "TLIstepActionPartGroup",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIsuboption_OptionId",
                table: "TLIsuboption",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLItablePartName_PartName",
                table: "TLItablePartName",
                column: "PartName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLItablesHistory_HistoryTypeId",
                table: "TLItablesHistory",
                column: "HistoryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLItablesHistory_PreviousHistoryId",
                table: "TLItablesHistory",
                column: "PreviousHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLItablesHistory_TablesNameId",
                table: "TLItablesHistory",
                column: "TablesNameId");

            migrationBuilder.CreateIndex(
                name: "IX_TLItablesHistory_UserId",
                table: "TLItablesHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TLItablesNames_TableName",
                table: "TLItablesNames",
                column: "TableName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLItablesNames_tablePartNameId",
                table: "TLItablesNames",
                column: "tablePartNameId");

            migrationBuilder.CreateIndex(
                name: "IX_TLItaskStatus_Name",
                table: "TLItaskStatus",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIticket_CreatorId",
                table: "TLIticket",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticket_IntegrationId",
                table: "TLIticket",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticket_SiteCode",
                table: "TLIticket",
                column: "SiteCode");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticket_StatusId",
                table: "TLIticket",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticket_TypeId",
                table: "TLIticket",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticket_WorkFlowId",
                table: "TLIticket",
                column: "WorkFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketAction_AssignedToId",
                table: "TLIticketAction",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketAction_ExecuterId",
                table: "TLIticketAction",
                column: "ExecuterId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketAction_StepActionId",
                table: "TLIticketAction",
                column: "StepActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketAction_TicketId",
                table: "TLIticketAction",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketActionFile_TicketActionId",
                table: "TLIticketActionFile",
                column: "TicketActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketEquipment_EquipemntId",
                table: "TLIticketEquipment",
                column: "EquipemntId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketEquipment_ItemStatusId",
                table: "TLIticketEquipment",
                column: "ItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketEquipment_TicketId",
                table: "TLIticketEquipment",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketEquipment_TicketTargetId",
                table: "TLIticketEquipment",
                column: "TicketTargetId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketEquipmentAction_ExecuterId",
                table: "TLIticketEquipmentAction",
                column: "ExecuterId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketEquipmentAction_ItemStatusId",
                table: "TLIticketEquipmentAction",
                column: "ItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketEquipmentAction_TicketActionId",
                table: "TLIticketEquipmentAction",
                column: "TicketActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketEquipmentAction_TicketEquipmentId",
                table: "TLIticketEquipmentAction",
                column: "TicketEquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketEquipmentActionFile_TicketEquipmentActionId",
                table: "TLIticketEquipmentActionFile",
                column: "TicketEquipmentActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketOptionNote_StepActionOptionId",
                table: "TLIticketOptionNote",
                column: "StepActionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketOptionNote_TicketActionId",
                table: "TLIticketOptionNote",
                column: "TicketActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketOptionNote_TicketEquipmentActionId",
                table: "TLIticketOptionNote",
                column: "TicketEquipmentActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketOptionNote_TicketId",
                table: "TLIticketOptionNote",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketStep_StepId",
                table: "TLIticketStep",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketStep_TicketId",
                table: "TLIticketStep",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIticketTarget_TicketId",
                table: "TLIticketTarget",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIuser_UserName",
                table: "TLIuser",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIuserPermission_userId",
                table: "TLIuserPermission",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "PermissionIdAndUserId",
                table: "TLIuserPermission",
                columns: new[] { "permissionId", "userId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TLIvalidation_DynamicAttId",
                table: "TLIvalidation",
                column: "DynamicAttId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIvalidation_OperationId",
                table: "TLIvalidation",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkFlow_SiteStatusId",
                table: "TLIworkFlow",
                column: "SiteStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkFlowGroup_ActorId",
                table: "TLIworkFlowGroup",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkFlowGroup_GroupId",
                table: "TLIworkFlowGroup",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkFlowGroup_IntegrationId",
                table: "TLIworkFlowGroup",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkFlowGroup_UserId",
                table: "TLIworkFlowGroup",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkFlowGroup_WorkFlowId",
                table: "TLIworkFlowGroup",
                column: "WorkFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkflowTableHistory_HistoryTypeId",
                table: "TLIworkflowTableHistory",
                column: "HistoryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkflowTableHistory_ItemStatusId",
                table: "TLIworkflowTableHistory",
                column: "ItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkflowTableHistory_PartId",
                table: "TLIworkflowTableHistory",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkflowTableHistory_PreviousHistoryId",
                table: "TLIworkflowTableHistory",
                column: "PreviousHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkflowTableHistory_TablesNameId",
                table: "TLIworkflowTableHistory",
                column: "TablesNameId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkflowTableHistory_TicketActionId",
                table: "TLIworkflowTableHistory",
                column: "TicketActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkflowTableHistory_TicketId",
                table: "TLIworkflowTableHistory",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkflowTableHistory_UserId",
                table: "TLIworkflowTableHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkFlowType_WorkFlowId",
                table: "TLIworkFlowType",
                column: "WorkFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_TLIworkFlowType_nextStepActionId",
                table: "TLIworkFlowType",
                column: "nextStepActionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TLIagendaGroup");

            migrationBuilder.DropTable(
                name: "TLIantennaRRUInst");

            migrationBuilder.DropTable(
                name: "TLIattachedFiles");

            migrationBuilder.DropTable(
                name: "TLIattActivatedCategory");

            migrationBuilder.DropTable(
                name: "TLIattributeViewManagment");

            migrationBuilder.DropTable(
                name: "TLIcity");

            migrationBuilder.DropTable(
                name: "TLIcivilLoadLegs");

            migrationBuilder.DropTable(
                name: "TLIcivilSiteDate");

            migrationBuilder.DropTable(
                name: "TLIcivilSupportDistance");

            migrationBuilder.DropTable(
                name: "TLIconditionType");

            migrationBuilder.DropTable(
                name: "TLIdependencyRow");

            migrationBuilder.DropTable(
                name: "TLIdynamicAttInstValue");

            migrationBuilder.DropTable(
                name: "TLIdynamicAttLibValue");

            migrationBuilder.DropTable(
                name: "TLIgroupRole");

            migrationBuilder.DropTable(
                name: "TLIgroupUser");

            migrationBuilder.DropTable(
                name: "TLIhistoryDetails");

            migrationBuilder.DropTable(
                name: "TLIimportSheets");

            migrationBuilder.DropTable(
                name: "TLIinstallationType");

            migrationBuilder.DropTable(
                name: "TLIlog");

            migrationBuilder.DropTable(
                name: "TLIlogisticalitem");

            migrationBuilder.DropTable(
                name: "TLIlogUsersActions");

            migrationBuilder.DropTable(
                name: "TLImailTemplate");

            migrationBuilder.DropTable(
                name: "TLInextStepAction");

            migrationBuilder.DropTable(
                name: "TLIotherInSite");

            migrationBuilder.DropTable(
                name: "TLIotherInventoryDistance");

            migrationBuilder.DropTable(
                name: "TLIrolePermission");

            migrationBuilder.DropTable(
                name: "TLIrowRule");

            migrationBuilder.DropTable(
                name: "TLIstepActionFileGroup");

            migrationBuilder.DropTable(
                name: "TLIstepActionGroup");

            migrationBuilder.DropTable(
                name: "TLIstepActionIncomeItemStatus");

            migrationBuilder.DropTable(
                name: "TLIstepActionItemStatus");

            migrationBuilder.DropTable(
                name: "TLIstepActionMailTo");

            migrationBuilder.DropTable(
                name: "TLIstepActionPartGroup");

            migrationBuilder.DropTable(
                name: "TLIsuboption");

            migrationBuilder.DropTable(
                name: "TLItaskStatus");

            migrationBuilder.DropTable(
                name: "TLIticketActionFile");

            migrationBuilder.DropTable(
                name: "TLIticketEquipmentActionFile");

            migrationBuilder.DropTable(
                name: "TLIticketOptionNote");

            migrationBuilder.DropTable(
                name: "TLIticketStep");

            migrationBuilder.DropTable(
                name: "TLIuserPermission");

            migrationBuilder.DropTable(
                name: "TLIvalidation");

            migrationBuilder.DropTable(
                name: "TLIworkFlowGroup");

            migrationBuilder.DropTable(
                name: "TLIagenda");

            migrationBuilder.DropTable(
                name: "TLIdocumentType");

            migrationBuilder.DropTable(
                name: "TLIeditableManagmentView");

            migrationBuilder.DropTable(
                name: "TLIcivilLoads");

            migrationBuilder.DropTable(
                name: "TLIdependency");

            migrationBuilder.DropTable(
                name: "TLIdynamicListValues");

            migrationBuilder.DropTable(
                name: "TLItablesHistory");

            migrationBuilder.DropTable(
                name: "TLIworkflowTableHistory");

            migrationBuilder.DropTable(
                name: "TLIlogistical");

            migrationBuilder.DropTable(
                name: "TLIallOtherInventoryInst");

            migrationBuilder.DropTable(
                name: "TLIrole");

            migrationBuilder.DropTable(
                name: "TLIlogicalOperation");

            migrationBuilder.DropTable(
                name: "TLIrow");

            migrationBuilder.DropTable(
                name: "TLIrule");

            migrationBuilder.DropTable(
                name: "TLIstepActionItemOption");

            migrationBuilder.DropTable(
                name: "TLIstepActionPart");

            migrationBuilder.DropTable(
                name: "TLIoption");

            migrationBuilder.DropTable(
                name: "TLIstepActionOption");

            migrationBuilder.DropTable(
                name: "TLIticketEquipmentAction");

            migrationBuilder.DropTable(
                name: "TLIstep");

            migrationBuilder.DropTable(
                name: "TLIpermission");

            migrationBuilder.DropTable(
                name: "TLIallCivilInst");

            migrationBuilder.DropTable(
                name: "TLIallLoadInst");

            migrationBuilder.DropTable(
                name: "TLIleg");

            migrationBuilder.DropTable(
                name: "TLIsideArm");

            migrationBuilder.DropTable(
                name: "TLIhistoryType");

            migrationBuilder.DropTable(
                name: "TLIlogisticalType");

            migrationBuilder.DropTable(
                name: "TLIgenerator");

            migrationBuilder.DropTable(
                name: "TLIsolar");

            migrationBuilder.DropTable(
                name: "TLIoperation");

            migrationBuilder.DropTable(
                name: "TLIattributeActivated");

            migrationBuilder.DropTable(
                name: "TLIdynamicAtt");

            migrationBuilder.DropTable(
                name: "TLIactionItemOption");

            migrationBuilder.DropTable(
                name: "TLIcondition");

            migrationBuilder.DropTable(
                name: "TLIactionOption");

            migrationBuilder.DropTable(
                name: "TLIticketAction");

            migrationBuilder.DropTable(
                name: "TLIticketEquipment");

            migrationBuilder.DropTable(
                name: "TLIcivilNonSteel");

            migrationBuilder.DropTable(
                name: "TLIcivilWithoutLeg");

            migrationBuilder.DropTable(
                name: "TLIloadOther");

            migrationBuilder.DropTable(
                name: "TLImwODU");

            migrationBuilder.DropTable(
                name: "TLImwOther");

            migrationBuilder.DropTable(
                name: "TLImwRFU");

            migrationBuilder.DropTable(
                name: "TLIpower");

            migrationBuilder.DropTable(
                name: "TLIradioOther");

            migrationBuilder.DropTable(
                name: "TLIRadioRRU");

            migrationBuilder.DropTable(
                name: "TLIcivilWithLegs");

            migrationBuilder.DropTable(
                name: "TLIsideArmInstallationPlace");

            migrationBuilder.DropTable(
                name: "TLIsideArmLibrary");

            migrationBuilder.DropTable(
                name: "TLIsideArmType");

            migrationBuilder.DropTable(
                name: "TLIbaseGeneratorType");

            migrationBuilder.DropTable(
                name: "TLIgeneratorLibrary");

            migrationBuilder.DropTable(
                name: "TLIcabinet");

            migrationBuilder.DropTable(
                name: "TLIsolarLibrary");

            migrationBuilder.DropTable(
                name: "TLIdataType");

            migrationBuilder.DropTable(
                name: "TLItablesNames");

            migrationBuilder.DropTable(
                name: "TLIequipment");

            migrationBuilder.DropTable(
                name: "TLIitemStatus");

            migrationBuilder.DropTable(
                name: "TLIticketTarget");

            migrationBuilder.DropTable(
                name: "TLIcivilNonSteelLibrary");

            migrationBuilder.DropTable(
                name: "TLIcivilWithoutLegLibrary");

            migrationBuilder.DropTable(
                name: "TLIsubType");

            migrationBuilder.DropTable(
                name: "TLIloadOtherLibrary");

            migrationBuilder.DropTable(
                name: "TLImwODULibrary");

            migrationBuilder.DropTable(
                name: "TLIoduInstallationType");

            migrationBuilder.DropTable(
                name: "TLImwOtherLibrary");

            migrationBuilder.DropTable(
                name: "TLImwPort");

            migrationBuilder.DropTable(
                name: "TLImwRFULibrary");

            migrationBuilder.DropTable(
                name: "TLIpowerLibrary");

            migrationBuilder.DropTable(
                name: "TLIpowerType");

            migrationBuilder.DropTable(
                name: "TLIradioOtherLibrary");

            migrationBuilder.DropTable(
                name: "TLIradioAntenna");

            migrationBuilder.DropTable(
                name: "TLIradioRRULibrary");

            migrationBuilder.DropTable(
                name: "TLIbaseCivilWithLegsType");

            migrationBuilder.DropTable(
                name: "TLIcivilWithLegLibrary");

            migrationBuilder.DropTable(
                name: "TLIguyLineType");

            migrationBuilder.DropTable(
                name: "TLIsupportTypeImplemented");

            migrationBuilder.DropTable(
                name: "TLIbaseType");

            migrationBuilder.DropTable(
                name: "TLIenforcmentCategory");

            migrationBuilder.DropTable(
                name: "TLIlocationType");

            migrationBuilder.DropTable(
                name: "TLIcabinetPowerLibrary");

            migrationBuilder.DropTable(
                name: "TLIcabinetTelecomLibrary");

            migrationBuilder.DropTable(
                name: "TLIrenewableCabinetType");

            migrationBuilder.DropTable(
                name: "TLIcapacity");

            migrationBuilder.DropTable(
                name: "TLItablePartName");

            migrationBuilder.DropTable(
                name: "TLIpart");

            migrationBuilder.DropTable(
                name: "TLIticket");

            migrationBuilder.DropTable(
                name: "TLIcivilNonSteelType");

            migrationBuilder.DropTable(
                name: "TLIcivilWithoutLegCategory");

            migrationBuilder.DropTable(
                name: "TLIInstCivilwithoutLegsType");

            migrationBuilder.DropTable(
                name: "TLIparity");

            migrationBuilder.DropTable(
                name: "TLImwBU");

            migrationBuilder.DropTable(
                name: "TLIboardType");

            migrationBuilder.DropTable(
                name: "TLIradioAntennaLibrary");

            migrationBuilder.DropTable(
                name: "TLIcivilSteelSupportCategory");

            migrationBuilder.DropTable(
                name: "TLIsectionsLegType");

            migrationBuilder.DropTable(
                name: "TLIstructureType");

            migrationBuilder.DropTable(
                name: "TLIsupportTypeDesigned");

            migrationBuilder.DropTable(
                name: "TLIcabinetPowerType");

            migrationBuilder.DropTable(
                name: "TLItelecomType");

            migrationBuilder.DropTable(
                name: "TLIsite");

            migrationBuilder.DropTable(
                name: "TLIworkFlowType");

            migrationBuilder.DropTable(
                name: "TLIbaseBU");

            migrationBuilder.DropTable(
                name: "TLImwDish");

            migrationBuilder.DropTable(
                name: "TLImwBULibrary");

            migrationBuilder.DropTable(
                name: "TLIarea");

            migrationBuilder.DropTable(
                name: "TLIregion");

            migrationBuilder.DropTable(
                name: "TLIstepAction");

            migrationBuilder.DropTable(
                name: "TLIinstallationPlace");

            migrationBuilder.DropTable(
                name: "TLIitemConnectTo");

            migrationBuilder.DropTable(
                name: "TLImwDishLibrary");

            migrationBuilder.DropTable(
                name: "TLIpolarityOnLocation");

            migrationBuilder.DropTable(
                name: "TLIrepeaterType");

            migrationBuilder.DropTable(
                name: "TLIowner");

            migrationBuilder.DropTable(
                name: "TLIdiversityType");

            migrationBuilder.DropTable(
                name: "TLIaction");

            migrationBuilder.DropTable(
                name: "TLIorderStatus");

            migrationBuilder.DropTable(
                name: "TLIstepActionMail");

            migrationBuilder.DropTable(
                name: "TLIworkFlow");

            migrationBuilder.DropTable(
                name: "TLIasType");

            migrationBuilder.DropTable(
                name: "TLIpolarityType");

            migrationBuilder.DropTable(
                name: "TLIgroup");

            migrationBuilder.DropTable(
                name: "TLIintegration");

            migrationBuilder.DropTable(
                name: "TLIuser");

            migrationBuilder.DropTable(
                name: "TLIsiteStatus");

            migrationBuilder.DropTable(
                name: "TLIactor");
        }
    }
}
