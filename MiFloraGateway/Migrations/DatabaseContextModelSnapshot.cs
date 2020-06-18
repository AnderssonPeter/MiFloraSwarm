﻿// <auto-generated />
using System;
using MiFloraGateway.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MiFloraGateway.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MiFloraGateway.Database.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:IdentityIncrement", 1)
                        .HasAnnotation("SqlServer:IdentitySeed", 1)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("IPAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(45)")
                        .HasMaxLength(45);

                    b.Property<string>("MACAddress")
                        .IsRequired()
                        .HasColumnType("nchar(17)")
                        .IsFixedLength(true)
                        .HasMaxLength(17);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Port")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IPAddress")
                        .IsUnique();

                    b.HasIndex("MACAddress")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("MiFloraGateway.Database.DeviceSensorDistance", b =>
                {
                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<int>("SensorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("When")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Rssi")
                        .HasColumnType("int");

                    b.HasKey("DeviceId", "SensorId", "When");

                    b.HasIndex("SensorId");

                    b.ToTable("DeviceSensorDistances");
                });

            modelBuilder.Entity("MiFloraGateway.Database.DeviceTag", b =>
                {
                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<string>("Tag")
                        .HasColumnType("nvarchar(32)")
                        .HasMaxLength(32);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(32)")
                        .HasMaxLength(32);

                    b.HasKey("DeviceId", "Tag");

                    b.ToTable("DevicesTags");
                });

            modelBuilder.Entity("MiFloraGateway.Database.LogEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("DeviceId")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("time");

                    b.Property<string>("Event")
                        .IsRequired()
                        .HasColumnType("nvarchar(21)")
                        .HasMaxLength(21);

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<int?>("PlantId")
                        .HasColumnType("int");

                    b.Property<string>("Result")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)")
                        .HasMaxLength(10);

                    b.Property<int?>("SensorId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("When")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("PlantId");

                    b.HasIndex("SensorId");

                    b.HasIndex("UserId");

                    b.ToTable("LogEntries");
                });

            modelBuilder.Entity("MiFloraGateway.Database.Plant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:IdentityIncrement", 1)
                        .HasAnnotation("SqlServer:IdentitySeed", 1)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Display")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LatinName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("LatinName")
                        .IsUnique();

                    b.ToTable("Plants");
                });

            modelBuilder.Entity("MiFloraGateway.Database.PlantBasic", b =>
                {
                    b.Property<int>("PlantId")
                        .HasColumnType("int");

                    b.Property<string>("Blooming")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Color")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FloralLanguage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Origin")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Production")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PlantId");

                    b.ToTable("PlantBasics");
                });

            modelBuilder.Entity("MiFloraGateway.Database.PlantMaintenance", b =>
                {
                    b.Property<int>("PlantId")
                        .HasColumnType("int");

                    b.Property<string>("Fertilization")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Pruning")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Size")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Soil")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sunlight")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Watering")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PlantId");

                    b.ToTable("PlantMaintenance");
                });

            modelBuilder.Entity("MiFloraGateway.Database.PlantParameters", b =>
                {
                    b.Property<int>("PlantId")
                        .HasColumnType("int");

                    b.HasKey("PlantId");

                    b.ToTable("PlantParameters");
                });

            modelBuilder.Entity("MiFloraGateway.Database.Sensor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:IdentityIncrement", 1)
                        .HasAnnotation("SqlServer:IdentitySeed", 1)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("MACAddress")
                        .IsRequired()
                        .HasColumnType("nchar(17)")
                        .IsFixedLength(true)
                        .HasMaxLength(17);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PlantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MACAddress");

                    b.HasIndex("PlantId");

                    b.ToTable("Sensors");
                });

            modelBuilder.Entity("MiFloraGateway.Database.SensorBatteryAndVersionReading", b =>
                {
                    b.Property<int>("SensorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("When")
                        .HasColumnType("datetime2");

                    b.Property<int>("Battery")
                        .HasColumnType("int");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("nvarchar(12)")
                        .HasMaxLength(12);

                    b.HasKey("SensorId", "When");

                    b.ToTable("SensorBatteryReadings");
                });

            modelBuilder.Entity("MiFloraGateway.Database.SensorDataReading", b =>
                {
                    b.Property<int>("SensorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("When")
                        .HasColumnType("datetime2");

                    b.Property<int>("Brightness")
                        .HasColumnType("int");

                    b.Property<int>("Conductivity")
                        .HasColumnType("int");

                    b.Property<int>("Moisture")
                        .HasColumnType("int");

                    b.Property<float>("Temperature")
                        .HasColumnType("real");

                    b.HasKey("SensorId", "When");

                    b.ToTable("SensorDataReadings");
                });

            modelBuilder.Entity("MiFloraGateway.Database.SensorTag", b =>
                {
                    b.Property<int>("SensorId")
                        .HasColumnType("int");

                    b.Property<string>("Tag")
                        .HasColumnType("nvarchar(32)")
                        .HasMaxLength(32);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(32)")
                        .HasMaxLength(32);

                    b.HasKey("SensorId", "Tag");

                    b.ToTable("SensorTags");
                });

            modelBuilder.Entity("MiFloraGateway.Database.Setting", b =>
                {
                    b.Property<int>("Key")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastChanged")
                        .HasColumnType("datetime2");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Key");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("MiFloraGateway.Database.DeviceSensorDistance", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Device", "Device")
                        .WithMany("SensorDistances")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MiFloraGateway.Database.Sensor", "Sensor")
                        .WithMany("DeviceDistances")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MiFloraGateway.Database.DeviceTag", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Device", "Device")
                        .WithMany("Tags")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MiFloraGateway.Database.LogEntry", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Device", "Device")
                        .WithMany("Logs")
                        .HasForeignKey("DeviceId");

                    b.HasOne("MiFloraGateway.Database.Plant", "Plant")
                        .WithMany()
                        .HasForeignKey("PlantId");

                    b.HasOne("MiFloraGateway.Database.Sensor", "Sensor")
                        .WithMany("Logs")
                        .HasForeignKey("SensorId");

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("MiFloraGateway.Database.PlantBasic", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Plant", "Plant")
                        .WithOne("Basic")
                        .HasForeignKey("MiFloraGateway.Database.PlantBasic", "PlantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MiFloraGateway.Database.PlantMaintenance", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Plant", "Plant")
                        .WithOne("Maintenance")
                        .HasForeignKey("MiFloraGateway.Database.PlantMaintenance", "PlantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MiFloraGateway.Database.PlantParameters", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Plant", "Plant")
                        .WithOne("Parameters")
                        .HasForeignKey("MiFloraGateway.Database.PlantParameters", "PlantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("MiFloraGateway.Database.Range", "EnvironmentHumidity", b1 =>
                        {
                            b1.Property<int>("PlantParametersPlantId")
                                .HasColumnType("int");

                            b1.Property<int?>("Max")
                                .HasColumnType("int");

                            b1.Property<int?>("Min")
                                .HasColumnType("int");

                            b1.HasKey("PlantParametersPlantId");

                            b1.ToTable("PlantParameters");

                            b1.WithOwner()
                                .HasForeignKey("PlantParametersPlantId");
                        });

                    b.OwnsOne("MiFloraGateway.Database.Range", "LightLux", b1 =>
                        {
                            b1.Property<int>("PlantParametersPlantId")
                                .HasColumnType("int");

                            b1.Property<int?>("Max")
                                .HasColumnType("int");

                            b1.Property<int?>("Min")
                                .HasColumnType("int");

                            b1.HasKey("PlantParametersPlantId");

                            b1.ToTable("PlantParameters");

                            b1.WithOwner()
                                .HasForeignKey("PlantParametersPlantId");
                        });

                    b.OwnsOne("MiFloraGateway.Database.Range", "LightMmol", b1 =>
                        {
                            b1.Property<int>("PlantParametersPlantId")
                                .HasColumnType("int");

                            b1.Property<int?>("Max")
                                .HasColumnType("int");

                            b1.Property<int?>("Min")
                                .HasColumnType("int");

                            b1.HasKey("PlantParametersPlantId");

                            b1.ToTable("PlantParameters");

                            b1.WithOwner()
                                .HasForeignKey("PlantParametersPlantId");
                        });

                    b.OwnsOne("MiFloraGateway.Database.Range", "SoilFertility", b1 =>
                        {
                            b1.Property<int>("PlantParametersPlantId")
                                .HasColumnType("int");

                            b1.Property<int?>("Max")
                                .HasColumnType("int");

                            b1.Property<int?>("Min")
                                .HasColumnType("int");

                            b1.HasKey("PlantParametersPlantId");

                            b1.ToTable("PlantParameters");

                            b1.WithOwner()
                                .HasForeignKey("PlantParametersPlantId");
                        });

                    b.OwnsOne("MiFloraGateway.Database.Range", "SoilHumidity", b1 =>
                        {
                            b1.Property<int>("PlantParametersPlantId")
                                .HasColumnType("int");

                            b1.Property<int?>("Max")
                                .HasColumnType("int");

                            b1.Property<int?>("Min")
                                .HasColumnType("int");

                            b1.HasKey("PlantParametersPlantId");

                            b1.ToTable("PlantParameters");

                            b1.WithOwner()
                                .HasForeignKey("PlantParametersPlantId");
                        });

                    b.OwnsOne("MiFloraGateway.Database.Range", "Temperature", b1 =>
                        {
                            b1.Property<int>("PlantParametersPlantId")
                                .HasColumnType("int");

                            b1.Property<int?>("Max")
                                .HasColumnType("int");

                            b1.Property<int?>("Min")
                                .HasColumnType("int");

                            b1.HasKey("PlantParametersPlantId");

                            b1.ToTable("PlantParameters");

                            b1.WithOwner()
                                .HasForeignKey("PlantParametersPlantId");
                        });
                });

            modelBuilder.Entity("MiFloraGateway.Database.Sensor", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Plant", "Plant")
                        .WithMany()
                        .HasForeignKey("PlantId");
                });

            modelBuilder.Entity("MiFloraGateway.Database.SensorBatteryAndVersionReading", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Sensor", "Sensor")
                        .WithMany("BatteryAndVersionReadings")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MiFloraGateway.Database.SensorDataReading", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Sensor", "Sensor")
                        .WithMany("DataReadings")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MiFloraGateway.Database.SensorTag", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Sensor", "Sensor")
                        .WithMany("Tags")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
