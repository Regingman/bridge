﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyDataCoinBridge.DataAccess;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyDataCoinBridge.Migrations
{
    [DbContext(typeof(WebApiDbContext))]
    [Migration("20230227120040_UserPrivacySettings")]
    partial class UserPrivacySettings
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CountryDataProvider", b =>
                {
                    b.Property<Guid>("CountriesId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("DataProvidersId")
                        .HasColumnType("uuid");

                    b.HasKey("CountriesId", "DataProvidersId");

                    b.HasIndex("DataProvidersId");

                    b.ToTable("CountryDataProvider");
                });

            modelBuilder.Entity("DataProviderRewardCategory", b =>
                {
                    b.Property<Guid>("DataProvidersId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RewardCategoriesId")
                        .HasColumnType("uuid");

                    b.HasKey("DataProvidersId", "RewardCategoriesId");

                    b.HasIndex("RewardCategoriesId");

                    b.ToTable("DataProviderRewardCategory");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.BridgeTransaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Claim")
                        .HasColumnType("boolean");

                    b.Property<decimal>("Commission")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Count")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<string>("ProviderId")
                        .HasColumnType("text");

                    b.Property<string>("ProviderName")
                        .HasColumnType("text");

                    b.Property<string>("RewardCategoryId")
                        .HasColumnType("text");

                    b.Property<string>("RewardCategoryName")
                        .HasColumnType("text");

                    b.Property<decimal>("USDMDC")
                        .HasColumnType("numeric");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("BridgeTransactions");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.BridgeUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<int>("IsVerified")
                        .HasColumnType("integer");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("Secret")
                        .HasColumnType("text");

                    b.Property<string>("TokenForService")
                        .HasColumnType("text");

                    b.Property<string>("VerificationCode")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("BridgeUsers");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.Country", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CountryCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CountryName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneCode")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.DataProvider", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<Guid?>("BridgeUserId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Icon")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BridgeUserId");

                    b.ToTable("DataProviders");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.RewardCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("RewardCategories");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.RewardCategoryByProvider", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<Guid>("DataProviderId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RewardCategoryId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("DataProviderId");

                    b.HasIndex("RewardCategoryId");

                    b.ToTable("RewardCategoryByProviders");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.Transaction", b =>
                {
                    b.Property<Guid>("TxId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<string>("From")
                        .HasColumnType("text");

                    b.Property<string>("To")
                        .HasColumnType("text");

                    b.Property<DateTime>("TxDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("TxId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.UserPrivacySetting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("BasicData")
                        .HasColumnType("boolean");

                    b.Property<bool>("Contacts")
                        .HasColumnType("boolean");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<bool>("PersonalInterests")
                        .HasColumnType("boolean");

                    b.Property<bool>("PlaceOfResidence")
                        .HasColumnType("boolean");

                    b.Property<bool>("Profile")
                        .HasColumnType("boolean");

                    b.Property<bool>("WorkAndEducation")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("UserPrivacySettings");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.UserRefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UserRefreshTokens");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.UserTermsOfUse", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("DataProviderId")
                        .HasColumnType("uuid");

                    b.Property<List<string>>("Email")
                        .HasColumnType("text[]");

                    b.Property<bool>("IsRegistered")
                        .HasColumnType("boolean");

                    b.Property<List<string>>("Phone")
                        .HasColumnType("text[]");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("DataProviderId");

                    b.ToTable("UserTermsOfUses");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.WebHook", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ContentType")
                        .HasColumnType("text");

                    b.Property<int[]>("HookEvents")
                        .HasColumnType("integer[]");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastTrigger")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Secret")
                        .HasColumnType("text");

                    b.Property<string>("WebHookUrl")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("WebHooks");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.WebHookHeader", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.Property<Guid>("WebHookID")
                        .HasColumnType("uuid");

                    b.HasKey("ID");

                    b.HasIndex("WebHookID");

                    b.ToTable("WebHookHeader");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.WebHookRecord", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Exception")
                        .HasColumnType("text");

                    b.Property<string>("Guid")
                        .HasColumnType("text");

                    b.Property<string>("RequestBody")
                        .HasColumnType("text");

                    b.Property<string>("RequestHeaders")
                        .HasColumnType("text");

                    b.Property<string>("ResponseBody")
                        .HasColumnType("text");

                    b.Property<int>("Result")
                        .HasColumnType("integer");

                    b.Property<int>("StatusCode")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("WebHookID")
                        .HasColumnType("uuid");

                    b.HasKey("ID");

                    b.HasIndex("WebHookID");

                    b.ToTable("WebHooksHistory");
                });

            modelBuilder.Entity("CountryDataProvider", b =>
                {
                    b.HasOne("MyDataCoinBridge.Entities.Country", null)
                        .WithMany()
                        .HasForeignKey("CountriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyDataCoinBridge.Entities.DataProvider", null)
                        .WithMany()
                        .HasForeignKey("DataProvidersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataProviderRewardCategory", b =>
                {
                    b.HasOne("MyDataCoinBridge.Entities.DataProvider", null)
                        .WithMany()
                        .HasForeignKey("DataProvidersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyDataCoinBridge.Entities.RewardCategory", null)
                        .WithMany()
                        .HasForeignKey("RewardCategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.DataProvider", b =>
                {
                    b.HasOne("MyDataCoinBridge.Entities.BridgeUser", "BridgeUser")
                        .WithMany()
                        .HasForeignKey("BridgeUserId");

                    b.Navigation("BridgeUser");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.RewardCategoryByProvider", b =>
                {
                    b.HasOne("MyDataCoinBridge.Entities.DataProvider", "DataProvider")
                        .WithMany()
                        .HasForeignKey("DataProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyDataCoinBridge.Entities.RewardCategory", "RewardCategory")
                        .WithMany()
                        .HasForeignKey("RewardCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DataProvider");

                    b.Navigation("RewardCategory");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.UserTermsOfUse", b =>
                {
                    b.HasOne("MyDataCoinBridge.Entities.DataProvider", "DataProvider")
                        .WithMany()
                        .HasForeignKey("DataProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DataProvider");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.WebHookHeader", b =>
                {
                    b.HasOne("MyDataCoinBridge.Entities.WebHook", "WebHook")
                        .WithMany("Headers")
                        .HasForeignKey("WebHookID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WebHook");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.WebHookRecord", b =>
                {
                    b.HasOne("MyDataCoinBridge.Entities.WebHook", "WebHook")
                        .WithMany("Records")
                        .HasForeignKey("WebHookID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WebHook");
                });

            modelBuilder.Entity("MyDataCoinBridge.Entities.WebHook", b =>
                {
                    b.Navigation("Headers");

                    b.Navigation("Records");
                });
#pragma warning restore 612, 618
        }
    }
}
