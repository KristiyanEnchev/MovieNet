﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Persistence.Context;

#nullable disable

namespace Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250106192627_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.33")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Comment", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<int>("MovieId")
                        .HasColumnType("integer")
                        .HasColumnName("movie_id");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_comments");

                    b.HasIndex("MovieId")
                        .HasDatabaseName("ix_comments_movie_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_comments_user_id");

                    b.ToTable("comments");
                });

            modelBuilder.Entity("Domain.Entities.Genre", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<int>("MediaType")
                        .HasColumnType("integer")
                        .HasColumnName("media_type");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<int>("TmdbId")
                        .HasColumnType("integer")
                        .HasColumnName("tmdb_id");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.HasKey("Id")
                        .HasName("pk_genres");

                    b.HasIndex("TmdbId")
                        .IsUnique()
                        .HasDatabaseName("ix_genres_tmdb_id");

                    b.ToTable("genres");
                });

            modelBuilder.Entity("Domain.Entities.Movie", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<int>("DislikeCount")
                        .HasColumnType("integer")
                        .HasColumnName("dislike_count");

                    b.Property<int>("LikeCount")
                        .HasColumnType("integer")
                        .HasColumnName("like_count");

                    b.Property<string>("PosterPath")
                        .HasColumnType("text")
                        .HasColumnName("poster_path");

                    b.Property<string>("ReleaseDate")
                        .HasColumnType("text")
                        .HasColumnName("release_date");

                    b.Property<string>("Title")
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<int>("TmdbId")
                        .HasColumnType("integer")
                        .HasColumnName("tmdb_id");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.Property<decimal?>("VoteAverage")
                        .HasColumnType("numeric")
                        .HasColumnName("vote_average");

                    b.HasKey("Id")
                        .HasName("pk_movies");

                    b.HasAlternateKey("TmdbId")
                        .HasName("ak_movies_tmdb_id");

                    b.ToTable("movies");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer")
                        .HasColumnName("access_failed_count");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("email");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("email_confirmed");

                    b.Property<string>("FirstName")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("first_name");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("LastName")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("last_name");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("lockout_enabled");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("lockout_end");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_email");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_user_name");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text")
                        .HasColumnName("phone_number");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("phone_number_confirmed");

                    b.Property<DateTime>("RefreshTokenExpiryTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("refresh_token_expiry_time");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text")
                        .HasColumnName("security_stamp");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("two_factor_enabled");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("email_index");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("user_name_index");

                    b.ToTable("users", "Identity");
                });

            modelBuilder.Entity("Domain.Entities.UserMovieInteraction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<bool>("IsDisliked")
                        .HasColumnType("boolean")
                        .HasColumnName("is_disliked");

                    b.Property<bool>("IsLiked")
                        .HasColumnType("boolean")
                        .HasColumnName("is_liked");

                    b.Property<bool>("IsWatchlisted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_watchlisted");

                    b.Property<int>("MediaType")
                        .HasColumnType("integer")
                        .HasColumnName("media_type");

                    b.Property<int>("MovieId")
                        .HasColumnType("integer")
                        .HasColumnName("movie_id");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_movie_interactions");

                    b.HasIndex("MovieId")
                        .HasDatabaseName("ix_user_movie_interactions_movie_id");

                    b.HasIndex("UserId", "MovieId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_movie_interactions_user_id_movie_id");

                    b.ToTable("user_movie_interactions");
                });

            modelBuilder.Entity("Domain.Entities.UserRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<string>("Description")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("name");

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_name");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("updated_by");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.HasKey("Id")
                        .HasName("pk_roles");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("role_name_index");

                    b.ToTable("roles", "Identity");
                });

            modelBuilder.Entity("GenreMovie", b =>
                {
                    b.Property<string>("GenresId")
                        .HasColumnType("text")
                        .HasColumnName("genres_id");

                    b.Property<string>("MoviesId")
                        .HasColumnType("text")
                        .HasColumnName("movies_id");

                    b.HasKey("GenresId", "MoviesId")
                        .HasName("pk_genre_movie");

                    b.HasIndex("MoviesId")
                        .HasDatabaseName("ix_genre_movie_movies_id");

                    b.ToTable("genre_movie");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("role_id");

                    b.HasKey("Id")
                        .HasName("pk_role_claims");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_role_claims_role_id");

                    b.ToTable("role_claims", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_claims");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_claims_user_id");

                    b.ToTable("user_claims", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text")
                        .HasColumnName("login_provider");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text")
                        .HasColumnName("provider_key");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text")
                        .HasColumnName("provider_display_name");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("LoginProvider", "ProviderKey")
                        .HasName("pk_user_logins");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_logins_user_id");

                    b.ToTable("user_logins", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.Property<string>("RoleId")
                        .HasColumnType("text")
                        .HasColumnName("role_id");

                    b.HasKey("UserId", "RoleId")
                        .HasName("pk_user_roles");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_user_roles_role_id");

                    b.ToTable("user_roles", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text")
                        .HasColumnName("login_provider");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Value")
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("UserId", "LoginProvider", "Name")
                        .HasName("pk_user_tokens");

                    b.ToTable("user_tokens", "Identity");
                });

            modelBuilder.Entity("Domain.Entities.Comment", b =>
                {
                    b.HasOne("Domain.Entities.Movie", "Movie")
                        .WithMany("Comments")
                        .HasForeignKey("MovieId")
                        .HasPrincipalKey("TmdbId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_comments_movies_movie_id");

                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_comments_users_user_id");

                    b.Navigation("Movie");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.UserMovieInteraction", b =>
                {
                    b.HasOne("Domain.Entities.Movie", "Movie")
                        .WithMany("UserInteractions")
                        .HasForeignKey("MovieId")
                        .HasPrincipalKey("TmdbId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_movie_interactions_movies_movie_id");

                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany("MovieInteractions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_movie_interactions_users_user_id");

                    b.Navigation("Movie");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GenreMovie", b =>
                {
                    b.HasOne("Domain.Entities.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_genre_movie_genres_genres_id");

                    b.HasOne("Domain.Entities.Movie", null)
                        .WithMany()
                        .HasForeignKey("MoviesId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_genre_movie_movies_movies_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Domain.Entities.UserRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_role_claims_roles_role_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_claims_users_user_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_logins_users_user_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Domain.Entities.UserRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_roles_roles_role_id");

                    b.HasOne("Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_roles_users_user_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_tokens_users_user_id");
                });

            modelBuilder.Entity("Domain.Entities.Movie", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("UserInteractions");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Navigation("MovieInteractions");
                });
#pragma warning restore 612, 618
        }
    }
}
