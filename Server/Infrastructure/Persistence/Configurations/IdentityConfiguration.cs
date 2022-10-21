namespace Persistence.Configurations
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class IdentityConfiguration
    {
        public class ApplicationRoleClaimConfig : IEntityTypeConfiguration<IdentityRoleClaim<string>>
        {
            public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder) =>
                builder
                    .ToTable("RoleClaims", "Identity");
        }

        public class IdentityUserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<string>>
        {
            public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder) =>
                builder
                    .ToTable("UserRoles", "Identity");
        }

        public class IdentityUserClaimConfig : IEntityTypeConfiguration<IdentityUserClaim<string>>
        {
            public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder) =>
                builder
                    .ToTable("UserClaims", "Identity");
        }

        public class IdentityUserLoginConfig : IEntityTypeConfiguration<IdentityUserLogin<string>>
        {
            public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder) =>
                builder
                    .ToTable("UserLogins", "Identity");
        }

        public class IdentityUserTokenConfig : IEntityTypeConfiguration<IdentityUserToken<string>>
        {
            public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder) =>
                builder
                    .ToTable("UserTokens", "Identity");
        }
    }
}